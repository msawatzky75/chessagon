using System.Collections.Generic;
using System.Linq;
using Godot;
using Chessagon.Common;


namespace Chessagon.UI;

public partial class SquareBoard : Node2D
{
	[Export] public float CellWidth = 80;
	[Export] public bool WhiteBottom = true;
	[Export] public Color DarkColor = Colors.SlateBlue;
	[Export] public Color LightColor = Colors.LightGray;
	[Export] public Color SelectedCellColor = new(Colors.Black, 0.3f);

	// this will be relative to the node position making it basically useless
	private Vector2 boardPositionOffset = new Vector2(0, 0);

	private int _selectedCell = -1;

	public int SelectedCell
	{
		get => _selectedCell;
		set
		{
			if (_selectedCell == value) return;
			
			_selectedCell = value;
			QueueRedraw();
		}
	}

	private ChessBoard _board;
	private List<ChessPiece> _pieces = new();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ReSharper disable StringLiteralTypo
		_board = ChessBoard.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

		for (int i = 0; i < _board.Length; i++)
		{
			if (_board[i] != 0)
			{
				var p = new ChessPiece()
				{
					// Index = i,
					// Position = IndexToVector(i) * new Vector2(CellWidth, CellWidth)
				};
				p.AddChild(GetSprite(_board[i]));
				p.MoveTo(i);
				AddChild(p);
				_pieces.Add(p);
			}
		}

		_board.PieceMove += OnPieceMove;
	}

	private void OnPieceMove(object sender, PieceMoveEventArgs e)
	{
		var sourcePiece = _pieces.First(x => x.Index == e.Move.From);
		var destPiece = _pieces.FirstOrDefault(x => x.Index == e.Move.To);
		
		if (destPiece is not null)
		{
			RemoveChild(destPiece);
			destPiece.Dispose();
		}

		sourcePiece.MoveTo(e.Move.To);
	}


	private List<Texture2D> LoadTextures()
	{
		var list = new List<Texture2D>();

		foreach (Piece color in new[] { Piece.White, Piece.Black })
		foreach (Piece piece in new[] { Piece.Pawn, Piece.Rook, Piece.Bishop, Piece.King, Piece.Knight, Piece.Queen })
		{
			string res = "res://Resources/";

			res += color.HasFlag(Piece.White) ? "White" : "Black";

			if (piece.HasFlag(Piece.Pawn)) res += "Pawn";
			else if (piece.HasFlag(Piece.Bishop)) res += "Bishop";
			else if (piece.HasFlag(Piece.Knight)) res += "Knight";
			else if (piece.HasFlag(Piece.Rook)) res += "Rook";
			else if (piece.HasFlag(Piece.Queen)) res += "Queen";
			else if (piece.HasFlag(Piece.King)) res += "King";
			res += ".png";

			list.Add(ResourceLoader.Load(res) as CompressedTexture2D);
		}

		return list;
	}

	private Sprite2D GetSprite(Piece piece)
	{
		var textures = LoadTextures();
		string res = "res://Resources/";

		res += piece.HasFlag(Piece.White) ? "White" : "Black";

		if (piece.HasFlag(Piece.Pawn)) res += "Pawn";
		else if (piece.HasFlag(Piece.Bishop)) res += "Bishop";
		else if (piece.HasFlag(Piece.Knight)) res += "Knight";
		else if (piece.HasFlag(Piece.Rook)) res += "Rook";
		else if (piece.HasFlag(Piece.Queen)) res += "Queen";
		else if (piece.HasFlag(Piece.King)) res += "King";
		res += ".png";

		var t = textures.First(x => x.ResourcePath.Equals(res));
		var sprite = new Sprite2D
		{
			Centered = false,
			Texture = t,
			Scale = new Vector2(CellWidth / t.GetWidth(), CellWidth / t.GetWidth())
		};

		return sprite;
	}

	private Vector2I IndexToVector(int index)
	{
		return new Vector2I(index % 8, index / 8);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseButton eventMouseButton) _MouseClick(eventMouseButton);
	}

	private void _MouseClick(InputEventMouseButton e)
	{
		var newSelection = _MousePositionToBoardIndex(e.Position);
		if (!e.Pressed) return;
		if (newSelection == SelectedCell) return;
		if (SelectedCell == -1)
		{
			SelectedCell = newSelection;
			return;
		}


		if (_board[SelectedCell] != 0)
		{
			// button was released
			_board.Move(new Move(SelectedCell, newSelection));
			SelectedCell = -1;
		}
		else
		{
			SelectedCell = newSelection;
		}

		DrawInformation(new Vector2(CellWidth, CellWidth), new Vector2(0, 0));
	}

	/// <summary>
	/// Converts a mouse position (in pixels) to a board index.
	/// Uses CellWidth.
	/// </summary>
	/// <param name="mousePos"></param>
	/// <returns></returns>
	private int _MousePositionToBoardIndex(Vector2 mousePos)
	{
		var correctedMouse = (mousePos - boardPositionOffset) / new Vector2(CellWidth, CellWidth);
		return ((int)correctedMouse.Y * 8) + (int)correctedMouse.X;
	}

	private Vector2 BoardIndexToPosition(int index)
	{
		return IndexToVector(index) * new Vector2(CellWidth, CellWidth);
	}

	public override void _Draw()
	{
		Vector2 size = new Vector2(CellWidth, CellWidth);
		DrawBoard();
		DrawInformation(size, boardPositionOffset);
	}

	private void DrawBoard()
	{
		Vector2 size = new Vector2(CellWidth, CellWidth);

		for (var i = 0; i < 64; i++)
		{
			Vector2I pos = IndexToVector(i);

			DrawRect(
				new Rect2((pos * size) + boardPositionOffset, size),
				(pos.X + pos.Y) % 2 == 0 ? LightColor : DarkColor
			);

			if (i == SelectedCell)
				DrawRect(
					new Rect2((pos * size) + boardPositionOffset, size), SelectedCellColor
				);

			// DrawPiece(pos, size, _board[i], boardPositionOffset);

			DrawLegends(pos, size, boardPositionOffset);
		}
	}

	private void DrawLegends(Vector2I pos, Vector2 size, Vector2 offset)
	{
		if (pos.X == 7)
			DrawString(
				new SystemFont(),
				(pos * size) + new Vector2(CellWidth, 0) + new Vector2(CellWidth / 2, CellWidth / 2) + offset,
				pos.Y.ToString(),
				modulate: Colors.Red
			);
		if (pos.Y == 7)
			DrawString(
				new SystemFont(),
				(pos * size) + new Vector2(0, CellWidth) + new Vector2(CellWidth / 2, CellWidth / 2) + offset,
				char.ConvertFromUtf32('A' + pos.X),
				modulate: Colors.Blue
			);
	}

	private void DrawInformation(Vector2 size, Vector2 offset)
	{
		var pos = new Vector2(9, 0);
		var infoLabel = GetNode("../Information") as Label;
		infoLabel.Position = (pos * size) + new Vector2(CellWidth, 0) +
		                     new Vector2(CellWidth / 2, CellWidth / 2) + offset;

		infoLabel.Text = $"WhiteToMove: {_board.WhiteToMove}\n" +
		                 $"Selected Cell: {SelectedCell}\n";
	}

	private void DrawPiece(Vector2I pos, Vector2 size, Piece piece, Vector2 offset = new())
	{
		var o = "";
		var color = piece.HasFlag(Piece.White) ? Colors.White : Colors.Black;

		if (piece.HasFlag(Piece.Pawn)) o = "P";
		else if (piece.HasFlag(Piece.Bishop)) o = "B";
		else if (piece.HasFlag(Piece.Knight)) o = "N";
		else if (piece.HasFlag(Piece.Rook)) o = "R";
		else if (piece.HasFlag(Piece.Queen)) o = "Q";
		else if (piece.HasFlag(Piece.King)) o = "K";

		DrawString(new SystemFont(), (pos * size) + (size / new Vector2(2, 2) + offset), o, modulate: color);
	}
}
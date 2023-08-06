using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Chessagon.Common;
using Chessagon.UI.Pieces;


namespace Chessagon.UI;

public partial class SquareBoard : Node2D
{
	[Export] public float CellWidth = 100;
	[Export] public bool WhiteBottom = true;
	[Export] public Color DarkColor = Colors.SlateBlue;
	[Export] public Color LightColor = Colors.LightGray;
	[Export] public Color SelectedCellColor = new(Colors.Black, 0.3f);
	[Export] public Color HighlightedCellColor = new(Colors.Red);
	[Export] public Color AttackedCellColor = new(Colors.Yellow);

	// this will be relative to the node position making it basically useless
	private Vector2 boardPositionOffset = new Vector2(0, 0);

	private List<int> selectedPieceMoves = new List<int>();
	private IEnumerable<int> attackedCells = new List<int>();
	private int _selectedCell = -1;

	public int SelectedCell
	{
		get => _selectedCell;
		set
		{
			if (_selectedCell == value || value > 64)
			{
				_selectedCell = -1;
				return;
			}

			_selectedCell = value;
			selectedPieceMoves.Clear();
			selectedPieceMoves.AddRange(_board.GetMoves().Where(x => x.From == _selectedCell).Select(x => x.To));

			attackedCells = _board.AttackedIndexes();

			// render the new selected cell
			QueueRedraw();
		}
	}

	private ChessBoard _board;

	private IEnumerable<ChessPiece> _pieces => GetChildren().Where(x => x is ChessPiece).Cast<ChessPiece>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ReSharper disable StringLiteralTypo
		// _board = ChessBoard.FromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
		_board = ChessBoard.FromFen("rnbq1bnr/pp1kpppp/2p5/3p4/2QPP3/8/PPP2PPP/RNB1KBNR w KQkq - 0 1");
		
		// empty 
		// _board = ChessBoard.FromFen("8/8/8/8/8/8/8/8 w KQkq - 0 1");


		for (int i = 0; i < _board.Length; i++)
		{
			if (_board[i] == 0) continue;
			var p = new ChessPiece();
			p.AddChild(GetSprite(_board[i]));
			p.MoveTo(i);
			AddChild(p);
		}

		_board.PieceMove += OnPieceMove;

		// var pawn = new Pawn() { CellSize = CellWidth };
		// pawn.MoveTo(18);
		// pawn.Scale = new Vector2(4, 4);
		// AddChild(pawn);
	}

	private void OnPieceMove(object sender, PieceMoveEventArgs e)
	{
		var sourcePiece = _pieces.First(x => x.Index == e.Move.From);
		var destPiece = _pieces.FirstOrDefault(x => x.Index == e.Move.To);

		destPiece?.QueueFree();
		sourcePiece.MoveTo(e.Move.To);
	}

	private Sprite2D GetSprite(Piece piece)
	{
		var textures = LoadTextures();
		var res = GetResourcePath(piece);

		var t = textures.First(x => x.ResourcePath.Equals(res));
		var sprite = new Sprite2D
		{
			Centered = false,
			Texture = t,
			Scale = new Vector2(CellWidth / t.GetWidth(), CellWidth / t.GetWidth()),
			TextureFilter = TextureFilterEnum.Linear
		};

		return sprite;
	}

	private static Vector2I IndexToVector(int index)
	{
		return new Vector2I(index % 8, index / 8);
	}

	private static int VectorToIndex(Vector2I pos)
	{
		return (Math.Clamp(pos.Y, 0, 7) * 8) + Math.Clamp(pos.X, 0, 7);
	}

	private Vector2 BoardIndexToPosition(int index)
	{
		return IndexToVector(index) * new Vector2(CellWidth, CellWidth);
	}

	#region Input Handling

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
		if (SelectedCell == -1 || newSelection == -1)
		{
			SelectedCell = newSelection;
			return;
		}

		if (_board[SelectedCell] != 0)
		{
			// button was released
			if (_board.Move(new Move(SelectedCell, newSelection)))
				SelectedCell = -1;
			else
				SelectedCell = newSelection;
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
		var x = (int)correctedMouse.X;
		var y = (int)correctedMouse.Y;
		if (x > 7 || y > 7) return -1;
		return (y * 8) + x;
	}

	#endregion

	#region Rendering

	public override void _Draw()
	{
		var size = new Vector2(CellWidth, CellWidth);
		DrawBoard(size);
		for (var i = 0; i < 64; i++)
			DrawPiece(IndexToVector(i), size, _board[i], boardPositionOffset);
		DrawHighlights(selectedPieceMoves, size, boardPositionOffset, new Color(Colors.Red, 0.2f));
		DrawHighlights(attackedCells, size, boardPositionOffset, new Color(Colors.Yellow, 0.2f));
		DrawInformation(size, boardPositionOffset);
	}

	private void DrawBoard(Vector2 size)
	{
		for (var i = 0; i < 64; i++)
		{
			var pos = IndexToVector(i);

			var color = (pos.X + pos.Y) % 2 == 0 ? LightColor : DarkColor;

			if (attackedCells.Contains(i)) color = AttackedCellColor;
			if (selectedPieceMoves.Contains(i)) color = HighlightedCellColor;

			DrawRect(
				new Rect2((pos * size) + boardPositionOffset, size),
				color
			);

			if (i == SelectedCell)
				DrawRect(
					new Rect2((pos * size) + boardPositionOffset, size), SelectedCellColor
				);

			DrawLegends(pos, size, boardPositionOffset);
		}
	}

	private void DrawHighlights(IEnumerable<int> highlights, Vector2 cellSize, Vector2 offset, Color color)
	{
		foreach (var h in highlights)
		{
			DrawRect(new Rect2(IndexToVector(h) * cellSize + offset, cellSize), color);
		}
	}

	private void DrawLegends(Vector2I pos, Vector2 size, Vector2 offset)
	{
		DrawString(
			new SystemFont(),
			(pos * size) + new Vector2(0, new SystemFont().GetHeight()) + offset,
			VectorToIndex(pos).ToString(),
			modulate: Colors.Brown
		);
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
		if (GetNode("../Information") is not Label infoLabel) return;

		infoLabel.Position = (pos * size) + new Vector2(CellWidth, 0) +
							 new Vector2(CellWidth / 2, CellWidth / 2) + offset;

		infoLabel.Text = $"WhiteToMove: {_board.WhiteToMove}\n" +
						 $"Selected Cell: {SelectedCell}\n" +
						 $"Highlighted Cells: {string.Join(", ", selectedPieceMoves)}\n" +
						 $"Attacked Cells: {string.Join(", ", attackedCells)}\n" +
						 $"FEN: {_board.ToFen()}";
	}

	private static IEnumerable<Texture2D> LoadTextures()
	{
		var list = new List<Texture2D>();

		foreach (var color in new[] { Piece.White, Piece.Black })
			list.AddRange(
				new[] { Piece.Pawn, Piece.Rook, Piece.Bishop, Piece.King, Piece.Knight, Piece.Queen }
					.Select(piece => GetResourcePath(piece | color))
					.Select(res => ResourceLoader.Load(res) as CompressedTexture2D)
			);

		return list;
	}

	private void DrawPiece(Vector2I pos, Vector2 size, Piece piece, Vector2 offset = new())
	{
		if (piece == 0) return;

		var o = piece.ToChar().ToString();
		var color = piece.HasFlag(Piece.White) ? Colors.White : Colors.Black;

		DrawString(new SystemFont(), (pos * size) + offset + new Vector2(0, size.Y), o, modulate: color);
	}

	private static string GetResourcePath(Piece piece)
	{
		var res = "res://Resources/Pieces/";
		res += piece.HasFlag(Piece.White) ? "White_" : "Black_";
		res += char.ToUpper(piece.ToChar());

		return ResourceLoader.Exists($"{res}.svg") ? $"{res}.svg" : $"{res}.png";
	}

	#endregion
}

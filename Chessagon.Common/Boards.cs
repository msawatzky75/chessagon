namespace Chessagon.Common;

[Flags]
public enum Piece
{
	Pawn = 1,
	Bishop = 2,
	Knight = 4,
	Rook = 8,
	Queen = 16,
	King = 32,

	Black = 64,
	White = 128,
}

[Flags]
public enum CastleStatus
{
	Black = 0,
	White = 1,
	Queen = 2,
	King = 4,
}

public class Move
{
	public int From;
	public int To;

	public Move(int from, int to)
	{
		From = from;
		To = to;
	}
}
public class PieceUpdateEventArgs : EventArgs
{
	public Piece Value { get; set; }
	public int Position;
	public int OldPosition;
}
public class PieceMoveEventArgs : EventArgs
{
	public Piece Piece { get; set; }
	public Move Move { get; set; }

	public PieceMoveEventArgs(Move move, Piece piece)
	{
		Move = move;
		Piece = piece;
	}
}

public class ChessBoard
{
	public event EventHandler<PieceUpdateEventArgs> PieceUpdate;
	public event EventHandler<PieceMoveEventArgs> PieceMove;

	protected Piece[] Pieces = new Piece[64];
	public CastleStatus CastleStatus { get; private set; }

	public bool WhiteToMove { get; private set; } = true;

	public int Length => Pieces.Length;

	public Piece this[int index]
	{
		get => Pieces[index];
		private set
		{
			Pieces[index] = value;
			PieceUpdate?.Invoke(this, new() { Value = value, Position = index });
		}
	}

	private ChessBoard()
	{
		for (var i = 0; i < Pieces.Length; i++)
		{
			this[i] = 0;
		}

		CastleStatus = CastleStatus.Black & CastleStatus.White & CastleStatus.King & CastleStatus.Queen;
	}

	public bool Move(Move move)
	{
		bool valid = true;

		if (this[move.From] == 0) return false;
		if ((this[move.From] & this[move.To]).HasFlag(Piece.Black)) return false;
		if ((this[move.From] & this[move.To]).HasFlag(Piece.White)) return false;

		this[move.To] = this[move.From];
		this[move.From] = 0;
		PieceMove?.Invoke(this, new PieceMoveEventArgs(move, this[move.To]));
		
		if (valid) WhiteToMove = !WhiteToMove;
		return valid;
	}

	/// <summary>
	/// Creates a new ChessBoard from a FEN string
	/// </summary>
	/// <param name="fen">FEN string to turn into a Chessboard</param>
	/// <returns>A ChessBoard</returns>
	public static ChessBoard FromFEN(string fen)
	{
		// rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
		var board = new ChessBoard();
		var fenSections = fen.Split(' ');
		board.WhiteToMove = fenSections[1].Equals("w");

		var ranks = fenSections[0].Split('/');
		int rank = 0;
		int file = 0;
		foreach (string r in ranks)
		{
			foreach (char f in r)
			{
				if (char.IsLetter(f))
				{
					board[(rank * 8) + file] = CharToPiece(f.ToString());
					file++;
				}
				else if (int.TryParse(f.ToString(), out var emptySpace)) file += emptySpace;
			}

			rank++;
			file = 0;
		}

		return board;
	}

	private static Piece CharToPiece(string c)
	{
		var color = c.ToUpper().Equals(c) ? Piece.Black : Piece.White;

		return c.ToLower() switch
		{
			"p" => Piece.Pawn | color,
			"r" => Piece.Rook | color,
			"n" => Piece.Knight | color,
			"b" => Piece.Bishop | color,
			"q" => Piece.Queen | color,
			"k" => Piece.King | color,
			_ => 0
		};
	}
}
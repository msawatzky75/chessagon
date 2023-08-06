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

public static class PieceExtensions
{
	private const Piece ColorMask = Piece.Black | Piece.White;

	public static bool HasColor(this Piece self, Piece other)
	{
		return (self & ColorMask) == (other & ColorMask);
	}

	public static Piece CharToPiece(char c)
	{
		var color = char.ToLower(c).Equals(c) ? Piece.Black : Piece.White;

		return c.ToString().ToLower() switch
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

	public static char ToChar(this Piece piece)
	{
		switch (piece)
		{
			case 0:
				throw new Exception("Piece must not be 0");
			case Piece.Black or Piece.White:
				throw new Exception("Piece must not be only a color");
		}

		char val = 'P';
		if (piece.HasFlag(Piece.Bishop)) val = 'B';
		if (piece.HasFlag(Piece.Knight)) val = 'N';
		if (piece.HasFlag(Piece.Rook)) val = 'R';
		if (piece.HasFlag(Piece.Queen)) val = 'Q';
		if (piece.HasFlag(Piece.King)) val = 'K';

		return piece.HasColor(Piece.Black) ? char.ToLower(val) : val;
	}

	public static Piece OppositeColor(this Piece piece)
	{
		if (piece.HasFlag(Piece.Black)) return Piece.White;
		if (piece.HasFlag(Piece.White)) return Piece.Black;

		throw new Exception("Piece has no color.");
	}

	public static bool IsSliding(this Piece piece)
	{
		return piece.HasFlag(Piece.Rook) || piece.HasFlag(Piece.Bishop) || piece.HasFlag(Piece.Queen);
	}
}

[Flags]
public enum CastleStatus
{
	Black = 0,
	White = 1,
	Queen = 2,
	King = 4,
}
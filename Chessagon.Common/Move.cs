namespace Chessagon.Common;

public readonly struct Move : IEquatable<Move>
{
	public readonly int From;
	public readonly int To;
	public readonly Piece Captured;
	public readonly bool Castle;
	
	public Move(int from, int to, bool castle = false, Piece captured = 0)
	{
		From = from;
		To = to;
		Castle = castle;
		Captured = captured;
	}


	public bool Equals(Move other)
	{
		return From == other.From && To == other.To && Castle == other.Castle;
	}
}
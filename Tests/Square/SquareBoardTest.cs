using Chessagon.Common;

namespace Tests.Square;

public class SquareBoardTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void TestWhiteBishop_HasWhitePawn()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.White | Piece.Pawn;

		Assert.That(test1.HasColor(test2));
	}

}
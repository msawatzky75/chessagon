using Chessagon.Common;

namespace Tests;

public class TestHasColor
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
	[Test]
	public void TestWhiteBishop_HasWhiteBishop()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.White | Piece.Bishop;
		
		Assert.That(test1.HasColor(test2));
	}
	[Test]
	public void TestWhiteBishop_HasBlackPawn()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.Black | Piece.Pawn;
		
		Assert.That(test1.HasColor(test2), Is.False);
	}
	[Test]
	public void TestWhiteBishop_HasBlackBishop()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.Black | Piece.Bishop;
		
		Assert.That(test1.HasColor(test2), Is.False);
	}
	
	[Test]
	public void TestWhiteBishop_HasBlack()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.Black;
		
		Assert.That(test1.HasColor(test2), Is.False);
	}
	
	[Test]
	public void TestWhiteBishop_HasWhite()
	{
		var test1 = Piece.White | Piece.Bishop;
		var test2 = Piece.White;
		
		Assert.That(test1.HasColor(test2));
	}
	
	[Test]
	public void TestWhiteBishop_HasNothing()
	{
		var test1 = Piece.White | Piece.Bishop;
		Piece test2 = 0;
		
		Assert.That(test1.HasColor(test2), Is.False);
	}
}
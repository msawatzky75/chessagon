using Chessagon.Common;

namespace Tests.Square;

public class Directions
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Test_None()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 16), Is.EqualTo(Direction.None));
			Assert.That(ChessBoard.GetDirectionTo(35, 25), Is.EqualTo(Direction.None));
			Assert.That(ChessBoard.GetDirectionTo(35, 29), Is.EqualTo(Direction.None));
			Assert.That(ChessBoard.GetDirectionTo(35, 22), Is.EqualTo(Direction.None));
			Assert.That(ChessBoard.GetDirectionTo(46, 61), Is.EqualTo(Direction.None));
			Assert.That(ChessBoard.GetDirectionTo(56, 55), Is.EqualTo(Direction.None));
		});
	}

	[Test]
	public void Test_NorthWest()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 17), Is.EqualTo(Direction.NorthWest));
			Assert.That(ChessBoard.GetDirectionTo(35, 26), Is.EqualTo(Direction.NorthWest));
			Assert.That(ChessBoard.GetDirectionTo(35, 8),  Is.EqualTo(Direction.NorthWest));
			Assert.That(ChessBoard.GetDirectionTo(46, 28), Is.EqualTo(Direction.NorthWest));
		});
	}

	[Test]
	public void Test_North()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 19), Is.EqualTo(Direction.North));
			Assert.That(ChessBoard.GetDirectionTo(35, 27), Is.EqualTo(Direction.North));
			Assert.That(ChessBoard.GetDirectionTo(35, 11), Is.EqualTo(Direction.North));
			Assert.That(ChessBoard.GetDirectionTo(46, 30), Is.EqualTo(Direction.North));
		});
	}

	[Test]
	public void Test_NorthEast()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 28), Is.EqualTo(Direction.NorthEast));
			Assert.That(ChessBoard.GetDirectionTo(35, 21), Is.EqualTo(Direction.NorthEast));
			Assert.That(ChessBoard.GetDirectionTo(35, 7),  Is.EqualTo(Direction.NorthEast));
			Assert.That(ChessBoard.GetDirectionTo(46, 39), Is.EqualTo(Direction.NorthEast));
		});
	}

	[Test]
	public void Test_East()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 36), Is.EqualTo(Direction.East));
			Assert.That(ChessBoard.GetDirectionTo(35, 37), Is.EqualTo(Direction.East));
			Assert.That(ChessBoard.GetDirectionTo(35, 39), Is.EqualTo(Direction.East));
			Assert.That(ChessBoard.GetDirectionTo(46, 47), Is.EqualTo(Direction.East));
		});
	}

	[Test]
	public void Test_SouthEast()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 44), Is.EqualTo(Direction.SouthEast));
			Assert.That(ChessBoard.GetDirectionTo(35, 53), Is.EqualTo(Direction.SouthEast));
			Assert.That(ChessBoard.GetDirectionTo(35, 62), Is.EqualTo(Direction.SouthEast));
			Assert.That(ChessBoard.GetDirectionTo(46, 55), Is.EqualTo(Direction.SouthEast));
		});
	}

	[Test]
	public void Test_South()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 43), Is.EqualTo(Direction.South));
			Assert.That(ChessBoard.GetDirectionTo(35, 51), Is.EqualTo(Direction.South));
			Assert.That(ChessBoard.GetDirectionTo(35, 59), Is.EqualTo(Direction.South));
			Assert.That(ChessBoard.GetDirectionTo(46, 54), Is.EqualTo(Direction.South));
		});
	}

	[Test]
	public void Test_SouthWest()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 42), Is.EqualTo(Direction.SouthWest));
			Assert.That(ChessBoard.GetDirectionTo(35, 49), Is.EqualTo(Direction.SouthWest));
			Assert.That(ChessBoard.GetDirectionTo(35, 56), Is.EqualTo(Direction.SouthWest));
			Assert.That(ChessBoard.GetDirectionTo(46, 53), Is.EqualTo(Direction.SouthWest));
		});
	}

	[Test]
	public void Test_West()
	{
		Assert.Multiple(() =>
		{
			Assert.That(ChessBoard.GetDirectionTo(35, 34), Is.EqualTo(Direction.West));
			Assert.That(ChessBoard.GetDirectionTo(35, 33), Is.EqualTo(Direction.West));
			Assert.That(ChessBoard.GetDirectionTo(35, 32), Is.EqualTo(Direction.West));
			Assert.That(ChessBoard.GetDirectionTo(46, 44), Is.EqualTo(Direction.West));
		});
	}
}

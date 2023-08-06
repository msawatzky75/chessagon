using Chessagon.Common;

namespace Tests.Square;

public class NumSquaresToEdgeTest
{
	private int[] result;

	[SetUp]
	public void Setup()
	{
		result = ChessBoard.NumSquaresToEdge(35);
	}


	[Test]
	public void Test_35NorthWest()
	{
		Assert.That(result[0], Is.EqualTo(3));
	}

	[Test]
	public void Test_35North()
	{
		Assert.That(result[1], Is.EqualTo(4));
	}

	[Test]
	public void Test_35NorthEast()
	{
		Assert.That(result[2], Is.EqualTo(4));
	}

	[Test]
	public void Test_35East()
	{
		Assert.That(result[3], Is.EqualTo(4));
	}

	[Test]
	public void Test_35SouthEast()
	{
		Assert.That(result[4], Is.EqualTo(3));
	}

	[Test]
	public void Test_35South()
	{
		Assert.That(result[5], Is.EqualTo(3));
	}

	[Test]
	public void Test_35SouthWest()
	{
		Assert.That(result[6], Is.EqualTo(3));
	}

	[Test]
	public void Test_35West()
	{
		Assert.That(result[7], Is.EqualTo(3));
	}
}
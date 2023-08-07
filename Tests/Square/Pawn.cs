using Chessagon.Common;

namespace Tests.Square;

public class Pawn
{
	private ChessBoard _board;

	[SetUp]
	public void Setup()
	{
		_board = ChessBoard.FromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
	}

	[Test]
	public void Test_MoveForwardOne()
	{
		Assert.That(_board.Move(new Move(9, 17)));
	}

	[Test]
	public void Test_MoveForwardTwo()
	{
		Assert.That(_board.Move(new Move(9, 25)));
	}

	[Test]
	public void Test_EnPassant()
	{
		var board = ChessBoard.FromFen("rnbq1bnr/p2kpppp/2p5/1p1p3P/2QPP3/8/PPP2PP1/RNB1KBNR b KQ - 0 1");
		Assert.Multiple(() =>
		{
			Assert.That(board.Move(new Move(54, 38)));
			Assert.That(board.Move(new Move(39, 46)));
			Assert.That((int)board[38], Is.EqualTo(0));
		});
	}

	[Test]
	public void Test_EnPassantFromFen()
	{
		var board = ChessBoard.FromFen("rnbq1bnr/p2kpp1p/2p5/1p1p2pP/2QPP3/8/PPP2PP1/RNB1KBNR w KQ g6 0 1");
		// index of g6 is 46
		Assert.Multiple(() =>
		{
			Assert.That(board.Move(new Move(39, 46)));
			Assert.That((int)board[38], Is.EqualTo(0));
		});
	}
}
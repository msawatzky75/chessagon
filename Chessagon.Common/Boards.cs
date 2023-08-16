namespace Chessagon.Common;

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

public enum Direction
{
	// Sum of North and West
	NorthWest = -9,
	North = -8,
	// Sum of North and East
	NorthEast = -7,
	East = 1,
	// Sum of South and East
	SouthEast = 9,
	South = 8,
	// Sum of South and West
	SouthWest = 7,
	West = -1,

	None = 0,
}

public class ChessBoard
{
	public event EventHandler<PieceMoveEventArgs>? PieceMove;

	/// <summary>
	/// Direction offsets starting northwest going clockwise
	/// </summary>
	private static readonly Direction[] DirectionOffsets =
	{
		Direction.NorthWest,
		Direction.North,
		Direction.NorthEast,
		Direction.East,
		Direction.SouthEast,
		Direction.South,
		Direction.SouthWest,
		Direction.West,
	};


	private readonly Piece[] _pieces = new Piece[64];
	private IEnumerable<int> _attacked;
	public CastleStatus CastleStatus { get; private set; }

	public bool WhiteToMove { get; private set; } = true;
	public int Length => _pieces.Length;

	public Piece this[int index]
	{
		get => _pieces[index];
		private set => _pieces[index] = value;
	}

	private ChessBoard()
	{
		for (var i = 0; i < _pieces.Length; i++)
		{
			this[i] = 0;
		}

		_attacked = new int[] { };
		CastleStatus = CastleStatus.Black & CastleStatus.White & CastleStatus.King & CastleStatus.Queen;
	}

	public bool Move(Move move)
	{
		if (!GetMoves().Contains(move)) return false;

		// perform the move
		this[move.To] = this[move.From];
		this[move.From] = 0;
		PieceMove?.Invoke(this, new PieceMoveEventArgs(move, this[move.To]));
		WhiteToMove = !WhiteToMove;
		_attacked = AttackedIndexes();
		return true;
	}

	/// <summary>
	/// Returns a list of indexes of attacked pieces for the current board state.
	/// By default only returns a list of attacks made by the opponent.
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	public IEnumerable<int> AttackedIndexes(Piece color = 0)
	{
		Piece colorAttacking = color is not 0 ? color : (WhiteToMove ? Piece.Black : Piece.White);
		return GetMoves(colorAttacking, attacking: true)
			.Where(m => !m.Castle)
			.Select(m => m.To);
		// .Where(target => !_pieces[target].HasColor(colorAttacking));
	}

	public IEnumerable<Move> GetMoves(Piece color = 0, bool attacking = false)
	{
		var colorFilter = color;
		if (colorFilter == 0) colorFilter = WhiteToMove ? Piece.White : Piece.Black;

		if (!attacking)
			_attacked = AttackedIndexes().ToList();

		IEnumerable<Move> opponentMoves = new List<Move>();
		if (!attacking)
			opponentMoves = GetMoves(color: colorFilter.OppositeColor(), attacking: true);

		var moves = new List<Move>();
		for (var i = 0; i < _pieces.Length; i++)
		{
			var piece = _pieces[i];
			if (!piece.HasColor(colorFilter)) continue;

			if (piece.HasFlag(Piece.Pawn)) moves.AddRange(GeneratePawnMoves(piece, i, attacking));
			else if (piece.IsSliding()) moves.AddRange(GenerateSlidingMoves(piece, i));
			else if (piece.HasFlag(Piece.Knight)) moves.AddRange(GetKnightMoves(piece, i));
			else if (piece.HasFlag(Piece.King)) moves.AddRange(GetKingMoves(piece, i));
		}

		var myKing = _pieces
			.Select((piece, index) => (piece, index))
			.Where(p => p.piece == (Piece.King | colorFilter))
			.Select(p => p.index).FirstOrDefault(-1);

		var attackingKingMoves = opponentMoves.Where(m => m.To == myKing).ToList();

		// if king is not under threat, return moves
		if (!attackingKingMoves.Any()) return moves;

		List<Move> possibleMoves = new List<Move>();

		// Move the king out of the attacked cells
		possibleMoves.AddRange(moves
			// option to move the king to a cell that isn't attacked
			.Where(m => (m.From == myKing && !_attacked.Contains(m.To))));


		if (attackingKingMoves.Count() != 1) return possibleMoves;

		// or if the attacking piece is a sliding piece, move into it's attack vector.
		var attackingSource = attackingKingMoves.DistinctBy(m => m.From).Select(m => m.From).ToList();

		var attackingSlides = attackingSource.Where(s => _pieces[s].IsSliding()).ToList();

		// don't retreat back along a slide
		if (attackingSlides.Count > 0)
		{
			IEnumerable<Move> reducedPossibleMoves = possibleMoves;

			List<int> attackingSlideIndexes = new List<int>();
			foreach (var aS in attackingSlides)
			{
				var direction = GetDirectionTo(aS, myKing);

				for (int i = 1; Math.Abs((int)direction * i) + aS < _pieces.Length; i++)
				{
					var next = aS + ((int)direction * i);
					attackingSlideIndexes.Add(next);
				}
			}

			possibleMoves = possibleMoves.ExceptBy(
				attackingSlideIndexes, m => m.To
			).ToList();
		}

		// if the king is attacked by more than one piece, blocking an attack is insufficient
		if (attackingSource.Count > 1) return possibleMoves;

		// figure out what moves can be considered "blocking" moves
		var attackingPiece = attackingSource.First();
		if (_pieces[attackingPiece].IsSliding())
		{
			var slidingAttackedIndexes = new List<int>();

			var direction = GetDirectionTo(myKing, attackingPiece);
			if (direction is Direction.None) return possibleMoves;

			for (int j = 1; myKing + ((int)direction * j) != attackingPiece && j < 8; j++)
			{
				slidingAttackedIndexes.Add(myKing + ((int)direction) * j);
			}

			// attacking the sliding piece will also prevent check
			slidingAttackedIndexes.Add(attackingPiece);

			possibleMoves.AddRange(moves.Where(m => slidingAttackedIndexes.Contains(m.To)));
		}

		if (!possibleMoves.Any() && !attacking) throw new Exception("Checkmate");
		return possibleMoves;
	}

	public static Direction GetDirectionTo(int source, int dest)
	{
		int sourceFile = GetFile(source);
		int sourceRank = GetRank(source);
		int destFile = GetFile(dest);
		int destRank = GetRank(dest);

		var vertical = sourceRank - destRank > 0 ? Direction.North : Direction.South;
		var horizontal = sourceFile - destFile > 0 ? Direction.West : Direction.East;

		if (sourceFile == destFile)
			return vertical;
		if (sourceRank == destRank)
			return horizontal;
		if (Math.Abs(sourceFile - destFile) == Math.Abs(sourceRank - destRank))
			// Adding horizontal and vertical directions _does_ result in a valid angled direction
			return (Direction)((int)vertical + (int)horizontal);

		return Direction.None;
	}

	private IEnumerable<Move> GetKingMoves(Piece piece, int index)
	{
		var moves = new List<Move>();

		for (int direction = 0; direction < 8; direction++)
		{
			if (NumSquaresToEdge(index)[direction] <= 0) continue;
			var target = index + (int)DirectionOffsets[direction];

			// don't attack friendlies
			if (_pieces[target].HasColor(piece)) continue;
			if (_attacked.Contains(target)) continue;

			moves.Add(new Move(index, target));
		}

		return moves;
	}

	private IEnumerable<Move> GetKnightMoves(Piece piece, int index)
	{
		int[] offsets = { -17, -10, 17, 10, -15, -6, 15, 6 };

		return offsets
			.Select(offset => index + offset)
			.Where(target => target is >= 0 and < 64)
			.Where(target => Math.Abs(GetRank(index) - GetRank(target)) + Math.Abs(GetFile(index) - GetFile(target)) == 3)
			.Where(target => !_pieces[target].HasColor(piece))
			.Select(target => new Move(index, target));
	}

	private IEnumerable<Move> GenerateSlidingMoves(Piece piece, int index)
	{
		// offsets are {startDirection, numberOfDirectionsToSkip}
		// queen
		int[] offset = { 0, 1 };
		if (piece.HasFlag(Piece.Bishop)) offset = new[] { 0, 2 };
		if (piece.HasFlag(Piece.Rook)) offset = new[] { 1, 2 };

		var moves = new List<Move>();

		for (int direction = offset[0]; direction < 8; direction += offset[1])
		{
			var blocked = 0;
			for (int o = 0; o < NumSquaresToEdge(index)[direction]; o++)
			{
				var target = index + (int)DirectionOffsets[direction] * (o + 1);

				// don't attack friendlies
				if (_pieces[target].HasColor(piece)) break;

				moves.Add(new Move(index, target));

				// we want all attacking moves where the king is in danger, so don't stop searching when finding an attackable piece
				// if (attacking && blocked < 1)
				// {
				// 	blocked = 0;
				// 	continue;
				// }

				// add the last move as an attack and stop adding moves on this slide
				if (_pieces[target] != 0) break;
			}
		}

		return moves;
	}

	/// <summary>
	/// Gets all valid moves a pawn can make.
	/// </summary>
	/// <param name="piece">Color (and type) of piece to generate pawn moves for.</param>
	/// <param name="index">Index of pawn piece.</param>
	/// <param name="attacking">Returns only attacking moves when true.</param>
	/// <returns>All legal moves a pawn can make.</returns>
	private IEnumerable<Move> GeneratePawnMoves(Piece piece, int index, bool attacking)
	{
		var moves = new List<Move>();

		if (piece == 0) return moves;

		var rank = GetRank(index);
		var file = GetFile(index);
		// negative is down
		var direction = piece.HasFlag(Piece.White) ? 1 : -1;
		if (rank is 0 or 7) return moves;

		moves.AddRange(
			new[] { -1, 1 }
				.Where(f => file + f is >= 0 and < 8)
				.Select(f => GetIndex(rank + (1 * direction), file + f))
				.Where(target => _pieces[target] != 0 || attacking)
				.Where(target => !_pieces[target].HasColor(piece) || attacking)
				.Select(target => new Move(index, target))
		);

		// moves following this are not attacking moves
		// TODO: en passant
		if (attacking) return moves;

		var rankMoves = 1;
		// if the pawn is in it's starting rank, it can move forward 2
		if (rank == 6 && piece.HasFlag(Piece.Black) || rank == 1 && piece.HasFlag(Piece.White))
			rankMoves = 2;

		for (int step = 1; step <= rankMoves; step++)
		{
			var target = GetIndex(rank + (step * direction), file);

			if (_pieces[target] == 0)
				moves.Add(new Move(index, target));
		}

		return moves;
	}

	private static int GetRank(int index)
	{
		return index / 8;
	}

	private static int GetFile(int index)
	{
		return index % 8;
	}

	private static int GetIndex(int rank, int file)
	{
		if (rank > 7) throw new Exception("Rank cannot be greater than 7");
		if (file > 7) throw new Exception("File cannot be greater than 7");
		return (rank * 8) + file;
	}

	public static int[] NumSquaresToEdge(int index)
	{
		var rank = GetRank(index);
		var file = GetFile(index);

		var north = rank;
		var south = 7 - north;
		var west = file;
		var east = 7 - west;

		// one count for each direction, starting northwest
		return new[]
		{
			Math.Min(north, west),
			north,
			Math.Min(north, east),
			east,
			Math.Min(south, east),
			south,
			Math.Min(south, west),
			west,
		};
	}

	/// <summary>
	/// Creates a new ChessBoard from a FEN string
	/// </summary>
	/// <param name="fen">FEN string to turn into a Chessboard</param>
	/// <returns>A ChessBoard</returns>
	public static ChessBoard FromFen(string fen)
	{
		// ReSharper disable CommentTypo
		// rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
		var board = new ChessBoard();
		var fenSections = fen.Split(' ');
		board.WhiteToMove = fenSections[1].Equals("w");

		var ranks = fenSections[0].Split('/');
		int rank = 7;
		int file = 0;
		foreach (string r in ranks)
		{
			foreach (char f in r)
			{
				if (char.IsLetter(f))
				{
					board[(rank * 8) + file] = PieceExtensions.CharToPiece(f);
					file++;
				}
				else if (int.TryParse(f.ToString(), out var emptySpace)) file += emptySpace;
			}

			rank--;
			file = 0;
		}

		return board;
	}

	public string ToFen()
	{
		var fen = "";
		for (int rank = 7; rank >= 0; rank--)
		{
			int empty = 0;
			for (int file = 0; file < 8; file++)
			{
				var piece = _pieces[GetIndex(rank, file)];
				if (piece == 0)
				{
					empty++;
					continue;
				}

				if (empty > 0)
				{
					fen += empty.ToString();
					empty = 0;
				}

				fen += piece.ToChar();
			}

			if (empty > 0)
			{
				fen += empty.ToString();
			}

			if (rank > 0)
				fen += "/";
		}

		// TODO: add castle status and en passant to fen string

		return fen;
	}
}

public static class MoveFactory
{
	public static List<Move> GetLegal(ChessBoard board)
	{
		throw new Exception("Not implemented");
	}

	public static List<Move> GetAttacks(ChessBoard board)
	{
		throw new Exception("Not implemented");
	}
}
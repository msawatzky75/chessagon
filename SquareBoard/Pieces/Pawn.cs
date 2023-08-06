using System;
using System.Linq;
using Godot;

namespace Chessagon.UI.Pieces;

public partial class Pawn : Node2D
{
	public int Index { get; private set; } = -1;

	public float CellSize { get; set; }

	public void MoveTo(int index)
	{
		if (Index == index) throw new Exception("Cannot move to same position");
		Index = index;

		Position = new Vector2()
		{
			X = (CellSize) * (index % 8),
			Y = (CellSize) * (index / 8)
		};

		QueueRedraw();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Draw()
	{
		var pawnOffset = new Vector2(CellSize / 2, CellSize / 2);
		var pawnScale = new Vector2(CellSize / 2, CellSize / 2);
		var head = GetPointyHex( pawnOffset, pawnScale, true );
		var body = GetBody(pawnOffset + new Vector2(0, CellSize / 16), pawnScale, true);
		var pBase = GetBase(pawnOffset + new Vector2(0, CellSize / 8), pawnScale, true);
		DrawColoredPolygon(head, Colors.Aqua);
		DrawColoredPolygon(body, Colors.Blue);
		DrawColoredPolygon(pBase, Colors.Aqua);
	}

	private static Vector2[] GetFlatHex(Vector2 offset, Vector2 scale)
	{
		float height = (float)(Math.Sqrt(3) / 2d);
		float width = 1;

		var hex = new Vector2[]
		{
			new(0, height / 2),
			new(width / 4, height),
			new(width * 3 / 4, height),
			new(width, height / 2),
			new(width * 3 / 4, 0),
			new(width / 4, 0),
		};

		return hex.Select(p => p * scale).Select(p => p + offset).ToArray();
	}

	private static Vector2[] GetPointyHex(Vector2 offset, Vector2 scale, bool centered)
	{
		float height = 1;
		float width = (float)(Math.Sqrt(3) / 2d);

		var centerOffset = centered ? new Vector2(-width / 2, -height / 2) : new Vector2();
		return new Vector2[]
			{
				// top-left, counter-clockwise
				new(0, height * 1 / 4),
				new(0, height * 3 / 4),
				new(width / 2f, height),
				new(width, height * 3 / 4),
				new(width, height * 1 / 4),
				new(width / 2f, 0),
			}
			.Select(p => p + centerOffset)
			.Select(p => p * scale)
			.Select(p => p + offset).ToArray();
	}

	private static Vector2[] GetBody(Vector2 offset, Vector2 scale, bool centered)
	{
		float height = 1;
		float width = (float)(Math.Sqrt(3) / 2d);

		var centerOffset = centered ? new Vector2(-width / 2, -height / 2) : new Vector2();

		return new Vector2[]
			{
				// hex shaped neck
				new(width, height * 3 / 4),
				new(width / 2f, height),
				new(0, height * 3 / 4),

				// left side
				new(0 - width / 3, height - height / 8),
				new(0 - width / 3, height + height / 12),

				// point bottom
				new(width / 2f, height + height / 2),

				// right side
				new(width + width / 3, height + height / 12),
				new(width + width / 3, height - height / 8),
			}
			.Select(p => p + centerOffset)
			.Select(p => p * scale)
			.Select(p => p + offset).ToArray();
	}

	private static Vector2[] GetBase(Vector2 offset, Vector2 scale, bool centered)
	{
		float height = 1;
		float width = (float)(Math.Sqrt(3) / 2d);

		var centerOffset = centered ? new Vector2(-width / 2, -height / 2) : new Vector2();

		return new Vector2[]
			{
				// hex shaped neck
				new(width, height * 3 / 4),
				new(width / 2f, height),
				new(0, height * 3 / 4),

				// left side
				new(0 - width / 3, height - height / 8),
				new(0 - width / 3, height + height / 12),

				// point bottom
				new(width / 2f, height + height / 2),

				// right side
				new(width + width / 3, height + height / 12),
				new(width + width / 3, height - height / 8),
			}
			.Select(p => p + centerOffset)
			.Select(p => p * scale)
			.Select(p => p + offset).ToArray();
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
using Godot;
using System;
using System.Linq;

namespace Chessagon.UI;

public partial class ChessPiece : Node2D
{
	public Sprite2D Sprite
	{
		get
		{
			return GetChildren().First(x => x is Sprite2D) as Sprite2D;
		}
	}

	public float Width => Sprite.Texture.GetWidth() * Sprite.Scale.X;
	public float Height => Sprite.Texture.GetHeight() * Sprite.Scale.Y;

	public int Index { get; private set; } = -1;

	public void MoveTo(int index)
	{
		if (Index == index) throw new Exception("Cannot move to same position");
		Index = index;

		if (Sprite is null) return;
		Position = new Vector2()
		{
			X = (Width) * (index % 8),
			Y = (Height) * (index / 8)
		};
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
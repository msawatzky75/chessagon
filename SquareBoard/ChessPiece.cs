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

	private int _index = -1;

	public int Index => _index;

	public void MoveTo(int index)
	{
		if (_index == index) throw new Exception("Cannot move to same position");
		_index = index;

		if (Sprite is null) return;
		Position = new Vector2()
		{
			X = (Sprite.Texture.GetWidth() * Sprite.Scale.X) * (index % 8),
			Y = (Sprite.Texture.GetHeight() * Sprite.Scale.Y) * (index / 8)
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
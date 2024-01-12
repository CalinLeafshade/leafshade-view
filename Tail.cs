using Godot;
using Leafshade;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Tail : Node2D
{

	[Export]
	public SpriteSet SpriteSet {get; set;}

	private Polygon2D polygon;
	List<Bone2D> bones = new();
	List<Vector2> bonePositions = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		polygon = GetNode<Polygon2D>("Polygon2D");
		Skeleton2D skel = GetNode<Skeleton2D>("Skeleton2D");
		Bone2D bone = skel.GetChildren().OfType<Bone2D>().FirstOrDefault();
		
		while (bone != null) {
			bones.Add(bone);
			bone = bone.GetChildren().OfType<Bone2D>().FirstOrDefault();
		}
		//bones.Reverse();
		bonePositions = bones.Select(x => x.Position).ToList();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float t = Time.GetTicksMsec() / 1000f;

		for(var i = 0; i < bones.Count; i++) {
			var offset = Mathf.Sin(t + i) * (20f + i * 20f);
			//bones[i].Position = bonePositions[i] + new Vector2(offset, 0f);
			bones[i].RotationDegrees = Mathf.Sin(t + (i * 0.5f)) * (2f + i * 1.5f);
		}

		if (SpriteSet != null) {
			polygon.Modulate = new Color(1f,1f,1f,SpriteSet.TransitionAmount);
		}
	}
}

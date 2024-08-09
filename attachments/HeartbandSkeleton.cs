using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HeartbandSkeleton : Skeleton2D
{

	[Export]
	public Bone2D RootOne {get; set;}

	[Export]
	public Bone2D RootTwo {get; set;}

	List<Bone2D> bonesOne = new();
	List<Bone2D> bonesTwo = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildChain(RootOne, bonesOne);
		buildChain(RootTwo, bonesTwo);
	}

	public void buildChain(Bone2D root, List<Bone2D> target) {
		Bone2D bone = root;
		
		while (bone != null) {
			target.Add(bone);
			bone = bone.GetChildren().OfType<Bone2D>().FirstOrDefault();
		}
	}

	private void sway(double delta, List<Bone2D> bones, float speed, float offset, float amp) {
		float t = Time.GetTicksMsec() / 1000f;

		for(var i = 0; i < bones.Count; i++) {
			bones[i].RotationDegrees = Mathf.Sin(t * speed + (i * 0.5f) + offset) * (2f + i * amp);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		sway(delta, bonesOne, 1f, 0f, 0.5f);
		sway(delta, bonesTwo, 1.2f, 4f, 1f);
	}
}

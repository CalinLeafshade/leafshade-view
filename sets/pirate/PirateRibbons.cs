using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PirateRibbons : Skeleton2D
{
	[Export]
	public Bone2D RootOne {get; set;}

	[Export]
	public Bone2D RootTwo {get; set;}

	[Export]
	public Bone2D RootThree {get; set;}

	List<Bone2D> bonesOne = new();
	List<Bone2D> bonesTwo = new();

	List<Bone2D> bonesThree = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildChain(RootOne, bonesOne);
		buildChain(RootTwo, bonesTwo);
		buildChain(RootThree, bonesThree);
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
			float r = Mathf.Sin(t * speed + (i * 0.5f) + offset) * (2f + i * amp);
			float r2 = Mathf.Cos(t * speed * 1.5f + (i * 0.5f) + offset) * (2f + i * amp);
			bones[i].RotationDegrees = (r + r2) / 2f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		sway(delta, bonesOne, 1.3f, 0f, 4f);
		sway(delta, bonesTwo, 1.3f, 0f, 6f);
		sway(delta, bonesThree, 1.3f, 0f, 8f);
	}
}

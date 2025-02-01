using System;
using Godot;
using Template.Scripts.Animations;

public partial class Enemy : CharacterBody3D
{
	[Export]
	public Rig Rig { get; set; }

	public override void _Ready()
	{
		Random random = new Random();
		Rig.SetActiveMesh(Rig.VillagerMeshes[random.Next(Rig.VillagerMeshes.Length)]);
		
	}
	
	public override void _PhysicsProcess(double delta)
	{

	}
}

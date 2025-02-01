using Godot;

namespace Template.Scripts.Cameras;

public partial class SmoothCamera : SpringArm3D
{
	[Export] public Node3D Target { get; set; }
	[Export] public float Decay {get; set;}	
	
	public override void _PhysicsProcess(double delta)
	{
		GlobalTransform = GlobalTransform.InterpolateWith(Target.GlobalTransform, 1.0f - Mathf.Exp(-Decay * (float)delta));
	}
}
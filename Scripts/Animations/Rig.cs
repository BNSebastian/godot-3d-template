using Godot;

namespace Template.Scripts.Animations;

public partial class Rig : Node3D
{
    [Export] public AnimationTree AnimationTree { get; set; }
    [Export] public float AnimationSpeed { get; set; }
    [Export] public AnimationNodeStateMachinePlayback Playback { get; set; }
    [Export] public Skeleton3D Skeleton { get; set; }
    [Export] public MeshInstance3D[] VillagerMeshes { get; set; }
    
    [Signal]
    public delegate void HeavyAttackEventHandler();
    
    private float _runWeighTarget = -1.0f;
    private string _runPath = "parameters/MoveSpace/blend_position";

    public override void _Ready()
    {
        // Initialize the Playback property after the AnimationTree is assigned
        if (AnimationTree != null)
        {
            Playback = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
        }
        else
        {
            GD.PrintErr("AnimationTree is not assigned!");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Get the current blend position value and cast it to float
        float currentValue = (float)AnimationTree.Get(_runPath);

        // Smoothly interpolate toward the target value
        float animationValue = Mathf.MoveToward(currentValue, _runWeighTarget, (float)delta * AnimationSpeed);

        // Update the blend position in the AnimationTree
        AnimationTree.Set(_runPath, animationValue);
    }

    public void UpdateAnimationTree(Vector3 direction)
    {
        // Update the target blend position based on movement direction
        if (direction.IsZeroApprox())
        {
            _runWeighTarget = -1.0f;
        }
        else
        {
            _runWeighTarget = 1.0f;
        }
    }

    public void Travel(string animationName)
    {
        Playback.Travel(animationName);
    }

    public bool IsIdle()
    {
        // MoveSpace is the blend space in the animation tree
        return Playback.GetCurrentNode().Equals("MoveSpace");
    }
    
    public bool IsSlashing()
    {
        // MoveSpace is the blend space in the animation tree
        return Playback.GetCurrentNode().Equals("Slash");
    }

    public bool IsOverhead()
    {
        // MoveSpace is the blend space in the animation tree
        return Playback.GetCurrentNode().Equals("Overhead");
    }
    
    public void SetActiveMesh(MeshInstance3D activeMesh)
    {
        foreach (var child in Skeleton.GetChildren())
        {
            if (child is MeshInstance3D mesh)
            {
                mesh.SetVisible(false);
            }
        }
        activeMesh.SetVisible(true);
    }

    public void OnAnimationTreeAnimationFinished(string animationName)
    {
        if (animationName.Equals("Overhead"))
        {
            EmitSignal(nameof(HeavyAttack));
        }
    }
}
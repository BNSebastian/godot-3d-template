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
    
    public float RunWeighTarget { get; set; } = -1.0f;
    private string _runPath = "parameters/MoveSpace/blend_position";
    private Node3D _shieldSlot;
    private Node3D _weaponSlot;

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
        
        _shieldSlot = GetNode<Node3D>("%ShieldSlot");
        _weaponSlot = GetNode<Node3D>("%WeaponSlot");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Get the current blend position value and cast it to float
        float currentValue = (float)AnimationTree.Get(_runPath);

        // Smoothly interpolate toward the target value
        float animationValue = Mathf.MoveToward(currentValue, RunWeighTarget, (float)delta * AnimationSpeed);

        // Update the blend position in the AnimationTree
        AnimationTree.Set(_runPath, animationValue);
    }

    public void UpdateAnimationTree(Vector3 direction)
    {
        // Update the target blend position based on movement direction
        if (direction.IsZeroApprox())
        {
            RunWeighTarget = -1.0f;
        }
        else
        {
            RunWeighTarget = 1.0f;
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
        return Playback.GetCurrentNode().Equals("Overhead") ||
               Playback.GetCurrentNode().Equals("OverheadRecover");
    }
    
    public bool IsDashing()
    {
        return Playback.GetCurrentNode().Equals("Dash");
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

    public void ReplaceShield(PackedScene shieldScene)
    {
        foreach (var child in _shieldSlot.GetChildren())
        {
            child.QueueFree();
        }

        var newShield = shieldScene.Instantiate();
        _shieldSlot.AddChild(newShield);
    }

    public void ReplaceWeapon(PackedScene weaponScene)
    {
        foreach (var child in _weaponSlot.GetChildren())
        {
            child.QueueFree();
        }

        var newShield = weaponScene.Instantiate();
        _weaponSlot.AddChild(newShield);
    }
}
using System.Numerics;
using Godot;
using Template.Scripts.Animations;
using Template.Scripts.Characters.Abilities;
using Template.Scripts.Managers;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Template.Scripts.Characters.Players;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f; // Speed of the player's movement
    public const float JumpVelocity = 4.5f; // Velocity applied when the player jumps
    private Vector3 _attackDirection = Vector3.Zero; // stores the direction the player moves when attacking

    // Stores the x/y direction the player is looking in (mouse/controller input)
    private Vector2 _look = Vector2.Zero;
    [Export] public SpringArm3D SpringArm { get; set; } // Camera spring arm for smooth camera movement
    [Export] public float MouseSensitivity { get; set; } = 0.00075f; // Sensitivity for mouse look
    [Export] public float MinBoundary { get; set; } = -60f; // Minimum vertical angle for camera rotation
    [Export] public float MaxBoundary { get; set; } = 10f; // Maximum vertical angle for camera rotation
    [Export] public Node3D HorizontalPivot { get; set; } // Node for horizontal camera rotation
    [Export] public Node3D VerticalPivot { get; set; } // Node for vertical camera rotation
    [Export] public Node3D RigPivot { get; set; } // Node for character rig rotation
    [Export] public float AnimationDecay { get; set; } = 20f; // Smoothing factor for character rotation
    [Export] public Rig Rig { get; set; } // Reference to the Rig component for animations

    [Export] public AttackCast AttackCast { get; set; }
    [Export] public float AttackMoveSpeed { get; set; } = 3.0f;

    [ExportCategory("Health")]
    /****************************************/
    [Export]
    public HealthComponent HealthComponent { get; set; }
    [Export] 
    public int MaxHealth { get; set; } = 100;
    [Export]
    public CollisionShape3D CollisionShape { get; set; }

    [ExportCategory("Attacks")]
    /****************************************/
    [Export]
    public AreaAttack AreaAttack { get; set; }
    [Export] 
    public int HeavyAttackDamage { get; set; } = 100;
    
    public override void _Ready()
    {
        // Capture the mouse so it doesn't leave the game window
        Input.MouseMode = Input.MouseModeEnum.Captured;
        
        HealthComponent.UpdateMaxHealth(MaxHealth);
        HealthComponent.Defeat += OnHealthComponentDefeated;

        Rig.HeavyAttack += OnRigHeavyAttack;
    }

    private void OnRigHeavyAttack()
    {
        AreaAttack.DealDamage(HeavyAttackDamage);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Update camera rotation based on mouse input
        FrameCameraRotation();
        // Get the current velocity of the player
        var velocity = Velocity;
        // Get the movement direction based on player input
        var direction = GetMovementDirection();
        // Update the animation tree in the Rig component based on the movement direction
        Rig.UpdateAnimationTree(direction);
        // Update the player's velocity and move the character
        Velocity = velocity;

        HandleIdlePhysicsFrame(delta, direction);
        HandleSlashingPhysicsFrame(delta);
        HandleOverheadPhysicsFrame(delta);
        if (!IsOnFloor()) velocity += GetGravity() * (float)delta; // Apply gravity if the player is not on the floor
        MoveAndSlide();
    }

    public Vector3 GetMovementDirection()
    {
        // Get input direction from the player (WASD or controller)
        var inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        // Convert the 2D input direction to a 3D vector and normalize it
        var inputVector = new Vector3(inputDirection.X, 0, inputDirection.Y).Normalized();
        // Transform the input direction to align with the camera's horizontal rotation
        return HorizontalPivot.GlobalTransform.Basis * inputVector;
    }

    private void FrameCameraRotation()
    {
        // Rotate the horizontal pivot (left/right) based on mouse input
        HorizontalPivot.RotateY(_look.X);
        // Rotate the vertical pivot (up/down) based on mouse input
        VerticalPivot.RotateX(_look.Y);

        // Clamp the vertical rotation to prevent the camera from flipping
        var clampedX = Mathf.Clamp(VerticalPivot.Rotation.X, Mathf.DegToRad(MinBoundary), Mathf.DegToRad(MaxBoundary));
        VerticalPivot.Rotation = new Vector3(clampedX, VerticalPivot.Rotation.Y, VerticalPivot.Rotation.Z);

        // Update the SpringArm's position to match the camera pivot
        SpringArm.GlobalTransform = VerticalPivot.GlobalTransform;

        // Reset the look input for the next frame
        _look = Vector2.Zero;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Toggle mouse visibility when the "ui_cancel" action is pressed (e.g., Escape key)
        if (@event.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;

        // Handle mouse look input when the mouse is captured
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
            if (@event is InputEventMouseMotion mouseMotion)
                _look = -mouseMotion.Relative * MouseSensitivity; // Update look direction based on mouse movement

        if (Rig.IsIdle())
        {
            if (@event.IsActionPressed("click"))
            {
                SlashAttack();
            }

            if (@event.IsActionPressed("right_click"))
            {
                HeavyAttack();
            }
        }
    }

    private void LookTowardDirection(Vector3 direction, float delta)
    {
        // Calculate the target transform for the RigPivot to face the movement direction
        var targetTransform = RigPivot.GlobalTransform.LookingAt(RigPivot.GlobalPosition + direction, Vector3.Up, true);
        // Smoothly interpolate the RigPivot's rotation toward the target transform
        RigPivot.GlobalTransform =
            RigPivot.GlobalTransform.InterpolateWith(targetTransform, 1.0f - Mathf.Exp(-AnimationDecay * delta));
    }

    // Placeholder method for a slash attack (to be implemented)
    public void SlashAttack()
    {
        Rig.Travel("Slash");
        _attackDirection = GetMovementDirection();
        if (_attackDirection.IsZeroApprox()) _attackDirection = Rig.GlobalBasis * Vector3.Back;
        // clear the exception in the raycast if it hit an object so it can hit it again
        AttackCast.ClearExceptions();
    }

    public void HeavyAttack()
    {
        Rig.Travel("Overhead");
        _attackDirection = GetMovementDirection();
        if (_attackDirection.IsZeroApprox()) _attackDirection = Rig.GlobalBasis * Vector3.Back;
        AttackCast.ClearExceptions();
    }

    public void HandleSlashingPhysicsFrame(double delta)
    {
        if (!Rig.IsSlashing()) return;

        Velocity = new Vector3(_attackDirection.X * AttackMoveSpeed, 0, _attackDirection.Z * AttackMoveSpeed);
        LookTowardDirection(_attackDirection, (float)delta);

        AttackCast.DealDamage();
    }
    
    public void HandleOverheadPhysicsFrame(double delta)
    {
        if (!Rig.IsOverhead()) return;
        Velocity = Vector3.Zero;
    }

    public void HandleIdlePhysicsFrame(double delta, Vector3 direction)
    {
        if (!Rig.IsIdle()) return;

        var velocity = Velocity;
        // Apply movement if there is a direction input
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed; // Move along the X-axis
            velocity.Z = direction.Z * Speed; // Move along the Z-axis
            LookTowardDirection(direction, (float)delta); // Smoothly rotate the character toward the movement direction
        }
        else
        {
            // Decelerate the player when there is no input
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
    }
    
    private void OnHealthComponentDefeated()
    {
        Rig.Travel("Defeat");
        CollisionShape.Disabled = true;
        SetPhysicsProcess(false);
        //QueueFree();
    } 
}
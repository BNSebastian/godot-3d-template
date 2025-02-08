using Godot;
using Template.Scripts.Characters.Players;

namespace Template.Scripts.Characters.Abilities;

public partial class Dash : Node3D
{
    [Export] 
    private Player Player { get; set; }
    //[Export]
    //private double AbilityCooldown { get; set; } = 1;
    [Export]
    private double AbilityDuration { get; set; } = 0.3;
    [Export]
    private float DashSpeed { get; set; } = 3.0f;
    
    private Timer _timer;
    private double _timeRemaining;
    private GpuParticles3D _particles;
    private Vector3 _direction = Vector3.Zero;

    public override void _Ready()
    {
       _timer = GetNode<Timer>("Timer"); 
       _particles = GetNode<GpuParticles3D>("Particles");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_direction.IsZeroApprox())
        {
            return;
        }
        
        Player.Velocity = -_direction * Player.Stats.GetBaseSpeed() * DashSpeed;
        
        _timeRemaining -= delta;
        
        if (_timeRemaining <= 0)
        {
            _direction = Vector3.Zero;
            _particles.Emitting = false;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_timer.IsStopped())
        {
            return;
        }

        if (!Player.IsPhysicsProcessing())
        {
            return;
        }
        
        if (@event.IsActionPressed("dash"))
        {
            _direction = Player.GetMovementDirection();

            if (!_direction.IsZeroApprox())
            {
                Player.Rig.Travel("Dash");
                _particles.Emitting = true;
                _timer.Start(Player.Stats.GetDashCooldown());
                _timeRemaining = AbilityDuration;
            }
            _timer.Start(Player.Stats.GetDashCooldown());
        }
    }
}
using Godot;
using Template.Scripts.Characters.Players;

namespace Template.Scripts.Characters.Abilities;

public partial class Dash : Node3D
{
    [Export] 
    private Player Player { get; set; }
    [Export]
    private Timer Timer {get; set;}
    [Export]
    private int Cooldown { get; set; } = 1;

    private Vector3 _direction = Vector3.Zero;
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Timer.IsStopped())
        {
            return;
        }
        
        if (@event.IsActionPressed("dash"))
        {
            _direction = Player.GetMovementDirection();

            if (!_direction.IsZeroApprox())
            {
                GD.Print("We can dash");
            }
            Timer.Start(Cooldown);
        }
    }
}
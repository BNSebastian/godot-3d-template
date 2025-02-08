using Godot;
using System.Threading.Tasks;

namespace Template.Scripts.Characters.Abilities;

public partial class DamageNumber : Node3D
{
    [Export]
    public Label3D Label { get; set; }

    public async void Setup(int damage, Color color, Vector3 positionIn)
    {
        // Wait until the node is inside the scene tree
        if (!IsInsideTree())
        {
            await ToSignal(this, "tree_entered");
        }

        // Ensure the Label is ready
        if (Label != null)
        {
            Label.Text = $"{damage}";
            Label.Modulate = color;
        }

        // Set the global position
        GlobalPosition = positionIn;
    }
}
using Godot;
using Template.Scripts.Characters.NPCs;
using Template.Scripts.Characters.Players;

namespace Template.Scripts.Characters.Abilities;

public partial class AreaAttack : ShapeCast3D
{
    public void DealDamage(int damage)
    {
        if (GetCollisionCount() > 0)
        {
            GD.Print("Collisions detected: ", GetCollisionCount());
            for (int i = 0; i < GetCollisionCount(); i++)
            {
                var collider = GetCollider(i);
                if (collider is Player player)
                {
                    GD.Print("Dealing damage to player: ", damage);
                    player.HealthComponent.TakeDamage(damage);
                }
                else if (collider is Enemy enemy)
                {
                    GD.Print("Dealing damage to enemy: ", damage);
                    enemy.HealthComponent.TakeDamage(damage);
                }
            }
        }
        else
        {
            GD.Print("No collisions detected.");
        }
    }
}
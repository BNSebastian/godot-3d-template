using System;
using Godot;
using Template.Scripts.Characters.NPCs;
using Template.Scripts.Characters.Players;

namespace Template.Scripts.Characters.Abilities;

public partial class AreaAttack : ShapeCast3D
{
    [Export]
    public bool Debug { get; set; } = false;

    private Random _random = new();
    
    public void DealDamage(int damage, float critChance )
    {
        if (GetCollisionCount() > 0)
        {
            if (Debug) GD.Print("Collisions detected: ", GetCollisionCount());
            for (int i = 0; i < GetCollisionCount(); i++)
            {
                var isCritical = !(_random.NextDouble() <= critChance);
                
                var collider = GetCollider(i);
                
                if (collider is Player player)
                {
                    if (Debug) GD.Print("Dealing damage to player: ", damage);
                    player.HealthComponent.TakeDamage(damage, isCritical);
                    AddException(player);
                }
                else if (collider is Enemy enemy)
                {
                    if (Debug)GD.Print("Dealing damage to enemy: ", damage);
                    enemy.HealthComponent.TakeDamage(damage, isCritical);
                    AddException(enemy);
                }
            }
        }
        else
        {
            GD.Print("No collisions detected.");
        }
    }
}
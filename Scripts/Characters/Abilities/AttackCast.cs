using System;
using Godot;
using Template.Scripts.Characters.NPCs;

namespace Template.Scripts.Characters.Abilities;

public partial class AttackCast : RayCast3D
{
	private Random _random = new();
	
	public void DealDamage(int damage, float critChance)
	{
		if (!IsColliding()) { return; }

		var isCritical = !(_random.NextDouble() <= critChance);
		
		var collider = GetCollider();
		
		// Check if the collider is a CollisionObject3D
		if (collider is Enemy enemy)
		{
			GD.Print(collider);
			
			if (isCritical) damage *= 2;
			
			enemy.HealthComponent.TakeDamage(damage);
			// adds an exception so the specified object will not collide with the raycast
			AddException(enemy);
		}
		else
		{
			GD.Print("Collider is not a CollisionObject3D");
		}
	}
}
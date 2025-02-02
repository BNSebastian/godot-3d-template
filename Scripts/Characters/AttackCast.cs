using Godot;

namespace Template.Scripts.Characters;

public partial class AttackCast : RayCast3D
{
	public void DealDamage()
	{
		if (!IsColliding()) { return; }

		var collider = GetCollider();
		
		// Check if the collider is a CollisionObject3D
		if (collider is NPCs.Enemy enemy)
		{
			GD.Print(collider);
			enemy.HealthComponent.TakeDamage(50);
			// adds an exception so the specified object will not collide with the raycast
			AddException(enemy);
		}
		else
		{
			GD.Print("Collider is not a CollisionObject3D");
		}
	}
}
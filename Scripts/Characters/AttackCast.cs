using Godot;

public partial class AttackCast : RayCast3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void DealDamage()
	{
		if (!IsColliding()) { return; }

		var collider = GetCollider();
		
		GD.Print(collider);

		// Check if the collider is a CollisionObject3D
		if (collider is CollisionObject3D collisionObject)
		{
			// adds an exception so the specified object will not collide with the raycast
			AddException(collisionObject);
		}
		else
		{
			GD.Print("Collider is not a CollisionObject3D");
		}
	}
}

using System;
using Godot;
using Template.Scripts.Animations;
using Template.Scripts.Characters.Abilities;
using Template.Scripts.Characters.Players;
using Template.Scripts.Managers;

namespace Template.Scripts.Characters.NPCs;

public partial class Enemy : CharacterBody3D
{
	[ExportCategory("Character")]
	[Export]
	public Rig Rig { get; set; }
	[Export]
	public CollisionShape3D CollisionShape { get; set; }
	
	[ExportCategory("Health")]
	[Export]
	public HealthComponent HealthComponent { get; set; }
	[Export] 
	public int MaxHealth { get; set; } = 100;

	[ExportCategory("Attacks")]
	[Export]
	public ShapeCast3D PlayerDetector { get; set; }
	[Export]
	public AreaAttack AreaAttack { get; set; }
	[Export] 
	public int Damage { get; set; } = 15;
	
	public override void _Ready()
	{
		Random random = new Random();
		Rig.SetActiveMesh(Rig.VillagerMeshes[random.Next(Rig.VillagerMeshes.Length)]);
		HealthComponent.UpdateMaxHealth(MaxHealth);
		
		// can be connected manually from the editor by connecting the
		// health component signal directly to the EnemyDeath() method
		HealthComponent.Defeat += OnHealthComponentDefeated;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Rig.IsIdle())
		{
			CheckForAttacks();
		}
	}

	private void CheckForAttacks()
	{
		// Check if there are any collisions
		if (PlayerDetector.GetCollisionCount() > 0)
		{
			// Iterate over the colliders
			for (int i = 0; i < PlayerDetector.GetCollisionCount(); i++)
			{
				var collider = PlayerDetector.GetCollider(i);
				if (collider is Player player)
				{
					Rig.Travel("Overhead");
				}
			}
		}
	}

	private void OnHealthComponentDefeated()
	{
		Rig.Travel("Defeat");
		CollisionShape.Disabled = true;
		SetPhysicsProcess(false);
		//QueueFree();
	}

	public void OnRigHeavyAttack()
	{
		GD.Print("heavy attack signal");
		AreaAttack.DealDamage(Damage);
	}

}
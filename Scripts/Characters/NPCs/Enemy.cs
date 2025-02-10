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
	public int MaxHealth { get; set; } = 30;

	[ExportCategory("Attacks")]
	[Export]
	public ShapeCast3D PlayerDetector { get; set; }
	[Export]
	public AreaAttack AreaAttack { get; set; }
	[Export] 
	public int Damage { get; set; } = 15;

	[ExportCategory("Mechanics")]
	[Export] 
	public int XpValue { get; set; } = 100;
	[Export]
	public float Speed { get; set; } = 3.0f;
	
	[ExportCategory("Items")]
	[Export]
	public PackedScene[] Shields { get; set; }
	[Export]
	public PackedScene[] Weapons { get; set; }
	
	private Player _player;
	private NavigationAgent3D _navigationAgent;
	private float _runVelocityThreshold = 2.0f;
	private readonly Random _random = new();
	
	public override void _Ready()
	{
		GetNodes();
		
		Rig.SetActiveMesh(Rig.VillagerMeshes[_random.Next(Rig.VillagerMeshes.Length)]);
		Rig.ReplaceShield(Shields[_random.Next(Shields.Length)]);
		Rig.ReplaceWeapon(Weapons[_random.Next(Weapons.Length)]);
		HealthComponent.UpdateMaxHealth(MaxHealth);
		
		HealthComponent.Defeat += OnHealthComponentDefeated;
		
		// synchronize with the navigation server
		CallDeferred(MethodName.NavigationSetup);

		_navigationAgent.VelocityComputed += OnNavigationAgent3DVelocityComputed;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocityTarget = Vector3.Zero;
		
		_navigationAgent.TargetPosition = _player.GlobalPosition;
		
		if (Rig.IsIdle())
		{
			CheckForAttacks();
			if (!_navigationAgent.IsTargetReached())
			{
				velocityTarget = GetLocalNavigationDirection() * Speed;
				OrientRig(_navigationAgent.GetNextPathPosition());
			}
		}
		
        // Apply gravity if the player is not on the floor
        if (!IsOnFloor())
        {
	       velocityTarget -= GetGravity() * (float)delta;
        } 
        
		_navigationAgent.Velocity = velocityTarget;
	}
	
	public void OnRigHeavyAttack()
	{
		GD.Print("heavy attack signal");
		AreaAttack.DealDamage(Damage, 0.0f);
		
		// start detecting other enemies again
		_navigationAgent.AvoidanceMask = 1;
	} 

	private async void NavigationSetup()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
	}

	private void OrientRig(Vector3 targetPosition)
	{
		targetPosition.Y = Rig.GlobalPosition.Y;
		if (Rig.GlobalPosition.IsEqualApprox(targetPosition))
		{
			return;
		}
		Rig.LookAt(targetPosition, Vector3.Up, true);
	}

	private void OnNavigationAgent3DVelocityComputed(Vector3 safeVelocity)
	{
		if (safeVelocity == Vector3.Zero) return;
		
		if (safeVelocity.Length() > _runVelocityThreshold)
		{
			Rig.RunWeighTarget = 1.0f;
		}
		else
		{
			Rig.RunWeighTarget = 0.0f;
		}
		
		Velocity = safeVelocity;
		MoveAndSlide();
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
					GD.Print("Player detected!");
					Rig.Travel("Overhead");
					
					// disable all the masks for the enemy so it will stop detecting other enemies
					// otherwise the enemy gets pushed around by other enemies
					_navigationAgent.AvoidanceMask = 0;
				}
			}
		}
	}

	private void OnHealthComponentDefeated()
	{
		_player.Stats.Xp += XpValue;
		Rig.Travel("Defeat");
		CollisionShape.Disabled = true;
		SetPhysicsProcess(false);
		// if not set, the enemy will fall through the floor
		// reason 1: gravity is still applied and the navigation agent is moving it along the y axis
		// reason 2: the collision shape is disabled and it can fall through the floor
		_navigationAgent.TargetPosition = GlobalPosition;
		_navigationAgent.Velocity = Vector3.Zero;
		//QueueFree();
	}

	private void GetNodes()
	{
		_player = GetTree().GetFirstNodeInGroup("PlayerGroup") as Player;
		_navigationAgent = GetNode<NavigationAgent3D>("%NavigationAgent");

		if (_navigationAgent == null)
		{
			GD.PrintErr("NavigationAgent3D node not found!");
		}
	}

	private Vector3 GetLocalNavigationDirection()
	{
		var destination = _navigationAgent.GetNextPathPosition();
		var localDestination = destination - GlobalPosition;
		return localDestination.Normalized();
	}
}
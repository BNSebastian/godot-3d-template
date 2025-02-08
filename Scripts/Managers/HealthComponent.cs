using Godot;

namespace Template.Scripts.Managers;

public partial class HealthComponent : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler();
	
	[Signal]
	public delegate void DefeatEventHandler();
	
	[Export]
	public PhysicsBody3D Body { get; set; }
	
	[Export]
	public bool Debug {get; set;} = false;
	
	public int MaxHealth { get; set; } = 100;
	
	public int CurrentHealth
	{
		get => _currentHealth;
		set
		{
			_currentHealth = Mathf.Max(value, 0);
			if (_currentHealth == 0)
			{
				EmitSignal(nameof(Defeat));
			}
			EmitSignal(nameof(HealthChanged));
		}
	}
	
	private int _currentHealth;

	private VfxManager _vfxManager;

	public override void _Ready()
	{
		_vfxManager = GetNode<VfxManager>("/root/VfxManager");
	}
	
	public void UpdateMaxHealth(int maxHp)
	{
		MaxHealth = maxHp;
		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(int damage, bool isCritical = false)
	{
		var color = Colors.White;
		
		if (isCritical)
		{
			damage *= 2;
			color = Colors.Red;
			if (Debug) GD.Print("Attack is a critical hit");
		}
		
		CurrentHealth -= damage;
		
		_vfxManager.SpawnDamageNumber(damage, color, Body.GetGlobalPosition());
	}
}

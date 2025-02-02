using Godot;

namespace Template.Scripts.Managers;

public partial class HealthComponent : Node
{
	[Signal]
	public delegate void HealthChangedEventHandler();
	
	[Signal]
	public delegate void DefeatEventHandler();
	
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

	public void UpdateMaxHealth(int maxHp)
	{
		MaxHealth = maxHp;
		CurrentHealth = MaxHealth;
	}

	public void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
	}
}

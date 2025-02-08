using Godot;

namespace Template.Scripts.Resources;

public partial class CharacterStats : Resource
{
    [Signal]
    public delegate void LevelUpNotificationEventHandler();
   
    [Signal]
    public delegate void UpdateStatsEventHandler();
    
    public int Level { get; set; } = 1;
    
    public int Xp
    {
        get => _xp;
        set
        {
            _xp = value;
            if (_xpBoundary == 0)
            {
                _xpBoundary = PercentageLevelUpBoundary();
            }
            
            while (_xp > _xpBoundary)
            {
                _xp -= _xpBoundary;
                LevelUp();
               _xpBoundary = PercentageLevelUpBoundary(); 
            }
            
            EmitSignal(nameof(UpdateStats));
        }
    }

    // damage bonus on attack
    public Ability Strength { get; set; } = new(2.0f, 12.0f);
    // crit chance
    public Ability Agility { get; set; } = new(0.05f, 0.25f);
    // hp bonus per level
    public Ability Endurance { get; set; } = new(5.0f, 25.0f);
    // movement speed in m/s
    public Ability Speed { get; set; } = new(3.0f, 7.0f);

    private int _xp = 0;
    private int _xpBoundary;
    private float _minDashCooldown = 1.5f;
    private float _maxDashCooldown = 0.5f;
    
    public float GetBaseSpeed()
    {
        return Speed.GetModifier();
    }

    public float GetDamageModifier()
    {
        return Strength.GetModifier();
    }

    public float GetCritChance()
    {
        return Agility.GetModifier();
    }

    public int GetMaxHp()
    {
        return 20 + (int)(Level * Endurance.GetModifier());
    }

    public float GetDashCooldown()
    {
        return Agility.PercentileLerp(_minDashCooldown, _maxDashCooldown);
    }
    
    public void LevelUp()
    {
        Level++;
        Strength.Increase();
        Agility.Increase();
        Endurance.Increase();
        Speed.Increase();
        GD.Print("Leveling up");
        EmitSignal(nameof(LevelUpNotification));
    }

    public int PercentageLevelUpBoundary()
    {
        return 50 * (int)Mathf.Pow(1.2f, Level);
    }

    public int CubicLevelUpBoundary()
    {
        return 50 + (int)Mathf.Pow(Level, 3);
    }
}
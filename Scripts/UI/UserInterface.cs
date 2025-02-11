using Godot;
using Template.Scripts.Characters.Players;

namespace Template.Scripts.UI;

public partial class UserInterface : Control
{
    [Export]
    public Player Player { get; set; }

    private Label _levelLabel;
    private TextureProgressBar _healthBar;
    private Label _healthLabel;
    private TextureProgressBar _experienceBar;

    private Inventory _inventory;
    
    public override void _Ready()
    {
        if (Player == null)
        {
            GD.PrintErr("Player node isn't set");
        }
        _levelLabel = GetNode<Label>("%LevelLabel");
        _healthBar = GetNode<TextureProgressBar>("%HealthBar");
        _healthLabel = GetNode<Label>("%HealthLabel");
        _experienceBar = GetNode<TextureProgressBar>("%ExperienceBar");
        _inventory = GetNode<Inventory>("%Inventory");
        
        UpdateStatsDisplay(); // update the ui when the game loads
        Player.Stats.UpdateStats += UpdateStatsDisplay;
        Player.Stats.UpdateStats += UpdateExperienceBar;
        Player.HealthComponent.HealthChanged += UpdateHealthBar;
        Player.HealthComponent.HealthChanged += UpdateHealthLabel;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("open_menu"))
        {
            if (_inventory.Visible)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }
    
    public void UpdateStatsDisplay()
    {
        _levelLabel.Text = Player.Stats.Level.ToString();
        // test, delete it afterwards
        _inventory.UpdateStats();
    }

    public void UpdateHealthBar()
    {
        _healthBar.MaxValue = Player.HealthComponent.MaxHealth;
        _healthBar.Value = Player.HealthComponent.CurrentHealth;
    }

    public void UpdateHealthLabel()
    {
        _healthLabel.Text = $"{Player.HealthComponent.CurrentHealth}/{Player.HealthComponent.MaxHealth}";
    }
    
    public void UpdateExperienceBar()
    {
        _experienceBar.MaxValue = Player.Stats.CubicLevelUpBoundary();
        _experienceBar.Value = Player.Stats.Xp;
    }

    public void OpenMenu()
    {
        _inventory.Visible = true;
        GetTree().Paused = true;
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }

    public void CloseMenu()
    {
        _inventory.Visible = false;
        GetTree().Paused = false;
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
    }
}
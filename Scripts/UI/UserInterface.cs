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

    public override void _Ready()
    {
        _levelLabel = GetNode<Label>("%LevelLabel");
        _healthBar = GetNode<TextureProgressBar>("%HealthBar");
        _healthLabel = GetNode<Label>("%HealthLabel");
        _experienceBar = GetNode<TextureProgressBar>("%ExperienceBar");

        Player.Stats.UpdateStats += UpdateStatsDisplay;
        UpdateStatsDisplay(); // update the ui when the game loads
        
        Player.HealthComponent.HealthChanged += UpdateHealthBar;
        Player.HealthComponent.HealthChanged += UpdateHealthLabel;
    }
    
    public void UpdateStatsDisplay()
    {
        _levelLabel.Text = Player.Stats.Level.ToString();
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
    
    
}
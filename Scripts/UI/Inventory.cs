using Godot;

namespace Template.Scripts.UI;

public partial class Inventory : Control
{
	[Export]
	public UserInterface UserInterface { get; set; }
	
	private Label _levelLabel;
	private Label _strengthValue;
	private Label _agilityValue;
	private Label _speedValue;
	private Label _enduranceValue;
	private Label _attackValue;

	private TextureButton _backButton;

	public override void _Ready()
	{
		InstantiateNodes();
		_backButton.Pressed += OnBackButtonPressed;
		UserInterface.Player.Stats.LevelUpNotification += UpdateStats;
	}

	public void UpdateStats()
	{
		_levelLabel.Text = $"Level: {UserInterface.Player.Stats.Level.ToString()}";
		
		_strengthValue.Text = UserInterface.Player.Stats.Strength.AbilityScore.ToString();
		_agilityValue.Text = UserInterface.Player.Stats.Agility.AbilityScore.ToString();
		_speedValue.Text = UserInterface.Player.Stats.Speed.AbilityScore.ToString();
		_enduranceValue.Text = UserInterface.Player.Stats.Endurance.AbilityScore.ToString();

		_attackValue.Text = $"{10 + UserInterface.Player.Stats.GetDamageModifier()}";
	}

	private void OnBackButtonPressed()
	{
		UserInterface.CloseMenu();
	}
	
	private void InstantiateNodes()
	{
		// Stats and level labels
		_levelLabel = GetNode<Label>("%LevelLabel");
		_strengthValue = GetNode<Label>("%StrengthValue");
		_agilityValue = GetNode<Label>("%AgilityValue");
		_speedValue = GetNode<Label>("%SpeedValue");
		_enduranceValue = GetNode<Label>("%EnduranceValue");
		// attack damage values
		_attackValue = GetNode<Label>("%AttackValue");
		// buttons
		_backButton = GetNode<TextureButton>("%BackButton");
	}
}
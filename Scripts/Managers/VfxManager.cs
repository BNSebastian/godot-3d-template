using Godot;
using Template.Scripts.Characters.Abilities;

namespace Template.Scripts.Managers;

[GlobalClass]
public partial class VfxManager : Node
{
	private PackedScene _damageNumberScene;

	public override void _Ready()
	{
		_damageNumberScene = ResourceLoader.Load<PackedScene>("res://Scenes/DamageNumber.tscn");
	}

	public void SpawnDamageNumber(int damage, Color color, Vector3 position)
	{
		var newInstance = _damageNumberScene.Instantiate() as DamageNumber;
		if (newInstance != null) newInstance.Setup(damage, color, position);
		AddChild(newInstance);
	} 
}
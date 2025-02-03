using Godot;

namespace Template.Scripts.Resources;

public partial class CharacterStats : Resource
{
    public int Level { get; set; } = 1;
    public int Xp { get; set; } = 0;
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Endurance { get; set; }
    public int Speed { get; set; }
}
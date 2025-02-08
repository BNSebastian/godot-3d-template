using System;
using Godot;

namespace Template.Scripts.Resources;

public class Ability
{
    public int AbilityScore
    {
        get => _abilityScore;
        set => _abilityScore = Mathf.Clamp(value, 0, 100);
    } 

    private int _abilityScore = 25;
    private readonly float _minModifier;
    private readonly float _maxModifier;

    public Ability(float min, float max)
    {
        _minModifier = min;
        _maxModifier = max;
    }

    public float PercentileLerp(float minBoundary, float maxBoundary)
    {
        return Mathf.Lerp(minBoundary, maxBoundary, _abilityScore/100f);
    }

    public float GetModifier()
    {
        return PercentileLerp(_minModifier, _maxModifier);
    }

    public void Increase()
    {
        Random random = new Random();
        _abilityScore += random.Next(2, 5);
    }
    
}
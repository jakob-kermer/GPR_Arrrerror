using UnityEngine;

public class Enemy : Entity
{
    // Fields
    [SerializeField] private int experienceValue;
    [SerializeField] private float dropRate;

    // Properties
    public int ExperienceValue
    {
        get { return experienceValue; }
        set { this.experienceValue = value; }
    }
    public float DropRate
    {
        get { return dropRate; }
        set { this.dropRate = value; }
    }
}

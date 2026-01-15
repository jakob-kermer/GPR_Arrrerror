using UnityEngine;
using System;

public abstract class Enemy : Entity
{
    // Fields
    [SerializeField] private int experienceValue;

    // Properties
    public int ExperienceValue
    {
        get { return experienceValue; }
        set { this.experienceValue = value; }
    }
}

using UnityEngine;
using System;

public abstract class Enemy : Entity
{
    // Fields
    [Header("Enemy specific Stats")]
    [SerializeField] private int experienceValue;

    // Properties
    public int ExperienceValue
    {
        get { return experienceValue; }
        set { this.experienceValue = value; }
    }
}

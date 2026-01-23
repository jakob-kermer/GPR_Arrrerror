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

    // Methods
    public void OnMouseEnter()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}

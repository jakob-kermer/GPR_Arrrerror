using UnityEngine;
using System;

public abstract class Enemy : Entity
{
    // Fields
    [Header("Enemy specific Stats")]
    [SerializeField] private int experienceValue;

    public bool enableTargetSelection = false;

    // Properties
    public int ExperienceValue
    {
        get { return experienceValue; }
        set { this.experienceValue = value; }
    }

    // Methods
    public void OnMouseEnter()
    {
        if (enableTargetSelection)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    public Enemy OnMouseDown()
    {
        return this;
    }
}

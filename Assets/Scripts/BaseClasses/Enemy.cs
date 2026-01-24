using UnityEngine;
using System;

public abstract class Enemy : Entity
{
    // Fields
    private bool enableSelector = false;

    // Properties
    public bool EnableSelector
    {
        get { return enableSelector; }
        set { this.enableSelector = value; }
    }

    // Methods
    public void OnMouseEnter()
    {
        if (EnableSelector)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}

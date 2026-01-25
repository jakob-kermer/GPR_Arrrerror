using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Player : Entity
{
    // Fields
    private BattleUI playerUI;

    // Properties
    public BattleUI PlayerUI
    {
        get { return playerUI; }
        set { this.playerUI = value; }
    }

    // Methods
    public virtual bool Action_Attack(Entity target)
    {
        return target.TakeDamage(this, 1f);
    }

    public virtual void Action_Defend()
    {
        Debug.Log($"{this.Name} is defending");
        this.DefenseModifier = 2;
    }
}

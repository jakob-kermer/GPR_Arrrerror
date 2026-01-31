using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Player : Entity
{
    // Fields
    private BattleUI playerUI;
    private AudioManager audioManager;

    // Properties
    public BattleUI PlayerUI
    {
        get { return playerUI; }
        set { this.playerUI = value; }
    }

    public AudioManager AudioManager
    {
        get { return audioManager; }
        set { this.audioManager = value; }
    }

    // Methods
    public virtual void Action_Attack(Entity target)
    {
        // play hit sound effect
        audioManager.PlaySFX(audioManager.Hit);

        // deal damage to target
        target.TakeDamage(this, 1f);
    }

    public virtual void Action_Defend()
    {
        Debug.Log($"{this.Name} is defending");

        this.DefenseModifier = 2;
    }
}

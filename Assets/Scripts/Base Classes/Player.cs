using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : Entity
{
    // Fields
    // [SerializeField] private float sanity;
    [SerializeField] private int experiencePoints;
    private int XP_Threshold = 200;
    [SerializeField] List<string> inventory = new List<string>();

    // Properties
    // public float Sanity
    // {
    //     get { return sanity; }
    //     protected set { this.sanity = value; }
    // }
    public int ExperiencePoints
    {
        get { return experiencePoints; }
        protected set { this.experiencePoints = value; }
    }
    public List<string> Inventory
    {
        get { return inventory; }
        protected set { this.inventory = value; }
    }

    // Methods
    public bool Action_Attack(Entity target)
    {
        return target.TakeDamage(this, target, 1f);
    }



    private void GainExperience(int XP)
    {
        // increase the XP of the player by the amount of XP gained
        this.ExperiencePoints += XP;
        Debug.Log($"They gained {XP} Experience Points.\n");

        // level-up logic
        while (ExperiencePoints >= XP_Threshold)     // as long as the current XP of the player are above a certain threshold...
        {
            Level++;        // ...increase the player's level by 1...

            // ...increase the player's stats...
            // HP
            int HP_Increase = 100 + Convert.ToInt32(UnityEngine.Random.Range(0, 40));           // HP increase = 100 + ~20 (100 min, 140 max, 120 on average)
            this.MaxHP += HP_Increase;
            this.CurrentHP += HP_Increase;      // add HP increase to the current HP as well

            // Attack
            int Attack_Increase = 16 + Convert.ToInt32(UnityEngine.Random.Range(0, 10));        // Attack increase = 16 + ~5 (16 min, 26 max, 21 on average)
            this.Attack += Attack_Increase;

            // Defense
            int Defense_Increase = 12 + Convert.ToInt32(UnityEngine.Random.Range(0, 8));        // Defense increase = 12 + ~4 (12 min, 20 max, 16 on average)
            this.Defense += Defense_Increase;

            // CritChance
            float CritChance_Increase = 0.01f + UnityEngine.Random.Range(-0.005f, 0.001f);      // Crit Chance increase = 1% - ~0.25% (0.5% min, 1% max, 0.75% on average)
            this.CritChance += CritChance_Increase;

            Debug.Log($"{this.Name}'s leveled up to level {this.Level}, new stats: MaxHP {this.MaxHP}, Attack {this.Attack}, Defense {this.Defense}, Crit Chance {this.CritChance * 100}%");        // ...write level-up message on the console...

            ExperiencePoints -= XP_Threshold;       // ...subtract the XP needed for the level-up from the current XP of the player...

            XP_Threshold += 75 + UnityEngine.Random.Range(-20, 40);        // ...and increase the threshold for the next level-up
        }
    }
}

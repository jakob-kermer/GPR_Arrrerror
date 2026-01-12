using Unity.VisualScripting;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    // Fields
    [SerializeField] private string entityName;
    [SerializeField] private int maxHP;
    [SerializeField] private int currentHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    // [SerializeField] private int magicAttack;
    // [SerializeField] private int magicDefense;
    [SerializeField] private float critChance;

    // Properties
    public string EntityName
    {
        get { return entityName; }
        set { this.entityName = value; }
    }
    public int MaxHP
    {
        get { return maxHP; }
        set { this.maxHP = value; }
    }
    public int CurrentHP
    {
        get { return currentHP; }
        set { this.currentHP = value; }
    }
    public int Attack
    {
        get { return attack; }
        set { this.attack = value; }
    }
    public int Defense
    {
        get { return defense; }
        set { this.defense = value; }
    }
    // public int MagicAttack
    // {
    //     get { return magicAttack; }
    //     set { this.magicAttack = value; }
    // }
    // public int MagicDefense
    // {
    //     get { return magicDefense; }
    //     set { this.magicDefense = value; }
    // }
    public float CritChance
    {
        get { return critChance; }
        set { this.critChance = value; }
    }

    // Methods
    public virtual void TakeDamage(Entity attacker, Entity target, float damageMultiplier)
    {
        // determine critical hit
        if (UnityEngine.Random.Range(0.0f, 1.0f) < attacker.CritChance)       // if the attacker lands a critical hit...
        {
            damageMultiplier *= 2;      // ...double damage dealt...
            Debug.Log($"critical hit");        // ...and write crit message on the console
        }

        // this is where the damage is calculated
        int damage = CalculateDamage(attacker.Attack, target.Defense, damageMultiplier);

        // HP check
        if (damage > target.CurrentHP)        // check if damage exceeds current HP
        {
            damage = target.CurrentHP;        // if yes, set damage to actual damage dealt (--> current HP of the target BEFORE applying damage) to prevent negative HP
        }

        // apply damage to current HP
        target.CurrentHP -= damage;

        // write attack message on the console
        Debug.Log($"{attacker.EntityName} deals {damage} damage to {target.EntityName}");
    }

    public int CalculateDamage(int atk, int def, float damageMultiplier)
    {
        // damage = (Attack - Defense) * given multiplier +/- up to 2% of Attack
        int damage = Convert.ToInt32(((atk - def) * damageMultiplier) + (atk * UnityEngine.Random.Range(-0.02f, 0.02f)));

        return damage;
    }
}

using Unity.VisualScripting;
using UnityEngine;
using System;

public abstract class Entity : MonoBehaviour
{
    // Fields
    [SerializeField] private string entityName;
    [SerializeField] private int maxHP;
    private int currentHP;
    [SerializeField] private int maxAP;
    private int currentAP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    // [SerializeField] private int magicAttack;
    // [SerializeField] private int magicDefense;
    [SerializeField] private float critChance;
    [SerializeField] private int speed;
    [SerializeField] private int level;

    // Properties
    public string Name
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
    public int MaxAP
    {
        get { return maxAP; }
        set { this.maxAP = value; }
    }
    public int CurrentAP
    {
        get { return currentAP; }
        set { this.currentAP = value; }
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
    public int Speed
    {
        get { return speed; }
        set { this.speed = value; }
    }
    public int Level
    {
        get { return level; }
        protected set { this.level = value; }
    }

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
    }

    public virtual bool TakeDamage(Entity attacker, Entity target, float damageMultiplier)
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
        Debug.Log($"{attacker.Name} deals {damage} damage to {target.Name}");

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int CalculateDamage(int atk, int def, float damageMultiplier)
    {
        // damage = (Attack - Defense) * given multiplier +/- up to 2% of Attack; at least 1
        int damage = Math.Max(1, Convert.ToInt32(((atk - def * 0.5f) * damageMultiplier) + (atk * UnityEngine.Random.Range(-0.02f, 0.02f))));

        return damage;
    }
}

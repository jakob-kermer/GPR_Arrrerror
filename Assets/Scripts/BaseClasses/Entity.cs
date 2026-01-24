using Unity.VisualScripting;
using UnityEngine;
using System;
using TMPro;
using System.Numerics;

public abstract class Entity : MonoBehaviour
{
    // Fields
    [Header("Stats")]
    [SerializeField] private string entityName;
    [SerializeField] private int maxHP;
    private int currentHP;
    [SerializeField] private int maxAP;
    private int currentAP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private float critChance = 0.05f; // default crit chance is 5%
    [SerializeField] private int speed;
    private int level = 1;

    // damage pop-up
    [SerializeField] private TMP_Text popUpDamage;
    [SerializeField] private GameObject popUpDamagePrefab;

    // Properties
    // stats properties
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

    // damage pop-up properties
    public TMP_Text PopUpDamage
    {
        get { return popUpDamage; }
        set { this.popUpDamage = value; }
    }
    public GameObject PopUpDamagePrefab
    {
        get { return popUpDamagePrefab; }
        set { this.popUpDamagePrefab = value; }
    }

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
    }

    public virtual bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        // determine critical hit
        if (UnityEngine.Random.Range(0.0f, 1.0f) < attacker.CritChance)       // if the attacker lands a critical hit...
        {
            damageMultiplier *= 2;      // ...double damage dealt...
            Debug.Log($"critical hit");        // ...and write crit message on the console
        }

        // this is where the damage is calculated
        int damage = CalculateDamage(attacker.Attack, this.Defense, damageMultiplier);

        popUpDamage.text = damage.ToString();
        Instantiate(popUpDamagePrefab, transform.position, UnityEngine.Quaternion.identity);

        // HP check
        if (damage > this.CurrentHP)        // check if damage exceeds current HP
        {
            damage = this.CurrentHP;        // if yes, set damage to actual damage dealt (--> current HP of the target BEFORE applying damage) to prevent negative HP
        }

        // apply damage to current HP
        this.CurrentHP -= damage;

        // write attack message on the console
        Debug.Log($"{attacker.Name} deals {damage} damage to {this.Name}");

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

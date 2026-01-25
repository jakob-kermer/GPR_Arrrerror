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
    [SerializeField] private int defenseModifier = 1; // 1 = not defending, 2 = defending

    // damage pop-up
    [SerializeField] private TMP_Text popUpDamage;
    [SerializeField] private GameObject popUpDamagePrefab;

    // Animator
    private Animator animator;

    // target selector
    private bool enableSelector = false;

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
    public int DefenseModifier
    {
        get { return defenseModifier; }
        set { this.defenseModifier = value; }
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

    // Animator property
    public Animator Animator
    {
        get { return animator; }
        set { this.animator = value; }
    }

    // target selector property
    public bool EnableSelector
    {
        get { return enableSelector; }
        set { this.enableSelector = value; }
    }

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
    }

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

    public virtual void TakeDamage(Entity attacker, float damageModifier)
    {
        this.PopUpDamage.color = Color.red;
        
        // determine critical hit
        if (UnityEngine.Random.Range(0.0f, 1.0f) < attacker.CritChance)       // if the attacker lands a critical hit...
        {
            damageModifier *= 2;      // ...double damage dealt...
            Debug.Log($"critical hit");        // ...and write crit message on the console

            // change pop-up color to yellow for critical hits
            this.PopUpDamage.color = Color.yellow;
        }

        // this is where the damage is calculated
        int damage = CalculateDamage(attacker.Attack, this.Defense, damageModifier);

        // display damage (before HP check) with pop-up
        this.PopUpDamage.text = damage.ToString();
        Instantiate(this.PopUpDamagePrefab, transform.position, UnityEngine.Quaternion.identity);

        // HP check
        if (damage > this.CurrentHP)        // check if damage exceeds current HP
        {
            damage = this.CurrentHP;        // if yes, set damage to actual damage dealt (--> current HP of the target BEFORE applying damage) to prevent negative HP
        }

        // apply damage to current HP
        this.CurrentHP -= damage;

        // play hit animation
        this.Animator.SetTrigger("Hit");

        // write attack message on the console
        Debug.Log($"{attacker.Name} deals {damage} damage to {this.Name}");
    }

    public int CalculateDamage(int atk, int def, float damageModifier)
    {
        // damage = (Attack - Defense) * damage modifier / defense modifier +/- up to 2% of Attack; at least 1
        int damage = Math.Max(1, Convert.ToInt32(((atk - def * 0.5f) * damageModifier / this.DefenseModifier) + (atk * UnityEngine.Random.Range(-0.02f, 0.02f))));

        return damage;
    }
}

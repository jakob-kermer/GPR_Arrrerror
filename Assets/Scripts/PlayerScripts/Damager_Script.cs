using System.Collections.Generic;
using UnityEngine;

public class Damager_Script : Player
{
    // Fields
    [Header("Damager-specific Stats")]
    [SerializeField] private float fireballModifier;
    [SerializeField] private float shitstormModifier;

    [Header("Ability AP costs")]
    [SerializeField] private int fireball_APCost;
    [SerializeField] private int shitstorm_APCost;

    // Properties
    public float FireballModifier
    {
        get { return fireballModifier; }
        set { this.fireballModifier = value;}
    }
    public float ShitstormModifier
    {
        get { return shitstormModifier; }
        set { this.shitstormModifier = value;}
    }
    public int Fireball_APCost
    {
        get { return fireball_APCost; }
        set { this.fireball_APCost = value; }
    }
    public int Shitstorm_APCost
    {
        get { return shitstorm_APCost; }
        set { this.shitstorm_APCost = value; }
    }
    
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Damager UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Damager-specific abilities
    public void Ability_Fireball(Entity target)
    {
        Debug.Log($"{this.name} casts Fireball on {target.name}");

        // cast fireball on target
        target.TakeDamage(this, this.FireballModifier);

        // reduce AP and update UI
        this.CurrentAP -= this.Fireball_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Shitstorm(List<Enemy> enemies)
    {
        Debug.Log($"{this.name} casts Shitstorm on enemy party");
        
        // deal damage to every enemy
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(this, this.ShitstormModifier);
        }

        // reduce AP and update UI
        this.CurrentAP -= this.Shitstorm_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

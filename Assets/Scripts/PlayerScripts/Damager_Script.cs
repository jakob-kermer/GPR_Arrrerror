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

    [Header("Ability Effect Animations")]
    [SerializeField] private GameObject AttackEffect;
    [SerializeField] private GameObject FireballEffect;
    [SerializeField] private GameObject ShitstormEffect;

    // Properties
    public float FireballModifier
    {
        get { return fireballModifier; }
        set { this.fireballModifier = value; }
    }
    public float ShitstormModifier
    {
        get { return shitstormModifier; }
        set { this.shitstormModifier = value; }
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

    public override void Action_Attack(Entity target)
    {
        // play attack effect animation at the targets' position
        SpawnAnimation(AttackEffect, target.transform.position);

        base.Action_Attack(target);
    }

    // Damager-specific abilities
    public void Ability_Fireball(Entity target)
    {
        Debug.Log($"{this.Name} casts Fireball on {target.Name}");

        // play cast fireball animation
        this.Animator.SetTrigger("Fireball");

        // play fireball effect animation at the targets' position
        SpawnAnimation(FireballEffect, target.transform.position);

        // cast fireball on target
        target.TakeDamage(this, this.FireballModifier);

        // reduce AP and update UI
        this.CurrentAP -= Fireball_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Shitstorm(List<Enemy> enemies)
    {
        Debug.Log($"{this.Name} casts Shitstorm on enemy party");

        // play cast shitstorm animation
        this.Animator.SetTrigger("Shitstorm");

        // play shitstorm effect animation at specified position
        SpawnAnimation(ShitstormEffect, new UnityEngine.Vector3(-2, 0, -2));

        // deal damage to every enemy
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(this, this.ShitstormModifier);
        }

        // reduce AP and update UI
        this.CurrentAP -= Shitstorm_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

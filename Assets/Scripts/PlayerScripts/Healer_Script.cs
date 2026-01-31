using System;
using System.Collections.Generic;
using UnityEngine;

public class Healer_Script : Player
{
    // Fields
    [Header("Healer-specific Stats")]
    [SerializeField] private int healPower;

    [Header("Ability AP costs")]
    [SerializeField] private int heal_APCost;
    [SerializeField] private int groupHeal_APCost;

    [Header("Ability Effect Animations")]
    [SerializeField] private GameObject AttackEffect;
    [SerializeField] private GameObject HealingEffect;

    // Properties
    public int HealPower
    {
        get { return healPower; }
        set { this.healPower = value; }
    }
    public int Heal_APCost
    {
        get { return heal_APCost; }
    }
    public int GroupHeal_APCost
    {
        get { return groupHeal_APCost; }
    }

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Healer UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    public override void Action_Attack(Entity target)
    {
        // play attack effect animation at the targets' position
        SpawnAnimation(AttackEffect, target.transform.position);
        
        base.Action_Attack(target);
    }

    // Healer-specific abilities
    public void Ability_Heal(Player target)
    {
        // amount healed = HealPower * 1.5 +/- up to 2% of HealPower
        int healAmount = Convert.ToInt32(this.HealPower * 1.5f + (this.HealPower * 1.5f * UnityEngine.Random.Range(-0.02f, 0.02f)));

        // play healing effect animation at the targets' position
        SpawnAnimation(HealingEffect, target.transform.position);
        
        // display amount healed (before HP check) with pop-up
        this.PopUpDamage.color = new Color32(24, 140, 20, 255);
        this.PopUpDamage.text = healAmount.ToString();
        SpawnAnimation(this.PopUpDamagePrefab, target.transform.position + Vector3.back * 0.2f);

        Debug.Log($"{this.Name} heals {target.Name} for {healAmount} HP.");

        // HP check
        if (target.CurrentHP + healAmount > target.MaxHP) // check if heal exceeds max HP
        {
            healAmount = target.MaxHP - target.CurrentHP; // if yes, set healAmount to actual HP healed (--> HP needed to reach max HP) to prevent over-healing
        }

        // apply healing
        target.CurrentHP += healAmount;
        target.PlayerUI.SetHP(target.CurrentHP);

        // reduce AP and update UI
        this.CurrentAP -= this.Heal_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Groupheal(List<Player> players)
    {
        this.PopUpDamage.color = new Color32(24, 140, 20, 255);

        foreach (Player player in players)
        {
            // amount healed = HealPower +/- up to 2% of HealPower
            int healAmount = Convert.ToInt32(this.HealPower + (this.HealPower * UnityEngine.Random.Range(-0.02f, 0.02f)));

            // play healing effect animation at each player's position
            SpawnAnimation(HealingEffect, player.transform.position);

            // display amount healed (before HP check) with pop-up
            this.PopUpDamage.text = healAmount.ToString();
            SpawnAnimation(this.PopUpDamagePrefab, player.transform.position + Vector3.back * 0.2f);

            Debug.Log($"{this.Name} heals {player.Name} for {healAmount} HP.");

            // HP check
            if (player.CurrentHP + healAmount > player.MaxHP) // check if heal exceeds max HP
            {
                healAmount = player.MaxHP - player.CurrentHP; // if yes, set healAmount to actual HP healed (--> HP needed to reach max HP) to prevent over-healing
            }

            // apply healing
            player.CurrentHP += healAmount;
            player.PlayerUI.SetHP(player.CurrentHP);
        }

        // reduce AP and update UI
        this.CurrentAP -= this.GroupHeal_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

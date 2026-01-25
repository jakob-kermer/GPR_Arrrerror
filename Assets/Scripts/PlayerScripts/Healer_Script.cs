using System;
using System.Collections.Generic;
using UnityEngine;

public class Healer_Script : Player
{
    // Fields
    [Header("Healer-specific Stats")]
    [SerializeField] private int healPower;

    // Properties
    public int HealPower
    {
        get { return healPower; }
        set { this.healPower = value; }
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

    // Healer-specific abilities
    public void Ability_Heal(Player target)
    {
        int APcost = 6;

        // check if player has enough AP
        if (this.CurrentAP < APcost)
        {
            Debug.Log($"{this.name} does not have enough AP to cast Heal.");
            return;
        }

        int healAmount = Convert.ToInt32(this.HealPower * 1.5f + (this.HealPower * 1.5f * UnityEngine.Random.Range(-0.02f, 0.02f)));

        // display amount healed (before HP check) with pop-up
        this.PopUpDamage.color = Color.green;
        this.PopUpDamage.text = healAmount.ToString();
        Instantiate(this.PopUpDamagePrefab, target.transform.position, UnityEngine.Quaternion.identity);

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
        this.CurrentAP -= APcost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Groupheal(List<Player> players)
    {
        int APcost = 10;

        // check if player has enough AP
        if (this.CurrentAP < APcost)
        {
            Debug.Log($"{this.name} does not have enough AP to cast Group Heal.");
            return;
        }

        this.PopUpDamage.color = Color.green;

        foreach (Player player in players)
        {
            int healAmount = Convert.ToInt32(this.HealPower + (this.HealPower * UnityEngine.Random.Range(-0.02f, 0.02f)));

            // display amount healed (before HP check) with pop-up
            this.PopUpDamage.text = healAmount.ToString();
            Instantiate(this.PopUpDamagePrefab, player.transform.position, UnityEngine.Quaternion.identity);

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
        this.CurrentAP -= APcost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

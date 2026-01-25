using System.Collections.Generic;
using UnityEngine;

public class Supporter_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Supporter UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Supporter-specific abilities
    public void Ability_ThrowGato(List<Enemy> enemies)
    {
        Debug.Log($"{this.name} deals damage to an enemy");
        // deal double damage to a random enemy
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_ThrowPotion(List<Player> players)
    {
        Debug.Log($"{this.name} heals a player");
        // heals a random hero
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

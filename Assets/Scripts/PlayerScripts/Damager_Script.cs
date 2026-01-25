using System.Collections.Generic;
using UnityEngine;

public class Damager_Script : Player
{
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
        target.TakeDamage(this, 3f);
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Shitstorm(List<Enemy> enemies)
    {
        Debug.Log($"{this.name} casts Shitstorm on enemy party");
        // cast shitstorm on enemy party
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(this, 1.0f);
        }
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

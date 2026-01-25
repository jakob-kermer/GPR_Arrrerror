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
    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        bool isDead = base.TakeDamage(attacker, damageMultiplier);
        PlayerUI.SetHP(this.CurrentHP);
        return isDead;
    }
}

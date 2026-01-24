using UnityEngine;

public class Supporter_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.playerUI = GameObject.Find("Supporter UI").GetComponent<BattleUI>();
        this.playerUI.SetUI(this);
    }

    // Supporter-specific abilities
    public void Ability_RandomCrit(Entity target)
    {
        Debug.Log($"{this.name} buffs {target.name}");
        // deal double damage to a random enemy
    }

    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        bool isDead = base.TakeDamage(attacker, damageMultiplier);
        playerUI.SetHP(this.CurrentHP);
        return isDead;
    }
}

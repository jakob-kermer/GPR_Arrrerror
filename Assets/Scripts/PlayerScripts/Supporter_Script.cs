using UnityEngine;

public class Supporter_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.PlayerUI = GameObject.Find("Supporter UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Supporter-specific abilities
    public void Ability_ThrowGato(Entity target)
    {
        Debug.Log($"{this.name} deals damage to {target.name}");
        // deal double damage to a random enemy
    }

    public void Ability_ThrowPotion(Entity target)
    {
        Debug.Log($"{this.name} heals {target.name}");
        // heals a random hero
    }

    // TakeDamage override to update UI
    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        bool isDead = base.TakeDamage(attacker, damageMultiplier);
        PlayerUI.SetHP(this.CurrentHP);
        return isDead;
    }
}

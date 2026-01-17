using UnityEngine;

public class Defender_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.playerUI = GameObject.Find("Defender UI").GetComponent<BattleUI>();
        this.playerUI.SetUI(this);
    }

    // Defender-specific abilities
    public void Ability_Block(Entity target)
    {
        Debug.Log($"{this.name} blocks the attack");
    }

    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        bool isDead = base.TakeDamage(attacker, damageMultiplier);
        playerUI.SetHP(this.CurrentHP);
        return isDead;
    }
}

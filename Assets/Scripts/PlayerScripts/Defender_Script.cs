using UnityEngine;

public class Defender_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(1).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Defender UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Defender-specific abilities
    public void Ability_Block(Entity target)

    {
        Debug.Log($"{this.name} blocks the attack of {target.name}.");
        // blocks the next incoming attack
    }

    public void Ability_Taunt(Entity target)
    {
        Debug.Log($"{this.name} taunts the enemy{target.name} to attack him.");
        // forces enemy to attack defender next
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

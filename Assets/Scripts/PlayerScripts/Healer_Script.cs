using UnityEngine;

public class Healer_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.playerUI = GameObject.Find("Healer UI").GetComponent<BattleUI>();
        this.playerUI.SetUI(this);
    }

    // Healer-specific abilities
    public void Ability_Heal(Entity target)
    {
        Debug.Log($"{this.name} heals {target.name}");
        // heal target
    }

    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        bool isDead = base.TakeDamage(attacker, damageMultiplier);
        playerUI.SetHP(this.CurrentHP);
        return isDead;
    public void Ability_Groupheal()
    {
        Debug.Log($"{this.name} heals the group");
        // heal the group
    }
}

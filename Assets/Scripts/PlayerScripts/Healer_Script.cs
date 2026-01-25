using UnityEngine;

public class Healer_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(1).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Healer UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Healer-specific abilities
    public void Ability_Heal(Entity target)
    {
        Debug.Log($"{this.name} heals {target.name}");
        // heal target
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Groupheal()
    {
        Debug.Log($"{this.name} heals the group");
        // heal the group
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

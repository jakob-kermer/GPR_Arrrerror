using UnityEngine;

public class Defender_Script : Player
{
    private bool isBlocking = false;
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.PlayerUI = GameObject.Find("Defender UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Defender-specific abilities
    public void Ability_Block()

    {
        Debug.Log($"{this.name} blocks the next incoming attack.");
        //blocks the next incoming attack
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
        this.isBlocking = true;
    }

    public void Ability_Taunt(Entity target)
    {
        Debug.Log($"{this.name} taunts the enemy{target.name} to attack him.");
        //forces enemy to attack defender next
        this.CurrentAP -= 20;
        PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override bool TakeDamage(Entity attacker, float damageMultiplier)
    {
        if (isBlocking == true)
        {
            damageMultiplier = 0f;
            isBlocking = false;

            bool isDead = base.TakeDamage(attacker, damageMultiplier);
            PlayerUI.SetHP(this.CurrentHP);
            return isDead;
        }
        else
        {
            bool isDead = base.TakeDamage(attacker, damageMultiplier);
            PlayerUI.SetHP(this.CurrentHP);
            return isDead;
        }
    }
}

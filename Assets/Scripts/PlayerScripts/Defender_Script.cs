using UnityEngine;

public class Defender_Script : Player
{
    // Fields
    [Header("Defender-specific Stats")]
    private bool isBlocking = false;
    private bool tauntUsed;

    [Header("Ability AP costs")]
    [SerializeField] private int block_APCost;
    [SerializeField] private int taunt_APCost;

    // Properties
    public bool IsBlocking
    {
        get { return isBlocking; }
        set { this.isBlocking = value; }
    }
    public bool TauntUsed
    {
        get { return tauntUsed; }
        set { this.tauntUsed = value; }
    }
    public int Block_APCost
    {
        get { return block_APCost; }
        set { this.block_APCost = value; }
    }
    public int Taunt_APCost
    {
        get { return taunt_APCost; }
        set { this.taunt_APCost = value; }
    }

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Defender UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Defender-specific abilities
    public void Ability_Block()
    {
        // check if player has enough AP
        if (this.CurrentAP < this.Block_APCost)
        {
            Debug.Log($"{this.name} does not have enough AP");
            return;
        }

        Debug.Log($"{this.name} blocks the next incoming attack.");

        // blocks the next incoming attack
        this.IsBlocking = true;

        // reduce AP and update UI
        this.CurrentAP -= this.Block_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Taunt()
    {
        // check if player has enough AP
        if (this.CurrentAP < this.Taunt_APCost)
        {
            Debug.Log($"{this.name} does not have enough AP");
            return;
        }

        Debug.Log($"{this.name} taunts the enemies to attack him.");

        // forces enemy to attack defender next
        this.TauntUsed = true;

        // reduce AP and update UI
        this.CurrentAP -= this.Taunt_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        if (this.IsBlocking)
        {
            damageModifier = 0f;
            this.IsBlocking = false;
        }

        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

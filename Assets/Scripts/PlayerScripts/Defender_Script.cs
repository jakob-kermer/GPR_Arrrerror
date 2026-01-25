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
            Debug.Log($"{this.Name} does not have enough AP");
            return;
        }

        Debug.Log($"{this.Name} blocks the next incoming attack.");
        // blocks the next incoming attack
        this.isBlocking = true;

        // reduce AP and update UI
        this.CurrentAP -= Block_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Taunt()
    {
        // check if player has enough AP
        if (this.CurrentAP < this.Taunt_APCost)
        {
            Debug.Log($"{this.Name} does not have enough AP");
            return;
        }

        Debug.Log($"{this.Name} taunts the enemies to attack him.");

        // forces enemy to attack defender next
        tauntUsed = true;

        // reduce AP and update UI
        this.CurrentAP -= Taunt_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        if (isBlocking == true)
        {
            damageModifier = 0f;
            isBlocking = false;

            base.TakeDamage(attacker, damageModifier);
            PlayerUI.SetHP(this.CurrentHP);

        }
        else
        {
            base.TakeDamage(attacker, damageModifier);
            PlayerUI.SetHP(this.CurrentHP);
        }
    }
}

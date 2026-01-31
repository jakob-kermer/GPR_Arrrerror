using Unity.VisualScripting;
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
        // set current HP & AP to maximum
        this.CurrentHP = this.MaxHP;
        this.CurrentAP = this.MaxAP;

        // get animator component from "Sprite"
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();

        // get the defender's UI from the scene and apply the defender's stats to it
        this.PlayerUI = GameObject.Find("Defender UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);

        // get the Audio Manager from the scene
        this.AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Defender-specific abilities
    public void Ability_Block()
    {
        Debug.Log($"{this.Name} blocks the next incoming attack.");

        // play block sound effect
        this.AudioManager.PlaySFX(this.AudioManager.Block);

        // play block animation and idle block animation
        this.Animator.SetTrigger("Block");
        this.Animator.SetBool("IsBlocking", true);

        // blocks the next incoming attack
        this.isBlocking = true;

        // reduce AP and update UI
        this.CurrentAP -= this.Block_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_Taunt()
    {
        Debug.Log($"{this.Name} taunts the enemies to attack him.");

        // play taunt sound effect
        this.AudioManager.PlaySFX(this.AudioManager.Taunt);

        // play taunt animation
        this.Animator.SetTrigger("Taunt");

        // forces enemy to attack defender next
        this.tauntUsed = true;

        // reduce AP and update UI
        this.CurrentAP -= this.Taunt_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        if (this.isBlocking == true)
        {
            damageModifier = 0f;
            this.isBlocking = false;

            // set IsBlocking to false to stop idle block animation
            this.Animator.SetBool("IsBlocking", false);

            base.TakeDamage(attacker, damageModifier);
            this.PlayerUI.SetHP(this.CurrentHP);
        }
        else
        {
            base.TakeDamage(attacker, damageModifier);
            this.PlayerUI.SetHP(this.CurrentHP);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class Supporter_Script : Player
{
    // Fields
    [Header("Supporter-specific Stats")]
    [SerializeField] private float gatoModifier;
    [SerializeField] private int potionPotency;

    [Header("Ability AP costs")]
    [SerializeField] private int throwPotion_APCost;
    [SerializeField] private int throwGato_APCost;

    // Properties
    public float GatoModifier
    {
        get { return gatoModifier; }
        set { this.gatoModifier = value;}
    }
    public int PotionPotency
    {
        get { return potionPotency; }
        set { this.potionPotency = value; }
    }
    public int ThrowPotion_APCost
    {
        get { return throwPotion_APCost; }
    }
    public int ThrowGato_APCost
    {
        get { return throwGato_APCost; }
    }
    
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();
        this.PlayerUI = GameObject.Find("Supporter UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);
    }

    // Supporter-specific abilities
    public void Ability_ThrowGato(List<Enemy> enemies)
    {
        Debug.Log($"{this.name} deals damage to an enemy");

        // choose random enemy
        Enemy selectedEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];

        // deal damage to that enemy
        selectedEnemy.TakeDamage(this, this.GatoModifier);

        // reduce AP and update UI
        this.CurrentAP -= ThrowGato_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_ThrowPotion(List<Player> players)
    {
        Debug.Log($"{this.name} heals a player");

        // choose random player
        Player selectedPlayer = players[UnityEngine.Random.Range(0, players.Count)];

        // amount healed = PotionPotency +/- up to 2% of PotionPotency
        int healAmount = Convert.ToInt32(this.PotionPotency + (this.PotionPotency * UnityEngine.Random.Range(-0.02f, 0.02f)));

        // display amount healed (before HP check) with pop-up
        this.PopUpDamage.color = Color.green;
        this.PopUpDamage.text = healAmount.ToString();
        Instantiate(this.PopUpDamagePrefab, selectedPlayer.transform.position, UnityEngine.Quaternion.identity);

        Debug.Log($"{this.Name} heals {selectedPlayer.Name} for {healAmount} HP.");

        // HP check
        if (selectedPlayer.CurrentHP + healAmount > selectedPlayer.MaxHP) // check if heal exceeds max HP
        {
            healAmount = selectedPlayer.MaxHP - selectedPlayer.CurrentHP; // if yes, set healAmount to actual HP healed (--> HP needed to reach max HP) to prevent over-healing
        }

        // apply healing
        selectedPlayer.CurrentHP += healAmount;
        selectedPlayer.PlayerUI.SetHP(selectedPlayer.CurrentHP);

        // reduce HP and update UI
        this.CurrentAP -= ThrowPotion_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        PlayerUI.SetHP(this.CurrentHP);
    }
}

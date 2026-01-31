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

    [Header("Ability Effect Animations")]
    [SerializeField] private GameObject ThrowGatoEffect;
    [SerializeField] private GameObject ThrowPotionEffect;

    // Properties
    public float GatoModifier
    {
        get { return gatoModifier; }
        set { this.gatoModifier = value; }
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
        // set current HP & AP to maximum
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;

        // get animator component from "Sprite"
        this.Animator = this.transform.GetChild(2).GetComponent<Animator>();

        // get the supporter's UI from the scene and apply the supporter's stats to it
        this.PlayerUI = GameObject.Find("Supporter UI").GetComponent<BattleUI>();
        this.PlayerUI.SetUI(this);

        // get the Audio Manager from the scene
        this.AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Supporter-specific abilities
    public void Ability_ThrowGato(List<Enemy> enemies)
    {
        Debug.Log($"{this.name} deals damage to an enemy");

        // choose random enemy
        Enemy selectedEnemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];

        // play cat sound effect
        this.AudioManager.PlaySFX(this.AudioManager.Cat);

        // play throw animation
        this.Animator.SetTrigger("Throw");

        // play throw gato effect animation at the targets' position
        SpawnAnimation(this.ThrowGatoEffect, selectedEnemy.transform.position);

        // deal damage to that enemy
        selectedEnemy.TakeDamage(this, this.GatoModifier);

        // reduce AP and update UI
        this.CurrentAP -= this.ThrowGato_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    public void Ability_ThrowPotion(List<Player> players)
    {
        Debug.Log($"{this.name} heals a player");

        // choose random player
        Player selectedPlayer = players[UnityEngine.Random.Range(0, players.Count)];

        // amount healed = PotionPotency +/- up to 2% of PotionPotency
        int healAmount = Convert.ToInt32(this.PotionPotency + (this.PotionPotency * UnityEngine.Random.Range(-0.02f, 0.02f)));

        // play potion sound effect
        this.AudioManager.PlaySFX(this.AudioManager.Potion);

        // play throw animation
        this.Animator.SetTrigger("Throw");

        // play throw potion effect animation at the targets' position
        SpawnAnimation(this.ThrowPotionEffect, selectedPlayer.transform.position);

        // display amount healed (before HP check) with pop-up
        this.PopUpDamage.color = new Color32(24, 140, 20, 255);
        this.PopUpDamage.text = healAmount.ToString();
        SpawnAnimation(this.PopUpDamagePrefab, selectedPlayer.transform.position);

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
        this.CurrentAP -= this.ThrowPotion_APCost;
        this.PlayerUI.SetAP(this.CurrentAP);
    }

    // TakeDamage override to update UI
    public override void TakeDamage(Entity attacker, float damageModifier)
    {
        base.TakeDamage(attacker, damageModifier);
        this.PlayerUI.SetHP(this.CurrentHP);
    }
}

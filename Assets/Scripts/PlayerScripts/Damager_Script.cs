using UnityEngine;

public class Damager_Script : Player
{
    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentHP = MaxHP;
        this.CurrentAP = MaxAP;
        this.playerUI = GameObject.Find("Damager UI").GetComponent<BattleUI>();
        this.playerUI.SetUI(this);
    }

    // Damager-specific abilities
    public void Ability_Fireball(Entity target)
    {
        Debug.Log($"{this.name} casts Fireball on {target.name}");
        // cast fireball on target
    }
}

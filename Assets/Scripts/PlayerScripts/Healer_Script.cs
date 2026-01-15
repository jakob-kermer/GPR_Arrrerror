using UnityEngine;

public class Healer_Script : Player
{
    // Methods
    // Healer-specific abilities
    public void Ability_Heal(Entity target)
    {
        Debug.Log($"{this.name} heals {target.name}");
        // heal target
    }
}

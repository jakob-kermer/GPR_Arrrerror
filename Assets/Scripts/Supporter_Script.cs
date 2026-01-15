using UnityEngine;

public class Supporter_Script : Player
{
    // Methods
    // Supporter-specific abilities
    public void Ability_Buff(Entity target)
    {
        Debug.Log($"{this.name} buffs {target.name}");
        // buff target
    }
}

using UnityEngine;

public class Damager_Script : Player
{
    // Methods
    // Damager-specific abilities
    public void Ability_Fireball(Entity target)
    {
        Debug.Log($"{this.name} casts Fireball on {target.name}");
        // cast fireball on target
    }
}

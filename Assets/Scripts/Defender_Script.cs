using UnityEngine;

public class Defender_Script : Player
{
    // Methods
    // Defender-specific abilities
    public void Ability_Block(Entity target)
    {
        Debug.Log($"{this.name} blocks the attack");
    }
}

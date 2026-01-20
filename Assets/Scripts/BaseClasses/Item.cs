using UnityEngine;

public class Item : MonoBehaviour
{
    // Fields
    [SerializeField] private string itemName;

    // Properties
    public string ItemName
    {
        get { return itemName; }
        set { this.itemName = value; }
    }
}

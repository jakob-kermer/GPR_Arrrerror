using UnityEngine;

public class PlayerAction : MonoBehaviour
{

private GameObject enemy;
private GameObject player;

[SerializeField]
private GameObject attackPrefab;

[SerializeField]
private GameObject defendPrefab;

private GameObject currentAction;
private GameObject attackAction;
private GameObject defendAction;
private GameObject specialAction;
private GameObject ItemAction;

public void SelectAction(string pressButton)
    {
        if (pressButton.CompareTo("attack button") == 0)
        {
            Debug.Log("Melee Mode!");
        }
        else if(pressButton.CompareTo("defend button") == 0)
        {
            Debug.Log("Defend Mode!");
        }
        else if(pressButton.CompareTo("special button") == 0)
        {
            Debug.Log("Special Mode!");
        }
        else
        {
            Debug.Log("Item Mode");
        }
    }



}

using UnityEngine;
using UnityEngine.UI;


public class MakeButton : MonoBehaviour
{
    [SerializeField]
   
    private bool physical;

    private GameObject player;

    void Start()
    {
        string temp = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallback(temp));
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void AttachCallback(string pressButton)
    {
        if (pressButton.CompareTo("Attack Button") == 0)
        {
            player.GetComponent<PlayerAction>().SelectAction("attack button");
        }
        else if(pressButton.CompareTo("Defend Button") == 0)
        {
            player.GetComponent<PlayerAction>().SelectAction("defend button");
        }
        else if(pressButton.CompareTo("Special Button") == 0)
        {
            player.GetComponent<PlayerAction>().SelectAction("special button");
        }
        else
        {
            player.GetComponent<PlayerAction>().SelectAction("item button");
        }
    }
}
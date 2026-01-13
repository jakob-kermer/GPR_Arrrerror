using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Entity> participants;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // fill the participants List with all current participants in the battle

        Battle(participants);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Battle(List<Entity> participants)
    {
        Debug.Log("Battle started");

        DetermineTurnOrder(participants);

        Debug.Log("Determined turn order:");

        foreach (Entity item in participants)
        {
            Debug.Log($"{item.Name}");
        }

        foreach (Entity participant in participants)
        {
            participant.SelectMove();
        }
    }

    List<Entity> DetermineTurnOrder(List<Entity> participants)
    {
        participants.Sort((x, y) => x.Speed.CompareTo(y.Speed));

        return participants;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    // Fields
    [Header("Prefabs and Spawn Points")]
    public List<GameObject> playerPrefabs;
    public List<GameObject> enemyPrefabs;

    public List<Transform> playerSpawns;
    public List<Transform> enemySpawns;

    [Header("List of Participants in Battle")]
    public List<Entity> participants;

    [Header("UI Elements")]
    public GameObject ActionMenu;
    public GameObject AbilityMenu;
    public GameObject ItemMenu;
    public GameObject BackButton;

    // player and enemy references
    private Player damager;
    private Player defender;
    private Player healer;
    private Player supporter;
    private List<Player> players = new List<Player>();
    private List<Enemy> enemies = new List<Enemy>();

    // turn management
    private int turnIndex = 0;
    private bool turnMade = false;

    // game state
    private GameState state;

    public Entity selectedTarget = null;

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.state = GameState.Start;
        StartCoroutine(Battle());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && this.state == GameState.PlayerTurn) // if the left mouse button is clicked and it's the player's turn
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Enemy clickedTarget = hit.transform.GetComponent<Enemy>();
                if (clickedTarget != null)
                {
                    selectedTarget = clickedTarget;
                }
            }
        }
    }

    public IEnumerator Battle()
    {
        StartBattle();

        while (this.state != GameState.Victory && this.state != GameState.Defeat)
        {
            DetermineTurnOrder();

            for (turnIndex = 0; turnIndex < participants.Count; turnIndex++)
            {
                if (this.state == GameState.Victory || this.state == GameState.Defeat)
                {
                    break;
                }
                else if (this.participants[turnIndex] is Player)
                {
                    this.state = GameState.PlayerTurn;
                    participants[turnIndex].transform.GetChild(0).gameObject.SetActive(true);
                    PlayerTurn();

                    yield return new WaitUntil(() => this.turnMade);

                    participants[turnIndex].transform.GetChild(0).gameObject.SetActive(false);
                    this.turnMade = false;
                }
                else if (this.participants[turnIndex] is Enemy)
                {
                    this.state = GameState.EnemyTurn;
                    EnemyTurn();

                    yield return new WaitUntil(() => this.turnMade);

                    this.turnMade = false;
                }
            }
        }
    }

    private void StartBattle()
    {
        Debug.Log("Battle started");

        GameObject damager_go = Instantiate(playerPrefabs[0], playerSpawns[0]);
        this.damager = damager_go.GetComponent<Damager_Script>();
        this.players.Add(damager);
        this.participants.Add(damager);

        GameObject defender_go = Instantiate(playerPrefabs[1], playerSpawns[1]);
        this.defender = defender_go.GetComponent<Defender_Script>();
        this.players.Add(defender);
        this.participants.Add(defender);

        GameObject healer_go = Instantiate(playerPrefabs[2], playerSpawns[2]);
        this.healer = healer_go.GetComponent<Healer_Script>();
        this.players.Add(healer);
        this.participants.Add(healer);

        GameObject supporter_go = Instantiate(playerPrefabs[3], playerSpawns[3]);
        this.supporter = supporter_go.GetComponent<Supporter_Script>();
        this.players.Add(supporter);
        this.participants.Add(supporter);

        // spawn random number of enemies (between 1 & 4)
        int randomEnemyAmount = UnityEngine.Random.Range(1, 5);

        for (int i = 0; i < randomEnemyAmount; i++)
        {
            // pick random enemy from enemyPrefabs list
            int randomEnemyIndex = UnityEngine.Random.Range(0, enemyPrefabs.Count);

            // instantiate this random enemy at enemy spawn point
            GameObject enemy_go = Instantiate(enemyPrefabs[randomEnemyIndex], enemySpawns[i]);

            // add enemy to enemies list and participants list
            this.enemies.Add(enemy_go.GetComponent<Enemy>());
            this.participants.Add(this.enemies[i]);
        }
    }

    public void DetermineTurnOrder()
    {
        // sort participants by speed + a random factor (between -5 & 5)
        this.participants.Sort((x, y) => y.Speed.CompareTo(x.Speed + UnityEngine.Random.Range(-5, 6)));
        // this.participants.Sort((x, y) => (y.Speed + UnityEngine.Random.Range(-5, 5)).CompareTo(x.Speed + UnityEngine.Random.Range(-5, 5)));

        Debug.Log("Turn order determined:");

        foreach (Entity entity in this.participants)
        {
            Debug.Log($"{entity.Name} (Speed: {entity.Speed})");
        }
    }

    // Player's turn implementation
    public void PlayerTurn()
    {
        // hide menus
        AbilityMenu.SetActive(false);
        ItemMenu.SetActive(false);

        // show action menu
        ActionMenu.SetActive(true);

        Debug.Log($"{participants[turnIndex].Name} makes their turn");
    }

    public void OnAttackButton()
    {
        StartCoroutine(AttackTargetSelection());
    }

    public IEnumerator AttackTargetSelection()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.SetActive(false);            // deactivate action menu

        foreach (Enemy enemy in enemies)
        {
            enemy.enableTargetSelection = true;
        }

        yield return new WaitUntil(() => selectedTarget != null);

        ((Player)participants[turnIndex]).Action_Attack(selectedTarget);
        DeathCheck(selectedTarget);


        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        selectedTarget = null;
    }

    public void OnDefendButton()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.gameObject.SetActive(false);

        ((Player)participants[turnIndex]).Action_Defend();

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    public void OnAbilityButton()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.gameObject.SetActive(false);
        AbilityMenu.SetActive(true);     //...activate the Abilites menu.

        // implement ability selection and target selection here

        // mark turn as made at the end of the turn
        // this.turnMade = true;
    }

    public void OnItemButton()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.gameObject.SetActive(false);
        ItemMenu.SetActive(true);         //..activate the Item menu.

        // implement item selection and target selection here

        // mark turn as made at the end of the turn
        // this.turnMade = true;
    }

    public void OnBackButton()                  //When pressing on Back...
    {
        // SelectAttackMenu.SetActive(false);      //...deactivate all sub menus and...
        ItemMenu.SetActive(false);
        AbilityMenu.SetActive(false);
        ActionMenu.SetActive(true);             //...activate the action menu.
    }

    // Enemy's turn implementation
    public void EnemyTurn()
    {
        Debug.Log($"{participants[turnIndex].Name} makes their turn");

        Entity selectedTarget = players[1]; // placeholder for selected target

        selectedTarget.TakeDamage(participants[turnIndex], 1f);
        DeathCheck(selectedTarget);

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    // death check and win conditions
    public void DeathCheck(Entity entity)
    {
        if (entity.CurrentHP == 0)
        {
            Debug.Log($"{entity.Name} has been defeated");
            participants.Remove(entity);
            entity.gameObject.SetActive(false);
            CheckWinConditions();
        }
    }

    public void CheckWinConditions()
    {
        bool allEnemiesDefeated = true;
        bool allPlayersDefeated = true;

        foreach (Entity entity in participants)
        {
            if (entity is Enemy && entity.CurrentHP > 0)
            {
                allEnemiesDefeated = false;
            }
            else if (entity is Player && entity.CurrentHP > 0)
            {
                allPlayersDefeated = false;
            }
        }

        if (allEnemiesDefeated)
        {
            this.state = GameState.Victory;
            Debug.Log("The battle is won");
        }
        else if (allPlayersDefeated)
        {
            this.state = GameState.Defeat;
            Debug.Log("The battle is lost");
        }
    }
}

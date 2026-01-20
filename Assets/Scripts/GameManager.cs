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

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.state = GameState.Start;
        StartCoroutine(Battle());
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
                    PlayerTurn();
                    yield return new WaitUntil(() => this.turnMade);
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

    /* public void NextTurn()
    {
        if (this.state == GameState.Victory || this.state == GameState.Defeat)
        {
            return;
        }
        else if (this.participants[turnIndex] is Player)
        {
            this.state = GameState.PlayerTurn;
            PlayerTurn((Player)this.participants[turnIndex]);
        }
        else if (this.participants[turnIndex] is Enemy)
        {
            this.state = GameState.EnemyTurn;
            EnemyTurn((Enemy)this.participants[turnIndex]);
        }
    } */

    // Player's turn implementation
    public void PlayerTurn()
    {
        // enables the action menu for the player to choose an action
        ActionMenu.SetActive(true);

        Debug.Log($"{participants[turnIndex].Name} makes their turn");
    }

    public void TargetSelection()
    {
        // implement target selection here
    }

    public void OnAttackButton()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.gameObject.SetActive(false);

        // implement target selection here
        Entity selectedTarget = enemies[0];
        // select first alive enemy as target
        foreach (Entity enemy in enemies)
        {
            if (enemy.CurrentHP > 0)
            {
                selectedTarget = enemy;
                break;
            }
        }

        ((Player)participants[turnIndex]).Action_Attack(selectedTarget);
        DeathCheck(selectedTarget);

        // mark turn as made at the end of the turn
        this.turnMade = true;
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

        // implement ability selection and target selection here

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    public void OnUseItemButton()
    {
        // disables the action menu after the player has chosen an action
        ActionMenu.gameObject.SetActive(false);

        // implement item selection here

        // mark turn as made at the end of the turn
        this.turnMade = true;
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

    /* private void PlayerTurn(Player player)
    {
        ActionMenu.gameObject.SetActive(true);

        Debug.Log($"{player.Name} makes their turn");
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = defender.Action_Attack(enemy1);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = GameState.Victory;
            enemy1.gameObject.SetActive(false);
            EndBattle();
        }
        else
        {
            state = GameState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    public void OnDefendButton()
    {
        ActionMenu.gameObject.SetActive(false);
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log($"{enemy1.Name} attacks");

        yield return new WaitForSeconds(1f);

        bool isDead = defender.TakeDamage(enemy1, defender, 1f);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = GameState.Defeat;
            EndBattle();
        }
        else
        {
            state = GameState.PlayerTurn;
            PlayerTurn(defender);
        }
    }

    void EndBattle()
    {
        if (state == GameState.Victory)
        {
            Debug.Log("The battle is won");
        }
        else if (state == GameState.Defeat)
        {
            Debug.Log("The battle is lost");
        }
    } */
}

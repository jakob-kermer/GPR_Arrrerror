using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public GameObject playerPrefab_1;
    public GameObject playerPrefab_2;
    public GameObject playerPrefab_3;
    public GameObject playerPrefab_4;
    public GameObject EnemyPrefab_1;
    public GameObject EnemyPrefab_2;
    public GameObject EnemyPrefab_3;
    public GameObject EnemyPrefab_4;

    public Transform playerSpawn_1;
    public Transform playerSpawn_2;
    public Transform playerSpawn_3;
    public Transform playerSpawn_4;
    public Transform enemySpawn_1;
    public Transform enemySpawn_2;
    public Transform enemySpawn_3;
    public Transform enemySpawn_4;

    private Player damager;
    private Player defender;
    private Player healer;
    private Player supporter;
    private Enemy enemy1;
    private Enemy enemy2;
    private Enemy enemy3;
    private Enemy enemy4;
    private List<Entity> participants;

    public GameObject ActionMenu;

    public GameState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.Start;
        Battle();
    }

    public void Battle()
    {
        StartBattle();

        DetermineTurnOrder();

        while (state != GameState.Victory && state != GameState.Defeat)
        {
            for (int i = 0; i < participants.Count; i++)
            {
                if (state == GameState.Victory || state == GameState.Defeat)
                {
                    break;
                }
                else if (participants[i] is Player)
                {
                    state = GameState.PlayerTurn;
                    PlayerTurn((Player)participants[i]);
                }
                else if (participants[i] is Enemy)
                {
                    state = GameState.EnemyTurn;
                    EnemyTurn((Enemy)participants[i]);
                }
            }
        }
    }

    private void StartBattle()
    {
        Debug.Log("Battle started");

        List<Entity> participants = new List<Entity>();

        GameObject damager_go = Instantiate(playerPrefab_1, playerSpawn_1);
        damager = damager_go.GetComponent<Damager_Script>();
        participants.Add(damager);

        GameObject defender_go = Instantiate(playerPrefab_2, playerSpawn_2);
        defender = defender_go.GetComponent<Defender_Script>();
        participants.Add(defender);

        GameObject healer_go = Instantiate(playerPrefab_3, playerSpawn_3);
        healer = healer_go.GetComponent<Healer_Script>();
        participants.Add(healer);

        GameObject supporter_go = Instantiate(playerPrefab_4, playerSpawn_4);
        supporter = supporter_go.GetComponent<Supporter_Script>();
        participants.Add(supporter);

        // implement random enemy selection here

        GameObject enemy1_go = Instantiate(EnemyPrefab_1, enemySpawn_1);
        enemy1 = enemy1_go.GetComponent<Goblin_Script>();
        participants.Add(enemy1);

        /* GameObject enemy2_go = Instantiate(EnemyPrefab_2, enemySpawn_2);
        enemy2 = enemy2_go.GetComponent<WonkyKnight_Script>();

        GameObject enemy3_go = Instantiate(EnemyPrefab_3, enemySpawn_3);
        enemy3 = enemy3_go.GetComponent<Slime_Script>();

        GameObject enemy4_go = Instantiate(EnemyPrefab_4, enemySpawn_4);
        enemy4 = enemy4_go.GetComponent<Skeleton_Script>(); */
    }

    public void DetermineTurnOrder()
    {
        participants.Sort((x, y) => y.Speed.CompareTo(x.Speed));
    }

    public void PlayerTurn(Player player)
    {
        ActionMenu.SetActive(true);

        Debug.Log($"{player.Name} makes their turn");
    }

    public void OnAttackButton()
    {
        ActionMenu.gameObject.SetActive(false);
        // StartCoroutine(PlayerAttack());
    }

    public void EnemyTurn(Enemy enemy)
    {
        Debug.Log($"{enemy.Name} makes their turn");
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

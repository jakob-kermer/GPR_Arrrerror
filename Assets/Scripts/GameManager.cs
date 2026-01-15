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

    private Player player1;
    private Player player2;
    private Player player3;
    private Player player4;
    private Enemy enemy1;
    private Enemy enemy2;
    private Enemy enemy3;
    private Enemy enemy4;

    public BattleUI playerUI_1;
    public BattleUI playerUI_2;
    public BattleUI playerUI_3;
    public BattleUI playerUI_4;

    public GameObject ActionMenu;

    public GameState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.Start;

        StartCoroutine(StartBattle());

        // implement turn order determination here

        state = GameState.PlayerTurn;

        if (state == GameState.PlayerTurn)
        {
            PlayerTurn(player1);
        }
        if (state == GameState.EnemyTurn)
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator StartBattle()
    {
        Debug.Log("Battle started");

        GameObject player1_go = Instantiate(playerPrefab_1, playerSpawn_1);
        player1 = player1_go.GetComponent<Player>();

        GameObject player2_go = Instantiate(playerPrefab_2, playerSpawn_2);
        player2 = player2_go.GetComponent<Player>();

        GameObject player3_go = Instantiate(playerPrefab_3, playerSpawn_3);
        player3 = player3_go.GetComponent<Player>();

        GameObject player4_go = Instantiate(playerPrefab_4, playerSpawn_4);
        player4 = player4_go.GetComponent<Player>();

        GameObject enemy1_go = Instantiate(EnemyPrefab_1, enemySpawn_1);
        enemy1 = enemy1_go.GetComponent<Enemy>();

        GameObject enemy2_go = Instantiate(EnemyPrefab_2, enemySpawn_2);
        enemy2 = enemy2_go.GetComponent<Enemy>();

        GameObject enemy3_go = Instantiate(EnemyPrefab_3, enemySpawn_3);
        enemy3 = enemy3_go.GetComponent<Enemy>();

        GameObject enemy4_go = Instantiate(EnemyPrefab_4, enemySpawn_4);
        enemy4 = enemy4_go.GetComponent<Enemy>();

        playerUI_1.SetUI(player1);
        playerUI_2.SetUI(player2);
        playerUI_3.SetUI(player3);
        playerUI_4.SetUI(player4);

        yield return new WaitForSeconds(2f);
    }

    private void PlayerTurn(Player player)
    {
        ActionMenu.gameObject.SetActive(true);

        Debug.Log($"{player.Name} makes their turn");
    }

    public void OnAttackButton()
    {
        ActionMenu.gameObject.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = player1.Action_Attack(enemy1);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = GameState.Victory;
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

        bool isDead = player1.TakeDamage(enemy1, player1, 1f);

        playerUI_1.SetHP(player1.CurrentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = GameState.Defeat;
            EndBattle();
        }
        else
        {
            state = GameState.PlayerTurn;
            PlayerTurn(player1);
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
    }
}

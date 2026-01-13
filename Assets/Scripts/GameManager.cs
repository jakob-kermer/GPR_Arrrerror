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
    public GameObject playerPrefab;
    public GameObject EnemyPrefab;

    public Transform playerSpawn1;
    public Transform enemySpawn1;

    private Player player1;
    private Enemy enemy1;

    public BattleUI playerUI;
    public BattleUI enemyUI;

    public Button attackButton;
    public Button defendButton;

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

        GameObject player1_go = Instantiate(playerPrefab, playerSpawn1);
        player1 = player1_go.GetComponent<Player>();

        GameObject enemy1_go = Instantiate(EnemyPrefab, enemySpawn1);
        enemy1 = enemy1_go.GetComponent<Enemy>();

        playerUI.SetUI(player1);
        enemyUI.SetUI(enemy1);

        yield return new WaitForSeconds(2f);
    }

    private void PlayerTurn(Player player)
    {
        attackButton.gameObject.SetActive(true);
        defendButton.gameObject.SetActive(true);

        Debug.Log($"{player.Name} makes their turn");
    }

    public void OnAttackButton()
    {
        attackButton.gameObject.SetActive(false);
        defendButton.gameObject.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemy1.TakeDamage(player1, enemy1, 1f);

        enemyUI.SetHP(enemy1.CurrentHP);

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

    IEnumerator EnemyTurn()
    {
        Debug.Log($"{enemy1.Name} attacks");

        yield return new WaitForSeconds(1f);

        bool isDead = player1.TakeDamage(enemy1, player1, 1f);

        playerUI.SetHP(player1.CurrentHP);

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

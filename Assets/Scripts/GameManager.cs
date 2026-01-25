using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    PlayerTurn,
    PlayerTargetSelect,
    EnemyTurn,
    Victory,
    Defeat
}

public class GameManager : MonoBehaviour
{
    // Fields
    [Header("Prefabs and Spawn Points")]
    [SerializeField] private List<GameObject> playerPrefabs;
    [SerializeField] private List<GameObject> enemyPrefabs;

    [SerializeField] private List<Transform> playerSpawns;
    [SerializeField] private List<Transform> enemySpawns;

    [Header("List of Participants in Battle")]
    [SerializeField] private List<Entity> participants;

    [Header("UI Elements")]
    [SerializeField] private GameObject ActionMenu;
    [SerializeField] private GameObject DefenderAbilityMenu;
    [SerializeField] private GameObject DamagerAbilityMenu;
    [SerializeField] private GameObject HealerAbilityMenu;
    [SerializeField] private GameObject SupporterAbilityMenu;
    [SerializeField] private GameObject BackButton;
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private TMP_Text HighscoreText;
    [SerializeField] private TMP_Text ScoreText;


    // player and enemy references
    private Damager_Script damager;
    private Defender_Script defender;
    private Healer_Script healer;
    private Supporter_Script supporter;
    private List<Player> players = new List<Player>();
    private List<Enemy> enemies = new List<Enemy>();

    // turn management
    private int turnIndex = 0;
    private bool turnMade = false;

    // game state
    private GameState state;

    // target selection
    private Entity selectedTarget = null;

    [Header("Score Tracking")]
    [SerializeField] private int score = 0;
    [SerializeField] private int highscore = 0;

    //-------------------------------------------------------------------------------------------------------|

    // Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // load highscore
        highscore = PlayerPrefs.GetInt("Highscore", 0);

        // hide all menus
        ActionMenu.SetActive(false);
        DefenderAbilityMenu.SetActive(false);
        DamagerAbilityMenu.SetActive(false);
        HealerAbilityMenu.SetActive(false);
        SupporterAbilityMenu.SetActive(false);

        this.state = GameState.Start;
        Debug.Log("Battle started");

        // spawn players and enemies
        SpawnPlayers();
        SpawnEnemies();

        // battle start
        StartCoroutine(Battle());
    }

    // Update is called once per frame
    void Update()
    {
        // target selection
        if (Input.GetMouseButtonDown(0) && this.state == GameState.PlayerTargetSelect) // if the left mouse button is clicked and it's the player's turn
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // cast a ray from the camera to the cursor's position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // if the ray hits an object
            {
                Enemy clickedTarget = hit.transform.GetComponent<Enemy>(); // get the Enemy script component

                if (clickedTarget != null) // if the object doesn't have an Enemy script, the following is skipped
                {
                    this.selectedTarget = clickedTarget; // the clicked enemy is the selected target
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------|

    // main battle loop
    public IEnumerator Battle()
    {
        // the battle continues as long as the players are not defeated
        while (this.state != GameState.Defeat)
        {
            DetermineTurnOrder();

            // reset defense modifiers at the start of a new round
            damager.DefenseModifier = 1;
            defender.DefenseModifier = 1;
            healer.DefenseModifier = 1;
            supporter.DefenseModifier = 1;

            for (turnIndex = 0; turnIndex < participants.Count; turnIndex++)
            {
                yield return new WaitForSeconds(2f);

                if (this.state == GameState.Defeat) // if the players are defeated...
                {
                    // game over screen
                    break; // ...stop the battle
                }
                else if (this.state == GameState.Victory) // if the players won...
                {
                    yield return new WaitForSeconds(1f);

                    Debug.Log("New battle started");
                    this.state = GameState.Start;
                    turnIndex = 0; // ...reset turn index...
                    DeleteEnemies(); // ...delete old enemies...
                    SpawnEnemies(); // ...and spawn new enemies
                }
                else if (this.participants[turnIndex] is Player)
                {
                    this.state = GameState.PlayerTurn;

                    // show selector above current player
                    participants[turnIndex].transform.GetChild(0).gameObject.SetActive(true);

                    PlayerTurn();

                    // wait until the player has made their turn
                    yield return new WaitUntil(() => this.turnMade);

                    // hide indicator above current player
                    participants[turnIndex].transform.GetChild(0).gameObject.SetActive(false);

                    // check if any participants have died
                    DeathCheck(this.participants);

                    // reset turnMade for next turn
                    this.turnMade = false;
                }
                else if (this.participants[turnIndex] is Enemy)
                {
                    this.state = GameState.EnemyTurn;
                    EnemyTurn_Attack();

                    // wait until the enemy has made their turn
                    yield return new WaitUntil(() => this.turnMade);

                    // check if any participants have died
                    DeathCheck(this.participants);

                    // reset turnMade for next turn
                    this.turnMade = false;
                }
            }
        }
    }

    private void SpawnPlayers()
    {
        // instantiate player characters at their spawn points
        GameObject damager_go = Instantiate(playerPrefabs[0], playerSpawns[0]);
        // get the player's script component of the instantiated player game object
        this.damager = damager_go.GetComponent<Damager_Script>();
        // add the player to the players list and participants list
        this.players.Add(damager);
        this.participants.Add(damager);

        // repeat for other player characters
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
    }

    public void SpawnEnemies()
    {
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

    public void DeleteEnemies()
    {
        for (int i = 0; i < enemySpawns.Count; i++)
        {
            // if there is an enemy object at this spawn point, destroy it
            if (enemySpawns[i].transform.childCount > 0)
            {
                Destroy(enemySpawns[i].transform.GetChild(0).gameObject);
            }
            Debug.Log("enemy object destroyed");
        }

        // clear enemies list
        enemies.Clear();
    }

    public void DetermineTurnOrder()
    {
        // sort participants by speed + a random factor (between -5 & 5)
        this.participants.Sort((x, y) => y.Speed.CompareTo(x.Speed + UnityEngine.Random.Range(-5, 6)));

        Debug.Log("Turn order determined:");

        foreach (Entity entity in this.participants)
        {
            Debug.Log($"{entity.Name} (Speed: {entity.Speed})");
        }
    }

    //-------------------------------------------------------------------------------------------------------|

    // Player's turn implementation
    public void PlayerTurn()
    {
        // hide menus
        DamagerAbilityMenu.SetActive(false);
        DefenderAbilityMenu.SetActive(false);
        HealerAbilityMenu.SetActive(false);
        SupporterAbilityMenu.SetActive(false);

        // show action menu
        ActionMenu.SetActive(true);

        Debug.Log($"{participants[turnIndex].Name} makes their turn");
    }

    // enable target selection
    public IEnumerator EnableTargetSelection()
    {
        foreach (Enemy enemy in enemies)
        {
            // enables the enemies selector to show up when hovering over them
            enemy.EnableSelector = true;
        }

        // enable target selection in update method
        this.state = GameState.PlayerTargetSelect;

        yield return new WaitUntil(() => this.selectedTarget != null);

        foreach (Enemy enemy in enemies)
        {
            // disables the enemies selector again
            enemy.EnableSelector = false;
        }
    }

    //--------------------------------------------------------------------------------------

    // Action menu buttons
    // Attack button
    public void OnAttackButton()
    {
        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        ActionMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        // play attack animation
        participants[turnIndex].Animator.SetTrigger("Attack");

        ((Player)participants[turnIndex]).Action_Attack(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    //--------------------------------------------------------------------------------------

    // Defend button
    public void OnDefendButton()
    {
        ActionMenu.SetActive(false); // deactivate action menu

        ((Player)participants[turnIndex]).Action_Defend();

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    //--------------------------------------------------------------------------------------

    // Ability buttons
    public void OnAbilityButton()
    {
        ActionMenu.SetActive(false); // deactivate action menu

        // activate the current player's ability menu
        if ((Player)participants[turnIndex] == damager)
        {
            DamagerAbilityMenu.SetActive(true);
        }
        else if ((Player)participants[turnIndex] == defender)
        {
            DefenderAbilityMenu.SetActive(true);
        }
        else if ((Player)participants[turnIndex] == healer)
        {
            HealerAbilityMenu.SetActive(true);
        }
        else
        {
            SupporterAbilityMenu.SetActive(true);
        }
    }

    //--------------------------------------------------------------------------------------

    // Damager ability buttons
    public void OnFireballAbility()
    {
        StartCoroutine(Damager_Fireball());
    }

    public IEnumerator Damager_Fireball()
    {
        DamagerAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        damager.Ability_Fireball(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    public void OnShitstormAbility()
    {
        DamagerAbilityMenu.SetActive(false); // deactivate action menu

        damager.Ability_Shitstorm();

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    //--------------------------------------------------------------------------------------

    // Defender ability buttons
    public void OnBlockAbility()
    {
        StartCoroutine(Defender_Block());
    }

    public IEnumerator Defender_Block()
    {
        DefenderAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        defender.Ability_Block(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    public void OnTauntAbility()
    {
        StartCoroutine(Defender_Taunt());
    }

    public IEnumerator Defender_Taunt()
    {
        DefenderAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        defender.Ability_Taunt(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    //--------------------------------------------------------------------------------------

    // Healer ability buttons
    public void OnHealAbility()
    {
        StartCoroutine(Healer_Heal());
    }

    public IEnumerator Healer_Heal()
    {
        HealerAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        healer.Ability_Heal(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    public void OnGrouphealAbility()
    {
        HealerAbilityMenu.SetActive(false); // deactivate action menu

        healer.Ability_Groupheal();

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    //--------------------------------------------------------------------------------------

    // Supporter ability buttons
    public void OnThrowGatoAbility()
    {
        StartCoroutine(Supporter_ThrowGato());
    }

    public IEnumerator Supporter_ThrowGato()
    {
        SupporterAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        supporter.Ability_ThrowGato(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    public void OnThrowPotionAbility()
    {
        StartCoroutine(Supporter_ThrowPotion());
    }

    public IEnumerator Supporter_ThrowPotion()
    {
        SupporterAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedTarget != null);

        supporter.Ability_ThrowPotion(this.selectedTarget);

        // mark turn as made and reset selectedTarget at the end of the turn
        this.turnMade = true;
        this.selectedTarget = null;
    }

    //--------------------------------------------------------------------------------------

    // Back button
    public void OnBackButton()
    {
        // hide menus
        DamagerAbilityMenu.SetActive(false);
        DefenderAbilityMenu.SetActive(false);
        HealerAbilityMenu.SetActive(false);
        SupporterAbilityMenu.SetActive(false);

        // show action menu
        ActionMenu.SetActive(true);
    }

    //-------------------------------------------------------------------------------------------------------|

    // Enemy's turn implementation
    public void EnemyTurn_Attack()
    {
        Debug.Log($"{participants[turnIndex].Name} makes their turn");

        // select random player to attack
        Player selectedPlayer = players[UnityEngine.Random.Range(0, players.Count)];
        selectedPlayer = defender;

        // play attack animation
        participants[turnIndex].Animator.SetTrigger("Attack");

        selectedPlayer.TakeDamage(participants[turnIndex], 1f);

        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    //-------------------------------------------------------------------------------------------------------|

    // death check and win conditions
    public void DeathCheck(List<Entity> participants)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            // if entity has 0 HP...
            if (participants[i].CurrentHP == 0)
            {
                Debug.Log($"{participants[i].Name} has been defeated");

                if (participants[i] is Enemy)
                {
                    // remove enemy from enemies list
                    enemies.Remove((Enemy)participants[i]);
                }
                else if (participants[i] is Player)
                {
                    // remove player from players list
                    players.Remove((Player)participants[i]);
                }

                // deactivate the entity in the scene
                participants[i].gameObject.SetActive(false);

                // remove entity from participants list
                participants.RemoveAt(i);

                CheckWinConditions();
            }
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
                // if there are any enemies alive, the players haven't won yet
                allEnemiesDefeated = false;
            }
            else if (entity is Player && entity.CurrentHP > 0)
            {
                // if there are any players alive, the enemies haven't won yet
                allPlayersDefeated = false;
            }
        }

        if (allEnemiesDefeated)
        {
            // if all enemies are defeated, the players win
            this.state = GameState.Victory;
            Debug.Log("The battle is won");

            // increase score by 1
            score++;
        }
        else if (allPlayersDefeated)
        {
            // if all players are defeated, the players lose
            this.state = GameState.Defeat;
            Debug.Log("The battle is lost");

            // save highscore if score is higher than current highscore
            if (score > highscore)
            {
                highscore = score;
                PlayerPrefs.SetInt("Highscore", highscore);
                Debug.Log($"New highscore: {highscore}");
            }

            GameOverScreen.SetActive(true);

            HighscoreText.text = $"Highscore: {highscore}";
            ScoreText.text = $"Score: {score}";
        }
    }
}

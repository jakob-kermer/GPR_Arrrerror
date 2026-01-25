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
    TargetSelect_Enemy,
    TargetSelect_Player,
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
    private Enemy selectedEnemy = null;
    private Player selectedPlayer = null;

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
        if (Input.GetMouseButtonDown(0)) // if the left mouse button is clicked and it's the player's turn
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // cast a ray from the camera to the cursor's position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && this.state == GameState.TargetSelect_Enemy) // if the ray hits an object
            {
                Enemy clickedEnemy = hit.transform.GetComponent<Enemy>(); // get the Enemy script component

                if (clickedEnemy != null) // if the object doesn't have an Enemy script, the following is skipped
                {
                    this.selectedEnemy = clickedEnemy; // the clicked enemy is the selected target
                }
            }
            else if (Physics.Raycast(ray, out hit) && this.state == GameState.TargetSelect_Player) // if the ray hits an object
            {
                Player clickedPlayer = hit.transform.GetComponent<Player>(); // get the Player script component

                if (clickedPlayer != null) // if the object doesn't have a Player script, the following is skipped
                {
                    this.selectedPlayer = clickedPlayer; // the clicked player is the selected target
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
            // end the effect of Taunt at the start of a new round
            defender.TauntUsed = false;

            for (turnIndex = 0; turnIndex < participants.Count; turnIndex++)
            {
                if (this.state == GameState.Defeat) // if the players are defeated...
                {
                    // game over screen is shown (see CheckWinConditions)
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

                    // show turn indicator above current player
                    participants[turnIndex].transform.GetChild(1).gameObject.SetActive(true);

                    PlayerTurn();

                    // wait until the player has made their turn
                    yield return new WaitUntil(() => this.turnMade);

                    // hide turn indicator above current player
                    participants[turnIndex].transform.GetChild(1).gameObject.SetActive(false);

                    // check if any participants have died
                    DeathCheck(this.participants);

                    // reset turnMade for next turn
                    this.turnMade = false;

                    yield return new WaitForSeconds(1f);
                }
                else if (this.participants[turnIndex] is Enemy)
                {
                    this.state = GameState.EnemyTurn;
                    EnemyTurn_Attack();

                    // show turn indicator above current enemy
                    participants[turnIndex].transform.GetChild(1).gameObject.SetActive(true);

                    // wait until the enemy has made their turn
                    yield return new WaitUntil(() => this.turnMade);

                    // hide turn indicator above current enemy
                    participants[turnIndex].transform.GetChild(1).gameObject.SetActive(false);

                    // check if any participants have died
                    DeathCheck(this.participants);

                    // reset turnMade for next turn
                    this.turnMade = false;

                    yield return new WaitForSeconds(1.5f);
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
        // sort participants by speed + a random factor (between -4 & 4)
        this.participants.Sort((x, y) => (y.Speed + UnityEngine.Random.Range(-4, 5)).CompareTo(x.Speed + UnityEngine.Random.Range(-4, 5)));

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
    public IEnumerator EnableTargetSelection_Enemy()
    {
        foreach (Enemy enemy in enemies)
        {
            // enables the enemies selector to show up when hovering over them
            enemy.EnableSelector = true;
        }

        // enable target selection in update method
        this.state = GameState.TargetSelect_Enemy;

        yield return new WaitUntil(() => this.selectedEnemy != null);

        foreach (Enemy enemy in enemies)
        {
            // disables the enemies selector again
            enemy.EnableSelector = false;
        }
    }

    public IEnumerator EnableTargetSelection_Player()
    {
        foreach (Player player in players)
        {
            // enables the players selector to show up when hovering over them
            player.EnableSelector = true;
        }

        // enable target selection in update method
        this.state = GameState.TargetSelect_Player;

        yield return new WaitUntil(() => this.selectedPlayer != null);

        foreach (Player player in players)
        {
            // disables the players selector again
            player.EnableSelector = false;
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

        StartCoroutine(EnableTargetSelection_Enemy());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedEnemy != null);

        // play attack animation
        participants[turnIndex].Animator.SetTrigger("Attack");

        ((Player)participants[turnIndex]).Action_Attack(this.selectedEnemy);

        // mark turn as made and reset selectedEnemy at the end of the turn
        this.turnMade = true;
        this.selectedEnemy = null;
    }

    //--------------------------------------------------------------------------------------

    // Defend button
    public void OnDefendButton()
    {
        ActionMenu.SetActive(false); // deactivate action menu

        ((Player)participants[turnIndex]).Action_Defend();

        // mark turn as made and reset selectedEnemy at the end of the turn
        this.turnMade = true;
        this.selectedEnemy = null;
    }

    //--------------------------------------------------------------------------------------

    // Ability buttons
    public void OnAbilityButton()
    {
        ActionMenu.SetActive(false); // deactivate action menu

        // activate the current player's ability menu
        // damager ability menu
        if ((Player)participants[turnIndex] == damager)
        {
            DamagerAbilityMenu.SetActive(true);
        }
        // defender ability menu
        else if ((Player)participants[turnIndex] == defender)
        {
            DefenderAbilityMenu.SetActive(true);

            // if the defender is blocking...
            if (defender.IsBlocking == true)
            {
                // ...deactivate the Block ability button
                DefenderAbilityMenu.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                // ...otherwise activate it
                DefenderAbilityMenu.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        // healer ability menu
        else if ((Player)participants[turnIndex] == healer)
        {
            HealerAbilityMenu.SetActive(true);
        }
        // supporter ability menu
        else if ((Player)participants[turnIndex] == supporter)
        {
            SupporterAbilityMenu.SetActive(true);
        }
    }

    //--------------------------------------------------------------------------------------

    // Damager ability buttons
    public void OnFireballAbility()
    {
        // check if player has enough AP
        if (damager.CurrentAP >= damager.Fireball_APCost)
        {
            StartCoroutine(Damager_Fireball());
        }
    }

    public IEnumerator Damager_Fireball()
    {
        DamagerAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection_Enemy());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedEnemy != null);

        damager.Ability_Fireball(this.selectedEnemy);

        // mark turn as made and reset selectedEnemy at the end of the turn
        this.turnMade = true;
        this.selectedEnemy = null;
    }

    public void OnShitstormAbility()
    {
        // check if player has enough AP
        if (damager.CurrentAP >= damager.Shitstorm_APCost)
        {
            DamagerAbilityMenu.SetActive(false); // deactivate action menu

            damager.Ability_Shitstorm(enemies);


            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
    }

    //--------------------------------------------------------------------------------------

    // Defender ability buttons
    public void OnBlockAbility()
    {
        // check if player has enough AP
        if (defender.CurrentAP >= defender.Block_APCost)
        {
            DefenderAbilityMenu.SetActive(false); // deactivate action menu

            defender.Ability_Block();

            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
    }

    public void OnTauntAbility()
    {
        // check if player has enough AP
        if (defender.CurrentAP >= defender.Taunt_APCost)
        {
            DefenderAbilityMenu.SetActive(false); // deactivate action menu
            defender.Ability_Taunt();

            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
    }

    //--------------------------------------------------------------------------------------

    // Healer ability buttons
    public void OnHealAbility()
    {
        // check if player has enough AP
        if (healer.CurrentAP >= healer.Heal_APCost)
        {
            StartCoroutine(Healer_Heal());
        }
    }

    public IEnumerator Healer_Heal()
    {
        HealerAbilityMenu.SetActive(false); // deactivate action menu

        StartCoroutine(EnableTargetSelection_Player());

        // wait until a target has been selected
        yield return new WaitUntil(() => this.selectedPlayer != null);

        healer.Ability_Heal(this.selectedPlayer);

        // mark turn as made and reset selectedPlayer at the end of the turn
        this.turnMade = true;
        this.selectedPlayer = null;
    }

    public void OnGrouphealAbility()
    {
        // check if player has enough AP
        if (healer.CurrentAP >= healer.GroupHeal_APCost)
        {
            HealerAbilityMenu.SetActive(false); // deactivate action menu

            healer.Ability_Groupheal(this.players);

            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
    }

    //--------------------------------------------------------------------------------------

    // Supporter ability buttons
    public void OnThrowGatoAbility()
    {
        // check if player has enough AP
        if (supporter.CurrentAP >= supporter.ThrowGato_APCost)
        {
            SupporterAbilityMenu.SetActive(false); // deactivate action menu

            supporter.Ability_ThrowGato(this.enemies);

            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
    }

    public void OnThrowPotionAbility()
    {
        // check if player has enough AP
        if (supporter.CurrentAP >= supporter.ThrowPotion_APCost)
        {
            SupporterAbilityMenu.SetActive(false); // deactivate action menu

            supporter.Ability_ThrowPotion(this.players);

            // mark turn as made at the end of the turn
            this.turnMade = true;
        }
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

        if (defender.TauntUsed == true)
        {
            Player selectedPlayer = defender;
            selectedPlayer.TakeDamage(participants[turnIndex], 1f);
            participants[turnIndex].Animator.SetTrigger("Attack"); // play attack animation
        }
        else // select random player to attack
        {
            Player selectedPlayer = players[UnityEngine.Random.Range(0, players.Count)];
            selectedPlayer.TakeDamage(participants[turnIndex], 1f);
            participants[turnIndex].Animator.SetTrigger("Attack"); // play attack animation
        }
        // mark turn as made at the end of the turn
        this.turnMade = true;
    }

    //-------------------------------------------------------------------------------------------------------|

    // death check and win conditions
    public void DeathCheck(List<Entity> participants)
    {
        // create a list for all entites who died
        List<Entity> corpsePile = new List<Entity>();

        for (int i = 0; i < participants.Count; i++)
        {
            // if entity has 0 HP...
            if (participants[i].CurrentHP == 0)
            {
                Debug.Log($"{participants[i].Name} has been defeated");

                // ...add it to the corpse pile
                corpsePile.Add(participants[i]);

                // if the dead entity is an enemy
                if (participants[i] is Enemy)
                {
                    // ...remove enemy from enemies list
                    enemies.Remove((Enemy)participants[i]);
                }
                // ...or if the dead entity is a player
                else if (participants[i] is Player)
                {
                    // ...or remove player from players list
                    players.Remove((Player)participants[i]);
                }

                // deactivate the entity in the scene
                participants[i].gameObject.SetActive(false);
            }
        }

        foreach (Entity corpse in corpsePile)
        {
            // remove all entities in the corpse pile from participants list
            participants.Remove(corpse);
        }

        CheckWinConditions();
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

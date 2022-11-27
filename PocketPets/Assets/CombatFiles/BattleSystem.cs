using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    Pet player;
    Pet enemy;
    public GameObject playerGameObject;
    public GameObject enemyGameObject;
    public GameObject playerHealth;
    public GameObject enemyHealth;
    public List<Sprite> itemSprites;
    public GameObject item;
    int index;
    public GameObject EndGameDialog;
    public GameObject CombatPanel;
    public TextMeshProUGUI ResultText;
    public GameObject playerManeBar;
    public GameObject enemyManaBar;
    public GameObject manaHealthTexts;
    public List<Sprite> sprites;

    [SerializeField] Animator animator1;
    [SerializeField] Animator animator2;
    [SerializeField] Animator animator3;
    [SerializeField] Animator animator4;
    [SerializeField] Animator animator5;
    [SerializeField] Animator animator6;

    [SerializeField] TextMeshProUGUI EnemyHP;
    [SerializeField] TextMeshProUGUI PlayerHP;

    private float playerAttack;

    void Start()
    {
        Debug.Log(sprites.Count);
        if(!DataTransfer.isTutorial)
        {
            GameObject.Find("enemySprite").GetComponent<SpriteRenderer>().sprite = sprites[DataTransfer.currentEnemyIndex];
            itemSprites = DataTransfer.itemSprites;
        }

        playerManeBar.GetComponent<Slider>().value = 0f;
        enemyManaBar.GetComponent<Slider>().value = 0f;

        state = BattleState.START;
        player = playerGameObject.GetComponent<Pet>();
        enemy = enemyGameObject.GetComponent<Pet>();

        state = BattleState.PLAYERS_TURN;

        index = 0;
        item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];

        EndGameDialog.SetActive(false);
    }
    
    IEnumerator Turn()
    {
        if(state == BattleState.PLAYERS_TURN)
        {
            state = BattleState.ENEMYS_TURN;
            enemy.TakeDamage(playerAttack);
            EnemySetHealthPoints(enemy.health);
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            animator1.SetBool("EnemyHurt", true);
            enemyHealth.GetComponent<Slider>().value = enemy.health;
            yield return new WaitForSeconds(1f);
            animator1.SetBool("EnemyHurt", false);
            yield return new WaitForSeconds(2f);
            if(enemy.health <= 0)
            {
                state = BattleState.WON;
                ShowEndGameDialog();
            }
        }
        if(state != BattleState.WON)
        {
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        System.Random random = new System.Random();
        int number = random.Next(1, 100);
        //If the enemies health less then 50 and the enemy has a healing item it uses it
        if(enemy.health <= 50 && enemy.items.Where(x=>x.Contains("h")).Count() >= 1)
        {
            if (enemy.mana < 3f)
            {
                enemy.mana++;
                enemyManaBar.GetComponent<Slider>().value = enemy.mana;
            }

            string item = enemy.items.Where(x => x.Contains("h")).ToList()[0].ToString();
            enemy.UseItem(item);
            enemy.items.Remove(item);
            EnemySetHealthPoints(enemy.health);
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
            animator3.SetBool("EnemyHealing", true);
            enemyHealth.GetComponent<Slider>().value = enemy.health;
            yield return new WaitForSeconds(2f);
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            animator3.SetBool("EnemyHealing", false);
        } // If the random number is less then 50 and enemy has attack modider it uses it
        else if (number<= 40 && enemy.items.Where(x=>x.Contains("a")).Count() >= 1)
        {
            if (enemy.mana < 3f)
            {
                enemy.mana++;
                enemyManaBar.GetComponent<Slider>().value = enemy.mana;
            }

            string item = enemy.items.Where(x => x.Contains("a")).ToList()[0].ToString();
            enemy.UseItem(item);
            enemy.items.Remove(item);
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
            animator5.SetBool("EnemyBoost", true);
            yield return new WaitForSeconds(2f);
            animator5.SetBool("EnemyBoost", false);
            enemyGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        }//Enemy attacks
        else
        {
            if(enemy.mana == 3f)
            {
                enemy.mana = 0f;
                enemyManaBar.GetComponent<Slider>().value = enemy.mana;

                player.TakeDamage(enemy.getUltimate());
                PlayerSetHealthPoints(player.health);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                animator2.SetBool("PlayerHurt", true);
                playerHealth.GetComponent<Slider>().value = player.health;
                yield return new WaitForSeconds(1f);
                animator2.SetBool("PlayerHurt", false);
                yield return new WaitForSeconds(2f);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            } else if(enemy.mana == 2f && number%2==0)
            {
                enemy.mana = 0f;
                enemyManaBar.GetComponent<Slider>().value = enemy.mana;

                player.TakeDamage(enemy.getAdvanced());
                PlayerSetHealthPoints(player.health);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                animator2.SetBool("PlayerHurt", true);
                playerHealth.GetComponent<Slider>().value = player.health;
                yield return new WaitForSeconds(1f);
                animator2.SetBool("PlayerHurt", false);
                yield return new WaitForSeconds(2f);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            } 
            else
            {
                enemy.mana++;
                enemyManaBar.GetComponent<Slider>().value = enemy.mana;

                player.TakeDamage(enemy.GetAttack());
                PlayerSetHealthPoints(player.health);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                animator2.SetBool("PlayerHurt", true);
                playerHealth.GetComponent<Slider>().value = player.health;
                yield return new WaitForSeconds(1f);
                animator2.SetBool("PlayerHurt", false);
                yield return new WaitForSeconds(2f);
                playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        
        if (player.health <= 0)
        {
            state = BattleState.LOST;
            ShowEndGameDialog();
        }
        else
        {
            state = BattleState.PLAYERS_TURN;
        }
    }

    public void ClickOnAttackButton()
    {
        if(state == BattleState.PLAYERS_TURN)
        {
            playerAttack = player.GetAttack();
            if(player.mana < 3f)
            {
                player.mana++;
                playerManeBar.GetComponent<Slider>().value = player.mana;
            }
            StartCoroutine(Turn());
        }
    }
    
    public void ClickOnNextItemButton()
    {
        if(player.items.Count() != 0)
        {
            if(index+1 >= player.items.Count())
            {
                index = 0;
                item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];
            }
            else
            {
                index++;
                item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];
            }
        }
    }

    public void ClickOnPreviousItemButton()
    {
        if(player.items.Count() != 0)
        {
            if(index-1 < 0)
            {
                index = player.items.Count-1;
                item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];
            }
            else
            {
                index--;
                item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];
            }
        }
    }

    public void ClickOnUseItemButton()
    {
        if(player.items.Count() != 0 && state == BattleState.PLAYERS_TURN)
        {
            state = BattleState.ENEMYS_TURN;
            if (player.mana < 3f)
            {
                player.mana++;
                playerManeBar.GetComponent<Slider>().value = player.mana;
            }
            StartCoroutine(UseItem());
        }
    }

    IEnumerator UseItem()
    {
        //Selected Item
        string item = player.items[index];
        
        //Decides which type of modifier did player select and use
        if (player.items[index].Contains("a"))
        {   
            playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
            animator6.SetBool("PlayerBoost", true);
            player.UseItem(item);
            player.items.Remove(item);
            itemSprites.RemoveAt(index);
        }
        else
        {
            playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
            animator4.SetBool("PlayerHealing", true);
            player.UseItem(item);
            player.items.Remove(item);
            playerHealth.GetComponent<Slider>().value = player.health;
            PlayerSetHealthPoints(player.health);
            itemSprites.RemoveAt(index);
        }

        //Resetting the player object color
        yield return new WaitForSeconds(2f);
        playerGameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        animator6.SetBool("PlayerBoost", false);
        animator4.SetBool("PlayerHealing", false);

        index = 0;
        state = BattleState.ENEMYS_TURN;
        
        //Check if we have anymore items
        if(player.items.Count() == 0)
        {
            this.item.SetActive(false);
        }
        else
        {
            this.item.GetComponent<SpriteRenderer>().sprite = itemSprites[index];
        }

        //Starts enemy turn
        yield return new WaitForSeconds(2f);
        StartCoroutine(Turn());
    }

    void ShowEndGameDialog()
    {
        playerGameObject.SetActive(false);
        enemyGameObject.SetActive(false);
        CombatPanel.SetActive(false);
        EndGameDialog.SetActive(true);
        playerHealth.SetActive(false);
        enemyHealth.SetActive(false);
        playerManeBar.SetActive(false);
        enemyManaBar.SetActive(false);
        manaHealthTexts.SetActive(false);
        if(state == BattleState.WON)
        {
            if (DataTransfer.defeatedEnemies.Where(x => x == true).Count() == DataTransfer.defeatedEnemies.Length)
            {
                ResultText.text = "You won the game!";
            }
            else
            {
                ResultText.text = "You won the battle!";
            }

            if(!DataTransfer.isTutorial)
            {
                DataTransfer.defeatedEnemies[DataTransfer.currentEnemyIndex] = true;
            }
        }
        else
        {
            ResultText.text = "You lost the battle!";
        }
    }

    public void ClickOnContinueButton()
    {
        if (DataTransfer.defeatedEnemies.Where(x => x == true).Count() == DataTransfer.defeatedEnemies.Length)
        {
            Invoke("LoadMenuIfGameIsWon", 3f);
        } 
        else
        {
            Invoke("LoadGame", 3f);
        }
    }

    public void clickOnAdvancedButton()
    {
        if (state == BattleState.PLAYERS_TURN && player.mana >= 2f)
        {
            playerAttack = player.getAdvanced();
            player.mana -= 2f;
            playerManeBar.GetComponent<Slider>().value = player.mana;
            StartCoroutine(Turn());
        }
    }

    public void clickOnUltimateButton()
    {
        if (state == BattleState.PLAYERS_TURN && player.mana == 3f)
        {
            playerAttack = player.getUltimate();
            player.mana = 0;
            playerManeBar.GetComponent<Slider>().value = player.mana;
            StartCoroutine(Turn());
        }
    }

    public void PlayerSetHealthPoints(float health)
    {
        PlayerHP.text = health.ToString("0");
    }

    public void EnemySetHealthPoints(float health)
    {
        EnemyHP.text = health.ToString("0");
    }

    //Akt�vra �ll�tja a fader-t sceenre val� bel�p�skor
    //(gyakran inakt�vra lett �ll�tva mert kitakarja ak�pet �s nem lehet vele dolgozni olyankor, viszont �gy meg inakt�von lett sokszor felejtve)
    public GameObject Fader;
    private void Awake()
    {
        Fader.SetActive(true);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    void LoadMenuIfGameIsWon()
    {
        SceneManager.LoadScene("Menu");
    }
}

public enum BattleState
{
    START,
    PLAYERS_TURN,
    ENEMYS_TURN,
    WON,
    LOST
}



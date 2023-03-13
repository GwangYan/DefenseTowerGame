using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum gameStatus
{
    pause, play, gameover, win
}
public class GameManager : Singleton<GameManager> {
    //SerializeField - Allows Inspector to get access to private fields.
    //If we want to get access to this from another class, we'll just need to make public getters
    [SerializeField]
    private int totalWaves;
    [SerializeField]
    private int totalHealthes;
    [SerializeField]
    private TextMeshProUGUI totalMoneyLabel;   //Refers to money label at upper left corner
    [SerializeField]
    private TextMeshProUGUI currentWaveLabel;
    [SerializeField]
    private TextMeshProUGUI totalEscapedLabel;
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;
    [SerializeField]
    private int totalEnemies = 3;
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private TextMeshProUGUI GameStatusLabel;
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button pauseButton;

    private int waveNumber = 0;
    private int totalMoney = 500;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private int enemiesToSpawn = 0;
    private AudioSource audioSource;

    public gameStatus currentState;// = gameStatus.play;

    public List<Enemy> EnemyList = new List<Enemy>();
    const float spawnDelay = 2f; //Spawn Delay in seconds

    public int TotalMoney
    {
        get { return totalMoney; }
        set
        {
            totalMoney = value;
            totalMoneyLabel.text = totalMoney.ToString();
        }
    }

    public int TotalEscape
    {
        get { return totalEscaped; }
        set { totalEscaped = value; }
    }
 
    public int RoundEscaped
    {
        get { return roundEscaped; }
        set { roundEscaped = value; }
    }
    public int TotalKilled
    {
        get { return totalKilled; }
        set { totalKilled = value; }
    }

    public AudioSource AudioSource
    {
        get { return audioSource; }
    }
    
    // Use this for initialization
    void Start () {
        currentState = gameStatus.play;
        audioSource = GetComponent<AudioSource>();
        ShowMenu();
	}
	
	// Update is called once per frame
	void Update () {
        handleEscape();
	}

    //This will spawn enemies, wait for the given spawnDelay then call itself again to spawn another enemy
    int numberOfEnemy = 0;
    IEnumerator spawn()
    {
        if (enemiesPerSpawn > 0 && numberOfEnemy < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (numberOfEnemy < totalEnemies)
                {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]);
                    newEnemy.transform.position = spawnPoint.transform.position;
                    numberOfEnemy++;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(spawn());
        }
    }

    ///Register - when enemy spawns
    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }
    ///Unregister - When they escape the screen
    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    ///Destroy - At the end of the wave
    public void DestroyAllEnemies()
    {
        foreach(Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }
        EnemyList.Clear();
    }

    public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }

    public void SubtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void isWaveOver()
    {
        totalEscapedLabel.text = (totalHealthes - TotalEscape).ToString();
        setCurrentGameState();
        ShowMenu();
    }

    public void setCurrentGameState()
    {
        if ((totalHealthes - totalEscaped) <= 0)
        {
            currentState = gameStatus.gameover;
        }
        else if(waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
        {
            currentState = gameStatus.play;
        }
        else if((totalHealthes - totalEscaped) > 0 && (TotalKilled + RoundEscaped) == totalEnemies)
        {
            currentState = gameStatus.win;
        }
        else
        {
            currentState = gameStatus.play;
        }

        Debug.Log("currentState: " + currentState);
    }

    public void ShowMenu()
    {
        switch (currentState)
        {
            case gameStatus.play:
                break;
            case gameStatus.gameover:
                GameStatusLabel.text = "Game Ended";
                AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                DestroyAllEnemies();
                pauseButton.gameObject.SetActive(false);
                playButton.gameObject.SetActive(false);
                break;
            case gameStatus.pause:
                GameStatusLabel.text = "Paused";
                break;
            case gameStatus.win:
                GameStatusLabel.text = "You are win";
                pauseButton.gameObject.SetActive(false);
                playButton.gameObject.SetActive(true);
                break;
        }
    }
    public void playButtonPressed()
    {
        if(currentState != gameStatus.pause)
        {
            DestroyAllEnemies();
            TotalKilled = 0;
            RoundEscaped = 0;
            //TowerManager.Instance.RenameTagsBuildSites();
            currentWaveLabel.text = (waveNumber + 1) + "/5";
            StartCoroutine(spawn());
            playButton.gameObject.SetActive(false);
        }

        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        currentState = gameStatus.play;
        ShowMenu();
    }

    public void pauseButtonPressed()
    {
        pauseButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        currentState = gameStatus.pause;
        ShowMenu();
    }
    private void handleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.towerButtonPressed = null;
        }
    }

    public int getHealth()
    {
        return (totalHealthes - totalEscaped);
    }
}

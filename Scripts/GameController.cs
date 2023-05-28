using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    //assigned in inspector
    public GameObject playerPrefab;
    public GameObject recoveryPrefab;
    public GameObject rerollPrefab;
    public GameObject upgradePrefab;
    public GameObject[] enemyPrefabs;
    public float shooterChance = 0.7f;
    public int level = 1;


    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public int healthLevel, attackLevel, attackSpeedLevel, projSpeedLevel, movSpeedLevel, reductionFlatLevel, coins;
    [HideInInspector] public GameObject playerObject { get; set; }
    [HideInInspector] public int destroyCount { get; set; }
    [HideInInspector] public bool isPaused { get; private set; }
    [HideInInspector] public bool isPlaying { get; set; } = false;
    [HideInInspector] public string saveFilePath { get; private set; }
    [HideInInspector] public SaveData saveFile { get; private set; }

    void Awake()
    {
        //singleton script
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        
        //get persistent data path
        saveFilePath = Application.persistentDataPath + "/gamedata.data";
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //read save data on game start
        ReadSaveData();
        UIMainMenu.instance.UpdateContinueButton(saveFile.inGame); //update the continue button on main screen
    }

    //reads save data from disk
    void ReadSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                //read save data if it exists
                BinaryFormatter converter = new BinaryFormatter();
                FileStream dataStream = new FileStream(saveFilePath, FileMode.Open);
                saveFile = converter.Deserialize(dataStream) as SaveData;
                dataStream.Close();

                //read from save file
                healthLevel = saveFile.health;
                attackLevel = saveFile.attack;
                attackSpeedLevel = saveFile.attackSpeed;
                projSpeedLevel = saveFile.projSpeed;
                movSpeedLevel = saveFile.movSpeed;
                reductionFlatLevel = saveFile.reductionFlat;
                coins = saveFile.coins;
            }
            catch (Exception e)
            {
                //create new save data if it is corrupted
                Debug.LogError(e);
                saveFile = new SaveData();
            }
        }
        else
        {
            //create new save data if none is found
            saveFile = new SaveData();
        }
    }

    //write save data to disk
    public void WriteSaveData()
    {
        //write save data
        BinaryFormatter converter = new BinaryFormatter();
        FileStream dataStream = new FileStream(saveFilePath, FileMode.Create);
        converter.Serialize(dataStream, saveFile);
        dataStream.Close();
    }

    //spawn the player object on position
    public void spawnPlayer(Vector2 position)
    {
        //spawn player at position
        playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
        playerObject.GetComponent<PlayerMovement>().Calibrate();
        CameraController.instance.SetCameraFollow(playerObject.transform);
    }

    //spawn recovery object on position
    public void spawnRecovery(Vector2 position)
    {
        Instantiate(recoveryPrefab, position, Quaternion.identity);
    }

    //spawn roll object on position
    public void spawnReroll(Vector2 position)
    {
        Instantiate(rerollPrefab, position, Quaternion.identity);
    }

    //spawn upgrade object on position
    public void spawnUpgrade(Vector2 position)
    {
        Instantiate(upgradePrefab, position, Quaternion.identity);
    }

    //spawn enemy object on position
    //also determine the enmy type with the provided random float from BoardController
    public void spawnEnemy(Vector2 position, float type)
    {
        float enemyType = type;
        if (enemyType < shooterChance)
        {
            GameObject enemy = Instantiate(enemyPrefabs[0], position, Quaternion.identity);
            enemy.GetComponent<EnemyController>().InitializeStats(new EnemyShooter());
        }
        else
        {
            GameObject enemy = Instantiate(enemyPrefabs[1], position, Quaternion.identity);
            enemy.GetComponent<EnemyController>().InitializeStats(new EnemyRammer());
        }
    }

    //called when pressing start button on main menu
    public void startGame()
    {
        isPlaying = true;
        PlayerController.instance.Initialize();
        PlayerController.instance.InitializeNewGame();
        SceneManager.LoadScene("LevelScene");  
        StartCoroutine(EnterNewLevel(-1, true));
    }

    //called when pressing continue button on main menu
    public void startGameFromSave()
    {
        isPlaying = true;
        level = saveFile.level;
        destroyCount = saveFile.destroyCount;
        PlayerController.instance.InitializeFromSave(saveFile);
        SceneManager.LoadScene("LevelScene");
        StartCoroutine(EnterNewLevel(saveFile.seed, true));
    }

    //called when pressing pause button in game
    public void pauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        if (UIIngame.instance != null) UIIngame.instance.gameObject.SetActive(false);
        if (UIPause.instance != null) UIPause.instance.gameObject.SetActive(true);
    }
    
    //caled when pressing continue button in pause menu in game
    public void resumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (UIIngame.instance != null) UIIngame.instance.gameObject.SetActive(true);
        if (UIPause.instance != null) UIPause.instance.gameObject.SetActive(false);
    }

    //called when pressing next level in upgrade menu
    public void NextLevel()
    {
        level++;
        PlayerController.instance.UpdateStats();
        SceneManager.LoadScene("LevelScene");
        StartCoroutine(EnterNewLevel(-1, false));
    }

    //called when the player touches the portal (goal)
    public void TouchPortal()
    {
        StartUpgrade(); //start upgrading
    }
    void StartUpgrade()
    {
        pauseGame();
        UIUpgrade.instance.gameObject.SetActive(true);   
    }

    //called when the main menu button is pressed in the end screen (after the player dies)
    public void FinishGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (UIIngame.instance != null) UIIngame.instance.gameObject.SetActive(false);
        if (UIPause.instance != null) UIPause.instance.gameObject.SetActive(false);
        SceneManager.LoadScene("MenuScene");
    }

    //called when main menu button is pressed in the pause menu in game
    public void MainMenu()
    {
        resumeGame();
        isPlaying = false;

        //save game data to variable
        saveFile.inGame = true;
        saveFile.level = level;
        saveFile.seed = BoardController.instance.seed;
        saveFile.currentHealth = PlayerController.instance.healthOnLevelEnter;
        saveFile.maxHealth = PlayerController.instance.maxHealth;
        saveFile.upgradePoint = PlayerController.instance.upgradePointOnEnter;
        saveFile.rerollPoint = PlayerController.instance.rerollPointOnEnter;
        saveFile.augments = PlayerController.instance.augments;
        saveFile.destroyCount = destroyCount;

        //write variable to drive
        WriteSaveData();

        SceneManager.LoadScene("MenuScene");
    }

    //called between each level
    IEnumerator EnterNewLevel(int seed, bool firstEnter)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        BoardController.instance.initializeLevel(seed); //generate the level map
        UIHealthBar.instance.SetValue(PlayerController.instance.health / (float) PlayerController.instance.maxHealth);
        if (firstEnter) PlayerMovement.instance.Calibrate(); //calibrate the movement if the level is loaded right after main menu
        yield return new WaitForSecondsRealtime(1f);
        resumeGame();

        //save game data to variable
        saveFile.inGame = true;
        saveFile.level = level;
        saveFile.seed = BoardController.instance.seed;
        saveFile.currentHealth = PlayerController.instance.healthOnLevelEnter;
        saveFile.maxHealth = PlayerController.instance.maxHealth;
        saveFile.upgradePoint = PlayerController.instance.upgradePointOnEnter;
        saveFile.rerollPoint = PlayerController.instance.rerollPointOnEnter;
        saveFile.augments = PlayerController.instance.augments;
        saveFile.damageDealt = PlayerController.instance.damageDealtOnEnter;
        saveFile.damageTaken = PlayerController.instance.damageTakenOnEnter;
        saveFile.damageHealed = PlayerController.instance.damageHealedOnEnter;
        saveFile.destroyCount = destroyCount;

        //write variable to drive
        WriteSaveData();
    }
}

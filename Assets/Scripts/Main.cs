using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject mapCam;
    public GameObject mainMenuCanvas;
    public GameObject pauseMenuCanvas;

    public int ennemyCount = 0;
    public int maxEnnemyCount = 25;

    private GameObject credits;

    bool loaded = false;
    Scene scene;

    public List<GameObject> players;
    int playerCount = 0;

    public bool Paused
    {
        get { return pauseMenuCanvas.activeSelf; }
    }

    void Start()
    {
        StartMainMenu();
    }

    void CheckPluggedPads()
    {
        var gamepads = Gamepad.all;

        for (int i = 0; i < 4; ++i)
        {
            GameObject player = players[i];

            GameObject buttonP = GameObject.Find("Button " + i + "P");

            if (null == buttonP || null == player || player.GetComponent<Player>().playerIndex >= gamepads.Count)
            {
                if (null != buttonP)
                {
                    buttonP.GetComponent<Button>().interactable = false;
                    buttonP.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
                }
                continue;
            }
            else
            {
                if (null != buttonP)
                {
                    buttonP.GetComponent<Button>().interactable = true;
                    buttonP.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    public void StartMainMenu()
    {
        if (null != menuCam)
            menuCam.SetActive(true);

        if (null != mapCam)
            mapCam.SetActive(false);

        if (null != mainMenuCanvas)
            mainMenuCanvas.SetActive(true);

        if (null != pauseMenuCanvas)
            pauseMenuCanvas.SetActive(false);

        CheckPluggedPads();

        for (int i = 0; i < 4; ++i)
        {
            GameObject player = players[i];

            GameObject buttonP = GameObject.Find("Button " + i + "P");

            if (null == player)
                continue;

            player.transform.Find("PlayerCanvas").gameObject.SetActive(false);

            string playerCameraName = "Player " + i + " Camera";
            GameObject playerCam = GameObject.Find(playerCameraName);
            if (null == playerCam)
                Debug.Log(playerCameraName + " not found");
            else
                playerCam.SetActive(false);
        }

        if (null == credits)
        {
            credits = GameObject.Find("Staff");
        }
        if (null != credits)
            credits.SetActive(false);

        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        GameObject button1P = GameObject.Find("Button 1P");
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button1P);

        // reinit players
        for (int i = 0; i < players.Count; ++i)
        {
            GameObject player = players[i];
            if (null != player)
            {
                Player playerAI = player.GetComponent<Player>();
                playerAI.Reinit();
                player.SetActive(true);
            }
        }
    }

    void initGame()
    {
        if (null != menuCam)
            menuCam.SetActive(false);

        if (null != credits)
            credits.SetActive(false);

        if (null != mapCam)
            mapCam.SetActive(true);

        if (null != mainMenuCanvas)
            mainMenuCanvas.SetActive(false);

        for (int i = 0; i < playerCount; ++i)
        {
            GameObject player = players[i];

            if (null == player)
                continue;

            var playerAI = player.GetComponent<Player>();
            if (null != playerAI)
            {
                playerAI.SetupPlayerViewportAndCamera(player, i, playerCount);
                playerAI.update = true;
            }

            player.transform.Find("PlayerCanvas").gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPluggedPads();

        if (loaded)
        {
            initGame();
            loaded = false;
        }
    }

    public void StartGame1Player()
    {
        StartGame(1);
    }

    public void StartGame2Players()
    {
        StartGame(2);
    }

    public void StartGame3Players()
    {
        StartGame(3);
    }

    public void ShowCredits()
    {
        if (credits.activeSelf == true)
            credits.SetActive(false);
        else
            credits.SetActive(true);
    }

    public void Resume()
    {
        ExitPauseMenu();
    }

    public void NewGame()
    {
        // kill all ennemies
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");

        for (int i = 0; i < ennemies.Length; ++i)
        {
            GameObject ennemy = ennemies[i];
            var en = ennemy.GetComponent<Ennemy>();
            if (null != en)
                en.Die(); ;
        }

        StartMainMenu();
    }

    private void StartGame(int _playerCount)
    {
        Time.timeScale = 1.0f;

        for (int i = 0; i < players.Count; ++i)
        {
            GameObject player = players[i];
            
            if (null != player)
            {
                if (i < _playerCount)
                    player.SetActive(true);
                else
                    player.SetActive(false);
            }
        }

        string name = "A600Motherboard";
        scene = SceneManager.GetSceneByName(name);
        playerCount = _playerCount;
        loaded = true;
        SceneManager.SetActiveScene(scene);

        // spawn stuff
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Respawn");
        for (int i = 0; i < spawners.Length; ++i)
        {
            GameObject spawner = spawners[i];
            spawner.GetComponent<SpawnPoint>().spawn = true;
        }
    }

    public void EnterPauseMenu()
    {
        if (pauseMenuCanvas.activeSelf == false)
        {
            pauseMenuCanvas.SetActive(true);

            EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            GameObject resume = GameObject.Find("Button Resume");
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(resume);
            Time.timeScale = 0.0f;
        }
    }

    public void ExitPauseMenu()
    {
        if (pauseMenuCanvas.activeSelf == true)
        {
            pauseMenuCanvas.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}

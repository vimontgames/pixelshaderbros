using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject canvas;
    public GameObject mapCam;

    private GameObject credits;

    bool loaded = false;
    Scene scene;

    public List<GameObject> players;
    int playerCount = 0;

    void Start()
    {
        if (null != menuCam)
            menuCam.SetActive(true);

        if (null != mapCam)
            mapCam.SetActive(false);

        if (null != canvas)
            canvas.SetActive(true);

        for (int i = 0; i < 4; ++i)
        {
            GameObject player = players[i];
            if (null == player)
                continue;

            player.transform.Find("Canvas").gameObject.SetActive(false);

            string playerCameraName = "Player " + i + " Camera";
            GameObject playerCam = GameObject.Find(playerCameraName);
            if (null == playerCam)
                Debug.Log(playerCameraName + " not found");
            else
                playerCam.SetActive(false);
        }

        credits = GameObject.Find("Staff");
        credits.SetActive(false);
    }

    void initGame()
    {
        if (null != menuCam)
            menuCam.SetActive(false);

        if (null != credits)
            credits.SetActive(false);

        if (null != mapCam)
            mapCam.SetActive(true);

        if (null != canvas)
            canvas.SetActive(false);

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

            player.transform.Find("Canvas").gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
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

    private void StartGame(int _playerCount)
    {
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
    }
}

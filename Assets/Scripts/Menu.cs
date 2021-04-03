using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject cam;
    public GameObject canvas;

    bool loaded = false;
    Scene scene;

    public List<GameObject> players;
    int playerCount = 0;

    void Start()
    {
        if (null != cam)
            cam.SetActive(true);

        if (null != canvas)
            canvas.SetActive(true);

        for (int i = 0; i < 4; ++i)
        {
            GameObject player = players[i];
            if (null == player)
                continue;

            player.SetActive(false);

            string playerCameraName = "Player " + i + " Camera";
            GameObject playerCam = GameObject.Find(playerCameraName);
            if (null == playerCam)
                Debug.Log(playerCameraName + " not found");
            else
                playerCam.SetActive(false);
        }
    }

    void initGame()
    {
        if (null != cam)
            cam.SetActive(false);

        if (null != canvas)
            canvas.SetActive(false);

        for (int i = 0; i < playerCount; ++i)
        {
            GameObject player = players[i];
            if (null == player)
                continue;

            var script = player.GetComponent<Spawn>();
            if (null != script)
                script.OnSpawn(player, i, playerCount);

            player.SetActive(true);
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

    private void StartGame(int _playerCount)
    {
        string name = "A600Motherboard";
        scene = SceneManager.GetSceneByName(name);
        playerCount = _playerCount;
        loaded = true;
        SceneManager.SetActiveScene(scene);
    }
}

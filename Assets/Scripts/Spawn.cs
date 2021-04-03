using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSpawn(GameObject player, int _index, int _count)
    {
        GameObject playerCam = GetComponent<Move>().cam as GameObject;
        if (null != playerCam && null != player)
        {
            playerCam.SetActive(true); 

            switch (_count)
            {
                case 1:
                    playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    player.GetComponent<PlayerUI>().SetPlayerUI(player, 0, 0);
                    break;

                case 2:
                    switch(_index)
                    {
                        case 0:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                            player.GetComponent<PlayerUI>().SetPlayerUI(player, 0, 0);
                            break;

                        case 1:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                            player.GetComponent<PlayerUI>().SetPlayerUI(player, 0.5f, 0);
                            break;
                    }
                    break;

                case 3:
                    switch (_index)
                    {
                        case 0:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                            player.GetComponent<PlayerUI>().SetPlayerUI(player, 0.0f, 0.0f);
                            break;

                        case 1:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                            player.GetComponent<PlayerUI>().SetPlayerUI(player, 0.5f, 0.0f);
                            break;

                        case 2:
                            playerCam.GetComponent<Camera>().rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                            player.GetComponent<PlayerUI>().SetPlayerUI(player, 0.0f, -0.5f);
                            break;
                    }
                    break;
            }
        }
    }
}

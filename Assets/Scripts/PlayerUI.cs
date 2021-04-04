using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public string nickname = "Player";
    public Color color = new Color(1, 1, 1);
    public int score = 0;
    public float life = 100;

    GameObject _player;

    public void SetPlayerUI(GameObject player, float x, float y)
    {
        if (null != player)
        {
            Canvas canvas = player.GetComponentsInChildren<Canvas>()[0];

            for (int i = 0; i < canvas.transform.childCount; ++i)
            {
                Transform child = canvas.transform.GetChild(i);

                child.transform.position += new Vector3(x * 1280.0f, y * 720.0f, 0.0f);
            }

            _player = player;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    } 

    // Update is called once per frame
    void Update()
    {
        if (null != _player)
        {
            Canvas canvas = _player.GetComponentsInChildren<Canvas>()[0];

            GameObject text = canvas.transform.Find("PlayerText").gameObject;
            text.GetComponent<Text>().text = nickname + " " + score + " " + life + "%";
        }

        Transform mesh = _player.transform.Find("Mesh");
        if (null != mesh)
        {
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();

            List<Material> mats = new List<Material>();
            meshRenderer.GetMaterials(mats);

            mats[0].color = Color.Lerp(new Color(1,1,1), color, life / 100.0f); 
        }

        // restore life
        life += 10.00f * Time.deltaTime;
        life = Mathf.Clamp(life, 0.0f, 100.0f);
    }
}

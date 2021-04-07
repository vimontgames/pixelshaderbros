using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Drop : MonoBehaviour
{
    public bool update = false;

    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    public float lifeTime = 10.0f;
    public float fadeTime = 1.0f;

    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.realtimeSinceStartup;

        var decalProjector = gameObject.GetComponentInChildren<DecalProjector>();
            decalProjector.material = new Material(decalProjector.material);

        SetDropColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        if (!update)
            return;

        float delta = Time.realtimeSinceStartup - spawnTime;

        if (delta > lifeTime)
        {
            float alpha = 1.0f - Mathf.Clamp01(((delta - lifeTime) / fadeTime));

            Color newCol = new Color(color.r, color.g, color.b, alpha);

            SetDropColor(newCol);

            if (delta > lifeTime + fadeTime)
                Destroy(gameObject);
        }
    }

    public void SetDropColor(Color color)
    {
        var decalProjector = gameObject.GetComponentInChildren<DecalProjector>();
        decalProjector.material.SetColor("_BaseColor", color);
        decalProjector.material.color = color;
        decalProjector.fadeFactor = color.a;

        //MeshRenderer meshRenderer = instDrop.GetComponentInChildren<MeshRenderer>();
        //List<Material> mats = new List<Material>();
        //meshRenderer.GetMaterials(mats);
        //mats[0].SetColor("_BaseColor", colors[colorIndex]);
        //mats[0].color = colors[colorIndex];
    }
}

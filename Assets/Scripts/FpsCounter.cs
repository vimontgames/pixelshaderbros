using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private Text text;
    private float refreshTime = 1f;
    private float timer;

    void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1.0f / Time.unscaledDeltaTime);
            text.text = ""+ fps;// + " FPS";
            timer = Time.unscaledTime + refreshTime;
        }
    }
}
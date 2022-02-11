using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateDebug : MonoBehaviour
{

    [SerializeField]private TextMeshProUGUI m_FpsText;
    private void Awake() {
        m_FpsText = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {

    }

    private float fps = 30f;

    void Update()
    {

        
        float newFPS = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, newFPS, 0.0005f);
        m_FpsText.text = $"FPS: {(int)fps}";
    }
}
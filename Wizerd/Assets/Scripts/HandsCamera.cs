using UnityEngine;
using UnityEngine.UI;

public class HandsCamera : MonoBehaviour
{
    [SerializeField] GameObject playerHands;
    Camera handsCamera;

    int cameraWidth = 640;
    int cameraHeight = 360;

    RenderTexture renderTexture;

    RawImage videoDisplay;

    void Awake()
    {
        videoDisplay = GetComponent<RawImage>();

        renderTexture = new RenderTexture(cameraWidth, cameraHeight, 16, RenderTextureFormat.ARGB32);
    }

    void OnEnable()
    {
        playerHands.SetActive(true);

        handsCamera = playerHands.transform.Find("HandsCamera").GetComponent<Camera>();

        handsCamera.targetTexture = renderTexture;

        videoDisplay.texture = renderTexture;
    }

    void OnDisable()
    {
        playerHands.SetActive(false);
    }

    void Update()
    {
        
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] DataTransmission dataTransmission;

    public int width = 640;
    public int height = 360;

    RawImage videoDisplay;
    Texture2D receivedTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        videoDisplay = GetComponent<RawImage>();
        receivedTexture = new Texture2D(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        byte[] frameData = dataTransmission.frameBytes;

        if (frameData.Length > 0) {
            receivedTexture.LoadImage(frameData);
            receivedTexture.Apply();

            videoDisplay.texture = receivedTexture;
        }
    }
}

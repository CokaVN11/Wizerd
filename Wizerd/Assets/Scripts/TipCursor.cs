using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TipCursor : MonoBehaviour
{
    [SerializeField] HandModule handModule;

    int resolutionWidth = 1920;
    int resolutionHeight = 1080;

    Image tipCursor;
    Vector3 velocity;
    [SerializeField] float smoothTime = 0.025f;

    int[] tipIds;

    public int handId = (int) HandModule.HandType.RIGHT_HAND;
    public int fingerId = (int) HandModule.Finger.INDEX;

    // Start is called before the first frame update
    void Start()
    {
        tipCursor = GetComponent<Image>();

        tipIds = handModule.tipIds;
    }

    // Update is called once per frame
    void Update()
    {
        if (handModule.handLandmarks[handId].Length > 0) {
            bool[] stretchedFingers = handModule.getStretchedFingers(handId);

            // If index finger is stretched and others are not
            if (stretchedFingers.Where((isStretched, idx) => idx != fingerId).All(isStretched => !isStretched) && stretchedFingers[fingerId]) {
                HandModule.Landmark handLandmark = handModule.handLandmarks[handId][tipIds[fingerId]];

                tipCursor.rectTransform.position = Vector3.SmoothDamp(tipCursor.rectTransform.position, new Vector3(handLandmark.x * resolutionWidth, handLandmark.y * resolutionHeight, tipCursor.rectTransform.position.z), ref velocity, smoothTime);
            }
        }
    }
}

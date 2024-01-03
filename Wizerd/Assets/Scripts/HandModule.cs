using UnityEngine;

public class HandModule : MonoBehaviour
{
    public enum HandType
    {
        LEFT_HAND = 0,
        RIGHT_HAND = 1
    }

    public enum Finger
    {
        THUMB = 0,
        INDEX = 1,
        MIDDLE = 2,
        RING = 3,
        PINKY = 4
    }

    public enum Knuckle
    {
        WRIST = 0,
        THUMB_CMC = 1,
        THUMB_MCP = 2,
        THUMB_IP = 3,
        THUMB_TIP = 4,
        INDEX_MCP = 5,
        INDEX_PIP = 6,
        INDEX_DIP = 7,
        INDEX_TIP = 8,
        MIDDLE_MCP = 9,
        MIDDLE_PIP = 10,
        MIDDLE_DIP = 11,
        MIDDLE_TIP = 12,
        RING_MCP = 13,
        RING_PIP = 14,
        RING_DIP = 15,
        RING_TIP = 16,
        PINKY_MCP = 17,
        PINKY_PIP = 18,
        PINKY_DIP = 19,
        PINKY_TIP = 20,
    }

    public class Landmark
    {
        public float x;
        public float y;
        public float z;

        public Landmark(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    // Get data
    DataTransmission dataTransmission;

    public int[] tipIds = new int[] {(int) Knuckle.THUMB_TIP, (int) Knuckle.INDEX_TIP, (int) Knuckle.MIDDLE_TIP, (int) Knuckle.RING_TIP, (int) Knuckle.PINKY_TIP};

    public Landmark[][] handLandmarks;

    // Start is called before the first frame update
    void Start()
    {
        dataTransmission = GetComponent<DataTransmission>();

        handLandmarks = new Landmark[2][];
        handLandmarks[0] = new Landmark[0];
        handLandmarks[1] = new Landmark[0];
    }

    // Update is called once per frame
    void Update()
    {
        string leftLandmarksData = dataTransmission.leftLandmarksData;
        string rightLandmarksData = dataTransmission.rightLandmarksData;

        if (leftLandmarksData.Length > 0) {
            string[] landmarks = leftLandmarksData.Split(',');

            handLandmarks[(int) HandType.LEFT_HAND] = new Landmark[21];

            for (int i = 0; i < 21; i++) {
                handLandmarks[(int) HandType.LEFT_HAND][i] = new Landmark(float.Parse(landmarks[i * 3]), float.Parse(landmarks[i * 3 + 1]), float.Parse(landmarks[i * 3 + 2]));
            }
        } else {
            handLandmarks[(int) HandType.LEFT_HAND] = new Landmark[0];
        }

        if (rightLandmarksData.Length > 0) {
            string[] landmarks = rightLandmarksData.Split(',');

            handLandmarks[(int) HandType.RIGHT_HAND] = new Landmark[21];

            for (int i = 0; i < 21; i++) {
                handLandmarks[(int) HandType.RIGHT_HAND][i] = new Landmark(float.Parse(landmarks[i * 3]), float.Parse(landmarks[i * 3 + 1]), float.Parse(landmarks[i * 3 + 2]));
            }
        } else {
            handLandmarks[(int) HandType.RIGHT_HAND] = new Landmark[0];
        }
    }

    public bool[] getStretchedFingers(int handId)
    {
        bool[] stretchedFingers = new bool[5];

        // Check thumb
        if (Vector2.Angle(createVector2(handId, 3, 4), createVector2(handId, 0, 1)) < 50) {
            stretchedFingers[0] = true;
        }
        // Check index
        if (Vector2.Angle(createVector2(handId, 6, 8), createVector2(handId, 0, 5)) < 100) {
            stretchedFingers[1] = true;
        }
        // Check middle
        if (Vector2.Angle(createVector2(handId, 10, 12), createVector2(handId, 0, 9)) < 100) {
            stretchedFingers[2] = true;
        }
        // Check ring
        if (Vector2.Angle(createVector2(handId, 14, 16), createVector2(handId, 0, 13)) < 100) {
            stretchedFingers[3] = true;
        }
        // Check pinky
        if (Vector2.Angle(createVector2(handId, 18, 20), createVector2(handId, 0, 17)) < 100) {
            stretchedFingers[4] = true;
        }

        return stretchedFingers;
    }

    public Vector2 createVector2(int handId, int startLandmarkId, int endLandmarkId)
    {
        Landmark startLandmark = handLandmarks[handId][startLandmarkId];
        Landmark endLandmark = handLandmarks[handId][endLandmarkId];

        return new Vector2(endLandmark.x - startLandmark.x, endLandmark.y - startLandmark.y);
    }

    public Vector3 createVector3(int handId, int startLandmarkId, int endLandmarkId)
    {
        Landmark startLandmark = handLandmarks[handId][startLandmarkId];
        Landmark endLandmark = handLandmarks[handId][endLandmarkId];

        return new Vector3(endLandmark.x - startLandmark.x, endLandmark.y - startLandmark.y, endLandmark.z - startLandmark.z);
    }
}

using UnityEngine;

public class HandConstruction : MonoBehaviour
{
    class HandBound
    {
        public float width;
        public float height;
        public float depth;

        public float widthOffset;
        public float heightOffset;
        public float depthOffset;

        public HandBound(float width, float height, float depth, float widthOffset, float heightOffset, float depthOffset)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;

            this.widthOffset = widthOffset;
            this.heightOffset = heightOffset;
            this.depthOffset = depthOffset;
        }
    }

    // Get data
    [SerializeField] HandModule handModule;

    HandBound handBound;

    public GameObject[] leftLandmarkObjects;
    public GameObject[] rightLandmarkObjects;

    public GameObject[] leftLandmarksConnection;
    public GameObject[] rightLandmarksConnection;
    LineRenderer[] leftLineRenderer = new LineRenderer[21];
    LineRenderer[] rightLineRenderer = new LineRenderer[21];

    Vector3[] leftVelocity = new Vector3[21];
    Vector3[] rightVelocity = new Vector3[21];
    [SerializeField] float smoothTime = 0.025f;

    void drawLandmarksConnection(GameObject[] landmarkObjects, LineRenderer[] lineRenderer, int startLandmarkIndex, int endLandmarkIndex, int lineIndex)
    {
        Vector3 start = landmarkObjects[startLandmarkIndex].transform.position;
        Vector3 end = landmarkObjects[endLandmarkIndex].transform.position;

        lineRenderer[lineIndex].SetPosition(0, start);
        lineRenderer[lineIndex].SetPosition(1, end);
    }

    void drawHandConnections(GameObject[] landmarkObjects, LineRenderer[] lineRenderer)
    {
        for (int i = 0; i < 4; i++) {
            drawLandmarksConnection(landmarkObjects, lineRenderer, i, i + 1, i);
        }
        for (int i = 5; i < 8; i++) {
            drawLandmarksConnection(landmarkObjects, lineRenderer, i, i + 1, i);
        }
        for (int i = 9; i < 12; i++) {
            drawLandmarksConnection(landmarkObjects, lineRenderer, i, i + 1, i);
        }
        for (int i = 13; i < 16; i++) {
            drawLandmarksConnection(landmarkObjects, lineRenderer, i, i + 1, i);
        }
        for (int i = 17; i < 20; i++) {
            drawLandmarksConnection(landmarkObjects, lineRenderer, i, i + 1, i);
        }
        drawLandmarksConnection(landmarkObjects, lineRenderer, 0, 5, 4);
        drawLandmarksConnection(landmarkObjects, lineRenderer, 5, 9, 8);
        drawLandmarksConnection(landmarkObjects, lineRenderer, 9, 13, 12);
        drawLandmarksConnection(landmarkObjects, lineRenderer, 13, 17, 16);
        drawLandmarksConnection(landmarkObjects, lineRenderer, 0, 17, 20);
    }

    void drawHand(HandModule.Landmark[] handLandmarks, GameObject[] landmarkObjects, LineRenderer[] lineRenderer, Vector3[] velocity)
    {
        for (int i = 0; i < 21; i++) {
            landmarkObjects[i].transform.localPosition = Vector3.SmoothDamp(landmarkObjects[i].transform.localPosition, new Vector3(handLandmarks[i].x * handBound.width + handBound.widthOffset, handLandmarks[i].y * handBound.height + handBound.heightOffset, handLandmarks[i].z * handBound.depth + handBound.depthOffset), ref velocity[i], smoothTime);
        }

        drawHandConnections(landmarkObjects, lineRenderer);
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform leftConnectionsTransform = transform.Find("LeftHandConnections");
        Transform rightConnectionsTransform = transform.Find("RightHandConnections");

        leftConnectionsTransform.localPosition = -transform.localPosition;
        rightConnectionsTransform.localPosition = -transform.localPosition;

        for (int i = 0; i < leftLandmarksConnection.Length; i++) {
            leftLineRenderer[i] = leftLandmarksConnection[i].GetComponent<LineRenderer>();
            leftLineRenderer[i].startWidth = 0.1f;
            leftLineRenderer[i].endWidth = 0.1f;
        }
        for (int i = 0; i < rightLandmarksConnection.Length; i++) {
            rightLineRenderer[i] = rightLandmarksConnection[i].GetComponent<LineRenderer>();
            rightLineRenderer[i].startWidth = 0.1f;
            rightLineRenderer[i].endWidth = 0.1f;
        }

        handBound = new HandBound(32f, 18f, 20f, -16f, -3f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandModule.Landmark[] leftHandLandmarks = handModule.handLandmarks[(int) HandModule.HandType.LEFT_HAND];
        HandModule.Landmark[] rightHandLandmarks = handModule.handLandmarks[(int) HandModule.HandType.RIGHT_HAND];

        if (leftHandLandmarks.Length > 0) {
            drawHand(leftHandLandmarks, leftLandmarkObjects, leftLineRenderer, leftVelocity);
        }

        if (rightHandLandmarks.Length > 0) {
            drawHand(rightHandLandmarks, rightLandmarkObjects, rightLineRenderer, rightVelocity);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour
{
    // Start is called before the first frame update 
    public GameObject pad;
    public GameObject brick;
    public GameObject anchor;

    public static float LeftBound;
    public static float RightBound;
    public static float TopBound;
    public static float BottomBound;
    public static float BoundWidth;
    public static float BoundHeight;
    public static float OneWidth;
    public static float OneHeight;
    public static float OneMagnitude;

    public static List<GameObject> balls = new List<GameObject>();

    private static GameObject scoreText;
    private static GameObject scoreQueue;
    private static int currentScore;
    private static int targetScore;
    private static int deltaScore;

    private static List<ScoreLine> queueScore = new List<ScoreLine>();

    void Start()
    {
        Physics2D.gravity = Vector2.zero;
        scoreText = GameObject.Find("ScoreText");
        scoreQueue = GameObject.Find("ScoreQueue");

        var boundObjects = GameObject.FindGameObjectsWithTag("Bounds");

        foreach (var bound in boundObjects)
        {
            var rect = bound.GetComponent<RectTransform>().rect;
            var boundTransform = bound.transform;
            Vector2 worldMax;

            switch (bound.name)
            {
                case "RightBound":
                    worldMax = boundTransform.TransformPoint(new Vector2(rect.xMin, 0));
                    RightBound = worldMax.x;
                    continue;
                case "LeftBound":
                    worldMax = boundTransform.TransformPoint(new Vector2(rect.xMax, 0));
                    LeftBound = worldMax.x;
                    continue;
                case "TopBound":
                    worldMax = boundTransform.TransformPoint(new Vector2(0, rect.yMin));
                    TopBound = worldMax.y;
                    continue;
                case "BottomBound":
                    worldMax = boundTransform.TransformPoint(new Vector2(0, rect.yMax));
                    BottomBound = worldMax.y;
                    continue;
            }
        }

        BoundWidth = RightBound - LeftBound;
        BoundHeight = TopBound - BottomBound;
        OneWidth = BoundWidth / 10;
        OneHeight = BoundHeight / 10;
        OneMagnitude = new Vector2(OneWidth, OneHeight).magnitude;

        pad.GetComponent<PadController>().RecalculateBounds();

        GameObject lastBrick = null;
        int BricksCount = 13;

        for (int i = 0; i < BricksCount; i++)
        {
            var newBrick = Instantiate(brick, new Vector3(-BricksCount / 2 + i, 0, 0), Quaternion.identity);
            newBrick.name = "Brick " + i.ToString();

            var controller = newBrick.GetComponent<BrickController>();
            controller.Previous = lastBrick;

            if (lastBrick != null)
                lastBrick.GetComponent<BrickController>().Next = newBrick;

            controller.prefabAnchorTop = anchor;
            controller.prefabAnchorBottom = anchor;
            controller.PreviousBound = false;
            controller.NextBound = false;

            Debug.Log("Set anchor initializer");

            if (i == 0)
            {
                controller.prefabAnchorLeft = anchor;
                controller.PreviousBound = true;
            }

            if (i == BricksCount - 1)
            {
                controller.prefabAnchorRight = anchor;
                controller.NextBound = true;
            }

            lastBrick = newBrick;
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        currentScore += deltaScore;
        if (currentScore > targetScore) currentScore = targetScore;

        scoreText.GetComponent<Text>().text = currentScore.ToString();
    }

    public static void AddScore(int amount)
    {
        var sl = ScriptableObject.CreateInstance<ScoreLine>();
        sl.Amount = amount;
        sl.Destroyed = () =>
        {
            queueScore.Remove(sl);            
            UpdateQueueDisplay();
        };

        queueScore.Add(sl);
        Destroy(sl, 2);
        UpdateQueueDisplay();

        targetScore += amount;
        deltaScore = (targetScore - currentScore) / 10;
    }

    private static void UpdateQueueDisplay()
    {
        var textComponent = scoreQueue.GetComponent<Text>();

        textComponent.text = "";

        for (int i = 0; i < queueScore.Count; i++)
            textComponent.text += queueScore[i].Amount + "\r\n";
    }
}

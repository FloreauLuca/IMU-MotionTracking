using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct GraphFrame
{
    public GraphFrame(float dt, float value)
    {
        this.dt = dt;
        this.value = value;
    }
    public float dt;
    public float value;
    public GameObject dot;
    public GameObject line;
}

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;

    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private RectTransform labelTextX;
    [SerializeField] private RectTransform labelTextY;
    [SerializeField] private RectTransform dashTemplateX;
    [SerializeField] private RectTransform dashTemplateY;

    private List<GameObject> gameObjectList = new List<GameObject>();

    private List<GraphFrame> frames;

    private float timer = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        frames = new List<GraphFrame>()
        {
            new GraphFrame(timer, Input.acceleration.x),
        };
        ShowGraph(frames);
    }

    void Update()
    {
        timer += Time.deltaTime;
        frames.Add(new GraphFrame(timer, Input.acceleration.x));
        List<GraphFrame> displayList = frames;
        if (frames.Count > 100)
        {
            displayList = new List<GraphFrame>();
            displayList.AddRange(frames.GetRange(frames.Count - 100, 100));
        }
        ShowGraph(displayList);

    }

    private GameObject CreateCircle(Vector2 anchorPosition)
    {
        GameObject gameObject = new GameObject("Dot", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchorMin = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowGraph(List<GraphFrame> frames)
    {
        foreach (var gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        float yMinimum = frames[0].value;
        float yMaximum = frames[0].value;
        float xMinimum = frames[0].dt;
        float xMaximum = frames[0].dt;

        foreach (var frame in frames)
        {
            if (Mathf.Abs(frame.value) > yMaximum) yMaximum = Mathf.Abs(frame.value);
            if (frame.dt > xMaximum) xMaximum = frame.dt;
            if (frame.dt < xMinimum) xMinimum = frame.dt;
        }

        yMinimum = -yMaximum;

        yMinimum -= (yMaximum - yMinimum) * 0.1f;
        yMaximum += (yMaximum - yMinimum)* 0.1f;
        xMaximum += (xMaximum - xMinimum) * 0.05f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < frames.Count; i++)
        {
            float xPosition = ((frames[i].dt - xMinimum )/ (xMaximum - xMinimum)) * graphWidth;
            float yPosition = ((frames[i].value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectList.Add(circleGameObject);
            if (lastCircleGameObject != null)
            {
                GameObject lineGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(lineGameObject);
            }
            lastCircleGameObject = circleGameObject;
        }

        float labelDeltaX = (xMaximum - xMinimum) / 5.0f;
        for (float i = xMinimum; i < xMaximum; i+= labelDeltaX)
        {
            float xPosition = i;
            RectTransform labelX = Instantiate(labelTextX);
            gameObjectList.Add(labelX.gameObject);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(((xPosition - xMinimum) / (xMaximum - xMinimum)) * graphWidth, -20.0f);
            labelX.GetComponent<TextMeshProUGUI>().text = (xPosition).ToString("#0.##");

            RectTransform dashX = Instantiate(dashTemplateX);
            gameObjectList.Add(dashX.gameObject);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(((xPosition - xMinimum) / (xMaximum - xMinimum)) * graphWidth, 0.0f);
        }
        float labelDeltaY = (yMaximum - yMinimum) / 5.0f;
        for (float i = yMinimum; i < yMaximum; i+= labelDeltaY)
        {
            float yPosition = i;
            RectTransform labelY = Instantiate(labelTextY);
            gameObjectList.Add(labelY.gameObject);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-7.0f, ((yPosition - yMinimum) / (yMaximum - yMinimum)) * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = (yPosition).ToString();

            RectTransform dashY = Instantiate(dashTemplateY);
            gameObjectList.Add(dashY.gameObject);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0.0f, ((yPosition - yMinimum) / (yMaximum - yMinimum)) * graphHeight);
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("DotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        return gameObject;
    }
}

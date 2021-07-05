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
        dot = null;
        line = null;
    }

    public void SetDot(GameObject dot)
    {
        this.dot = dot;
    }

    public void SetLine(GameObject line)
    {
        this.line = line;
    }

    public void MoveDot(Vector2 newPosition)
    {
        dot.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }

    public void UpdateLine(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        if (!line) return;
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.sizeDelta = new Vector2(distance, 3);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public float dt;
    public float value;
    public GameObject dot;
    public GameObject line;
}

public class WindowGraph : MonoBehaviour
{
    private enum GraphTypes
    {
        RAW_ACC,
        KALMAN_ACC,
        COMPUTE_ACC,
        RAW_VEL,
        KALMAN_VEL,
        COMPUTE_VEL,
        RAW_POS,
        KALMAN_POS,
        COMPUTE_POS
    }

    private enum AxisTypes
    {
        X,
        Y,
        Z
    }

    [Header("Display")]
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Color graphColor;
    [SerializeField] private bool displayDot;

    [Header("Transform")]
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private RectTransform labelTextX;
    [SerializeField] private RectTransform labelTextY;
    [SerializeField] private RectTransform dashTemplateX;
    [SerializeField] private RectTransform dashTemplateY;

    [Header("GraphParameter")]
    [SerializeField] private int axesCountX = 5;
    [SerializeField] private int axesCountY = 5;
    [SerializeField] private float timeDelta = 5.0f;
    [SerializeField] private float yMaxDelta = 0;

    [Header("GraphData")]
    [SerializeField] private GraphTypes graphTypes = GraphTypes.RAW_ACC;
    [SerializeField] private AxisTypes axisTypes = AxisTypes.X;

    private Vector2 graphSize;

    private List<GameObject> axesX = new List<GameObject>();
    private List<GameObject> textX = new List<GameObject>();
    private List<GameObject> axesY = new List<GameObject>();
    private List<GameObject> textY = new List<GameObject>();

    private List<GraphFrame> frames;

    private float timer = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        graphSize = graphContainer.sizeDelta;
        frames = new List<GraphFrame>();
        CreateLines();
    }

    private CalculationFarm calculationFarm;
    // Start is called before the first frame update
    void Start()
    {
        calculationFarm = FindObjectOfType<CalculationFarm>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        frames.Add(new GraphFrame(timer, GetData()));
        ShowGraph();
        UpdateAxes(timer - timeDelta);
    }


    private void ShowGraph()
    {

        foreach (var frame in frames)
        {
            if (Mathf.Abs(frame.value) > yMaxDelta/2) yMaxDelta = Mathf.Abs(frame.value)*2.25f;
        }

        float yMinimum = -yMaxDelta / 2;
        float xMinimum = timer - timeDelta;
        float xMaximum = timer;
        

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < frames.Count; i++)
        {
            if (frames[i].dt < xMaximum - timeDelta)
            {
                if (frames[i].dot)
                    frames[i].dot.SetActive(false);
                if (frames[i].line)
                    frames[i].line.SetActive(false);
                continue;
            }
            float xPosition = ((frames[i].dt - xMinimum )/ timeDelta) * graphSize.x;
            float yPosition = ((frames[i].value - yMinimum) / yMaxDelta) * graphSize.y;
            if (!frames[i].dot)
            {
                GraphFrame frame = frames[i];
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                frame.dot = circleGameObject;
                if (lastCircleGameObject != null)
                {
                    GameObject lineGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                    frame.line = lineGameObject;
                }

                frames[i] = frame;
                lastCircleGameObject = circleGameObject;
            }
            else
            {
                frames[i].MoveDot(new Vector2(xPosition, yPosition));
                if (lastCircleGameObject != null)
                {
                    frames[i].UpdateLine(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                        frames[i].dot.GetComponent<RectTransform>().anchoredPosition);
                }

                lastCircleGameObject = frames[i].dot;
            }
        }

    }

    private GameObject CreateCircle(Vector2 anchorPosition)
    {
        GameObject gameObject = new GameObject("Dot", typeof(Image));
        gameObject.SetActive(displayDot);
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = graphColor;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchorMin = new Vector2(0, 0);
        return gameObject;
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("DotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = graphColor;
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
    
    private void CreateLines()
    {
        for (float i = 0; i < axesCountX; i ++)
        {
            float xPosition = i;
            RectTransform labelX = Instantiate(labelTextX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2((i / axesCountX) * graphSize.x, -20.0f);
            labelX.GetComponent<TextMeshProUGUI>().text = (xPosition).ToString("#0.##");
            textX.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2((i / axesCountX) * graphSize.x, 0.0f);
            axesX.Add(dashX.gameObject);
        }
        for (float i = 0; i < axesCountY; i ++)
        {
            float yPosition = i;
            RectTransform labelY = Instantiate(labelTextY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-7.0f, (i / axesCountY) * graphSize.y);
            labelY.GetComponent<TextMeshProUGUI>().text = (yPosition).ToString();
            textY.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0.0f, (i / axesCountY) * graphSize.y);
            axesY.Add(dashY.gameObject);
        }
    }

    private void UpdateAxes(float startTime)
    {
        for (int i = 0; i < axesCountX; i ++)
        {
            float xValue = ((float)i / axesCountX) * timeDelta + startTime;
            float xPosition = ((float)i /axesCountX) * graphSize.x;
            RectTransform labelX = textX[i].GetComponent<RectTransform>();
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -20.0f);
            labelX.GetComponent<TextMeshProUGUI>().text = (xValue).ToString("#0.##");

            RectTransform dashX = axesX[i].GetComponent<RectTransform>();
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, 0.0f);
        }
        for (int i = 0; i < axesCountY; i++)
        {
            float yValue = (((float)i / (axesCountY-1)) * yMaxDelta) - (yMaxDelta / 2);
            float yPosition = ((float)i / (axesCountY-1)) * graphSize.y;
            RectTransform labelY = textY[i].GetComponent<RectTransform>();
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-7.0f, yPosition);
            labelY.GetComponent<TextMeshProUGUI>().text = (yValue).ToString("#0.###");

            RectTransform dashY = axesY[i].GetComponent<RectTransform>();
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0.0f, yPosition);
        }
    }

    private float GetData()
    {
        Vector3 dataVec = Vector3.zero;
        switch (graphTypes)
        {
            case GraphTypes.RAW_ACC:
                dataVec = calculationFarm.usedAcceleration;
                break;
            case GraphTypes.KALMAN_ACC:
                dataVec = calculationFarm.currKalmanFrame.kalmanRawAcc;
                break;
            case GraphTypes.COMPUTE_ACC:
                dataVec = calculationFarm.currProcessAccFrame.computeResetAcceleration;
                break;
            case GraphTypes.RAW_VEL:
                dataVec = calculationFarm.currRawAccFrame.rawVelocity;
                break;
            case GraphTypes.KALMAN_VEL:
                dataVec = calculationFarm.currKalmanFrame.kalmanRawVel;
                break;
            case GraphTypes.COMPUTE_VEL:
                dataVec = calculationFarm.currProcessAccFrame.computeResetVelocity;
                break;
            case GraphTypes.RAW_POS:
                dataVec = calculationFarm.currRawAccFrame.rawPosition;
                break;
            case GraphTypes.KALMAN_POS:
                dataVec = calculationFarm.currKalmanFrame.kalmanRawPos;
                break;
            case GraphTypes.COMPUTE_POS:
                dataVec = calculationFarm.currProcessAccFrame.computeResetPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (axisTypes)
        {
            case AxisTypes.X:
                return dataVec.x;
                break;
            case AxisTypes.Y:
                return dataVec.y;
                break;
            case AxisTypes.Z:
                return dataVec.z;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return 0;
    }
}

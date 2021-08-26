using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewCalculationFarm : MonoBehaviour
{
    [SerializeField] private List<CalculationAlgo> algos;
    [Space]
    [SerializeField] private bool useRunTimeData;
    [SerializeField] private string accFileName;
    [SerializeField] private string gyrFileName;
    private RawAccGraph readAccGraph = new RawAccGraph();
    private RawGyrGraph readGyrGraph = new RawGyrGraph();
    private int frameIndex = 0;
    [SerializeField] private bool resetCount = true;
    [SerializeField] private bool autoRestart;
    [SerializeField] private int skipFrame = 5;

    //Current Frame data
    public float deltaTime;
    public float time;

    public Vector3 initAcceleration;
    public Vector3 usedAcceleration;

    public RawAccFrame currRawAccFrame = new RawAccFrame();

    public RawGyrFrame currRawGyrFrame = new RawGyrFrame();

    public GlobalFrame currGlobalAccFrame = new GlobalFrame();

    public ComputeFrame currComputeFrame = new ComputeFrame();

    public KalmanFrame currKalmanFrame = new KalmanFrame();

    public RCFrame currRCFrame = new RCFrame();

    public WindowGraph[] windowGraph = new WindowGraph[100];

    public OutputManager outputManager;

    // Start is called before the first frame update
    void Start()
    {
        resetCount = true;
        Input.gyro.enabled = true;
        if (!useRunTimeData)
        {
            readAccGraph = JsonUtility.FromJson<RawAccGraph>(ReadFile("Assets/Graph/SavedGraph/" + accFileName + ".graph"));
            readGyrGraph = JsonUtility.FromJson<RawGyrGraph>(ReadFile("Assets/Graph/SavedGraph/" + gyrFileName + ".graph"));
            
            if (readGyrGraph != null)
            {
                currRawGyrFrame.attitude = readGyrGraph.frames[0].attitude;
                currRawGyrFrame.rotationRate = readGyrGraph.frames[0].rotationRate;
            }
            else
            {
                currRawGyrFrame.attitude = Quaternion.identity;
            }
        }

        var graphs = FindObjectsOfType<WindowGraph>();
        Debug.Log(graphs + " : " + graphs.Length);
        for (int i = 0; i < graphs.Length; i++)
        {
            windowGraph[i] = graphs[i];
        }

        outputManager = FindObjectOfType<OutputManager>();

    }
    
    void Update()
    {
        frameIndex += 1;

        if (useRunTimeData)
        {
            deltaTime = Time.deltaTime;
            time += deltaTime;

            //Wait acceleration initialisation
            if (Input.acceleration == Vector3.zero && Input.gyro.attitude == new Quaternion(0, 0, 0, 0)) return;

            if (Input.acceleration != Vector3.zero && initAcceleration == Vector3.zero)
            {
                currRawGyrFrame.attitude = Input.gyro.attitude;
                ResetVelocity();
                initAcceleration = Input.gyro.userAcceleration;
            }

            if (Input.accelerationEventCount != 1)
                Debug.LogError("Multiple Acceleration during the last frame");

            currRawAccFrame.acceleration = Input.acceleration;
            currRawAccFrame.userAcceleration = Input.gyro.userAcceleration;
            currRawAccFrame.gravity = Input.gyro.gravity;

            currRawGyrFrame.attitude = Input.gyro.attitude;
            currRawGyrFrame.rotationRate = Input.gyro.rotationRate;
        }
        else
        {
            if (resetCount)
            {
                if (readGyrGraph != null)
                {
                    currRawGyrFrame.attitude = readGyrGraph.frames[0].attitude;
                    currRawGyrFrame.rotationRate = readGyrGraph.frames[0].rotationRate;
                }
                else
                {
                    currRawGyrFrame.attitude = Quaternion.identity;
                }
                frameIndex = 0;
                resetCount = false;
                ResetVelocity();
            }

            initAcceleration = readAccGraph.frames[0].userAcceleration;

            if (frameIndex < readAccGraph.frames.Count)
            {
                if (readAccGraph != null)
                {
                    RawAccFrame frame = readAccGraph.frames[frameIndex];
                    time = frame.time;
                    deltaTime = time - readAccGraph.frames[frameIndex - 1].time;
                    currRawAccFrame.acceleration = frame.acceleration;
                    currRawAccFrame.gravity = frame.gravity;
                    currRawAccFrame.userAcceleration = frame.userAcceleration;
                }

                if (readGyrGraph != null && readGyrGraph.frames.Count > frameIndex)
                {
                    currRawGyrFrame.attitude = readGyrGraph.frames[frameIndex].attitude;
                    currRawGyrFrame.rotationRate = readGyrGraph.frames[frameIndex].rotationRate;
                }
            }
            else
            {
                currRawAccFrame.userAcceleration = Vector3.zero;
                resetCount = autoRestart;
            }
        }

        algos[0].UpdateData(deltaTime);
        usedAcceleration = currGlobalAccFrame.globalAcc;

        currRawAccFrame.rawVel += currRawAccFrame.userAcceleration * deltaTime;
        currRawAccFrame.rawPos += currRawAccFrame.rawVel * deltaTime;

        currRawAccFrame.time = time;
        currRawGyrFrame.time = time;

        foreach (CalculationAlgo calculationAlgo in algos)
        {
            calculationAlgo.UpdateData(deltaTime);
        }

        if (frameIndex % skipFrame == 0)
        {
            foreach (WindowGraph graph in windowGraph)
            {
                if (graph)
                {
                    graph.UpdateGraph(deltaTime * skipFrame);
                }
            }
        }

        outputManager.UpdateGraph();
    }


    public void ResetVelocity()
    {
        currRawAccFrame.rawVel = Vector3.zero;
        currRawAccFrame.rawPos = Vector3.zero;

        foreach (CalculationAlgo calculationAlgo in algos)
        {
            calculationAlgo.ResetValue();
        }

        time = 0;

    }

    string ReadFile(string path)
    {
        var json = "";
        try
        {
            var fileStream = new FileStream(path, FileMode.Open);
            using (var reader = new StreamReader(fileStream))
            {
                var text = reader.ReadLine();
                while (text != null)
                {
                    json += text;
                    json += '\n';
                    text = reader.ReadLine();
                }
            }
        }
        catch
        {
        }

        return json;
    }
}

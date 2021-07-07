using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//https://github.com/xioTechnologies/Gait-Tracking-With-x-IMU/blob/master/Gait%20Tracking%20With%20x-IMU/Script.m

[Serializable]
public class XIMUGaitFrame
{
    public float dt;
    public float deltaTime;
}

[Serializable]
public class XIMUGaitGraph
{
    public List<XIMUGaitFrame> frames = new List<XIMUGaitFrame>();
}

public class XIMUGaitTracking : MonoBehaviour
{
    public XIMUGaitGraph ximuGaitGraph = new XIMUGaitGraph();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        XIMUGaitFrame ximuGaitFrame = new XIMUGaitFrame();
        ximuGaitFrame.dt = Time.time;
        ximuGaitFrame.deltaTime = Time.deltaTime;


        ximuGaitGraph.frames.Add(ximuGaitFrame);

    }

    /*
    % -------------------------------------------------------------------------
    % Detect stationary periods

    % Compute accelerometer magnitude
    acc_mag = sqrt(accX.*accX + accY.*accY + accZ.*accZ);

    % HP filter accelerometer data
    filtCutOff = 0.001;
    [b, a] = butter(1, (2*filtCutOff)/(1/samplePeriod), 'high');
    acc_magFilt = filtfilt(b, a, acc_mag);

    % Compute absolute value
    acc_magFilt = abs(acc_magFilt);

    % LP filter accelerometer data
    filtCutOff = 5;
    [b, a] = butter(1, (2*filtCutOff)/(1/samplePeriod), 'low');
    acc_magFilt = filtfilt(b, a, acc_magFilt);

    % Threshold detection
    stationary = acc_magFilt < 0.05;
    % -------------------------------------------------------------------------
    */
    void DetectStationaryPeriods()
    {

    }

    /*
     % -------------------------------------------------------------------------
    % Compute orientation

    quat = zeros(length(time), 4);
    AHRSalgorithm = AHRS('SamplePeriod', 1/256, 'Kp', 1, 'KpInit', 1);

    % Initial convergence
    initPeriod = 2;
    indexSel = 1 : find(sign(time-(time(1)+initPeriod))+1, 1);
    for i = 1:2000
        AHRSalgorithm.UpdateIMU([0 0 0], [mean(accX(indexSel)) mean(accY(indexSel)) mean(accZ(indexSel))]);
    end

    % For all data
    for t = 1:length(time)
        if(stationary(t))
            AHRSalgorithm.Kp = 0.5;
        else
            AHRSalgorithm.Kp = 0;
        end
        AHRSalgorithm.UpdateIMU(deg2rad([gyrX(t) gyrY(t) gyrZ(t)]), [accX(t) accY(t) accZ(t)]);
        quat(t,:) = AHRSalgorithm.Quaternion;
    end

    % -------------------------------------------------------------------------
     */

    void ComputeOrientation()
    {

    }
    /*
     % -------------------------------------------------------------------------
       % Compute translational accelerations
       
       % Rotate body accelerations to Earth frame
       acc = quaternRotate([accX accY accZ], quaternConj(quat));
       
       % % Remove gravity from measurements
       % acc = acc - [zeros(length(time), 2) ones(length(time), 1)];     % unnecessary due to velocity integral drift compensation
       
       % Convert acceleration measurements to m/s/s
       acc = acc * 9.81;
       
       % Plot translational accelerations
       figure('Position', [9 39 900 300], 'NumberTitle', 'off', 'Name', 'Accelerations');
       hold on;
       plot(time, acc(:,1), 'r');
       plot(time, acc(:,2), 'g');
       plot(time, acc(:,3), 'b');
       title('Acceleration');
       xlabel('Time (s)');
       ylabel('Acceleration (m/s/s)');
       legend('X', 'Y', 'Z');
       hold off;
       
       % -------------------------------------------------------------------------
     */
    void ComputeTransitionalAcceleration()
    {

    }


    /*
    % -------------------------------------------------------------------------
       % Compute translational velocities
       
       acc(:,3) = acc(:,3) - 9.81;
       
       % Integrate acceleration to yield velocity
       vel = zeros(size(acc));
       for t = 2:length(vel)
       vel(t,:) = vel(t-1,:) + acc(t,:) * samplePeriod;
       if(stationary(t) == 1)
       vel(t,:) = [0 0 0];     % force zero velocity when foot stationary
       end
       end
       
       
       % Compute integral drift during non-stationary periods
       velDrift = zeros(size(vel));
       stationaryStart = find([0; diff(stationary)] == -1);
       stationaryEnd = find([0; diff(stationary)] == 1);
       for i = 1:numel(stationaryEnd)
       driftRate = vel(stationaryEnd(i)-1, :) / (stationaryEnd(i) - stationaryStart(i));
       enum = 1:(stationaryEnd(i) - stationaryStart(i));
       drift = [enum'*driftRate(1) enum'*driftRate(2) enum'*driftRate(3)];
       velDrift(stationaryStart(i):stationaryEnd(i)-1, :) = drift;
       end
       
       % Remove integral drift
       vel = vel - velDrift;
       
       % Plot translational velocity
       figure('Position', [9 39 900 300], 'NumberTitle', 'off', 'Name', 'Velocity');
       hold on;
       plot(time, vel(:,1), 'r');
       plot(time, vel(:,2), 'g');
       plot(time, vel(:,3), 'b');
       title('Velocity');
       xlabel('Time (s)');
       ylabel('Velocity (m/s)');
       legend('X', 'Y', 'Z');
       hold off;
       
       % -------------------------------------------------------------------------
    */
    void ComputeTransitionalVelocities()
    {

    }
    /*
     % -------------------------------------------------------------------------
       % Compute translational position
       
       % Integrate velocity to yield position
       pos = zeros(size(vel));
       for t = 2:length(pos)
       pos(t,:) = pos(t-1,:) + vel(t,:) * samplePeriod;    % integrate velocity to yield position
       end
       
       % Plot translational position
       figure('Position', [9 39 900 600], 'NumberTitle', 'off', 'Name', 'Position');
       hold on;
       plot(time, pos(:,1), 'r');
       plot(time, pos(:,2), 'g');
       plot(time, pos(:,3), 'b');
       title('Position');
       xlabel('Time (s)');
       ylabel('Position (m)');
       legend('X', 'Y', 'Z');
       hold off;
       
       % -------------------------------------------------------------------------
     */
    void ComputeTransitionalPosition()
    {

    }

}

[CustomEditor(typeof(XIMUGaitTracking))] //1
public class XIMUGaitGraphButton : GraphButton
{
    protected override void ButtonPushed()
    {
        XIMUGaitTracking ximuGaitTracking = (XIMUGaitTracking)target;
        CreateJson(ximuGaitTracking.ximuGaitGraph, "Assets/Graph/xIMUGait.graph");
    }
}

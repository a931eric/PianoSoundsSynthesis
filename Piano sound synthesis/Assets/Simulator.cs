using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    public float Δt= 0.00002439024f;
    
    public String str;
    public ComputeShader cs;
    int dispatchSize;
    ComputeBuffer y_buffer,Dty_buffer, recordedData_buffer;
    int[] kid;

    [Header("audio")]
    public float sampleRate = 41000;
    public int samplePoint;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public float recordingStartTime;
    public bool recording = false;
    float[] recordedData;
    public float amplitude=10;

    void Start()
    {
        recordedData = new float[200000];
        Initialize();
        kid = new int[3];
        kid[0] = cs.FindKernel("StringWave0");
        kid[1] = cs.FindKernel("StringWave1");
        kid[2] = cs.FindKernel("StringWave2");
        SetUpComputeShader();
    }

    void Initialize()
    {
        str.lineRenderer.positionCount = str.N_segment;
        str.positions = new Vector3[str.N_segment];
    }


    public void SetUpComputeShader()
    {
        dispatchSize = str.N_segment % GROUP_SIZE == 0 ? str.N_segment : str.N_segment + GROUP_SIZE - str.N_segment % GROUP_SIZE;

        y_buffer = new ComputeBuffer(dispatchSize,sizeof(float));
        y_buffer.SetData(str.y);
        cs.SetBuffer(kid[0], "y", y_buffer);
        cs.SetBuffer(kid[1], "y", y_buffer);
        cs.SetBuffer(kid[2], "y", y_buffer);

        Dty_buffer = new ComputeBuffer(dispatchSize, sizeof(float));
        Dty_buffer.SetData(str.Dty);
        cs.SetBuffer(kid[0], "Dty", Dty_buffer);
        cs.SetBuffer(kid[1], "Dty", Dty_buffer);
        cs.SetBuffer(kid[2], "Dty", Dty_buffer);

        cs.SetFloat("dt", Δt);
        cs.SetFloat("dx", str.Δx);
        cs.SetFloat("vv", str.F / str.μ);
        cs.SetFloat("l", str.N_segment);

        recordedData_buffer = new ComputeBuffer(recordedData.Length, sizeof(float));
        cs.SetFloat("samplePoint", samplePoint);
        cs.SetInt("sampleIdx", 0);
        cs.SetInt("samplePoint", samplePoint);
        cs.SetBuffer(kid[2], "sampleResult", recordedData_buffer);

        simulationTime = 0;
    }

    int GROUP_SIZE = 256;
    public int N_iter = 1;
    public float simulationTime;
    void Update()
    {
        for(int it = 0; it < N_iter; it++)
        {
            cs.Dispatch(kid[0], dispatchSize / GROUP_SIZE,1, 1);
            cs.Dispatch(kid[1], dispatchSize / GROUP_SIZE,1, 1);
            cs.Dispatch(kid[2], 1, 1, 1);
            simulationTime +=  Δt;
        }
        
        y_buffer.GetData(str.y);
        str.Display();
    }

    public void StartRecording()
    {
        recordingStartTime = simulationTime;
        recording = true;

        cs.SetInt("sampleIdx", 0);
        cs.SetInt("samplePoint", samplePoint);
        cs.SetBuffer(kid[2],"sampleResult", recordedData_buffer);
    }
    public void StopRecording()
    {
        recording = false;
        recordedData_buffer.GetData(recordedData);
        for(int i = 0; i < recordedData.Length; i++)
        {
            recordedData[i] *= amplitude;
        }
        audioClip.SetData(recordedData, 0);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}

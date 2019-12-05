using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class String : MonoBehaviour
{
    

    [Header("simulation")]
    public Simulator simulator;
    public int N_segment = 128;
    public float[] y,Dty;
    public float width;
    public float F,μ, Δx;

    [Header("display")]
    public float scale;
    public LineRenderer lineRenderer;
    public Vector3[] positions;

    

    public void Initialize()
    {
        Δx= width/N_segment;
        y = new float[N_segment];
        for(int i = 0; i < N_segment; i++)
        {
            y[i] = Mathf.Sin(i * 6.283f / N_segment)*0.01f;
        }
        Dty = new float[N_segment];
    }

    public void Display()
    {
        for (int i = 0; i < N_segment; i++)
        {
            positions[i] = new Vector3(i * Δx, y[i] )*scale;
        }
        lineRenderer.SetPositions(positions);
    }
}

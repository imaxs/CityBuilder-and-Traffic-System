
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAIUpdateProcess : MonoBehaviour
{
    [Header("Update Interval (sec.)")]
    public float UpdateInterval;

    private float m_timePerFrame;

    void FixedUpdate()
    {
        m_timePerFrame += Time.deltaTime;
        if (m_timePerFrame > UpdateInterval)
        {
            VehiclesManager.Ref.CalculateDistanceMatrix();

            m_timePerFrame = 0.0f;
            for(int item = 0; item < VehiclesManager.Ref.Length; item++)
                VehiclesManager.Ref[item].UpdateAI();
        }
    }
}
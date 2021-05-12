using System;
using System.Collections;
using System.Collections.Generic;
using CityBuilder;
using UnityEngine;

//=============================================
//   Vehicles Manager
//=============================================
public class VehiclesManager : BasicManager<VehiclesManager, Vehicle>
{
    [Header("Minimum distance to the nearest vehicle:")]
    public float MinDistanceNearestVehicle = 4.0f;
    [Header("Minimum critical distance:")]
    public float MinCriticalDistance = 0.04f;

    protected VehiclesManager() { }

    void Start()
    {
        this.InitializeManager();
        this.InitializeDistanceMatrix();
    }

    protected override void InitializeManager()
    {
        if (InitialCharactersManager.Ref != null)
        {
            m_gameObjectsItems = new List<Vehicle>(capacity:16);
            for (int i = 0; i < InitialCharactersManager.Ref.Length; i++)
            {
                if (((1 << InitialCharactersManager.Ref[i].gameObject.layer) & m_layerMask) > 0)
                {
                    var vehicle = InitialCharactersManager.Ref[i].GetComponent<Vehicle>();
                    if (vehicle != null) vehicle.AddToManagerLibrary<VehiclesManager, Vehicle>(Ref);
                }
            }
        }
    }
}
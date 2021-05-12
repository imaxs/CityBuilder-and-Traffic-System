using System;
using System.Collections;
using System.Collections.Generic;
using CityBuilder;
using UnityEngine;

//=============================================
//   Road Manager
//=============================================
public class RoadsManager : BasicManager<RoadsManager, BasicRoad>
{
    protected RoadsManager() { }

    void Start()
    {
        this.InitializeManager();
    }

    protected override void InitializeManager()
    {
        if (InitialBuildingsManager.Ref != null)
        {
            m_gameObjectsItems = new List<BasicRoad>(64);
            for (int i = 0; i < InitialBuildingsManager.Ref.Length; i++)
            {
                if (((1 << InitialBuildingsManager.Ref[i].gameObject.layer) & m_layerMask) > 0)
                {
                    var road = InitialBuildingsManager.Ref[i].GetComponent<BasicRoad>();
                    if (road != null)
                    {
                        road.OnBuild();
                        road.AddToManagerLibrary<RoadsManager, BasicRoad>(Ref);
                    }
                }
            }
        }
    }
}
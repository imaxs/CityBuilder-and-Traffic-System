
using System;
using System.Collections;
using System.Collections.Generic;
using CityBuilder;
using UnityEngine;

//=============================================
//   Initial Buildings Manager
//=============================================
public class InitialBuildingsManager : BasicManager<InitialBuildingsManager, GameEntity>
{
    protected InitialBuildingsManager() { }

    protected override void Awake()
    {
        base.Awake();
        base.InitializeManager();
    }
}

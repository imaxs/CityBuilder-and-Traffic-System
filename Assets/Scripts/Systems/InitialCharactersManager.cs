
using System;
using System.Collections;
using System.Collections.Generic;
using CityBuilder;
using UnityEngine;

//=============================================
//   Initial Characters Manager
//=============================================
public class InitialCharactersManager : BasicManager<InitialCharactersManager, GameEntity>
{
    protected InitialCharactersManager() { }

    protected override void Awake()
    {
        base.Awake();
        base.InitializeManager();
    }
}

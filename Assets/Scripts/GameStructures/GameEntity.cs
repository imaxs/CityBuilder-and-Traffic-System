using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneComponent
{
    Transform transform { get; }
    GameObject gameObject { get; }
    void AddToManagerLibrary<T1, T2>(T1 reference) where T1 : IManager<T2> where T2 : class, ISceneComponent;
}

namespace CityBuilder
{
    //=============================================
    //   Game Entity Abstract Class
    //=============================================

    abstract public class GameEntity : MonoBehaviour, ISceneComponent
    {
        //
        // Index of item in a manager library list
        //
        private int _id;

        public void AddToManagerLibrary<T1, T2>(T1 reference) where T1 : IManager<T2> where T2 : class, ISceneComponent
        {
            this._id = reference.Length;
            reference.Add(this as T2);
        }

        public int ID
        {
            get
            {
                return _id;
            }
        }

        protected (int id, float distance) GetClosest<T1, T2>(T1 reference) where T1 : IManager<T2> where T2 : class, ISceneComponent
        {
            return reference.GetClosestId(this._id);
        }

        protected int[] GetClosest<T1, T2>(T1 reference, float minDistance) where T1 : IManager<T2> where T2 : class, ISceneComponent
        {
            return reference.GetClosestId(this._id, minDistance);
        }
    }
}

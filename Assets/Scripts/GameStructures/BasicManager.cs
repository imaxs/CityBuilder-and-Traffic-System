
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public interface IManager<T> 
{
    int Length { get; }
    void Add(T item);
    (int id, float distance) GetClosestId(int id);
    int[] GetClosestId(int id, float minDist);
    T this[int index] { get; }
}

//=============================================
//   Basic Manager Class
//=============================================

abstract public class BasicManager<T1, T2> : MonoBehaviour, IManager<T2>
    where T1  : class, IManager<T2>
    where T2 : class, ISceneComponent
{
    public string[] layerNames;

    [HideInInspector]
    private static T1 _instance;

    [HideInInspector]
    protected List<T2> m_gameObjectsItems;

    [HideInInspector]
    protected int m_layerMask;

    [HideInInspector]
    private float[,] m_distanceMatrix;

    protected BasicManager()
    {

    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = GetComponent<T1>();
        }
        else
            Destroy(this.gameObject);

        this.m_layerMask = LayerMask.GetMask(layerNames);
    }

    public static T1 Ref
    {
        get
        {
            return _instance;
        }
    }

    public void CalculateDistanceMatrix()
    {
        for (int v = 1; v < this.m_gameObjectsItems.Count; v++)
        {
            for (int h = 0; h <= v - 1; h++)
            {
                float sqrDistance = (this.m_gameObjectsItems[v].gameObject.transform.position - this.m_gameObjectsItems[h].gameObject.transform.position).sqrMagnitude;
                this.m_distanceMatrix[v, h] = sqrDistance;
            }
        }
    }

    protected void InitializeDistanceMatrix()
    {
        this.m_distanceMatrix = new float[m_gameObjectsItems.Count, m_gameObjectsItems.Count];
    }

    protected virtual void InitializeManager()
    {
        this.m_gameObjectsItems = new List<T2>(this.transform.childCount);
        for (int childIndex = 0; childIndex < this.transform.childCount; childIndex++)
        {
            var child = this.transform.GetChild(childIndex).GetComponent<T2>();
            if (child != null)
            {
                if (child.gameObject.activeSelf && ((1 << child.gameObject.layer) & m_layerMask) > 0)
                    child.AddToManagerLibrary<T1, T2>(Ref);
            }
        }
    }

    public int Mask
    {
        get
        {
            return m_layerMask;
        }
    }

    //
    // This method add an item to the list
    //
    public void Add(T2 item)
    {
        this.m_gameObjectsItems.Add(item);
    }

    //
    // These methods are the best way to find closest object(s)
    //
    public (int id, float distance) GetClosestId(int id)
    {
        int closestId = int.MinValue;
        float min = float.MaxValue;

        int index;
        for (index = 0; index < id; index++)
        {
            if (min > this.m_distanceMatrix[id, index])
            {
                closestId = index;
                min = this.m_distanceMatrix[id, index];
            }
        }

        for (++index; index < this.m_gameObjectsItems.Count; index++)
        {
            if (min > this.m_distanceMatrix[index, id])
            {
                closestId = index;
                min = this.m_distanceMatrix[index, id];
            }
        }

        return ( id: closestId, distance: min );
    }

    public int[] GetClosestId(int id, float minDist)
    {
        int[] ids = new int[m_gameObjectsItems.Count / 31 + 1];

        int index;
        for (index = 0; index < id; index++)
        {
            if (minDist > this.m_distanceMatrix[id, index])
            {
                ids.SetMask(index);
            }
        }

        for (++index; index < this.m_gameObjectsItems.Count; index++)
        {
            if (minDist > this.m_distanceMatrix[index, id])
            {
                ids.SetMask(index);
            }
        }

        return ids;
    }

    public T2 GetClosest(Vector3 position)
    {
        float minSqrDist = float.MaxValue;
        int closestIndex = -1;
        for (int index = 0; index < this.m_gameObjectsItems.Count; index++)
        {
            var gameobject = this.m_gameObjectsItems[index] as ISceneComponent;
            if (gameobject != null)
            {
                float sqrDist = (position - gameobject.transform.position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closestIndex = index;
                }
            }
        }
        return (closestIndex > -1 ? this.m_gameObjectsItems[closestIndex] : null);
    }

    //
    // Get the closest object without considering the excluded object
    //
    public T2 GetClosest(Vector3 position, T2 excludedObject, out float minSqrDist)
    {
        minSqrDist = float.MaxValue;
        int closestIndex = -1;
        for (int index = 0; index < this.m_gameObjectsItems.Count; index++)
        {
            if (this.m_gameObjectsItems[index].Equals(excludedObject)) continue;
            var gameobject = this.m_gameObjectsItems[index] as ISceneComponent;
            if (gameobject != null)
            {
                float sqrDist = (position - gameobject.transform.position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closestIndex = index;
                }
            }
        }
        return (closestIndex > -1 ? this.m_gameObjectsItems[closestIndex] : null);
    }

    public T2 GetClosest(Vector3 position, T2 excludedObject)
    {
        return GetClosest(position, excludedObject, out _);
    }

    //
    // Get the nearby objects
    //
    public T2[] GetNearbyObjects(Vector3 position, T2 excludedObject, float maxSqrDist, int initialLength = 8)
    {
        var result = new List<T2>(initialLength);
        for (int index = 0; index < this.m_gameObjectsItems.Count; index++)
        {
            if (this.m_gameObjectsItems[index].Equals(excludedObject)) continue;
            ISceneComponent gameobject = this.m_gameObjectsItems[index] as ISceneComponent;
            if (gameobject != null)
            {
                if ((position - gameobject.transform.position).sqrMagnitude < maxSqrDist)
                    result.Add(this.m_gameObjectsItems[index]);
            }
        }
        return (result.Count > 0 ? result.ToArray() : null);
    }

    public int Length
    {
        get
        {
            return m_gameObjectsItems.Count;
        }
    }

    public T2 this[int index]
    {
        get
        {
            return this.m_gameObjectsItems[index];
        }
    }
}
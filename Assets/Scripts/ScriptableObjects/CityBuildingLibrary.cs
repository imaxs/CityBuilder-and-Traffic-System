using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace CityBuilder
{
    [CreateAssetMenu(fileName = "BuildingLibrary", menuName = "CityBuilder/New Library")]
    public class CityBuildingLibrary : ScriptableObject
    {
        [SerializeField]
        private BasicBuilding[] buildings = null;

        public Dictionary<int, BasicBuilding> GetHashMap()
        {
            Dictionary<int, BasicBuilding> hashMap = new Dictionary<int, BasicBuilding>();
            foreach (var building in buildings)
            {
                if (hashMap.ContainsKey(building.descriptor.uuid))
                    continue;

                hashMap.Add(building.descriptor.uuid, building);
            }
            return hashMap;
        }
    }
}

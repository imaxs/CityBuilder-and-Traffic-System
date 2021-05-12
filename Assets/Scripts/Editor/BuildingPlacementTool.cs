using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CityBuilder
{
    public class BuildingPlacementTool : MonoBehaviour
    {
        [MenuItem("CityBuilderTools/RefreshBuildings")]
        static void RefreshBuildings()
        {
            var buildings = FindObjectsOfType<BasicBuilding>();
            foreach (var element in buildings)
            {
                element.OnBuild();
            }
        }
    }
}

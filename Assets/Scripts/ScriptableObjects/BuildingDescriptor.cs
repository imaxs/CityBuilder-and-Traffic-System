using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [CreateAssetMenu(fileName = "BuildingInfo", menuName = "CityBuilder/New BuildingInfo")]
    public class BuildingDescriptor : ScriptableObject
    {
        [System.Serializable]
        public enum Category
        {
            None,
            Building,
            Roadway,
            PowerLine,
            Aqueduct
        }

        public int uuid = 0;
        public Category category = Category.None;
    }
}

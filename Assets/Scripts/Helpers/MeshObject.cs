using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshObject : MonoBehaviour
    {
        public MeshFilter meshFilter
        {
            get
            {
                return GetComponent<MeshFilter>();
            }
        }

        public MeshRenderer meshRenderer
        {
            get
            {
                return GetComponent<MeshRenderer>();
            }
        }
    }
}

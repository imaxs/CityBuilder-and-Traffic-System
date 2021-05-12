using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [SelectionBase]
    public class BasicBuilding : GameEntity
    {
        public enum Location
        {
            Front,
            Back,
            Right,
            Left,
            All,
            Undefined
        }

        [System.Serializable]
        public struct BuildingState
        {
            public int uuid;
            public BuildingDescriptor.Category category;
        }

        [System.Serializable]
        public struct BuildingNeighbors
        {
            public List<BasicBuilding> Front;
            public List<BasicBuilding> Back;
            public List<BasicBuilding> Right;
            public List<BasicBuilding> Left;

            public void AddNeighbor(BasicBuilding gameObject, Location location)
            {
                switch (location)
                {
                    case Location.Front:
                        Front.AddUniqueItem(gameObject);
                        break;
                    case Location.Back:
                        Back.AddUniqueItem(gameObject);
                        break;
                    case Location.Right:
                        Right.AddUniqueItem(gameObject);
                        break;
                    case Location.Left:
                        Left.AddUniqueItem(gameObject);
                        break;
                }
            }

            public BasicBuilding[] GetNeighbors(Location location, int layerMask)
            {
                List<BasicBuilding> result = new List<BasicBuilding>();
                switch (location)
                {
                    case Location.Front:
                        {
                            for (int i = 0; i < Front.Count; i++)
                                if (((1 << Front[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Front[i]);
                        }
                        break;
                    case Location.Back:
                        {
                            for (int i = 0; i < Back.Count; i++)
                                if (((1 << Back[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Back[i]);
                        }
                        break;
                    case Location.Right:
                        {
                            for (int i = 0; i < Right.Count; i++)
                                if (((1 << Right[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Right[i]);
                        }
                        break;
                    case Location.Left:
                        {
                            for (int i = 0; i < Left.Count; i++)
                                if(((1 << Left[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Left[i]);
                        }
                        break;
                    case Location.All:
                        {
                            for (int i = 0; i < Front.Count; i++)
                                if (((1 << Front[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Front[i]);
                            for (int i = 0; i < Back.Count; i++)
                                if (((1 << Back[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Back[i]);
                            for (int i = 0; i < Right.Count; i++)
                                if (((1 << Right[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Right[i]);
                            for (int i = 0; i < Left.Count; i++)
                                if (((1 << Left[i].gameObject.layer) & layerMask) > 0)
                                    result.Add(Left[i]);
                        }
                        break;
                }
                return (result.Count > 0 ? result.ToArray() : null);
            }

            public bool IsContainsLayer(int layer, Location location)
            {
                switch (location)
                {
                    case Location.Front:
                        for (int i = 0; i < Front.Count; i++)
                            if (Front[i].gameObject.layer == layer) return true;
                        break;
                    case Location.Back:
                        for (int i = 0; i < Back.Count; i++)
                            if (Back[i].gameObject.layer == layer) return true;
                        break;
                    case Location.Right:
                        for (int i = 0; i < Right.Count; i++)
                            if (Right[i].gameObject.layer == layer) return true;
                        break;
                    case Location.Left:
                        for (int i = 0; i < Left.Count; i++)
                            if (Left[i].gameObject.layer == layer) return true;
                        break;
                }

                return false;
            }

            public int GetNumberOccupiedSides
            {
                get
                {
                    return ((Front.Count > 0 ? 1 : 0) +
                            (Back.Count > 0 ? 1 : 0) +
                            (Right.Count > 0 ? 1 : 0) +
                            (Left.Count > 0 ? 1 : 0));
                }
            }

            public int GetNumberOccupiedSidesByLayer(int layerMask)
            {
                int sides = 0;
                for (int i = 0; i < Front.Count; i++)
                {
                    if (((1 << Front[i].gameObject.layer) & layerMask) > 0)
                    {
                        sides++;
                        break;
                    }
                }
                for (int i = 0; i < Back.Count; i++)
                {
                    if (((1 << Back[i].gameObject.layer) & layerMask) > 0)
                    {
                        sides++;
                        break;
                    }
                }
                for (int i = 0; i < Right.Count; i++)
                {
                    if (((1 << Right[i].gameObject.layer) & layerMask) > 0)
                    {
                        sides++;
                        break;
                    }
                }
                for (int i = 0; i < Left.Count; i++)
                {
                    if (((1 << Left[i].gameObject.layer) & layerMask) > 0)
                    {
                        sides++;
                        break;
                    }
                }

                return sides;
            }

            public int Count
            {
                get
                {
                    return ((Front.Count) + (Back.Count) + (Right.Count) + (Left.Count));
                }
            }

            public void Clear()
            {
                this.Front.Clear();
                this.Back.Clear();
                this.Right.Clear();
                this.Left.Clear();
            }
        }

        //
        // Configurable Parameters
        //
        public BuildingDescriptor descriptor;
        public BuildingState state;
        public BuildingNeighbors Neighbors;

        //
        // Internal Variables
        //
        protected Collider p_searchResult = null;
        protected BoxCollider p_boxCollider = null;

        private void Awake()
        {
            state.uuid = descriptor.uuid;
            state.category = descriptor.category;
            p_boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();
            if (p_boxCollider == null || transform.GetChild(0).name != "Collider")
                throw new System.Exception("Prefab has no collider! The prefab needs a first object named \"Collider\" and a BoxCollider component for it.");
        }

        public MeshFilter[] meshFilters
        {
            get
            {
                return GetComponentsInChildren<MeshFilter>(false);
            }
        }

        public MeshRenderer[] meshRenderers
        {
            get
            {
                return GetComponentsInChildren<MeshRenderer>(false);
            }
        }

        public virtual void OnBuild()
        {
            RefreshVisuals();
        }

        public virtual void OnErase()
        {

        }

        public virtual void RefreshVisuals()
        {
            this.SearchAllNeighbors(LayerMask.GetMask("Buildings", "Roads"));

            if (Math.Abs(p_boxCollider.size.x - p_boxCollider.size.z) > 0.0f) return; // for square only

            int layer = LayerMask.NameToLayer("Roads");
            if (Neighbors.IsContainsLayer(layer, GetLocation())) return;

            if (Neighbors.IsContainsLayer(layer, Location.Front))
                transform.rotation = Quaternion.LookRotation(Vector3.forward);
            else if (Neighbors.IsContainsLayer(layer, Location.Left))
                transform.rotation = Quaternion.LookRotation(Vector3.left);
            else if (Neighbors.IsContainsLayer(layer, Location.Back))
                transform.rotation = Quaternion.LookRotation(Vector3.back);
            else if (Neighbors.IsContainsLayer(layer, Location.Right))
                transform.rotation = Quaternion.LookRotation(Vector3.right);
        }

        public virtual void RefreshVisualsAndChainOnce()
        {
            throw new System.NotImplementedException();
        }

        //
        // Get Location from local space
        //
        public Location GetLocation()
        {
            if (Vector3.forward == transform.forward)
                return Location.Front;
            if (Vector3.back == transform.forward)
                return Location.Back;
            if (Vector3.right == transform.forward)
                return Location.Right;
            if (Vector3.left == transform.forward)
                return Location.Left;

            return Location.Undefined;
        }

        protected void SearchAllNeighbors(LayerMask layerMask)
        {
            Neighbors.Clear();

            float halfXSize = p_boxCollider.size.x / 2.0f,
                  halfZSize = p_boxCollider.size.z / 2.0f,
                  halfGridSize = 0.25f;

            int zNum = Convert.ToInt32(p_boxCollider.size.z / 0.5f),
                xNum = Convert.ToInt32(p_boxCollider.size.x / 0.5f);

            RaycastHit hitResult;

            for (int zi = 0; zi < zNum; zi++)
            {
                float posZ = (0.5f * zi) - halfZSize + halfGridSize;

                var posRight = new Vector3(transform.position.x + halfXSize + halfGridSize,
                                             transform.position.y - 5.0f,
                                             transform.position.z + posZ);

                if (Physics.Raycast(posRight, Vector3.up, out hitResult, 50.0f, layerMask))
                {
                    p_searchResult = hitResult.collider;
                    Neighbors.AddNeighbor(hitResult.transform.parent.gameObject.GetComponent<BasicBuilding>(), Location.Right);
                }

                var posLeft = new Vector3(transform.position.x - halfXSize - halfGridSize,
                                            transform.position.y - 5.0f,
                                            transform.position.z + posZ);
                if (Physics.Raycast(posLeft, Vector3.up, out hitResult, 50.0f, layerMask))
                {
                    p_searchResult = hitResult.collider;
                    Neighbors.AddNeighbor(hitResult.transform.parent.gameObject.GetComponent<BasicBuilding>(), Location.Left);
                }
            }

            for (int xi = 0; xi < xNum; xi++)
            {
                float posX = (0.5f * xi) - halfXSize + halfGridSize;

                var posFront = new Vector3(transform.position.x + posX,
                                           transform.position.y - 5.0f,
                                           transform.position.z + halfXSize + halfGridSize);

                if (Physics.Raycast(posFront, Vector3.up, out hitResult, 50.0f, layerMask))
                {
                    p_searchResult = hitResult.collider;
                    Neighbors.AddNeighbor(hitResult.transform.parent.gameObject.GetComponent<BasicBuilding>(), Location.Front);
                }

                var posBack = new Vector3(transform.position.x + posX,
                                          transform.position.y - 5.0f,
                                          transform.position.z - halfXSize - halfGridSize);

                if (Physics.Raycast(posBack, Vector3.up, out hitResult, 50.0f, layerMask))
                {
                    p_searchResult = hitResult.collider;
                    Neighbors.AddNeighbor(hitResult.transform.parent.gameObject.GetComponent<BasicBuilding>(), Location.Back);
                }
            }
        }
    }
}

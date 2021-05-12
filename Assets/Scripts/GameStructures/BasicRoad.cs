using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class BasicRoad : BasicBuilding
    {
        [System.Serializable]
        public struct RoadTiles
        {
            public GameObject straight;
            public GameObject turn;
            public GameObject threeWay;
            public GameObject fourWay;
        }

        //
        // Configurable Parameters
        //
        public RoadTiles roadTiles;

        //
        // Internal Variables
        //
        public RoadPoint[] RoadPoints = null;

        private bool isRoadPointСontained(RoadPoint point)
        {
            for (int i = 0; i < RoadPoints.Length; i++)
            {
                for (int p = 0; p < RoadPoints[i].connectedPoints.Length; p++)
                    if (RoadPoints[i].connectedPoints[p].gameObject.Equals(point))
                        return true;
            }

            return false;
        }

        public override void OnBuild()
        {
            base.RefreshVisuals();
            RefreshVisualsAndChainOnce();
        }

        public override void OnErase()
        {
            GetComponentInChildren<BoxCollider>().enabled = false;
            RefreshVisualsAndChainOnce();
            base.OnErase();
        }

        public override void RefreshVisuals()
        {
            base.RefreshVisuals();
            RefreshTileVisual();
            RefreshRoadPoints();
        }

        public override void RefreshVisualsAndChainOnce()
        {
            RefreshTileVisual();
            UpdateTileConnections();
            RefreshRoadPoints();
        }

        //
        // Grid Objects could simply lookup their surroundings using this.
        // Grid Objects would subsribe on start and unsubscribe on destroy
        //
        private void UpdateTileConnections()
        {
            //
            // Chain update to connections (1 chain only)
            //
            var neighbors = Neighbors.GetNeighbors(Location.All, LayerMask.GetMask("Buildings", "Roads"));
            if (neighbors != null)
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    BasicRoad road = neighbors[i] as BasicRoad;
                    if (road != null)
                        road.RefreshVisuals();
                    else
                        neighbors[i].RefreshVisuals();
                }
            }
        }

        private void RefreshTileVisual()
        {
            roadTiles.straight.SetActive(false);
            roadTiles.turn.SetActive(false);
            roadTiles.threeWay.SetActive(false);
            roadTiles.fourWay.SetActive(false);

            int layer = LayerMask.NameToLayer("Roads");
            switch (Neighbors.GetNumberOccupiedSidesByLayer(LayerMask.GetMask("Roads")))
            {
                case 1:
                {
                    roadTiles.straight.SetActive(true);
                    RoadPoints = roadTiles.straight.GetComponentsInChildren<RoadPoint>();
                    if (Neighbors.IsContainsLayer(layer, Location.Front))
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    else if (Neighbors.IsContainsLayer(layer, Location.Right))
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    else if (Neighbors.IsContainsLayer(layer, Location.Back))
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    else // Neighbors.IsContainsLayer(layer, Location.Left)
                        transform.rotation = Quaternion.LookRotation(Vector3.left);

                    break;
                }
                case 2:
                {
                    if (Neighbors.IsContainsLayer(layer, Location.Front) && Neighbors.IsContainsLayer(layer, Location.Back))
                    {
                        roadTiles.straight.SetActive(true);
                        RoadPoints = roadTiles.straight.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    }
                    else if (Neighbors.IsContainsLayer(layer, Location.Left) && Neighbors.IsContainsLayer(layer, Location.Right))
                    {
                        roadTiles.straight.SetActive(true);
                        RoadPoints = roadTiles.straight.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else if (Neighbors.IsContainsLayer(layer, Location.Front) && Neighbors.IsContainsLayer(layer, Location.Left))
                    {
                        roadTiles.turn.SetActive(true);
                        RoadPoints = roadTiles.turn.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    }
                    else if (Neighbors.IsContainsLayer(layer, Location.Front) && Neighbors.IsContainsLayer(layer, Location.Right))
                    {
                        roadTiles.turn.SetActive(true);
                        RoadPoints = roadTiles.turn.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    }
                    else if (Neighbors.IsContainsLayer(layer, Location.Back) && Neighbors.IsContainsLayer(layer, Location.Right))
                    {
                        roadTiles.turn.SetActive(true);
                        RoadPoints = roadTiles.turn.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    }
                    else // Neighbors.IsContainsLayer(layer, Location.Back) && Neighbors.IsContainsLayer(layer, Location.Left)
                    {
                        roadTiles.turn.SetActive(true);
                        RoadPoints = roadTiles.turn.GetComponentsInChildren<RoadPoint>();
                        transform.rotation = Quaternion.LookRotation(Vector3.left);
                    }

                    break;
                }
                case 3:
                {
                    roadTiles.threeWay.SetActive(true);
                    RoadPoints = roadTiles.threeWay.GetComponentsInChildren<RoadPoint>();
                    if (Neighbors.IsContainsLayer(layer, Location.Left) && Neighbors.IsContainsLayer(layer, Location.Front) && Neighbors.IsContainsLayer(layer, Location.Right))
                        transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    else if (Neighbors.IsContainsLayer(layer, Location.Front) && Neighbors.IsContainsLayer(layer, Location.Right) && Neighbors.IsContainsLayer(layer, Location.Back))
                        transform.rotation = Quaternion.LookRotation(Vector3.right);
                    else if (Neighbors.IsContainsLayer(layer, Location.Right) && Neighbors.IsContainsLayer(layer, Location.Back) && Neighbors.IsContainsLayer(layer, Location.Left))
                        transform.rotation = Quaternion.LookRotation(Vector3.back);
                    else // Neighbors.IsContainsLayer(layer, Location.Back) && Neighbors.IsContainsLayer(layer, Location.Left) & Neighbors.IsContainsLayer(layer, Location.Front)
                        transform.rotation = Quaternion.LookRotation(Vector3.left);

                    break;
                }
                case 4:
                {
                    roadTiles.fourWay.SetActive(true);
                    RoadPoints = roadTiles.fourWay.GetComponentsInChildren<RoadPoint>();
                    transform.rotation = Quaternion.identity;
                    break;
                }
                default:
                {
                    roadTiles.straight.SetActive(true);
                    RoadPoints = roadTiles.straight.GetComponentsInChildren<RoadPoint>();
                    transform.rotation = Quaternion.identity;
                    break;
                }
            }
        }

        //
        // The roads have points that the cars follow, this method gets the closest point to connect them.
        //
        private RoadPoint[] GetAllNearestRoadPoints()
        {
            var neighborRoads = Neighbors.GetNeighbors(Location.All, LayerMask.GetMask("Roads"));
            if (neighborRoads != null)
            {
                var result = new List<RoadPoint>();
                for (int indexNeighborRoad = 0; indexNeighborRoad < neighborRoads.Length; indexNeighborRoad++)
                {
                    BasicRoad road = neighborRoads[indexNeighborRoad] as BasicRoad;
                    if (road.RoadPoints != null)
                    {
                        for (int indexNeighborPoint = 0; indexNeighborPoint < road.RoadPoints.Length; indexNeighborPoint++)
                            result.Add(road.RoadPoints[indexNeighborPoint]);
                    }
                }
                return (result.Count > 0 ? result.ToArray() : null);
            }

            return null;
        }

        public RoadPoint GetClosestPoint(Vector3 position, float maxSqrDist)
        {
            return GetClosestPoint(RoadPoints, position, maxSqrDist);
        }

        public RoadPoint GetClosestPoint(Vector3 position, RoadPoint excludedPoint, float maxSqrDist)
        {
            return GetClosestPoint(RoadPoints, position, excludedPoint, maxSqrDist);
        }

        private RoadPoint GetClosestPoint(RoadPoint[] roadPoints, Vector3 closestTo, float maxSqrDist = 0.25f)
        {
            float minSqrDist = float.MaxValue;
            int indexPoint = -1;
            for(int index = 0; index < roadPoints.Length; index++)
            {
                float sqrDist = (closestTo - roadPoints[index].transform.position).sqrMagnitude;
                if (sqrDist > maxSqrDist) continue;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    indexPoint = index;
                }
            }
            return (indexPoint > -1 ? roadPoints[indexPoint] : null);
        }

        private RoadPoint GetClosestPoint(RoadPoint[] roadPoints, Vector3 closestTo, RoadPoint excludedPoint, float maxSqrDist = 0.25f)
        {
            float minSqrDist = float.MaxValue;
            int indexPoint = -1;
            for (int index = 0; index < roadPoints.Length; index++)
            {
                if (roadPoints[index].Equals(excludedPoint)) continue;
                float sqrDist = (closestTo - roadPoints[index].transform.position).sqrMagnitude;
                if (sqrDist > maxSqrDist) continue;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    indexPoint = index;
                }
            }
            return (indexPoint > -1 ? roadPoints[indexPoint] : null);
        }

        //
        // The roads have points that the cars follow, this method gets the closest point to connect them
        //
        private void RefreshRoadPoints()
        {
            if (RoadPoints == null) return;

            var nearestPoints = GetAllNearestRoadPoints();
            if (nearestPoints == null) return;

            for (int indexPoint = 0; indexPoint < RoadPoints.Length; indexPoint++)
            {
                var closestPoint = GetClosestPoint(nearestPoints, RoadPoints[indexPoint].transform.position);
                if (closestPoint != null)
                {
                    if (RoadPoints[indexPoint].state == RoadPoint.State.Out)
                    {
                        if (closestPoint.state == RoadPoint.State.In)
                            RoadPoints[indexPoint].connectedPoints = new RoadPoint[] { closestPoint };
                    }
                    else if (RoadPoints[indexPoint].state == RoadPoint.State.In)
                    {
                        if (closestPoint.state == RoadPoint.State.Out)
                            closestPoint.connectedPoints = new RoadPoint[] { RoadPoints[indexPoint] };
                    }
                }
            }
        }
    }
}

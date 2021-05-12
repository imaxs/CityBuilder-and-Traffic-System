using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilder
{
    public class RoadPoint : MonoBehaviour
    {
        //
        // This is the actual point that the vehicle follow
        //
        public RoadPoint[] connectedPoints;
        public StateAxis robotAxis;
        public PointType pointType;
        public State state;
        public bool go;

        [HideInInspector]
        public PointState roadState;

        public enum PointState
        {
            Go,
            Ready,
            Stop
        }

        public enum State
        {
            In,
            Out,
            Middle
        }

        public enum StateAxis
        {
            None,
            XRobot,
            ZRobot
        }

        public enum PointType
        {
            None,
            StoppingPoint,
        }

        void OnDrawGizmos()
        {
            if (pointType == PointType.StoppingPoint)
            {
                if (roadState == PointState.Go)
                    Gizmos.color = Color.green;
                else if (roadState == PointState.Ready)
                    Gizmos.color = Color.yellow;
                else if (roadState == PointState.Stop)
                    Gizmos.color = Color.red;
                if (connectedPoints.Length > 0)
                {
                    for (int i = 0; i < connectedPoints.Length; i++)
                    {
                        if (connectedPoints[i] != null)
                        {
                            Gizmos.DrawLine(transform.position, connectedPoints[i].transform.position);
                        }
                    }
                }
                Gizmos.DrawWireSphere(transform.position, 0.01f);
            }
            else if (pointType == PointType.None)
            {
                Gizmos.color = Color.cyan;
                if (connectedPoints.Length > 0)
                {
                    for (int i = 0; i < connectedPoints.Length; i++)
                    {
                        if (connectedPoints[i] != null)
                        {
                            Gizmos.DrawLine(transform.position, connectedPoints[i].transform.position);
                        }
                    }
                }
                Gizmos.DrawWireSphere(transform.position, 0.01f);
            }
        }
    }
}

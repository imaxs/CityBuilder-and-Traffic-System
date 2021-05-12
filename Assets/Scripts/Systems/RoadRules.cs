using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    //=============================================
    //   The intersection robot system
    //=============================================

    public class RoadRules : MonoBehaviour
    {
        public enum RoadType
        {
            Intersection,
            TIntersection
        }

        public RoadType intersectionType;
        public RoadPoint[] roadPoints;

        void Start()
        {
            if (roadPoints.Length > 0)
            {
                switch (intersectionType)
                {
                    case RoadType.Intersection:
                        StartCoroutine(IntersectionCalculation());
                        break;
                    case RoadType.TIntersection:
                        StartCoroutine(TIntersectionCalculation());
                        break;
                }
            }
        }

        IEnumerator TIntersectionCalculation()
        {
            roadPoints[0].roadState = RoadPoint.PointState.Go;
            roadPoints[1].roadState = RoadPoint.PointState.Go;
            roadPoints[2].roadState = RoadPoint.PointState.Stop;
            yield return new WaitForSeconds(7.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Ready;
            roadPoints[1].roadState = RoadPoint.PointState.Ready;
            roadPoints[2].roadState = RoadPoint.PointState.Stop;
            yield return new WaitForSeconds(3.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Stop;
            roadPoints[1].roadState = RoadPoint.PointState.Stop;
            roadPoints[2].roadState = RoadPoint.PointState.Go;
            yield return new WaitForSeconds(7.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Stop;
            roadPoints[1].roadState = RoadPoint.PointState.Stop;
            roadPoints[2].roadState = RoadPoint.PointState.Ready;
            yield return new WaitForSeconds(3.0f);
            StartCoroutine(TIntersectionCalculation());
        }


        IEnumerator IntersectionCalculation()
        {
            roadPoints[0].roadState = RoadPoint.PointState.Go;
            roadPoints[1].roadState = RoadPoint.PointState.Go;
            roadPoints[2].roadState = RoadPoint.PointState.Stop;
            roadPoints[3].roadState = RoadPoint.PointState.Stop;
            yield return new WaitForSeconds(7.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Ready;
            roadPoints[1].roadState = RoadPoint.PointState.Ready;
            roadPoints[2].roadState = RoadPoint.PointState.Stop;
            roadPoints[3].roadState = RoadPoint.PointState.Stop;
            yield return new WaitForSeconds(3.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Stop;
            roadPoints[1].roadState = RoadPoint.PointState.Stop;
            roadPoints[2].roadState = RoadPoint.PointState.Go;
            roadPoints[3].roadState = RoadPoint.PointState.Go;
            yield return new WaitForSeconds(7.0f);
            roadPoints[0].roadState = RoadPoint.PointState.Stop;
            roadPoints[1].roadState = RoadPoint.PointState.Stop;
            roadPoints[2].roadState = RoadPoint.PointState.Ready;
            roadPoints[3].roadState = RoadPoint.PointState.Ready;
            yield return new WaitForSeconds(3.0f);
            StartCoroutine(IntersectionCalculation());
        }
    }
}

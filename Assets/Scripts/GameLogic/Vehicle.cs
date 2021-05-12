using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CityBuilder
{
    public class Vehicle : GameEntity
    {
        public enum AIState
        {
            Idle,
            Looking,
            StartMoving,
            Moving,
            Stoping,
            Waiting
        }

        [Header("Vehicle parameters")]
        public float maxAccel = 25;
        public float maxBrake = 50;
        public float maxRotate = 60;
        public int maxSpeed = 60;

        [Header("Box Collider")]
        public BoxCollider BoxColliderObject;

        [Header("Wheels Colliders")]
        public WheelCollider FrontLeftWheel;
        public WheelCollider FrontRightWheel;
        public WheelCollider BackLeftWheel;
        public WheelCollider BackRightWheel;

        [Header("Wheels Transforms")]
        public Transform FrontLeftWheelTransform;
        public Transform FrontRightWheelTransform;
        public Transform BackLeftWheelTransform;
        public Transform BackRightWheelTransform;

        [Header("Front Point")]
        public Transform FrontPoint;
        [Header("Rear Point")]
        public Transform RearPoint;

        private Rigidbody m_rigidbody;
        private RoadPoint m_pointTarget;
        private AIState m_state, m_futureState;

        //
        // Internal Variables
        //
        private float m_minDistNearestVehicle, m_minCriticalDistance;
        private (bool value, DateTime time) isIntersection;    //check if it's at an intersection
        private Vehicle m_initiatorVehicle;
        private int m_currentSpeed;

        // Variables for Debug Only
        private Vehicle[] debug_closestVehicles;

        public void Start()
        {
            this.m_rigidbody = GetComponent<Rigidbody>();
            this.m_state = AIState.Looking;
            this.m_minDistNearestVehicle = VehiclesManager.Ref.MinDistanceNearestVehicle;
            this.m_minCriticalDistance = VehiclesManager.Ref.MinCriticalDistance;
            this.m_currentSpeed = this.maxSpeed;
        }

        [System.Obsolete]
        private void FixedUpdate()
        {            
            switch (m_state)
            {
                case AIState.Looking:
                    {
                        if (this.FindClosestRoadPoint())
                            this.SetState(AIState.Moving);
                    }
                    break;
                case AIState.StartMoving:
                    {
                        BackLeftWheel.brakeTorque = 0.0f;
                        BackRightWheel.brakeTorque = 0.0f;
                        FrontLeftWheel.brakeTorque = 0.0f;
                        FrontRightWheel.brakeTorque = 0.0f;
                        m_state = this.m_futureState;
                    }
                    break;
                case AIState.Moving:
                    {
                        Vector3 minsVect = (m_pointTarget.transform.position - transform.position);
                        if (minsVect.sqrMagnitude < 0.0625f)
                        {
                            this.IntersectionCheck();
                            if (!this.GetNextRoadPoint())
                                m_state = AIState.Looking;
                        }
                        float angleRotate = (Vector3.SignedAngle(minsVect, transform.forward, Vector3.up)) * (-1);
                        this.Moving(1.0f, (angleRotate > maxRotate ? maxRotate : (angleRotate < -maxRotate ? -maxRotate : angleRotate)));
                        this.UpdateWheelsMeshes();
                    }
                    break;
                case AIState.Stoping:
                    {
                        BackLeftWheel.brakeTorque = maxBrake;
                        BackRightWheel.brakeTorque = maxBrake;
                        FrontLeftWheel.brakeTorque = maxBrake;
                        FrontRightWheel.brakeTorque = maxBrake;
                        m_state = this.m_futureState;
                    }
                    break;
                case AIState.Waiting:
                    {
                        BackLeftWheel.brakeTorque = maxBrake;
                        BackRightWheel.brakeTorque = maxBrake;
                        FrontLeftWheel.brakeTorque = maxBrake;
                        FrontRightWheel.brakeTorque = maxBrake;
                    }
                    break;
                case AIState.Idle:
                default:
                    {

                    }
                    break;
            }
        }

        private void Moving(float accel, float rotare)
        {
            float maxspeed = (this.m_currentSpeed * 5.0f);
            if (BackLeftWheel.rpm > maxspeed || BackRightWheel.rpm > maxspeed)
            {
                BackLeftWheel.motorTorque = 0.0f;
                BackRightWheel.motorTorque = 0.0f;
            }
            else
            {
                BackLeftWheel.motorTorque = accel * maxAccel;
                BackRightWheel.motorTorque = accel * maxAccel;
            }

            FrontLeftWheel.steerAngle = rotare;
            FrontRightWheel.steerAngle = rotare;
        }


        private void UpdateWheelsMeshes()
        {
            var turn = FrontLeftWheelTransform.localEulerAngles;
            turn.y = FrontLeftWheel.steerAngle - FrontLeftWheelTransform.localEulerAngles.z;
            FrontLeftWheelTransform.localEulerAngles = turn;

            turn = FrontRightWheelTransform.localEulerAngles;
            turn.y = FrontRightWheel.steerAngle - FrontRightWheelTransform.localEulerAngles.z;
            FrontRightWheelTransform.localEulerAngles = turn;

            float rpm = BackRightWheel.rpm / 60 * 360 * Time.deltaTime;
            BackLeftWheelTransform.Rotate(rpm, 0, 0);
            BackRightWheelTransform.Rotate(rpm, 0, 0);
            FrontLeftWheelTransform.Rotate(rpm, 0, 0);
            FrontRightWheelTransform.Rotate(rpm, 0, 0);
        }

        public void SetState(AIState state)
        {
            this.m_futureState = state;
            switch (this.m_futureState)
            {
                case AIState.Moving:
                    this.m_state = (this.m_state == AIState.Moving ? AIState.Moving : AIState.StartMoving);
                    break;
                case AIState.Waiting:
                    this.m_state = (this.m_state == AIState.Moving || this.m_state == AIState.StartMoving ? this.m_state = AIState.Stoping : AIState.Waiting);
                    break;
                case AIState.Idle:
                    this.m_state = (this.m_state == AIState.Moving || this.m_state == AIState.StartMoving ? this.m_state = AIState.Stoping : AIState.Waiting);
                    break;
            }
        }

        public (bool value, DateTime time) isOnInteraction
        {
            get
            {
                return this.isIntersection;
            }
        }

        //
        // AI
        //
        public void UpdateAI()
        {
            int[] closest = GetClosest<VehiclesManager, Vehicle>(VehiclesManager.Ref, m_minDistNearestVehicle);
            int indexClosest = closest.BitPos();
            while (indexClosest != Extensions.NULL) 
            {
                Vehicle closestVehicle = VehiclesManager.Ref[indexClosest];
                float trgt = Vector3.Dot((m_pointTarget.transform.position - transform.position).normalized, transform.right);
                var adv = (closestVehicle.transform.position - transform.position).normalized;
                float fvsbl = Vector3.Dot(adv, transform.forward);
                float rvsbl = Vector3.Dot(adv, transform.right);

                if (fvsbl > 0.7f)
                {
                    //Debug.Log(name + " --> " + closestVehicle.name + " | " + fvsbl + " | " + rvsbl + " Intersection: " +
                    //        (!isIntersection.value ? "False" : "True [First: " + (isIntersection.time < closestVehicle.isIntersection.time ? "YES" : "NO") + "]")); //(rvsbl < 0 = left | rvsbl > 0 = right)
                    if (isIntersection.value)
                    {
                        if (closestVehicle.isIntersection.value &&
                            isIntersection.time > closestVehicle.isIntersection.time)
                        {
                            this.SetState(AIState.Waiting);
                            return;
                        }
                        else
                        {
                            if (Vector3.Dot((transform.position - closestVehicle.transform.position).normalized, closestVehicle.transform.forward) > 0.25f)
                                closestVehicle.SetState(AIState.Waiting);
                        }
                    }
                    else if (closestVehicle.isIntersection.value)
                    {
                        if (rvsbl > -0.5f)
                        {
                            this.SetState(AIState.Waiting);
                            return;
                        }
                    }
                    else
                    {
                        if (rvsbl > (-0.2f + trgt))
                        {
                            if ((closestVehicle.RearPoint.position - FrontPoint.position).sqrMagnitude < (m_minCriticalDistance + Mathf.Abs(trgt / 5)))
                            {
                                if (this.m_currentSpeed == this.maxSpeed)
                                {
                                    this.m_currentSpeed = closestVehicle.maxSpeed;
                                    this.m_initiatorVehicle = closestVehicle;
                                }

                                this.SetState(AIState.Waiting);
                                return;
                            }
                        }
                    }
                }

                indexClosest = closest.BitPos();
            }

            if (m_state == AIState.Moving)
            {
                if (m_pointTarget.roadState == RoadPoint.PointState.Stop)
                    this.SetState(AIState.Waiting);

                if (this.m_currentSpeed != this.maxSpeed)
                {
                    if (Vector3.Dot((this.m_initiatorVehicle.transform.position - transform.position).normalized, transform.forward) < 0.25f)
                        this.m_currentSpeed = this.maxSpeed;
                }
            }

            if (m_state == AIState.Waiting)
            {
                if (m_pointTarget.roadState == RoadPoint.PointState.Go)
                    this.SetState(AIState.Moving);
            }
        }

        private bool FindClosestRoadPoint()
        {
            var closestRoad = RoadsManager.Ref.GetClosest(transform.position);
            m_pointTarget = closestRoad.GetClosestPoint(transform.position, float.MaxValue);
            if (m_pointTarget == null) return false;

            if(m_pointTarget.connectedPoints.Length == 0)
                m_pointTarget = closestRoad.GetClosestPoint(transform.position, m_pointTarget, float.MaxValue);

            return !(m_pointTarget == null);
        }

        private void IntersectionCheck()
        {
            if (isIntersection.value)
            {
                isIntersection.value = !(m_pointTarget.state == RoadPoint.State.Out);
                if (debug_closestVehicles != null)
                {
                    for (int i = 0; i < debug_closestVehicles.Length; i++)
                        debug_closestVehicles[i].SetState(AIState.StartMoving);
                    debug_closestVehicles = null;
                }
            }
            else
            {
                isIntersection.value = (m_pointTarget.connectedPoints.Length > 1);
                isIntersection.time = System.DateTime.Now;
            }
        }

        [System.Obsolete]
        private bool GetNextRoadPoint()
        {
            if (m_pointTarget.connectedPoints.Length > 0)
            {
                m_pointTarget = m_pointTarget.connectedPoints[Random.RandomRange(0, m_pointTarget.connectedPoints.Length - 1)];
                return true;
            }
            return false;
        }

        void OnDrawGizmos()
        {
            if (m_pointTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, m_pointTarget.transform.position);
                for (int i = 0; i < m_pointTarget.connectedPoints.Length; i++)
                    Gizmos.DrawLine(m_pointTarget.transform.position, m_pointTarget.connectedPoints[i].transform.position);
            }

            if (debug_closestVehicles != null)
            {
                /*
                vec = B - A
                angle = atan2(vec.y, vec.x)
                degrees = angle * (180 / PI) // or angle * 57.295779513
                */
                //for (int i = 0; i < debug_closestVehicles.Length; i++)
                //{
                //    float visibleRadius = Vector3.Dot((debug_closestVehicles[i].transform.position - transform.position).normalized, transform.forward);
                //    Handles.Label(transform.position + Vector3.up, Convert.ToString((float)System.Math.Round(visibleRadius, 2)));
                //    float dot = Vector3.Dot(debug_closestVehicles[i].transform.forward, transform.forward);
                //    Handles.Label(transform.position + Vector3.up / 2, Convert.ToString((float)System.Math.Round(dot, 2)));

                //    Gizmos.color = (visibleRadius > 0.7f ? Color.green : Color.red);
                //    Gizmos.DrawLine(FrontPoint.position, debug_closestVehicles[i].transform.position);

                //    //float angle = Vector3.SignedAngle(debug_closestVehicles[i].transform.position - transform.position, transform.forward, Vector3.up);
                //    //if (Mathf.Abs(angle) < 40)
                //    //{
                //    //    if (angle < 0.0f) // Right side
                //    //        Gizmos.color = Color.red;
                //    //    else // left side
                //    //        Gizmos.color = Color.green;
                //    //    Gizmos.DrawLine(FrontPoint.position, debug_closestVehicles[i].transform.position);
                //    //}
                //}
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (transform.forward / 2 + transform.position));
        }
    }
}

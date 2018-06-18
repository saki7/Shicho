using Shicho.Core;

using UnityEngine;
using System;
using System.Collections.Generic;


namespace Shicho.Dive
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class Person : MonoBehaviour
    {
        public void Awake()
        {
            body_ = null;
            moveVel_ = Vector3.zero;
        }

        public void Init(CameraController cc)
        {
            cc_ = cc;

            body_ = GetComponent<Rigidbody>();
            body_.mass = 5500f;
            body_.detectCollisions = true;
            col_ = GetComponent<BoxCollider>();
            col_.isTrigger = false;
            col_.center = new Vector3(25, 85, 25);
            col_.size = new Vector3(50, 170, 50);
        }

        public void SpawnAtCamera()
        {
            body_.position = cc_.m_currentPosition;
            //body_.rotation = cc_.transform.rotation;

            body_.velocity = Vector3.zero;
            body_.useGravity = false;
        }

        public void Update()
        {
            // float mouseSensitivity = 1.2f;
            var mouseAxis = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            //mouseAxis.x *= DiveController.Instance.AspectRatio;
            //Log.Debug($"trans: {cc.transform}, {cc.m_targetPosition}, {cc.m_targetAngle}, {mousePosition} {mouseAxis}");

            moveVel_ = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) {
                moveVel_.z += 1;
            }
            if (Input.GetKey(KeyCode.S)) {
                moveVel_.z += -1;
            }
            if (Input.GetKey(KeyCode.A)) {
                moveVel_.x += -1;
            }
            if (Input.GetKey(KeyCode.D)) {
                moveVel_.x += 1;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                moveVel_.y += 1;

                //body_.AddForce(Vector3.up * 30f, ForceMode.Impulse);
            }

            var mouseAngle = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            cc_.m_currentAngle += mouseAngle;
        }

        public void LateUpdate()
        {
        }

        public void FixedUpdate()
        {
            AddGravityForce();

            body_.rotation = cc_.transform.rotation;
            //body_.MoveRotation(Quaternion.AngleAxis(cc_.m_currentAngle.x, Vector3.right) * Quaternion.AngleAxis(cc_.m_currentAngle.y, Vector3.up));

            var point = moveVel_ * 800f * Time.deltaTime;
            var right = body_.transform.right;
            var rotation = Quaternion.FromToRotation(Vector3.right, right);
            point = rotation * point;
            point.y = 0;

            //body_.velocity = Vector3.zero;
            body_.AddForce(point, ForceMode.VelocityChange);
            //body_.AddForce(point, ForceMode.Force);

            // inverse inertia for no movement
            //if (Mathf.Approximately(velocity.z, 0)) {
            //    body_.AddForce(0, 0, -body_.velocity.z * 0.98f);
            //}
            //if (Mathf.Approximately(velocity.x, 0)) {
            //    body_.AddForce(-body_.velocity.x * 0.98f, 0, 0);
            //}
            // body_.AddForce(-body_.velocity * 0.998f * Time.fixedDeltaTime);

            col_.transform.position = body_.transform.position;
            col_.transform.rotation = body_.transform.rotation;

            cc_.m_currentPosition = body_.position;
        }

        void OnCollisionEnter(Collision col)
        {
            Log.Debug($"c!!!: {col}");
        }

        private void AddGravityForce()
        {
            body_.AddForce(Vector3.down * Spec.gA, ForceMode.Acceleration);
        }


        private CameraController cc_;
        private Rigidbody body_;
        public Rigidbody Body { get => body_; }
        private BoxCollider col_;

        private Vector3 moveVel_;

        public void OnDestroy()
        {
            Dispose();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    GameObject.Destroy(body_);
                    body_ = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Character() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion IDisposable Support
    }
}

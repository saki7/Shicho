extern alias Cities;
using Shicho.Core;

using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

using System;


namespace Shicho.Dive
{
    using Core.UnityExtension;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class DiveController : MonoBehaviour
    {
        public void Awake()
        {
            if (instance_ == null) instance_ = this;
        }

        public void OnDestroy()
        {

        }

        public void OnEnable()
        {
            Log.Debug("OnEnable");

            Reset();

            cc_ = ToolsModifierControl.cameraController;

            screenRect = new Rect(0, 0, Screen.width, Screen.height);
            aspectRatio = (float)screenRect.width / screenRect.height;

            lock (App.Config.GraphicsLock) {
                wasDOFEnabled_ = App.Config.Graphics.dofEnabled;
                //App.Config.Graphics.dofEnabled = true;
            }

            Log.Debug($"body: {gr_} {grBody_}");

            grBody_.useGravity = false;
            grBody_.isKinematic = true;
            //grBody_.detectCollisions = true;

            gr_.size.Set(150, 90, 150);
            gr_.center.Set(75, 90, 75);
            //Log.Debug($"body: {gr_.attachedRigidbody} {grBody_}");

            me_ = this.AppendChildComponent<Drone>("Drone");
            me_.Init(cc_);
            me_.SpawnAtCamera();
        }

        public void OnDisable()
        {
            Log.Debug("OnDisable");

            cc_ = null;

            Destroy(me_.gameObject);
            me_ = null;

            lock (App.Config.GraphicsLock) {
                App.Config.Graphics.dofEnabled = wasDOFEnabled_;
            }
        }

        public void Start()
        {
        }

        public void Update()
        {
        }

        public void LateUpdate()
        {

        }

        public void FixedUpdate()
        {
            var grHit = Raycaster.Cast(Camera.main, -180f, Camera.main.transform.up);
            Log.Debug($"{me_.Body.position.y}, {grHit.GetValueOrDefault(Vector3.up * 80f)}");

            var grPos = Vector3.zero;
            if (grHit.HasValue) {
                grPos.Set(me_.Body.position.x, Mathf.Max(grHit.Value.y + 20f, 100f), me_.Body.position.z);

            } else {
                // default
                grPos.Set(me_.Body.position.x,  100f, me_.Body.position.z);
            }
            //Log.Debug($"{gr_.attachedRigidbody}");
            grBody_.position = grPos;

            //Raycaster
            //Physics.Raycast(pos, Vector3.down, out var hitInfo);
            //Log.Debug($"h: {rayDistance}");
        }

        public void Reset()
        {
            grBody_ = GetComponent<Rigidbody>();
            gr_ = GetComponent<BoxCollider>();
        }

        private static DiveController instance_;
        public static DiveController Instance { get => instance_; }

        private CameraController cc_;

        private Rect screenRect;
        private float aspectRatio;
        public float AspectRatio { get => aspectRatio; }

        private bool wasDOFEnabled_;

        private Drone me_;
        private Rigidbody grBody_;
        private BoxCollider gr_;
    }
}

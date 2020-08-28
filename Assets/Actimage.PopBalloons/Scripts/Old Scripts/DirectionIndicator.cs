﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using PopBalloons.Utilities;

/// <summary>
/// this script is copied from HTK
/// </summary>
namespace PopBalloons
{
    public class DirectionIndicator : MonoBehaviour
    {
        [Tooltip("The Cursor object the direction indicator will be positioned around.")]
        public GameObject Cursor;

        [Tooltip("Model to display the direction to the object this script is attached to.")]
        public GameObject DirectionIndicatorObject;

        [Tooltip("Allowable percentage inside the holographic frame to continue to show a directional indicator.")]
        [Range(-0.3f, 0.3f)]
        public float VisibilitySafeFactor = 0.1f;

        [Tooltip("Multiplier to decrease the distance from the cursor center an object is rendered to keep it in view.")]
        [Range(0.0f, 1.0f)]
        public float MetersFromCursor = 0.1f;

        // The default rotation of the cursor direction indicator.
        private Quaternion directionIndicatorDefaultRotation = Quaternion.identity;

        // Cache the MeshRenderer for the on-cursor indicator since it will be enabled and disabled frequently.
        private Renderer directionIndicatorRenderer;
        [SerializeField]
        // Cache the Material to prevent material leak.
        private Material indicatorMaterial;

        // Check if the cursor direction indicator is visible.
        private bool isDirectionIndicatorVisible;


        public IMixedRealityCursor cursor;

        public void Awake()
        {
            Cursor = GameObject.Find("DefaultCursor(Clone)"); 
            if (Cursor == null)
            {
                Debug.LogError("Please include a GameObject for the cursor.");
            }

            if (DirectionIndicatorObject == null)
            {
                Debug.LogError("Please include a GameObject for the Direction Indicator.");
            }

            // Instantiate the direction indicator.
            DirectionIndicatorObject = InstantiateDirectionIndicator(DirectionIndicatorObject);

            if (DirectionIndicatorObject == null)
            {
                Debug.LogError("Direction Indicator failed to instantiate.");
            }
        }




        public void OnDestroy()
        {
            DestroyImmediate(indicatorMaterial);
            Destroy(DirectionIndicatorObject);
        }

        private GameObject InstantiateDirectionIndicator(GameObject directionIndicator)
        {
            if (directionIndicator == null)
            {
                return null;
            }

            GameObject indicator = Instantiate(directionIndicator);

            // Set local variables for the indicator.
            directionIndicatorDefaultRotation = indicator.transform.rotation;
            directionIndicatorRenderer = indicator.GetComponent<Renderer>();

            // Start with the indicator disabled.
            directionIndicatorRenderer.enabled = false;

            // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
            foreach (Collider indicatorCollider in indicator.GetComponents<Collider>())
            {
                Destroy(indicatorCollider);
            }

            foreach (Rigidbody rigidBody in indicator.GetComponents<Rigidbody>())
            {
                Destroy(rigidBody);
            }

            indicatorMaterial = directionIndicatorRenderer.material;

            return indicator;
        }

        private float valeurZ = 0;
        //private Vector3 offsetZ = new Vector3(0,0,0);
        private bool done = true;

        public void Update()
        {
            if (done)
            {
                valeurZ += Time.deltaTime;
                if (valeurZ >= 1)
                    done = false;
            }
            if (!done)
            {
                valeurZ -= Time.deltaTime;
                if (valeurZ <= 0.02f)
                    done = true;
            }

            if (DirectionIndicatorObject == null)
            {
                return;
            }
            Camera mainCamera = Camera.main;
            // Direction from the Main Camera to this script's parent gameObject.
            Vector3 camToObjectDirection = gameObject.transform.position - mainCamera.transform.position;
            camToObjectDirection.Normalize();

            // The cursor indicator should only be visible if the target is not visible.
            isDirectionIndicatorVisible = !IsTargetVisible(mainCamera);
            directionIndicatorRenderer.enabled = isDirectionIndicatorVisible;

            if (isDirectionIndicatorVisible)
            {
                Vector3 position;
                Quaternion rotation;
                GetDirectionIndicatorPositionAndRotation(
                    camToObjectDirection,
                    mainCamera.transform,
                    out position,
                    out rotation);
                //offsetZ = new Vector3(0, 0, valeurZ);
                //Debug.Log(valeurZ);

                DirectionIndicatorObject.transform.position = position;// + offsetZ;
                DirectionIndicatorObject.transform.rotation = rotation;
                DirectionIndicatorObject.transform.position += DirectionIndicatorObject.transform.forward.normalized * MetersFromCursor * Mathf.SmoothStep(0, 1, valeurZ);
            }
        }


        /// <summary>
        /// Only in motricity mode
        /// </summary>
        /// <param name="mainCamera"></param>
        /// <returns></returns>
        private bool IsTargetVisible(Camera mainCamera)
        {
            if(GameManager.Instance.CurrentGameType != GameManager.GameType.CLASSIC)
            {
                return true;
            }

            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = mainCamera.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > VisibilitySafeFactor && targetViewportPosition.x < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.y > VisibilitySafeFactor && targetViewportPosition.y < 1 - VisibilitySafeFactor &&
                    targetViewportPosition.z > 0);
        }

        private void GetDirectionIndicatorPositionAndRotation(Vector3 camToObjectDirection, Transform cameraTransform, out Vector3 position, out Quaternion rotation)
        {
            // Find position:
            // Save the cursor transform position in a variable.
            Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;//Cursor.transform.position;

            // Project the camera to target direction onto the screen plane.
            Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * cameraTransform.forward);
            cursorIndicatorDirection.Normalize();

            // If the direction is 0, set the direction to the right.
            // This will only happen if the camera is facing directly away from the target.
            if (cursorIndicatorDirection == Vector3.zero)
            {
                cursorIndicatorDirection = cameraTransform.right;
            }

            // The final position is translated from the center of the screen along this direction vector.
            position = origin + cursorIndicatorDirection * MetersFromCursor;

            // Find the rotation from the facing direction to the target object.
            rotation = Quaternion.LookRotation(cameraTransform.forward, cursorIndicatorDirection) * directionIndicatorDefaultRotation;
        }
    }

}


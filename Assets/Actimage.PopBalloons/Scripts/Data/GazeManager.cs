using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.Data
{
    public enum GazableElement { BALLOON, JULIE_BODY, JULIE_HEAD, PANEL, PROPS }
    /// <summary>
    /// Handle Gaze data processing info
    /// </summary>
    public class GazeManager : MonoBehaviour
    {
        

        private static GazeManager instance;

        public static GazeManager Instance { get { return instance; } }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        private void RegisterGazePosition()
        {
            var eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
            if (eyeGazeProvider != null)
            {
                // ToDo: Handle raycasting layers
                RaycastHit hitInfo = default(RaycastHit);
                Ray lookRay = new Ray(eyeGazeProvider.GazeOrigin, eyeGazeProvider.GazeDirection.normalized);
                bool isHit = UnityEngine.Physics.Raycast(lookRay, out hitInfo);
                Vector3 point = eyeGazeProvider.GazeOrigin + eyeGazeProvider.GazeDirection.normalized;
                if (isHit)
                {
                    point = hitInfo.point;
                }

                if(DataManager.instance != null)
                {
                    GazeDatas datas = new GazeDatas();
                    datas.targetIsValid = isHit;
                    datas.origin = eyeGazeProvider.GazeOrigin;
                    datas.direction = eyeGazeProvider.GazeDirection.normalized;
                    datas.target = point;
                    datas.timeStamp = System.DateTime.Now.ToString("HH-mm-ss.fff");
                    DataManager.instance.AddGazeDatas(datas);
                }
            }
            
        }


        private void Update()
        {
            RegisterGazePosition();
        }

        public void RegisterGazeData(GazableElement target, string timestamp, string duration)
        {
            //TODO: Add object id / position for balloon identification
            GazableItemDatas datas = new GazableItemDatas();
            datas.objectType = target.ToString();
            datas.timeOfLook = timestamp;
            datas.duration = duration;

            if (DataManager.instance != null)
                DataManager.instance.AddGazeDatas(datas);
        }
    }
}
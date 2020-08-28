using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.Data
{

public class GazableItem : MonoBehaviour
{
        [SerializeField]
        private GazableElement itemType;

        DateTime gazeTimestamp;

        public void Awake()
        {
            gazeTimestamp = System.DateTime.Now;
        }

        public void OnGazeBegin()
        {
            gazeTimestamp = System.DateTime.Now;
        }

        public void OnGazeEnd()
        {
            TimeSpan duration = System.DateTime.Now - gazeTimestamp ;//Time.time - gazeTimestamp;
            GazeManager.Instance.RegisterGazeData(this.itemType, gazeTimestamp.ToString("HH-mm-ss.fff"), duration.ToString());
        }

}

}
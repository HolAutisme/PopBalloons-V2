using PopBalloons.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.Data
{
    public class BalloonDataRegistrer : MonoBehaviour
    {


        BalloonDatas currentBalloonData;


        // Use this for initialization
        void Start()
        {
            BalloonBehaviour.OnBalloonSpawned += balloonSpawned;
            BalloonBehaviour.OnDestroyBalloon += balloonDestroyed;
            BalloonBehaviour.OnBalloonMissed += balloonMissed;
        }

        void InitBalloonData()
        {
            currentBalloonData = new BalloonDatas();
        }

        private void OnDestroy()
        {
            BalloonBehaviour.OnBalloonSpawned -= balloonSpawned;
            BalloonBehaviour.OnDestroyBalloon -= balloonDestroyed;
            BalloonBehaviour.OnBalloonMissed -= balloonMissed;
        }

        private void balloonMissed(float timeStamp, float duration, bool timeout)
        {
            currentBalloonData.lifeTime = duration;
            currentBalloonData.balloonWasDestroyByUser = false;
            currentBalloonData.balloonTimout = timeout;
            currentBalloonData.balloonPointGain = 0;
            currentBalloonData.timeOfDestroy = timeStamp;
            //currentBalloonData.distance = (FootStepManager.instance != null)
            //? FootStepManager.instance.getDistance()
            //: 0;
            AddDatas(currentBalloonData);
        }

        private void balloonDestroyed(float timeStamp, float duration, bool isBonus)
        {
            currentBalloonData.dateOfDestroy = System.DateTime.Now.ToString("HH-mm-ss.fff");
            currentBalloonData.lifeTime = duration;
            currentBalloonData.balloonWasDestroyByUser = true;
            currentBalloonData.balloonTimout = false;
            currentBalloonData.balloonPointGain = ScoreManager.GetScore(duration);
            currentBalloonData.timeOfDestroy = timeStamp;
            //currentBalloonData.distance = (FootStepManager.instance != null)
            //    ? FootStepManager.instance.getDistance()
            //    : 0;
            AddDatas(currentBalloonData);
        }

        private void balloonSpawned(BalloonBehaviour balloon)
        {
            InitBalloonData();
            //if (FootStepManager.instance != null)
            //    FootStepManager.instance.initFootStep();
            currentBalloonData.id = balloon.Id.ToString();
            currentBalloonData.color = balloon.GetColor().ToString();
            currentBalloonData.balloonInitialPosition = balloon.transform.position;
            currentBalloonData.dateOfSpawn = balloon.DateOfSpawn;
            currentBalloonData.timeOfSpawn = TimerManager.GetTime();
            //Distance to cover in meters
            currentBalloonData.distance = Vector3.Distance(Vector3.Scale(new Vector3(1f,0f,1f),balloon.transform.position), Vector3.Scale(new Vector3(1f, 0f, 1f), Camera.main.transform.position));
        }


        private void AddDatas(BalloonDatas data)
        {
            if (DataManager.instance != null)
            {
                DataManager.instance.AddBalloonsDatas(data);
            }
        }


    }

}

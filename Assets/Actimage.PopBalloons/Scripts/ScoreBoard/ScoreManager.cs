using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PopBalloons
{
    public class ScoreManager : MonoBehaviour
    {
        #region variables
        public int score;


        public static int maxScoreByBalloon = 35;
        public static int scoreBonusBalloon = 10;

        private int correctBalloon = 0;
        private int wrongBalloon = 0;


        public delegate void scoreChange(int score, int scoreGain);
        public static event scoreChange onScoreChange;

        public delegate void balloonWasPopped(float time, int scoreGain, bool isBonus);
        public static event balloonWasPopped onBalloonPopped;


        // Use this for initialization
        public static ScoreManager instance
        {
            get; private set;
        }
        public int CorrectBalloon { get => correctBalloon; }
        public int WrongBalloon { get => wrongBalloon;  }


        #endregion

        #region unity functions

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(this);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

        }
        #endregion

        #region functions
        public static void initScore()
        {
            if (instance != null)
            {
                instance.SetScore(0);
                instance.wrongBalloon = 0;
                instance.correctBalloon = 0;
            }

        }

        public void SetScore(int sc)
        {
            instance.score = sc;
            //Update Score UI
            if (onScoreChange != null)
            {
                onScoreChange(sc, 0);
            }
        }



        private void OnEnable()
        {

            BalloonBehaviour.OnDestroyBalloon += ManageBalloonData;
            BonusBehaviour.OnDestroyBonus += ManageBalloonData;
        }
        private void OnDisable()
        {
            BalloonBehaviour.OnDestroyBalloon -= ManageBalloonData;
            BonusBehaviour.OnDestroyBonus -= ManageBalloonData;
        }

        public static float GetScore(float duration)
        {
            return (duration < 7.0f)
                    ? maxScoreByBalloon - 5 * (int)Mathf.Floor(duration)
                    : 5;
        }

        public void ClearWrongBalloon()
        {
            this.wrongBalloon = 0;
        }

        public void CognitiveBalloonPopped(bool isCorrect)
        {
            if (isCorrect)
            {
                correctBalloon++;
                this.score += 10;
                onScoreChange?.Invoke(score, 10);
            }
            else
            {
                wrongBalloon++;
                this.score -= 5;
                onScoreChange?.Invoke(score, -5);
            }
            
        }


        void ManageBalloonData(float time, float duration, bool isBonus)
        {
            int scoreToAdd = (isBonus)
                ? scoreBonusBalloon
                : (duration < 7.0f)
                    ? maxScoreByBalloon - 5 * (int)Mathf.Floor(duration)
                    : 5;

            score += scoreToAdd;

            //Update Score UI
            if (onScoreChange != null)
            {
                onScoreChange(score, scoreToAdd);
            }

            if (onBalloonPopped != null)
            {
                onBalloonPopped(time, scoreToAdd, isBonus);
            }
        }


        /** OLD Incremente Score 
         void IncrementeScore()
         {
             StarScore.Play();
             if (BalloonBehaviour.highScore )
             {
                 
                 int i = (int)Mathf.Floor( BalloonBehaviour.timeOfCollision);
                     int scoreGiven = 35 - (5 * i);
                   
                         score += scoreGiven;
                 
                 if (UIScoreAnim.isPlaying)
                 {
                     UIScoreAnim.Stop();
                     UIScoreText.text = "+" + scoreGiven.ToString();
                 }
                 else
                     UIScoreText.text = "+"+ scoreGiven.ToString();
                 UIScoreAnim.Play();
             }
             else if (BonusBehaviour.isBonus)
             {
                 score += 10;

                 if (UIScoreAnim.isPlaying)
                 {
                     UIScoreAnim.Stop();
                     UIScoreText.text = "+" + (int.Parse(UIScoreText.text) + 10).ToString();
                 }
                 else
                     UIScoreText.text = "+10";
                 UIScoreAnim.Play();
             }
             else
             {
                 score += 5;
                 if (UIScoreAnim.isPlaying)
                 {
                     UIScoreAnim.Stop();
                     UIScoreText.text = "+" + (int.Parse(UIScoreText.text) + 5).ToString();
                 }
                 else
                     UIScoreText.text = "+5";
                 UIScoreAnim.Play();
             }

             if(onScoreChange != null)
             {
                 onScoreChange(score);
             }

             
         }
         **/
    }
    #endregion
}

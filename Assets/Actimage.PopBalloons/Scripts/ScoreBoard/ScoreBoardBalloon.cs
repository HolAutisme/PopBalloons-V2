
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using PopBalloons.Utilities;

namespace PopBalloons.UI
{
    public class ScoreBoardBalloon : MonoBehaviour,IMixedRealityFocusHandler 

    {

        [SerializeField]
        private List<BalloonData> balloons;

        [SerializeField]
        private TimerObserver RevealTimer;

        [Tooltip("Prefab du pointeur vers l'objet")]
        [SerializeField]
        private GameObject DirectionnalIndicatorPrefab;

        private bool bonusStarWasPopped = false;
        private bool alreadyRevealed = false;

        private GameObject Indicator3D;
        private int currentBalloon = 0;

        [SerializeField]
        private BalloonData bonusStar;

        /// <summary>
        /// Vitesse de la révélation finale
        /// </summary>
        [Range(0.5f, 10f)]
        [SerializeField]
        private float revealDuration = 5f;
        private float speedFactor;

        [SerializeField]
        private AudioSource timerAudioSource;
        private AudioSource balloonAudioSource;

        [SerializeField]
        private AudioClip focusSound;

        [SerializeField]
        private List<AudioClip> balloonSound;

        [SerializeField]
        private AudioClip timerSound;

        [SerializeField]
        private AudioClip bonusSound;


        [System.Serializable]
        public class BalloonData
        {
            /// <summary>
            /// Champ du ballon dans le score board
            /// </summary>
            [SerializeField]
            private GameObject balloon;
            /// <summary>
            /// Champ texte du score du ballon
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Text scoreDisplay;
            /// <summary>
            /// Champ 
            /// </summary>
            [SerializeField]
            private UnityEngine.UI.Text timeDisplay;

            public float Time
            {
                get; set;
            }

            public int Score
            {
                get; set;
            }

            public void filedBalloon()
            {
                if (this.balloon != null)
                {
                    this.balloon.SetActive(true);
                }
            }

            public void DontFiilTheBalloon()
            {
                if(this.balloon != null)
                {
                    this.balloon.SetActive(false);
                }
            }

            public void displayScore()
            {
                if (this.scoreDisplay != null)
                {
                    this.scoreDisplay.gameObject.SetActive(true);
                    this.scoreDisplay.text = this.Score.ToString();
                }
            }

            public void displayTime()
            {
                if (this.timeDisplay != null)
                {
                    this.timeDisplay.gameObject.SetActive(true);
                    this.timeDisplay.text = this.Time.ToString("0.00") + "s";
                }
            }

            public void cleanBoard()
            {
                if (this.scoreDisplay != null)
                {
                    this.scoreDisplay.gameObject.SetActive(false);
                }

                if (this.timeDisplay != null)
                {
                    this.timeDisplay.gameObject.SetActive(false);
                }

                if (this.balloon != null)
                {
                    this.balloon.SetActive(false);
                }
            }
        }




        // Use this for initialization
        void Start()
        {
            ScoreManager.onBalloonPopped += AddBalloonField;
            //ScoreBoard.OnBoardStatusChange += handleChange;
            MotricityPanel.Instance.OnStateChanged += HandleChange;
            ScoreBoard.OnBoardGaze += StartRevealing;
            this.balloonAudioSource = this.GetComponent<AudioSource>();
        }


        private void OnDestroy()
        {
            ScoreManager.onBalloonPopped -= AddBalloonField;
            MotricityPanel.Instance.OnStateChanged -= HandleChange;
            ScoreBoard.OnBoardGaze -= StartRevealing;
        }

        private void HandleChange(MotricitySubState status)
        {

            if (status == MotricitySubState.ENDGAME)
            {
                timerAudioSource = MainPanel.Instance.GetComponent<AudioSource>();

                //Starting focus sound
                timerAudioSource.clip = focusSound;
                timerAudioSource.loop = true;
                timerAudioSource.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.SB_FOCUS);
                timerAudioSource.Play();

                //Display Indicator
                if (DirectionnalIndicatorPrefab != null)
                    Indicator3D = Instantiate(DirectionnalIndicatorPrefab, this.transform);

                //startRevealing();
                if (bonusStar != null)
                    bonusStar.cleanBoard();
                foreach (BalloonData bd in balloons)
                {
                    bd.cleanBoard();
                }

                alreadyRevealed = false;
            }
            else
            {
                if (Indicator3D != null)
                    Destroy(Indicator3D.gameObject);
            

            
                //On stop le reveal.
                StopAllCoroutines();
                //On libère le timer (au cas où)

                OverrideRevealTimer(false);
                currentBalloon = 0;
                bonusStarWasPopped = false;
                if (bonusStar != null)
                    bonusStar.cleanBoard();

                foreach (BalloonData bd in balloons)
                {
                    bd.cleanBoard();
                }

                if (timerAudioSource != null)
                {
                    timerAudioSource.Stop();
                }
                if (balloonAudioSource != null)
                {
                    balloonAudioSource.Stop();
                }
            }
        }

        private void AddBalloonField(float time, int scoreGain, bool isBonus)
        {
          
            if (currentBalloon < balloons.Count && !isBonus)
            {


                //May have ordering issue to fix.
                BalloonData b = balloons[currentBalloon];
                b.Score = scoreGain;
                b.Time = time;
                b.filedBalloon();
                currentBalloon++;
            }

            if (isBonus && bonusStar != null)
            {
                bonusStarWasPopped = true;
                bonusStar.filedBalloon();
                bonusStar.Score = scoreGain;
            }
        }

        private void OverrideRevealTimer(bool b)
        {
            if (RevealTimer != null)
            {
                RevealTimer.TimeOverride = b;
            }
        }


        private void UpdateRevealTimer(float f)
        {
            if (RevealTimer != null && RevealTimer.TimeOverride)
            {
                RevealTimer.setTime(f);
            }
        }

        public void StartRevealing()
        {
            //TODO: Reveal only if end of a Game !
            if (Indicator3D != null)
                Destroy(Indicator3D.gameObject);
            if (!alreadyRevealed)
            {

                alreadyRevealed = true;
                StartCoroutine(Results());
            }
        }


        #region Interfaces
        public void OnFocusEnter(FocusEventData eventData)
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.PLAY && MotricityPanel.Instance.GetState() == MotricitySubState.ENDGAME && GameCreator.Instance.BalloonDestroyed >= 5)
            {
                this.StartRevealing();
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            //Nothing
        }
        #endregion

        private IEnumerator Results()
        {
            //  we stop focus sound
            timerAudioSource.Stop();
            timerAudioSource.loop = false;


            float finalTime = TimerManager.GetTimeStamp();
            int counter = 0;
            float timeReveal = 0;
            float balloonFrequency = finalTime / balloons.Count;
            OverrideRevealTimer(true);
            UpdateRevealTimer(timeReveal);

            speedFactor = finalTime / revealDuration;

            timerAudioSource.PlayOneShot(timerSound, SoundMixManager.getVolume(SoundMixManager.SoundType.SB_TIMER));

            while (timeReveal < finalTime)
            {
                timeReveal += Time.deltaTime * speedFactor;
                UpdateRevealTimer(timeReveal);
                //while (balloons.Count > counter && timeReveal > balloons[counter].Time)
                if (balloons.Count > counter && timeReveal > counter * balloonFrequency)
                {
                    balloons[counter].filedBalloon();
                    balloons[counter].displayScore();
                    balloons[counter].displayTime();
                    balloonAudioSource.PlayOneShot(balloonSound[counter], SoundMixManager.getVolume(SoundMixManager.SoundType.SB_BALLOONS));
                    counter++;
                    //yield return new WaitForSeconds(0.75f);
                }
                yield return null;
            }
            //yield return new WaitForSeconds(0.75f);
            if (bonusStar != null && bonusStarWasPopped)
            {
                balloonAudioSource.PlayOneShot(bonusSound, SoundMixManager.getVolume(SoundMixManager.SoundType.SB_BONUS));
                bonusStar.filedBalloon();
                bonusStar.displayScore();
                bonusStarWasPopped = false;
            }
            OverrideRevealTimer(false);
            yield return null;
        }

    }

}

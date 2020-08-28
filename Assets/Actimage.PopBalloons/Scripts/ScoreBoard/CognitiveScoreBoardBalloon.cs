using Microsoft.MixedReality.Toolkit.Input;
using PopBalloons.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{
    public class CognitiveScoreBoardBalloon : MonoBehaviour, IMixedRealityFocusHandler
    {


        /// <summary>
        /// Champ texte du score du ballon
        /// </summary>
        [SerializeField]
        private TMPro.TextMeshProUGUI endQuote;

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
            private TMPro.TextMeshProUGUI scoreDisplay;

            /// <summary>
            /// Same as Score but with a string
            /// </summary>
            public string Results
            {
                get; set;
            }

            public Transform GetParent()
            {
                return balloon.transform.parent;
            }

            public void FilledBalloon()
            {
                if (this.balloon != null)
                {
                    this.balloon.SetActive(true);
                }
            }
            
            public void DisplayScore()
            {
                if (this.scoreDisplay != null)
                {
                    this.scoreDisplay.gameObject.SetActive(true);
                    this.scoreDisplay.text = this.Results;
                }
            }

            public void CleanBoard()
            {
                if (this.scoreDisplay != null)
                {
                    this.scoreDisplay.gameObject.SetActive(false);
                }

                if (this.balloon != null)
                {
                    this.balloon.SetActive(false);
                }
            }
        }


        [SerializeField]
        private List<BalloonData> balloons;


        [Tooltip("Prefab du pointeur vers l'objet")]
        [SerializeField]
        private GameObject DirectionnalIndicatorPrefab;

        private bool alreadyRevealed = false;

        private GameObject Indicator3D;

        /// <summary>
        /// Champ texte du score du ballon
        /// </summary>
        [SerializeField]
        private TMPro.TextMeshProUGUI progression;

        [SerializeField]
        private Image scoreBar;


        /// <summary>
        /// Vitesse de la révélation finale
        /// </summary>
        [Range(0.5f, 10f)]
        [SerializeField]
        private float revealDuration = 2f;
        private float speedFactor;

        [SerializeField]
        private AudioSource timerAudioSource;

        [SerializeField]
        private AudioClip focusSound;

        [SerializeField]
        private AudioClip timerSound;



        // Use this for initialization
        void Start()
        {
            ScoreManager.onScoreChange += RefreshScore;
            CognitivePanel.Instance.OnStateChanged += HandleChange;
            ScoreBoard.OnBoardGaze += StartRevealing;
        }

        private void OnDestroy()
        {
            ScoreManager.onScoreChange -= RefreshScore;
            CognitivePanel.Instance.OnStateChanged -= HandleChange;
            ScoreBoard.OnBoardGaze -= StartRevealing;
        }

        private void HandleChange(CognitiveSubState status)
        {

            if (status == CognitiveSubState.ENDGAME)
            {
                endQuote.text = "Bravo !\nTu as fini le niveau " + (GameManager.Instance.CurrentLevelIndex + 1);


                timerAudioSource = MainPanel.Instance.GetComponent<AudioSource>();

                //Starting focus sound
                timerAudioSource.clip = focusSound;
                timerAudioSource.loop = true;
                timerAudioSource.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.SB_FOCUS);
                timerAudioSource.Play();

                //Display Indicator
                if (DirectionnalIndicatorPrefab != null)
                    Indicator3D = Instantiate(DirectionnalIndicatorPrefab, this.transform);

                //foreach (BalloonData bd in balloons)
                //{
                //    bd.CleanBoard();
                //}
                balloons[0].GetParent().gameObject.SetActive(false);

                alreadyRevealed = false;
            }
            else
            {
                if (Indicator3D != null)
                    Destroy(Indicator3D.gameObject);

                //On stop le reveal.
                StopAllCoroutines();
                //On libère le timer (au cas où)


                if (timerAudioSource != null)
                {
                    timerAudioSource.Stop();
                }
            }
        }

        private void RefreshScore(int score, int scoreGain)
        {
            //TODO : Animate this
            this.scoreBar.fillAmount = GameCreator.Instance.GetAdvancement();
            this.progression.text = "<color=\"yellow\">" + (ScoreManager.instance.CorrectBalloon + ScoreManager.instance.WrongBalloon) + "/</color>" + GameCreator.Instance.MaxBalloon;
        }

        public void StartRevealing()
        {
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
            if (GameManager.Instance.CurrentState == GameManager.GameState.PLAY && CognitivePanel.Instance.GetState() == CognitiveSubState.ENDGAME)
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
            float balloonFrequency = finalTime / 3f;

            speedFactor = finalTime / revealDuration;

            timerAudioSource.PlayOneShot(timerSound, SoundMixManager.getVolume(SoundMixManager.SoundType.SB_TIMER));

            foreach (BalloonData balloon in balloons)
            {
                    balloon.Results = (counter == 0)
                            ? "<color=\"yellow\">" + ScoreManager.instance.CorrectBalloon.ToString() + "/</color>" + GameCreator.Instance.MaxBalloon
                            : (counter == 1)
                                ? "<color=\"yellow\">" + ScoreManager.instance.WrongBalloon.ToString() + "/</color>" + GameCreator.Instance.MaxBalloon
                                : "<color=\"yellow\">" + GameCreator.Instance.MaxBalloon + "/</color>" + GameCreator.Instance.MaxBalloon;
                    balloon.FilledBalloon();
                    balloon.DisplayScore();
                    counter++;
            }
            
                




            balloons[0].GetParent().gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);

            //while (timeReveal < finalTime)
            //{
            //    timeReveal += Time.deltaTime * speedFactor;
               
            //    if (balloons.Count > counter && timeReveal > counter * balloonFrequency)
            //    {
            //        balloons[counter].Results = (counter == 0)
            //            ? "<color=\"yellow\">" + ScoreManager.instance.CorrectBalloon.ToString() + "/</color>" + GameCreator.Instance.MaxBalloon
            //            : (counter == 1)
            //                ? "<color=\"yellow\">" + ScoreManager.instance.WrongBalloon.ToString() + "/</color>" + GameCreator.Instance.MaxBalloon
            //                : "<color=\"yellow\">" + GameCreator.Instance.MaxBalloon + "/</color>" +  GameCreator.Instance.MaxBalloon ;
            //        balloons[counter].FilledBalloon();
            //        balloons[counter].DisplayScore();
            //        counter++;

            //        yield return new WaitForSeconds(0.75f);
            //    }
            //    yield return null;
            //}
            
            yield return null;
        }

    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using PopBalloons.Utilities;

namespace PopBalloons
{

    public class ScoreBoard : MonoBehaviour, IMixedRealityFocusHandler
    {

       
        #region variables
           public static ScoreBoard Instance;


        private int balloonDisplayed = 0;


        private GameObject normalState;

        /// <summary>
        /// Affichage du score
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.Text scoreText;

        /// <summary>
        /// Affichage du nombre de points gagnés
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.Text scorePointAdd;

        /// <summary>
        /// Animator du game object du score.
        /// </summary>
        [SerializeField]
        private Animator scoreAnim;


        /// <summary>
        /// Enumération définissant le statut actuel du tableau de score
        /// </summary>
        public enum BoardStatus { INITIAL, LEVEL, END, FINAL, LAST_LEVEL_FINAL, TUTORIAL, FREE_PLAY, FREE_PLAY_FINAL }

        public BoardStatus currentState = BoardStatus.INITIAL;
        #endregion
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region events & delegates
        public delegate void UIBoardChange(BoardStatus status);
        public static event UIBoardChange OnBoardStatusChange;
        public delegate void UIBoardGaze();
        public static event UIBoardGaze OnBoardGaze;
        public static event ScoreManager.balloonWasPopped OnBalloonPopped;
        public static event LoadLevels.NextLevel OnNextLevel;
        public static event LoadLevels.LoadScene OnLoadScene;
        #endregion

        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region unity functions
        void Start()
        {
            OnBoardStatusChange += UpdateStatus;
            GameManager.OnGameStateChanged += StateChanged;
            ScoreManager.onBalloonPopped += BalloonPopped;
            ScoreManager.onScoreChange += UpdateScore;
            ScoreManager.onBalloonPopped += HandleBalloonPop;
            LoadLevels.OnLevelEnd += LevelEnd;
        }



        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }



        private void OnDestroy()
        {
            OnBoardStatusChange -= UpdateStatus;
            GameManager.OnGameStateChanged -= StateChanged;
            ScoreManager.onScoreChange -= UpdateScore;
            ScoreManager.onBalloonPopped -= HandleBalloonPop;
            LoadLevels.OnLevelEnd -= LevelEnd;
        }
        #endregion


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region functions
        public void BalloonPopped(float time, int scoreGain, bool isBonus)
        {
            // BalloonPop(time, scoreGain, isBonus);
            if (OnBalloonPopped != null)
            {
                OnBalloonPopped.Invoke(time, scoreGain, isBonus);
            }
        }



        private void NewStatus(BoardStatus current)
        {
            if (GameManager.Instance.CurrentLevelIndex == GameManager.Instance.MaxLevelCount && current == ScoreBoard.BoardStatus.FINAL)
            {
                current = ScoreBoard.BoardStatus.LAST_LEVEL_FINAL;
            }

            if (OnBoardStatusChange != null) OnBoardStatusChange.Invoke(current);
        }

        public void UpdateStatus(BoardStatus status)
        {
                 currentState = status;
        }

        public void LevelEnd()
        {
            Invoke("LevelEndNoDelay", 8f);
        }

        private void LevelEndNoDelay()
        {
            NewStatus((currentState == BoardStatus.FREE_PLAY) ? BoardStatus.FREE_PLAY_FINAL : BoardStatus.END);
        }

        public void NextPanel()
        {
            NewStatus(BoardStatus.FINAL);
        }


        public void NextLevel()
        {
            if(OnNextLevel != null)
            {
                OnNextLevel.Invoke();
            }
        }

        public void ReturnToMenu(string sceneName)
        {
            //if (OnLoadScene != null)
            //{
            //    OnLoadScene.Invoke(sceneName);
            //}
            GameManager.Instance.Home();
        }

        private void HandleBalloonPop(float time, int scoreGain, bool isBonus)
        {

            if (currentState == BoardStatus.LEVEL)
            {
                if (!isBonus)
                    balloonDisplayed++;

                return;
            }
            //DO nothing, we are not in level
        }



        public void UpdateScore(int score, int scoreToAdd)
        {
            Debug.Log("ScoreBoard : " + score + " Gain :" + scoreToAdd);
            if (GameCreator.Instance != null)
            {
                if (scoreText != null)
                {
                    scoreText.text = score.ToString();
                }
                if (scoreToAdd != 0)
                {

                    if (scorePointAdd != null)
                    {
                        scorePointAdd.text = "+" + scoreToAdd.ToString();
                    }

                    if (scoreAnim != null)
                    {
                        scoreAnim.Play("MainScored");
                    }
                }
            }
    
        }

        public void UpdateStatuss(BoardStatus current)
        {

        }

        private void StateChanged(GameManager.GameState newState)
        {
            ScoreManager.initScore();
            TimerManager.InitTimer();
            BoardStatus newStatus;

            switch (newState)
            {
                case GameManager.GameState.INIT:
                case GameManager.GameState.SETUP:
                case GameManager.GameState.HOME:
                    newStatus = BoardStatus.INITIAL;
                    break;
                case GameManager.GameState.PLAY:
                    switch (GameManager.Instance.CurrentGameType)
                    {
                        case GameManager.GameType.CLASSIC:
                            newStatus = BoardStatus.LEVEL;
                            break;
                        case GameManager.GameType.COGNITIVE:
                            newStatus = BoardStatus.LEVEL; //TODO
                            break;
                        case GameManager.GameType.FREEPLAY:
                            newStatus = BoardStatus.FREE_PLAY;
                            break;
                        default:
                            newStatus = BoardStatus.INITIAL;
                            break;
                    }
                    break;
                default:
                    newStatus = BoardStatus.INITIAL;
                    break;
            }

                NewStatus(newStatus);
        }


        public void move(Vector3 position, Quaternion rotation)
        {
            this.transform.position = position;
            this.transform.rotation = rotation;
        }


        public void OnFocusEnter(FocusEventData eventData)
        {
            if (OnBoardGaze != null)
            {
                OnBoardGaze();
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
           
        }

        #endregion
    }

}

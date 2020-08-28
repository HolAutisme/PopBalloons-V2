using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopBalloons.Utilities;


namespace PopBalloons.UI
{
    public enum CognitiveSubState {SETUP, INGAME, ENDGAME, TUTORIAL,TUTORIAL_END};
    public class CognitivePanel : SubPanel<CognitiveSubState>
    {
        #region Variables
        private static CognitivePanel instance;


        /// <summary>
        /// Singleton instance accessor
        /// </summary>
        public static CognitivePanel Instance { get => instance; }
        #endregion Variables


        #region Unity Functions
        /// <summary>
        /// Singleton's implementation.
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Should'nt have two instances of MotricityPanel.");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }


        /// <summary>
        /// Initialization
        /// </summary>
        private void Start()
        {
            this.SetState(CognitiveSubState.SETUP);

            GameManager.OnGameStateChanged += HandleStateChanged;
            GameCreator.OnGameStarted += (type) => { if (type == GameManager.GameType.COGNITIVE) this.HandleNewGame();};
            GameCreator.OnGameEnded += (type) => { if (type == GameManager.GameType.COGNITIVE) this.HandleGameEnded();};
        }


        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleStateChanged;
        }
        #endregion Unity Functions 


        #region Functions

        public void ExitLevel()
        {
            GameManager.Instance.Home();
        }

        private void HandleStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.INIT:
                case GameManager.GameState.SETUP:
                case GameManager.GameState.HOME:
                    this.SetState(CognitiveSubState.SETUP);
                    //Debug.Log("Resetting to Setup");
                    break;
                case GameManager.GameState.PLAY:
                    this.HandleNewGame();
                    break;
            }
        }


        private void HandleNewGame()
        {
            if(GameManager.Instance.CurrentGameType == GameManager.GameType.NONE || GameManager.Instance.CurrentState != GameManager.GameState.PLAY)
            {
                //Debug.Log("Had to bypass newGame");
                this.SetState(CognitiveSubState.SETUP);
            }
            else if (GameManager.Instance.CurrentLevelIndex ==  0)
            {
                //Tutorial
                this.SetState(CognitiveSubState.TUTORIAL);
            }
            else
            {
                //TODO : this.ResetDisplay();
                this.SetState(CognitiveSubState.INGAME);
            }
           
        }

        private void HandleGameEnded()
        {
            if (GameManager.Instance.CurrentLevelIndex == 0)
            {
                //Tutorial
                this.SetState(CognitiveSubState.TUTORIAL_END);
            }
            else
            {
                this.SetState(CognitiveSubState.ENDGAME);
            }
            //Wait for scoring animation
        }

        public override void Init()
        {
            this.SetState(CognitiveSubState.SETUP);
        }


        #endregion

    }

}
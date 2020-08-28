using PopBalloons.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PopBalloons.UI
{
    public enum MotricitySubState {SETUP, INGAME, ENDGAME, TUTORIAL, TUTORIAL_END};
    public class MotricityPanel : SubPanel<MotricitySubState>
    {
        #region Variables
        private static MotricityPanel instance;


        /// <summary>
        /// Singleton instance accessor
        /// </summary>
        public static MotricityPanel Instance { get => instance; }
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
            this.SetState(MotricitySubState.SETUP);

            GameCreator.OnGameStarted += (type) => { if (type == GameManager.GameType.CLASSIC) this.HandleNewGame();};
            GameCreator.OnGameEnded += (type) => { if (type == GameManager.GameType.CLASSIC) this.HandleGameEnded();};
            GameCreator.OnGameInterrupted += (type) => { if (type == GameManager.GameType.CLASSIC) this.HandleGameInterrupted();};
        }
        #endregion Unity Functions 


        #region Functions
        private void HandleStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.INIT:
                case GameManager.GameState.SETUP:
                case GameManager.GameState.HOME:
                    this.SetState(MotricitySubState.SETUP);
                    break;
                case GameManager.GameState.PLAY:
                    this.HandleNewGame();
                    break;
            }
        }


        private void HandleNewGame()
        {
            if (GameManager.Instance.CurrentLevelIndex ==  0)
            {
                //Tutorial
                this.SetState(MotricitySubState.TUTORIAL);
            }
            else
            {
                //TODO : this.ResetDisplay();
                this.SetState(MotricitySubState.INGAME);
            }
           
        }

        private void HandleGameInterrupted()
        {
            this.SetState(MotricitySubState.SETUP);
        }

        private void HandleGameEnded()
        {
            if (GameManager.Instance.CurrentLevelIndex == 0)
            {
                //Tutorial
                this.SetState(MotricitySubState.TUTORIAL_END);
            }
            else
            {
                //TODO: Wait few second scoring animation
                this.SetState(MotricitySubState.ENDGAME);
            }
            
        }

        public override void Init()
        {
            this.SetState(MotricitySubState.SETUP);
        }

        #endregion

    }

}
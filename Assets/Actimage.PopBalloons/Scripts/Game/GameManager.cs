using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using PopBalloons.Configuration;
using PopBalloons.Data;

namespace PopBalloons.Utilities
{
    /// <summary>
    /// Handle the game status, scene initialisation, and the level management
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Variables

        public static GameManager Instance;
        public enum GameState {INIT,SETUP,HOME,PLAY}
        public enum GameType {NONE,CLASSIC,COGNITIVE,FREEPLAY}


        private GameState currentState;
        private int currentLevelIndex = 0; //0 means tutorial
        private GameType currentGameType;
 
        [SerializeField]
        private List<LevelInfo> AvailableLevels;

        /// <summary>
        /// Instance of JulieManager
        /// </summary>
        private JulieManager julie;

        //Accessors
        public GameType CurrentGameType { get => currentGameType; }
        public int CurrentLevelIndex { get => currentLevelIndex;  }
        public GameState CurrentState { get => currentState;}
        public int MaxLevelCount
        {
            get
            {
                switch (CurrentGameType)
                {
                    case GameType.CLASSIC:
                        return 4;
                    case GameType.COGNITIVE:
                        return 7;
                    case GameType.FREEPLAY:
                        return 1;
                    case GameType.NONE:
                        return 0;
                    default:
                        return 0;
                };
            }
        }
        #endregion

        #region Events
        public delegate void GameStateChange(GameState newState);
        public static event GameStateChange OnGameStateChanged;

        #endregion

        #region Unity Functions

        /// <summary>
        /// Singleton execution
        /// </summary>
        private void Awake()
        {
            if(Instance != null)
            {
                DestroyImmediate(this);
            }
            else
            {
                DontDestroyOnLoad(this);
                Instance = this;
            }
        }

        private void Start()
        {
            ScoreBoard.OnNextLevel += NextLevel;
        }

        private void OnDestroy()
        {
            ScoreBoard.OnNextLevel -= NextLevel;
        }

        private void Update()
        {
            //Switch that will handle the 
            switch (currentState)
            {
                case GameState.INIT:
                    //LoadScene, then wait for setup  
                    UnloadAllScenes();
                    //SceneManager.LoadScene("PlaySpace", LoadSceneMode.Additive);
                    this.currentState = GameState.SETUP;
                    julie = this.GetComponent<JulieManager>();
                    break;
                case GameState.SETUP:
                    //Wait for area to be set
                    break;
                default:
                    //Do nothing
                    break;
            }

        }

        #endregion

        #region Functions 

        /// <summary>
        /// Create a new game and set the Game Settings
        /// </summary>
        /// <param name="type"></param>
        /// <param name="levelIndex"></param>
        public void NewGame(GameType type, int levelIndex)
        {
            if(currentState == GameState.HOME)
            {
                UnloadAllScenes(); 
            }

            if(currentState == GameState.PLAY)
            {
                TimerManager.LevelEnd();
                GameCreator.Instance.QuitLevel();
                //TODO: Manage level quitting?
            }

            this.currentLevelIndex = levelIndex;
            this.currentGameType = type;
            this.currentState = GameState.PLAY;
            ScoreManager.initScore();
            OnGameStateChanged?.Invoke(this.currentState);
            GameCreator.Instance.Play(type);
        }

        /// <summary>
        /// Will play the next level in the same mode as current.
        /// </summary>
        public void NextLevel()
        {
            this.NewGame(currentGameType, currentLevelIndex + 1);
        }


        /// <summary>
        /// Will display home menu scene
        /// </summary>
        public void Home()
        {
            if(currentState == GameState.PLAY)
            {
                GameCreator.Instance.QuitLevel();
                //TODO : Save session Data
            }

            this.currentState = GameState.HOME;
            this.currentGameType = GameType.NONE;
            this.currentLevelIndex = 0;
            OnGameStateChanged?.Invoke(this.currentState);
            //Debug.Log("Home sweet home");
            //We remove unwanted scene because off additing loading
            UnloadAllScenes();
            //SceneManager.LoadScene("Menu", LoadSceneMode.Additive);

        }

        /// <summary>
        /// Will save current session data
        /// </summary>
        public void Save()
        {
            if (ProfilesManager.Instance != null)
                ProfilesManager.Instance.Save(CurrentGameType.ToString() + "_" + (currentLevelIndex + 1).ToString(), ScoreManager.instance.score);
            //if (DataManager.instance != null)
            //    DataManager.instance.SaveDatas(CurrentGameType.ToString()+"_"+(currentLevelIndex+1).ToString(), ScoreManager.instance.score);
        }

        
        
        /// <summary>
        /// Unloads all the scenes in the game except for the "Setup" which is the principal scene of the game and the the scene which we have it's name here.
        /// </summary>
        /// <param name="SceneName"></param>
        public void UnloadAllScenes()
        {
            int c = SceneManager.sceneCount;
            for (int i = 0; i < c; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Setup")
                {
                    SceneManager.UnloadSceneAsync(scene);
                }

            }
        }

        public List<LevelInfo> GetAvailableLevels(GameType type)
        {
            if(AvailableLevels != default)
            {
                return AvailableLevels.FindAll((level) => level.Type == type);
            }
            return new List<LevelInfo>();
        }


        public void SetupCompleted()
        {
         //   julie.Init();
            Home();
        }
        #endregion
    }
}


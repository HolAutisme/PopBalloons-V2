using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.EventSystems;
using System;
using Random = UnityEngine.Random;
using PopBalloons.Boundaries;
using PopBalloons.Data;

namespace PopBalloons.Utilities
{
    /// <summary>
    /// Begining the game and managing it in different levels
    /// </summary>
    public class GameCreator : MonoBehaviour
    {
        /// <summary>
        /// Enum handeling different tutorial state
        /// </summary>
        public enum TutorialState { INITIALISATION, POSITIONNING, WAITING_FOR_DEMONSTRATION, DEMONSTRATION, DIY, FINISH }

        #region variables
        [Header("Classic Game settings: ")]
        [SerializeField]
        [Range(0f,2f)]
        [Tooltip("Determine the position of the balloon")]
        private float heightRange = 1.4f;
        [SerializeField]
        [Range(-1f, 1f)]
        [Tooltip("Offset of the height range (Vector3.UP * this factor)")]
        private float headOffset = -0.25f;
        [SerializeField]
        [Range(0f, 1.5f)]
        private float minDistance = 1f;
        [SerializeField]
        [Range(1,5)]
        private int maxElementsOnScene = 1;
        [Header("Cognitive Game settings: ")]
        [SerializeField]
        [Range(2, 20)]
        [Tooltip("Determine the minimum number of wave the children has to pass")]
        private int nbWave = 20;
        //[SerializeField]
        //[Range(2, 20)]
        //[Tooltip("Determine the number of option displayed to the user")]
        //private int maxOptions = 5;
        //[SerializeField]
        //[Range(2, 10)]
        //[Tooltip("Determine the number of option displayed to the user")]
        //private int minOptions = 2;



        [Header("Object settings: ")]
        [SerializeField]
        private GameObject countdown;

        [SerializeField]
        private List<BalloonBehaviour> balloonsPrefabs;
        [SerializeField]
        private List<BonusBehaviour> bonusBalloonsPrefabs;
       
        public enum BalloonColor {BLUE,RED,GREEN,YELLOW,NONE}

        private TutorialState currentTutorialState = TutorialState.FINISH;
        private static GameCreator instance;
        private float balloonDestroyed = 0;

        private bool GameIsRunning;
        private int balloonsInScene; //the number of balloons in scene
        private List<BalloonBehaviour> balloons;
        private List<BalloonColor> availableColors;
        private List<BalloonColor> usedColors;
        private BalloonColor intendedColor;
        /// <summary>
        /// Determine how many balloon there is to be destroy in this lvl
        /// </summary>
        private int maxBalloon;

        public static GameCreator Instance { get => instance; }
        public int BalloonsInScene { get => balloonsInScene; }
        public int MaxBalloon { get => maxBalloon; }
        private bool MoreThanEnough { get => this.BalloonsInScene >= maxElementsOnScene; }
        private bool CorrectBalloonsRemains { get => this.balloons.FindAll(balloon => balloon.GetColor() == IntendedColor).Count > 0; }
        public float BalloonDestroyed { get => balloonDestroyed; set => balloonDestroyed = value; }
        public BalloonColor IntendedColor { get => intendedColor; }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Events
        public delegate void GameStateChange(GameManager.GameType type);
        public static event GameStateChange OnGameStarted;
        public static event GameStateChange OnGameInterrupted;
        public static event GameStateChange OnGameEnded;
        #endregion

        #region unity functions

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
 
        }
        private void Start()
        {
            balloons = new List<BalloonBehaviour>();
            BalloonDestroyed = 0;
            balloonsInScene = 0;
            BalloonBehaviour.OnCognitiveBalloonDestroyed += HandleCognitiveBalloonPopped;
        }

        private void OnDestroy()
        {
            BalloonBehaviour.OnCognitiveBalloonDestroyed -= HandleCognitiveBalloonPopped;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Functions
        
        /// <summary>
        /// Will handle the destruction of a balloon inside a cognitive level
        /// </summary>
        /// <param name="color"></param>
        private void HandleCognitiveBalloonPopped(BalloonBehaviour balloon)
        {
            if (balloons.Contains(balloon))
            {
                ScoreManager.instance.CognitiveBalloonPopped(balloon.GetColor() == IntendedColor);
                JulieManager.Instance.Play((balloon.GetColor() == IntendedColor) ? JulieManager.JulieAnimation.Clap : JulieManager.JulieAnimation.Sad);


                BalloonDatas data = new BalloonDatas();
                data.id = balloon.Id.ToString();
                data.color = balloon.GetColor().ToString();
                data.dateOfSpawn = balloon.DateOfSpawn;
                data.dateOfDestroy = System.DateTime.Now.ToString("HH-mm-ss.fff");
                data.timeOfDestroy = TimerManager.GetTime();
                

                if (DataManager.instance != null)
                {
                    DataManager.instance.AddBalloonsDatas(data);
                }
                

                //Balloon handle its animation itself
                balloon.PopBalloon();
                GameCreator.Instance.BalloonDestroyed++;
                this.DeflateAllBalloons();

            }
            else
            {
                Debug.Log("A cognitive balloon has been destroyed by user, but such balloon wasn't known by GameCreator. This should not happened");
            }
        }

        private void DeflateAllBalloons()
        {
            for (int i = balloons.Count - 1; i >= 0; i--)
            {
                balloons[i].DeflateBalloon();
                this.RemoveBalloon(balloons[i], false);
            }
        }

        /// <summary>
        /// Increse balloon counting
        /// </summary>
        public void AddBalloon(BalloonBehaviour balloon)
        {
            balloons.Add(balloon);
            balloonsInScene++;
        }

        /// <summary>
        /// decrese balloon counting 
        /// </summary>
        public void RemoveBalloon(BalloonBehaviour balloon,bool destroy = true)
        {
            if (balloons.Contains(balloon))
                balloonsInScene--;
            balloons.Remove(balloon);
            if (destroy)
                Destroy(balloon.gameObject);
        }

        /// <summary>
        /// Spawn a bonus balloon inside the area
        /// </summary>
        private void CreateRandomBonusBalloon()
        {
            Vector3 pos = PlaySpace.GetRandomPointInArea();
            //Random height
            pos.y = Camera.main.transform.position.y + headOffset - heightRange / 2f + Random.Range(0f, heightRange);
            Quaternion rot = Quaternion.identity;
            int random = Random.Range(0, bonusBalloonsPrefabs.Count);
            BonusBehaviour balloon = Instantiate(bonusBalloonsPrefabs[random], pos, rot); 
        }

        /// <summary>
        /// Will add a random balloon in the area
        /// </summary>
        private void CreateRandomBalloon()
        {
            Vector3 pos;
            Quaternion rot;
            int random = 0;

            if (BalloonDestroyed == 0)
            {
                rot = Quaternion.Euler(-90, -90, 0);
                pos = Camera.main.transform.position + Vector3.Scale(new Vector3(1f,0f,1f),Camera.main.transform.forward * 1.0f);
            }
            else
            {
                pos = PlaySpace.GetRandomPointInArea();
                float referenceHeight = PlaySpace.Instance.FloorHeight.Value + 1.20f;


                //Random height
                pos.y = Mathf.Max(Camera.main.transform.position.y, referenceHeight) + headOffset - heightRange / 2f + Random.Range(0f, heightRange);
                rot = Quaternion.Euler(-90, -90, 0);
                random = Random.Range(0, balloonsPrefabs.Count);
            }

            BalloonBehaviour balloon = Instantiate(balloonsPrefabs[random], pos, rot);
            this.AddBalloon(balloon);
        }
        
        /// <summary>
        /// Will create a specific balloon between user and Julie
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="totalBalloon"></param>
        private Data.BalloonOption CreateBalloon(BalloonBehaviour prefab,int position,int totalBalloon)
        {
            Vector3 pos;
            Quaternion rot;
            float spaceBetweenBalloon = 20;
            Vector3 center = PlaySpace.Instance.GetCenter().transform.position;
            Vector3 julie = JulieManager.Instance.GetPosition();

            Vector3 direction = (julie - center);
            //Quaternion.LookRotation(julie - center, Vector3.up).
            float angle = (totalBalloon % 2 == 0)
                ? (position % 2 == 0)
                    ? 0.5f * spaceBetweenBalloon + position / 2 * spaceBetweenBalloon
                    : -0.5f * spaceBetweenBalloon - position / 2 * spaceBetweenBalloon
                : (position % 2 == 0)
                    ? position / 2 * spaceBetweenBalloon
                    : -(position + 1) / 2 * spaceBetweenBalloon;

            //Debug.Log("Angle : "+angle + " Balloon ID: " + position + " TotalBalloon :" + totalBalloon+"Calc : "+ position / 2);
            Vector3 finalDir = Quaternion.Euler(0, angle, 0) * direction;
            pos = center + finalDir.normalized * minDistance;
            float referenceHeight = PlaySpace.Instance.FloorHeight.Value + 1.20f;


            //Random height
            pos.y = Mathf.Max(Camera.main.transform.position.y, referenceHeight) + headOffset;
            rot = Quaternion.Euler(-90, -90, 0);

            BalloonBehaviour balloon = Instantiate(prefab, pos, rot);
            //TODO: Save ID in balloonBehaviour;

            this.AddBalloon(balloon);
            Data.BalloonOption option = new Data.BalloonOption();
            option.position = pos;
            option.index = position;
            option.id = balloon.Id.ToString();
            option.color = balloon.GetColor().ToString();

            return option;

        }


        private void Init()
        {
            BalloonDestroyed = 0;
            balloonsInScene = 0;
            ScoreManager.instance.SetScore(0);
        }

        public void Play(GameManager.GameType type)
        {
            if (GameIsRunning)
            {
                //Quit current session ?
                Debug.Log("A game is already running, should quit.");
                
            }

            Init();

            switch (type)
            {
                case GameManager.GameType.CLASSIC:
                    this.PlayClassic();
                    break;
                case GameManager.GameType.COGNITIVE:
                    this.PlayCognitive();
                    break;
                case GameManager.GameType.FREEPLAY:
                    this.FreePlay();
                    break;
            }

            //We notify panels that a game has started
            OnGameStarted?.Invoke(type);
        }

        /// <summary>
        /// Will launch a classic game of popballoon.
        /// </summary>
        public void PlayClassic()
        {
            StartCoroutine(ClassicSpawning());
        }

        /// <summary>
        /// Will launch game of the new version of popballoon.
        /// </summary>
        public void PlayCognitive()
        {
            StartCoroutine(CognitiveSpawning());
        }

        /// <summary>
        /// Will launch a with countdown and infinite balloons.
        /// </summary>
        public void FreePlay()
        {
            Debug.Log("Not implemented yet");
        }

        /// <summary>
        /// Will abort current level status
        /// </summary>
        public void QuitLevel()
        {

            this.StopAllCoroutines();
            DeflateAllBalloons();
            countdown?.SetActive(false);
            OnGameInterrupted?.Invoke(GameManager.Instance.CurrentGameType);
        }

        /// <summary>
        /// Return a prefab of the correct balloon for this level
        /// </summary>
        /// <returns>A balloon behaviour prefab</returns>
        private BalloonBehaviour GetCorrectBalloonPrefab()
        {
            return balloonsPrefabs.Find(balloon => balloon.GetColor() == IntendedColor);
        }
        /// <summary>
        /// Return a prefab one of the wrong balloon for this level
        /// </summary>
        /// <returns>A balloon behaviour prefab</returns>
        private BalloonBehaviour GetWrongBalloonPrefab()
        {
            
            int rand = Random.Range(0, availableColors.Count-usedColors.Count);
            BalloonBehaviour b = balloonsPrefabs.FindAll(balloon => availableColors.Contains(balloon.GetColor()) && !usedColors.Contains(balloon.GetColor()))[rand];
            usedColors.Add(b.GetColor());
            return b;
        }


        private List<BalloonColor> GetAvailableColors(int maxOption = 2)
        {
            List<BalloonColor> colors = new List<BalloonColor>();
            switch (GameManager.Instance.CurrentLevelIndex)
            {
                case 0:
                    colors.Add(BalloonColor.BLUE);
                    colors.Add(BalloonColor.RED);
                    break;
                case 1:
                    colors.Add(BalloonColor.BLUE);
                    colors.Add(BalloonColor.RED);
                    break;
                default:
                    Debug.Log("DEFAULT LEVEL");
                    for (int i = 0; i<4; i++)
                    {
                        colors.Add((BalloonColor)i);
                    }

                    while(colors.Count > maxOption)
                    {
                        colors.RemoveAt(Random.Range(0, colors.Count)); //Remove N random color from the list
                    }
                    break;
                    
            }
            return colors;
        }

        /// <summary>
        /// Cognitve mode's settings
        /// </summary>
        /// <returns></returns>
        private int GetMaxOptions()
        {
            switch (GameManager.Instance.CurrentLevelIndex)
            {
                case 0:
                case 1:
                case 2: 
                case 5: return 2;
                case 3: 
                case 6: return 3;
                case 4:
                case 7: return 4;
                default: return 2;
            }
        }

        public float GetAdvancement()
        {
            if(maxBalloon != 0)
                return ((float) ScoreManager.instance.CorrectBalloon + ScoreManager.instance.WrongBalloon) / (float)this.maxBalloon;
            return 0f;
        }

        #endregion


        #region Coroutines
        IEnumerator HandleMotricityTutorialState()
        {
            switch (currentTutorialState)
            {
                case TutorialState.INITIALISATION:
                    JulieManager.Instance.Init(true);
                    this.currentTutorialState = TutorialState.POSITIONNING;
                    break;
                case TutorialState.POSITIONNING:
                    if (JulieManager.Instance.CurrentState == JulieManager.JulieState.READY)
                    {
                        JulieManager.Instance.Play(JulieManager.JulieAnimation.Setup_Motricity);
                        this.currentTutorialState = TutorialState.WAITING_FOR_DEMONSTRATION;
                        yield return new WaitForSeconds(1f);
                    }
                    break;
                case TutorialState.WAITING_FOR_DEMONSTRATION:
                    if (JulieManager.Instance.IsFocused())
                    {
                        this.currentTutorialState = TutorialState.DEMONSTRATION;
                    }
                    break;
                case TutorialState.DEMONSTRATION:
                    JulieManager.Instance.Play(JulieManager.JulieAnimation.Demonstrate_Motricity);
                    yield return new WaitForSeconds(3f);
                    balloonDestroyed = 0;
                    this.currentTutorialState = TutorialState.DIY;
                    break;
                case TutorialState.DIY:
                    //TODO: spawn a ballon
                    if(balloonDestroyed == 0 && balloonsInScene == 0)
                    {
                        yield return new WaitForSeconds(1f);
                        CreateRandomBalloon();
                    }

                    if(balloonDestroyed != 0)
                    {
                        this.currentTutorialState = TutorialState.FINISH;
                    }

                    break;
                case TutorialState.FINISH:
                    //JulieManager.Instance.Play(JulieManager.JulieAnimation.Disappear);
                    break;
            }

            yield return null;
        }

        IEnumerator HandleCognitiveTutorialState()
        {
            switch (currentTutorialState)
            {
                case TutorialState.INITIALISATION:
                    JulieManager.Instance.Init(true);
                    this.currentTutorialState = TutorialState.POSITIONNING;
                    break;
                case TutorialState.POSITIONNING:
                    if (JulieManager.Instance.CurrentState == JulieManager.JulieState.READY)
                    {
                        JulieManager.Instance.Play(JulieManager.JulieAnimation.Setup_Cognitive);
                        this.currentTutorialState = TutorialState.WAITING_FOR_DEMONSTRATION;
                        yield return new WaitForSeconds(1f);
                    }
                    break;
                case TutorialState.WAITING_FOR_DEMONSTRATION:
                    if (JulieManager.Instance.IsFocused())
                    {
                        this.currentTutorialState = TutorialState.DEMONSTRATION;
                    }
                    break;
                case TutorialState.DEMONSTRATION:
                    JulieManager.Instance.Play(JulieManager.JulieAnimation.Demonstrate_Cognitive);
                    yield return new WaitForSeconds(10.5f);
                    balloonDestroyed = 0;
                    ScoreManager.initScore();
                    this.currentTutorialState = TutorialState.DIY;
                    break;
                case TutorialState.DIY:
                    //the tutorial balloon is always blue.
                    maxBalloon = 2;

                    if (BalloonDestroyed > 4 || (BalloonDestroyed >= 3 && ScoreManager.instance.CorrectBalloon < 2))
                    {
                        this.currentTutorialState = TutorialState.POSITIONNING;
                        break;
                    }

                    if (balloonsInScene == 0 && ScoreManager.instance.CorrectBalloon == 0)
                    {
                        yield return new WaitForSeconds(3f);
                        usedColors.Clear();
                        //We spawn balloons
                        this.CreateBalloon(GetCorrectBalloonPrefab(), 0, 2);
                        this.CreateBalloon(GetWrongBalloonPrefab(), 1, 2);
                        
                        break;
                    }

                    if (balloonsInScene == 0 && ScoreManager.instance.CorrectBalloon == 1)
                    {
                        yield return new WaitForSeconds(3f);
                        usedColors.Clear();
                        //We spawn balloons
                        this.CreateBalloon(GetWrongBalloonPrefab(), 0, 2);
                        this.CreateBalloon(GetCorrectBalloonPrefab(), 1, 2);
                        //yield return new WaitForSeconds(3f);
                        break;
                    }


                    if (balloonsInScene == 0 && ScoreManager.instance.CorrectBalloon == 2)
                    {
                        this.currentTutorialState = TutorialState.FINISH;
                        break;
                    }

                    break;
                case TutorialState.FINISH:
                    
                    break;
            }

            yield return null;
        }

        /// <summary>
        /// Classic game level, five balloon to destroy inside the area
        /// </summary>
        /// <returns></returns>
        IEnumerator ClassicSpawning()
        {
            TimerManager.LevelEnd();
            TimerManager.InitTimer();
            yield return new WaitForSeconds(1.0f);

            intendedColor = BalloonColor.BLUE;
            RefreshJulieShirt();

            if (GameManager.Instance.CurrentLevelIndex == 0)//level is Tutorial (simplified version)
            {
                this.currentTutorialState = TutorialState.INITIALISATION;
                while (currentTutorialState != TutorialState.FINISH)
                {
                    yield return HandleMotricityTutorialState();
                }
                JulieManager.Instance.Play(JulieManager.JulieAnimation.Disappear);
            }
            else
            {

                countdown.SetActive(true);
                maxBalloon = 5;


                // Level intro delay
                yield return new WaitForSeconds(3.2f);
                TimerManager.LevelStart();

                while (BalloonDestroyed < maxBalloon)
                {
                    if (!MoreThanEnough)
                    {
                        CreateRandomBalloon();
                        yield return new WaitForSeconds(2);
                    }
                    else
                    {
                        yield return null;
                    }
                }

                CreateRandomBonusBalloon();
                
                //TODO: Subscribe TimerManager to this dedicated event.
                TimerManager.LevelEnd();
                //We wait a few second before ending level completly
                yield return new WaitForSeconds(5f);

                //ScoreBoard.Instance.LevelEnd();
                //GameManager.LevelEnd();
            }
            OnGameEnded?.Invoke(GameManager.GameType.CLASSIC);
        }

        /// <summary>
        /// Classic game level, five balloon to destroy inside the area
        /// </summary>
        /// <returns></returns>
        IEnumerator CognitiveSpawning()
        {
            TimerManager.LevelEnd();
            TimerManager.InitTimer();
            yield return new WaitForSeconds(1.0f);

            int nbOption = GetMaxOptions();
            availableColors = GetAvailableColors(nbOption);
            usedColors = new List<BalloonColor>();
           

            if (GameManager.Instance.CurrentLevelIndex == 0)//level is Tutorial (simplified version)
            {
                intendedColor = BalloonColor.BLUE;
                availableColors.Remove(intendedColor);
                this.RefreshJulieShirt();
                this.currentTutorialState = TutorialState.INITIALISATION;
                while (this.currentTutorialState != TutorialState.FINISH)
                {
                    yield return HandleCognitiveTutorialState();
                }
            }
            else
            {
                intendedColor = availableColors[Random.Range(0, availableColors.Count)];
                availableColors.Remove(intendedColor);
                RefreshJulieShirt();


                //Permet de pondérer les échecs en fonction du nombre de ballons présents dans la scene. = MAX_BALLOON + (NB_COLOR - 2)*2
                maxBalloon = nbWave + (nbOption - 2) * 2;

                while (JulieManager.Instance.CurrentState != JulieManager.JulieState.READY)
                {
                    yield return null;
                }



                countdown.SetActive(true);
                // Level intro delay
                yield return new WaitForSeconds(3.2f);
                
                TimerManager.LevelStart();
                //We pick one color at random

                while (BalloonDestroyed < maxBalloon)
                {
                    //Reversal
                    if (BalloonDestroyed == maxBalloon / 2)
                    {
                        BalloonColor tmp = intendedColor;
                        intendedColor = availableColors[Random.Range(0, availableColors.Count)];
                        availableColors.Add(tmp);
                        availableColors.Remove(intendedColor);
                        RefreshJulieShirt();
                    }

                    int correctBalloonPosition = Random.Range(0, nbOption);
                    usedColors.Clear();
                    List<BalloonOption> options = new List<BalloonOption>();
                    for (int i = 0; i < nbOption; i++)
                    {
                        BalloonBehaviour prefabToInstantiate;
                        //We ensure we have at least one correct balloon in the list
                        if (correctBalloonPosition == i)
                        {
                            prefabToInstantiate = this.GetCorrectBalloonPrefab();
                        }
                        else
                        {
                            prefabToInstantiate = this.GetWrongBalloonPrefab();
                        }
                       
                        options.Add(this.CreateBalloon(prefabToInstantiate, i, nbOption));
                    }

                    //Register Balloon data
                    CognitiveWave wave = new CognitiveWave();
                    wave.nbOption = nbOption;
                    wave.intendedColor = intendedColor.ToString();
                    wave.intendedBalloonIndex = correctBalloonPosition;
                    wave.options = options;
                    if(DataManager.instance != null)
                        DataManager.instance.AddCognitiveWaves(wave);
                    while (BalloonsInScene > 0)
                    {
                        yield return null;
                        //TODO: Make Julie move once in a while ? Maybe inside Julie's code.
                    }

                    //Debug.Log("All Balloons have been destroyed");

                    //Prevent intempestive spawning. // Check if child is in place
                    yield return new WaitForSeconds(3f);
                }
                TimerManager.LevelEnd();
            }

            //Old 
            //if (!CorrectBalloonsRemains)
            //{
            //    balloonDestroyed++;
            //    if (BalloonDestroyed < 5)
            //    {
            //        int correctBalloonPosition = Random.Range(0, nbColor);
            //        for (int i = 0; i < nbColor; i++)
            //        {
            //            BalloonBehaviour prefabToInstantiate;
            //            //We ensure we have at least one correct balloon in the list
            //            if (correctBalloonPosition == i)
            //            {
            //                prefabToInstantiate = this.GetCorrectBalloonPrefab();
            //            }
            //            else
            //            {
            //                prefabToInstantiate = this.GetWrongBalloonPrefab();
            //            }

            //            this.CreateBalloon(prefabToInstantiate, i, nbColor);
            //        }

            //        yield return new WaitForSeconds(2);
            //    }
            //}
            //else
            //{
            //    yield return null;
            //}



            yield return null;
            OnGameEnded?.Invoke(GameManager.GameType.COGNITIVE);

            //GameManager.LevelEnd();
            //ScoreBoard.Instance.LevelEnd();
        }

        private void RefreshJulieShirt()
        {
            if (GameManager.Instance.CurrentLevelIndex < 5 && GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE)
            {
                JulieManager.Instance.UpdateShirtColor(intendedColor);
            }
            else
            {
                JulieManager.Instance.UpdateShirtColor(BalloonColor.NONE);
            }
        }
        #endregion

    }
}



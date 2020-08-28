using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PopBalloons.Utilities;
using static PopBalloons.Utilities.GameCreator;
using Microsoft.MixedReality.Toolkit;


#if !NETFX_CORE
using System.Threading;
#else
using Windows.System.Threading;
using Windows.Foundation;
#endif

namespace PopBalloons.Data
{


    public class DataManager : MonoBehaviour
    {

        /// <summary>
        /// This script serialize several informations gathered from user's 
        /// behaviour and play results.
        /// </summary>
        public static DataManager instance;
        private string filePath;
        private string targetFolder;
        private List<Datas> datasList;
        private DatasCollection datasCollection;
        private Datas datas;
        private string currentSaveTime;


        private string currentDay;

        private bool isRecording = false;

    #if !NETFX_CORE
        Thread DataManagement;
    #endif
        private void Awake()
        {

            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != null)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //SceneManager.sceneLoaded += handleLevelEnd;
            GameCreator.OnGameStarted += HandleLevelStart;
            GameCreator.OnGameEnded += HandleLevelEnd;
            GameCreator.OnGameInterrupted += HandleLevelEnd;
            //GameManager.OnGameStateChanged += 
        }

        
        private void HandleLevelEnd(GameManager.GameType type)
        {

            int datasIndex = datasList.Count - 1;

            datasList[datasIndex].levelDatas.score = ScoreManager.instance.score;
            //    datasList[datasIndex].levelDatas.name = level;
            //    datasList[datasIndex].levelDatas.score = score;
            switch (type)
            {
                case GameManager.GameType.COGNITIVE:
                    //CognitiveDatas cognitiveInfos = new CognitiveDatas();
                    datasList[datasIndex].levelDatas.cognitiveDatas.correctBalloons = ScoreManager.instance.CorrectBalloon;
                    datasList[datasIndex].levelDatas.cognitiveDatas.wrongBalloons = ScoreManager.instance.WrongBalloon;
                    //TODO: Add more infos
                    

                    break;
                case GameManager.GameType.FREEPLAY:
                    break;
                default:

                    break;
            }
            isRecording = false;
            SaveDatas();
        }

        private void HandleLevelStart(GameManager.GameType type)
        {
            Debug.Log("InitRecording");
            InitRecording();
            isRecording = true;
        }

        public async void InitRecording()
        {
            //Create Data folder if needed
            if (!Directory.Exists(Application.persistentDataPath + "/Datas"))
                Directory.CreateDirectory(Application.persistentDataPath + "/Datas");

            if (ProfilesManager.Instance.GetCurrentProfile() != null && ProfilesManager.Instance.GetCurrentProfile().data != null)
            {
                //Debug.Log("Player's id is : " + ProfilesManager.Instance.GetCurrentProfile().data.id);
                string subfolder = ProfilesManager.Instance.GetCurrentProfile().data.id.Substring(0, 8);

                //Define a subfolder for current day sessions
                currentDay = DateTime.Now.ToString("yyyy-MM-dd");

                //CurrentSave time
                currentSaveTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string source = Application.persistentDataPath;
                
                if (IsHoloLens2())
                {
#if UNITY_WSA && !UNITY_EDITOR
                    Debug.Log("Storage begin");
                  
                    try
                    {
                        Debug.Log("Root");
                        Windows.Storage.StorageFolder root = await Windows.Storage.DownloadsFolder.CreateFolderAsync("HA-PopBalloons",Windows.Storage.CreationCollisionOption.OpenIfExists);
                        Debug.Log("Datas");
                        Windows.Storage.StorageFolder datas = await root.CreateFolderAsync("Datas",Windows.Storage.CreationCollisionOption.OpenIfExists);
                        Debug.Log("Sub");
                        Windows.Storage.StorageFolder sub = await datas.CreateFolderAsync(subfolder, Windows.Storage.CreationCollisionOption.OpenIfExists);
                        Debug.Log("Folder");
                        Windows.Storage.StorageFolder folder = await sub.CreateFolderAsync(currentDay, Windows.Storage.CreationCollisionOption.OpenIfExists);
                        
                        targetFolder = folder.Path;

                        Debug.Log("Folder acquired" + folder.Path);
                        Windows.Storage.StorageFile file = await folder.CreateFileAsync(Path.Combine(string.Format("{0}.json", currentSaveTime)));
                        filePath = file.Path;
                        Debug.Log("Final Path acquired" + file.Path);

                        //currentSaveTime = DateTime.Now.ToString("yyyy/MM/dd/ HH:mm");
                        //currentSaveTime = DateTime.Now.ToString("yyyy/MM/dd/ HH:mm");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }
                   
                    ////Intended to load previous instance of data if any (not fully implemented yet)
                    LoadDatas();
                    InitDatas();
                    Debug.Log("Done initializing");
                    return;
#endif
                }

                Debug.Log("Not UWP or HoloLens 1");
                //Create user folder if required
                if (!Directory.Exists(source + string.Format("/Datas/{0}", subfolder)))
                    Directory.CreateDirectory(source + string.Format("/Datas/{0}", subfolder));

                targetFolder = source + string.Format("/Datas/{0}/{1}", subfolder, currentDay);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                filePath = targetFolder + string.Format("/{0}.json", currentSaveTime);


                //currentSaveTime = DateTime.Now.ToString("yyyy/MM/dd/ HH:mm");
                //currentSaveTime = DateTime.Now.ToString("yyyy/MM/dd/ HH:mm");
                LoadDatas();
                InitDatas();
                Debug.Log("Done initializing");


            }
        }

        private void OnDestroy()
        {
            //SceneManager.sceneLoaded -= HandleLevelEnd;
            GameCreator.OnGameStarted -= HandleLevelStart;
            GameCreator.OnGameEnded -= HandleLevelEnd;
            GameCreator.OnGameInterrupted -= HandleLevelEnd;
        }

        // Gather all the datas about User's Gaze
        public void AddGazeDatas(GazableItemDatas datas)
        {
            if (datasList != null && isRecording)
            {
                int datasIndex = datasList.Count - 1;
                datasList[datasIndex].levelDatas.listGazeItemDatas.Add(datas);
            }
        }

        public void AddGazeDatas(GazeDatas datas)
        {
            if (datasList != null && isRecording)
            {
                int datasIndex = datasList.Count - 1;
                datasList[datasIndex].levelDatas.listGazeDatas.Add(datas);
            }
        }


        // Gather all the datas about balloons events
        public void AddBalloonsDatas(BalloonDatas datas)
        {
            if(datasList != null && isRecording)
            {
                int datasIndex = datasList.Count - 1;
                datasList[datasIndex].levelDatas.listBalloonDatas.Add(datas);
            }
        }
        /// <summary>
        /// Will save the new wave data
        /// </summary>
        /// <param name="wave"></param>
        public void AddCognitiveWaves(CognitiveWave wave)
        {
            if (datasList != null && isRecording)
            {
                int datasIndex = datasList.Count - 1;
                if (datasList[datasIndex].levelDatas.cognitiveDatas.waves == null)
                    datasList[datasIndex].levelDatas.cognitiveDatas.waves = new List<CognitiveWave>();
                datasList[datasIndex].levelDatas.cognitiveDatas.waves.Add(wave);
            }
        }

        // Gather all the datas about user while playing
        public void AddUsersDatas(UserDatas datas)
        {
            if (datasList != null && isRecording)
            {
                int datasIndex = datasList.Count - 1;
                datasList[datasIndex].levelDatas.userDatas.Add(datas);
            }
        }

        // Save all datas gathered during current level before loading the next one.
        public void SaveDatas()
        {
        //    int datasIndex = datasList.Count - 1;
            
        //    datasList[datasIndex].levelDatas.name = level;
        //    datasList[datasIndex].levelDatas.score = score;
    
            string json = JsonUtility.ToJson(datasCollection, true);

#if !NETFX_CORE
            DataManagement = new Thread( () => File.WriteAllText(filePath, json));
            DataManagement.Start();
#else
            IAsyncAction asyncAction = ThreadPool.RunAsync((workItem)=>File.WriteAllText(saveFile, json));
#endif
        }

        //Load all players profiles from json file
        public void LoadDatas()
        {
            string json = null;
            datasList = new List<Datas>();


            //Currently doesn't load any previous data for performance issue
            //if (!Directory.Exists(Application.persistentDataPath + string.Format("/Datas/{0}", ProfilesManager.Instance.GetCurrentProfile().data.username)))
            //    return;
        
            //if (File.Exists(filePath))
            //{
            //    json = File.ReadAllText(filePath);
            //    datasCollection = JsonUtility.FromJson<DatasCollection>(json);
            //    datasList = datasCollection.datasList;
            //}
        }

        // Initialization of serialization container and objects
        private void InitDatas()
        {
            datas = new Datas();

            datas.userId = ProfilesManager.Instance.CurrentProfile.id;
            datas.dateTime = currentSaveTime;

            //NEw level is started
            LevelDatas levelData = new LevelDatas();
            datas.levelDatas = levelData;
            levelData.mode = GameManager.Instance.CurrentGameType.ToString();
            levelData.name = GameManager.Instance.CurrentGameType.ToString() + "_" + (GameManager.Instance.CurrentLevelIndex + 1).ToString();
            levelData.score = 0;
            levelData.userDatas = new List<UserDatas>();
            levelData.listBalloonDatas = new List<BalloonDatas>();
            levelData.listGazeItemDatas = new List<GazableItemDatas>();
            levelData.listGazeDatas = new List<GazeDatas>();
            //Debug.Log("Game is inilized");
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE)
            {
                //Debug.Log("Game type is Cognitive");
                levelData.cognitiveDatas = new CognitiveDatas();
                levelData.cognitiveDatas.waves = new List<CognitiveWave>();
            }

            //Usefull for multiple level inside one json file
            //for (int i = 1; i <= 4; i++)
            //{
            //    datas.listLevelDatas.Add(new LevelDatas());
            //    datas.listLevelDatas[i - 1].name = "Level" + i.ToString();
            //    datas.listLevelDatas[i - 1].score = 0;
            //    datas.listLevelDatas[i - 1].userDatas = new List<UserDatas>();
            //    datas.listLevelDatas[i - 1].listBalloonDatas = new List<BalloonDatas>();
            //    //datas.listLevelDatas[i - 1].userDatas.handPos = new List<Vector3>();
            //}
            //
            
            datasList.Add(datas);
            datasCollection = new DatasCollection(datasList);
        }

        /// <summary>
        /// Check if the device is able to detect all finger tips, determine if HoloLens 2 or not
        /// </summary>
        /// <returns></returns>
        public bool IsHoloLens2()
        {
#if UNITY_EDITOR
            return true;
#endif

            IMixedRealityCapabilityCheck capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                return capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand);
            }


            return false;

        }
    }

    // Datas serialization objects
    [Serializable]
    public class UserDatas
    {
        public Vector3 headPos;
        public Vector3 headRotation;
        public float scoreBoardTime;
        //public float BPM;
        public string timeStamp;
        //public List<Vector3> headRot;
        //public List<Vector3> handPos;
    }

    [Serializable]
    public class BalloonDatas
    {
        public string id;
        public string color;

        public string dateOfSpawn;
        public string dateOfDestroy;


        // Temps du balloon //@Deprecated, should be remove for using dateTime Only
        public float timeOfSpawn;
        public float timeOfDestroy;
        public float lifeTime;

        //Gain de point ou condition de reussite / echec
        public float balloonPointGain;
        public bool balloonWasDestroyByUser;
        public bool balloonTimout;

        // distance parcourue depuis l'apparition du ballon.
        public float distance;

        //position du balloon
        public Vector3 balloonInitialPosition;
    }

    [Serializable]
    public class GazeDatas
    {
        public string timeStamp;
        public bool targetIsValid;
        public Vector3 origin;
        public Vector3 direction;
        public Vector3 target;

    }

    [Serializable]
    public class GazableItemDatas
    {
        public string objectType;
        public string timeOfLook;
        public string duration;
    }

    [Serializable]
    public class CognitiveDatas
    {
        //TODO: Add color option
        //TODO: Add reversal color 
        public int correctBalloons;
        public int wrongBalloons;
        public List<CognitiveWave> waves;
    }

    [Serializable]
    public class CognitiveWave
    {
        public string intendedColor;
        public int nbOption;
        public int intendedBalloonIndex;
        public List<BalloonOption> options;
    }

    [Serializable]
    public class BalloonOption
    {
        public string color;
        public string id;
        public int index;
        public Vector3 position;
    }

    [Serializable]
    public class LevelDatas
    {
        public string mode;
        public string name;
        public int score;
        public CognitiveDatas cognitiveDatas = null;
        public List<UserDatas> userDatas = new List<UserDatas>();
        public List<BalloonDatas> listBalloonDatas = new List<BalloonDatas>();
        public List<GazableItemDatas> listGazeItemDatas = new List<GazableItemDatas>();
        public List<GazeDatas> listGazeDatas = new List<GazeDatas>();
    }

    [Serializable]
    public class Datas
    {
        public string userId = "";
        public string dateTime = "";
        public LevelDatas levelDatas = new LevelDatas();
        //public List<LevelDatas> listLevelDatas = new List<LevelDatas>();
    }

    [Serializable]
    public class DatasCollection
    {
        [SerializeField]
        public List<Datas> datasList;

        public DatasCollection(List<Datas> _datasList)
        {
            datasList = _datasList;
        }


        
    }


}
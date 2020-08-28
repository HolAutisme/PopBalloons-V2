using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace PopBalloons
{
    /// <summary>
    /// This class is deprecated and will be removed
    /// </summary>
    public class LoadLevels : MonoBehaviour
    {

        #region variables
        public int balloonToDestroyed = 0;
        public GameObject balloonBonus;
        public GameObject winScreen;
        public Text winScore;
        public TextMesh levelName;
        public bool levelLoaded;
        public bool bonusOnce;
        public bool levelPlaySpace = false;
        public static Vector3 position;

        [Header("The stable Objects in scene: ")]
        [SerializeField]
        private GameObject zone;
        private GameObject scoreBoard;
        #endregion



        #region delegates and events
        public delegate void NextLevel();
        public delegate void LoadScene(string scene);
        public delegate void LevelEnd();
        public static event LevelEnd OnLevelEnd;
        public delegate void Begin();
        public static event Begin OnBegin;
        #endregion



        #region unity functions
        private void Start()
        {
            ////ScoreBoard.OnLoadScene += NextLevelByButton;
            //bonusOnce = true;
            ////  position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.0f);    
            //position = Camera.main.transform.position + new Vector3(0, 0, 2.0f);
            //levelLoaded = false;
            //if(zone != null)
            //{
            //    SceneManager.MoveGameObjectToScene(zone, SceneManager.GetSceneByName("Setup"));
            //}
        }



        private void Update()
        {
                //if (GameCreator.Instance.BalloonDestroyed == 5 && bonusOnce)
                //{
                //   if(balloonBonus != null) Instantiate(balloonBonus, position, Quaternion.identity);
                //    bonusOnce = false;

                //  if (!levelLoaded)
                //  {
                //    TimerManager.levelEnd();
                //    if (OnLevelEnd != null)
                //    {
                //        OnLevelEnd.Invoke();
                //    }
                //    //ScoreBoard.OnNextLevel += NextLevelAutomatically;
                //    Invoke("SaveData", 5);
                //}
                //}
        }

        private void OnDestroy()
        {
            //ScoreBoard.OnLoadScene -= NextLevelByButton;
            //ScoreBoard.OnNextLevel -= NextLevelAutomatically;
        }



        #endregion



        #region functions



        private void SaveData()
        {
            //if (GameControl.control != null)
            //    GameControl.control.Save(SceneManager.GetActiveScene().name, ScoreManager.instance.score);
            //if (DataManager.instance != null)
            //    DataManager.instance.SaveDatas(SceneManager.GetActiveScene().name, ScoreManager.instance.score);
        }



        public void LevelEndRequest()
        {
            //TimerManager.levelEnd();
            //if (OnLevelEnd != null)
            //{
            //    OnLevelEnd.Invoke();
            //}
        }

        #endregion
    }

}

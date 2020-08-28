using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using PopBalloons.Boundaries;
using PopBalloons.Utilities;

namespace PopBalloons
{
    public class FreePlaySpawner : MonoBehaviour
    {
        /// <summary>
        /// Collection d'hologramme partagée par tous les utilisateurs
        /// </summary>
        /// 

        #region variables
        private Transform sharedCollection;

        [Header("Game Settings :")]

        [SerializeField]
        [Range(30f, 120f)]
        float levelDuration = 90f;

        [SerializeField]
        AnimationCurve difficulty;

        [SerializeField]
        float initialBalloonFrequency;

        [SerializeField]
        float maximumBalloonFrequency;

        [SerializeField]
        [Range(1, 8f)]
        int maxElementOnScene;

        [SerializeField]
        int currentMaxElementOnScene;

        [SerializeField]
        private GameObject countdown;

        [Header("Scene settings :")]
        [SerializeField]
        private LoadLevels levelManager;

        public float maxRadius;
        public List<GameObject> balloonPrefabs;
        public List<GameObject> balloonWithoutIndicatorPrefabs;
        public List<GameObject> balloonBonus;
        public GameObject CenterObject;
        public float secondsBetweenBalloon;
        public bool isFirstBalloon = true;


        private float nbBalloonPopped = 0;
        private bool noMoreBalloons = false;

        Vector3 center;
        GameObject clone;
        GameObject cloneBonus;
        Transform posBonus;
        Vector3 axis;


        static public List<GameObject> balloonsArray;
        public static float balloonDestroyed;
        private static FreePlaySpawner instance;

        public float IntermediateMaxValue
        {
            get
            {
                return Mathf.Lerp(currentMaxElementOnScene, maxElementOnScene, GetDifficultyFactor());
            }
        }

        /// <summary>
        /// Vérifie qu'il n'y a pas déjà trop de ballon dans la scene.
        /// </summary>
        public bool TooMuch
        {
            get
            {
                return GameCreator.Instance.BalloonsInScene >= IntermediateMaxValue;
            }
        }

        public static FreePlaySpawner Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region unity functions
        private void Awake()
        {
            instance = this;
            isFirstBalloon = true;
            balloonDestroyed = 0;
        }



        void Start()
        {
            Physics.gravity = new Vector3(0, -0.5f, 0);

            ScoreManager.onBalloonPopped += UpdateDifficulty;

            if (PlaySpace.Instance != null && PlaySpace.Instance.GetCenter() != null)
            {
                CenterObject = PlaySpace.Instance.GetCenter();
            }

            center = CenterObject.transform.position;
            center.y = Camera.main.transform.position.y;
            launchSpawning();

        }

        private void OnDestroy()
        {
            ScoreManager.onBalloonPopped -= UpdateDifficulty;
        }


        private void Update()
        {
            LoadLevels.OnLevelEnd += StopCreatingBalloons;
        }
        #endregion


        #region other functions

        public void StopCreatingBalloons()
        {
            noMoreBalloons = true;
        }


        private void UpdateDifficulty(float time, int scoreGain, bool isBonus)
        {
            nbBalloonPopped++;
        }
         


        /// <summary>
        ///     Launch coroutine of spawning balloon if and only if local player if the Child.
        /// </summary>
        void launchSpawning()
        {
           StartCoroutine(LoopSpawn());
        }



        private bool CreateBalloons()
        {
            Vector3 pos;
            Quaternion rot;
            int rand = 0;
            rot = Quaternion.Euler(-90, -90, 0);
            if(GameCreator.Instance.BalloonsInScene == 0 && !noMoreBalloons)
            {
                if (isFirstBalloon)
                {
                    pos = Camera.main.transform.position + new Vector3(0, 0, 1);
                    isFirstBalloon = false;
                }
                else
                    pos = RandomCircle(center, maxRadius);
                    if (PlaySpace.Instance)
                        if (!LimitArea.ContainsPoint(PlaySpace.Instance.GetLandmarksPoint(), new Vector2(pos.x, pos.z)))
                            return false;

                if (GameCreator.Instance.BalloonsInScene >= 1)
                {
                    rand = UnityEngine.Random.Range(0, balloonWithoutIndicatorPrefabs.Count);
                    Instantiate(balloonWithoutIndicatorPrefabs[rand], pos, rot);
                }
                else
                {
                    rand = UnityEngine.Random.Range(0, balloonPrefabs.Count);
                    Instantiate(balloonPrefabs[rand], pos, rot);
                }
                //GameCreator.Instance.AddBalloon();
            }
   

            return true;

        }



        IEnumerator LoopSpawn()
        {
            yield return new WaitForSeconds(4f);
            if (countdown != null)
            {
                countdown.SetActive(true);
                yield return new WaitForSeconds(3.2f);
            }

            nbBalloonPopped = 0;
            float t = 0;
            float spawnTime = 0;

            TimerManager.LevelBonusStart(levelDuration);
            while (t < levelDuration)
            {
              if (!TooMuch)
              {
                    if (spawnTime <= 0)
                    {
                        spawnTime = (CreateBalloons()) ? GetSpawnTime() : spawnTime; 
                    }
                    else
                    {
                        spawnTime -= Time.deltaTime;
                    }
              }

                t += Time.deltaTime;
                yield return null;
            }

            levelManager.LevelEndRequest();
        }



        private float GetSpawnTime()
        {
            return Mathf.Lerp(initialBalloonFrequency, maximumBalloonFrequency, GetDifficultyFactor());
        }



        public float GetDifficultyFactor()
        {

            float nbBalloonFactor = 0;

            if (TimerManager.GetTime() > 0)
            {
                nbBalloonFactor = nbBalloonPopped / (TimerManager.GetTime() / (initialBalloonFrequency + 2f));

            }
            return difficulty.Evaluate(Mathf.Clamp01(nbBalloonFactor / 2f) * (TimerManager.GetTime() / levelDuration));
        }



        public float GetWeightingDifficultyFactor()
        {

            float nbBalloonFactor = 0;

            if (TimerManager.GetTime() > 0)
            {
                nbBalloonFactor = nbBalloonPopped / (TimerManager.GetTime() / (initialBalloonFrequency + 3.5f));

            }
            return difficulty.Evaluate(Mathf.Clamp01(nbBalloonFactor / 2f) * (TimerManager.GetTime() - (0.6f * levelDuration)) / (levelDuration - 0.6f * levelDuration));
        }



        Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = UnityEngine.Random.value * 360;
            Vector3 pos;

            pos.x = center.x + UnityEngine.Random.Range(0.8f, radius) * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = UnityEngine.Random.Range(center.y + 0.25f, (center.y - 0.4f));
            pos.z = center.z + UnityEngine.Random.Range(0.2f, radius) * Mathf.Cos(ang * Mathf.Deg2Rad);
            return pos;
        }



        public void AdaptBehaviour(BalloonBehaviour balloonBehaviour)
        {
            var timeFactor = TimerManager.GetTime() / levelDuration;
            if (timeFactor < 0.6f)
            {

                balloonBehaviour.AdaptBehaviour();
            }
            else
            {

                //var d = GetDifficultyFactor();
                //balloonBehaviour.Rigidbody.useGravity = true;
                //balloonBehaviour.Rigidbody.isKinematic = false;
                //balloonBehaviour.Rigidbody.drag = Mathf.Lerp(5.0f,0.0f, d);
                //r.AddForce(Vector3.down * d * 10,ForceMode.Impulse);

            }


        }
#endregion
    }
}

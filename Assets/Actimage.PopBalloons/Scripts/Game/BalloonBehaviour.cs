using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using PopBalloons.Utilities;
using PopBalloons.HandTracking;

namespace PopBalloons
{
    /// <summary>
    /// we have all the balloon's behaviour in this class 
    /// should never use singleton for this script
    /// </summary>
    /// 

    public class BalloonBehaviour : MonoBehaviour  /*IMixedRealitySourceStateHandler*/
    {
        #region variables
        [SerializeField]
        private GameObject particleBurst;

        [SerializeField]
        private GameObject particlePoof;

        [SerializeField]
        private TMPro.TextMeshPro scoreDisplay;

        [SerializeField]
        private bool isFloating = false;

        [SerializeField]
        private float amplitude = 0.02f;

        [SerializeField]
        private float frequency = 0.33f;

        [SerializeField]
        private GameCreator.BalloonColor color = GameCreator.BalloonColor.BLUE;

        private bool wasDeflated = false;
        private Animator anim;
        private Rigidbody rigidBody;
        private GameObject particleBurstClone;
        private Vector3 tempPos = new Vector3();
        private Vector3 posOffset = new Vector3();
        private bool isOnFloor = false;
        private float initializationTime;
        private DateTime dateOfSpawn;
        private float balloonDuration = -1f;
        static public float timeOfCollision;
        static public bool highScore;
        private bool prefered = false;
        private bool normalballoon = false;
        private bool balloonDestroyedByUser = false;
        private bool popOnce = false;
        private bool isTheWrongOne = false;
        private HandDetected HD;
        private Guid id;

        public Guid Id { get => id; }
        public string DateOfSpawn { get => dateOfSpawn.ToString("HH-mm-ss.fff"); }

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region events and delegates
        public delegate void DestroyAction(float timeStamp, float duration, bool isBonus);
        public static event DestroyAction OnDestroyBalloon;


        public delegate void BalloonMissed(float timeStamp, float duration, bool timeout);
        public static event BalloonMissed OnBalloonMissed;
        
        public delegate void BalloonSpawned(BalloonBehaviour balloon);
        public static event BalloonSpawned OnBalloonSpawned;

        public delegate void CognitiveBalloonDestroy(BalloonBehaviour balloon);
        public static event CognitiveBalloonDestroy OnCognitiveBalloonDestroyed;
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region unity functions

        private void Awake()
        {
            //Unique ID for this balloon instance
            id = Guid.NewGuid();
            dateOfSpawn = System.DateTime.Now;
        }
        private void Start()
        {
            
            initializationTime = TimerManager.GetTimeStamp();
            anim = this.GetComponentInChildren<Animator>();
            rigidBody = this.GetComponent<Rigidbody>();
            if(OnBalloonSpawned != null)
            {
                OnBalloonSpawned.Invoke(this);
            }
            AdaptBehaviour();
            posOffset = transform.position;
            if(GameManager.Instance.CurrentGameType != GameManager.GameType.COGNITIVE)
                StartCoroutine(AutoDestroyBalloon());
        }



        private void Update()
        {
            //floating
            if (isFloating)
            {
                tempPos = posOffset;
                tempPos.x += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
                transform.position = tempPos;
            }

            //score
            float timeSinceInstantiation = TimerManager.GetTimeStamp() - initializationTime;
            highScore = (timeSinceInstantiation <= 7.0f);

            timeOfCollision = Mathf.FloorToInt(timeSinceInstantiation); 
        }
        
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region functions
        /// <summary>
        /// Will return the current instance color
        /// </summary>
        /// <returns>Balloon color (enumeration)</returns>
        public GameCreator.BalloonColor GetColor()
        {
            return this.color;
        }

       
        public void OnCollisionEnter(Collision collision)
        {
            this.OnCollisionStay(collision);
        }

        /// <summary>
        /// this fonction detrmines the behavoiur of balloons and it depends on different types of collisions. 
        /// </summary>
        /// <param name="collision"></param>
        public void OnCollisionStay(Collision collision)
        {
            //Debug.Log("Collision !" + collision.gameObject.name);
            if (popOnce)
            {
                //Debug.Log("Popped Once already");
                Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<Collider>());
                return;
            }

            //Local backup if error in
            bool initialPop = popOnce;
            popOnce = true;

            if(GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE)
            {
               // Debug.Log("Cognitive balloon");
                if (collision.gameObject.tag == "VirtualHand" && !this.wasDeflated)
                {
                   // Debug.Log("Destroyed balloon");
                    OnCognitiveBalloonDestroyed?.Invoke(this);
                    return;
                }

                popOnce = false;
                //Debug.Log("Ignoring collision with the rest of the world");
                Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<Collider>());
                return;
            }


            if (collision.gameObject.tag == "VirtualHand")
            {
                //Debug.Log("Collision with a hand!");
                balloonDestroyedByUser = true;
                balloonDuration = TimerManager.GetTimeStamp() - initializationTime;
                ScoreManager.onScoreChange += DisplayScore;
                if (OnDestroyBalloon != null) OnDestroyBalloon(TimerManager.GetTimeStamp(), (float) (DateTime.Now - dateOfSpawn).TotalSeconds, false);
                ScoreManager.onScoreChange -= DisplayScore;
                DisposeBalloon();
              
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Spatial Awareness"))
            {
                //Debug.Log("Collision with the floor!");
                balloonDestroyedByUser = false;
                if (OnBalloonMissed != null) OnBalloonMissed(TimerManager.GetTimeStamp(), (float)(DateTime.Now - dateOfSpawn).TotalSeconds, false);
                PopBalloon();
            }
            else
            {
                //Debug.Log("Ignoring collision");
                Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<Collider>());
                popOnce = initialPop;
            }

        }

        private void DisplayScore(int score, int scoreGain)
        {
            //Debug.Log("Balloon : " + score + " Gain :" + scoreGain);
            TMPro.TextMeshPro scoreDisplayClone = Instantiate(scoreDisplay, this.gameObject.transform.position, Quaternion.identity);
            scoreDisplayClone.text = "+" + scoreGain.ToString();
            Destroy(scoreDisplayClone, 3.0f);
        }

        /// <summary>
        /// this function is using when we delete a balloon. if the user blows it, the balloon destroys and will be counted. 
        /// </summary>
        /// <param name="Bref"></param>
        public void DisposeBalloon()
        {
            if (this.balloonDestroyedByUser)
            {
                GameCreator.Instance.BalloonDestroyed++;
                //JulieManager.Instance.Play(JulieManager.JulieAnimation.Clap);
            }
            PopBalloon();
        }

        /// <summary>
        /// this function manages the sound and confetti of the balloon and destroys it
        /// </summary>
        public void PopBalloon()
        {
           
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE && this.color != GameCreator.Instance.IntendedColor)
            {
                //TODO: Error sound
                //particleBurstClone.GetComponent<SoundManager>().PlayErrorSound();
                particleBurstClone = Instantiate(particlePoof, this.gameObject.transform.position, Quaternion.identity);
                particleBurstClone.GetComponent<SoundManager>().PlayPop();
                Destroy(particleBurstClone, 3.0f);
            }
            else
            {
                particleBurstClone = Instantiate(particleBurst, this.gameObject.transform.position, Quaternion.identity);
                particleBurstClone.GetComponent<SoundManager>().PlayPopAndConfetti();
                Destroy(particleBurstClone, 3.0f);
            }
            GameCreator.Instance.RemoveBalloon(this);
        }

        /// <summary>
        /// Play deflate animation, then destroy balloon
        /// </summary>
        public void DeflateBalloon()
        {
            //Prevent destruction from other balloon
            wasDeflated = true;
            this.anim.SetTrigger("Deflate");
            Destroy(this.gameObject, 2.0f);
            //TODO: Start deflate animation
        }
        //TODO: Vanish  balloon


        /// <summary>
       /// After 15 seconds the balloon will be disappeared 
       /// </summary>
      /// <returns></returns>
        IEnumerator AutoDestroyBalloon()
        {
            yield return new WaitForSeconds(15.0f); 
            isTheWrongOne = false;
            PopBalloon();
            if (OnBalloonMissed != null) OnBalloonMissed(TimerManager.GetTimeStamp(), (float)(DateTime.Now - dateOfSpawn).TotalSeconds, true);
        }


        /// <summary>
        /// adapts behaviour of the balloons depend on the levels they are in it. 
        /// </summary>
        public void AdaptBehaviour()
        {
            if (rigidBody == null)
            {
                Debug.LogError("RigidBody should'nt be null");
                return;
            }

            switch (GameManager.Instance.CurrentGameType)
            {
                case GameManager.GameType.CLASSIC:
                    switch (GameManager.Instance.CurrentLevelIndex)
                    {
                        case 1:
                            this.isFloating = false;
                            this.frequency = 0f;
                            rigidBody.useGravity = false;
                            break;

                        case 2:
                            this.isFloating = true;
                            this.frequency = 0.33f;
                            this.amplitude = 0.15f;
                            rigidBody.useGravity = false;
                            break;

                        case 3:
                            this.isFloating = false;
                            rigidBody.useGravity = true;
                            rigidBody.isKinematic = false;
                            rigidBody.drag = 35.0f;
                            break;

                        case 4:
                            this.isFloating = false;
                            rigidBody.useGravity = true;
                            rigidBody.isKinematic = false;
                            rigidBody.drag = 28.0f;
                            break;
                        default:
                            this.isFloating = false;
                            this.frequency = 0f;
                            rigidBody.useGravity = false;
                            break;
                    }
                    break;
                case GameManager.GameType.FREEPLAY:
                    this.isFloating = false;
                    if (FreePlaySpawner.Instance != null)
                    {
                        rigidBody.drag = Mathf.Lerp(5.0f, 0f, FreePlaySpawner.Instance.GetWeightingDifficultyFactor());
                        FreePlaySpawner.Instance.AdaptBehaviour(this);
                    }
                    break;
                default:
                    this.isFloating = false;
                    rigidBody.useGravity = false;
                    this.frequency = 0f;
                    break;
            }
            
        }

        #endregion
    }
}


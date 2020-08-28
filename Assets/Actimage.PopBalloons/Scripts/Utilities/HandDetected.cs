using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.XR.WSA.Input;
using UnityEngine.SceneManagement;
using PopBalloons.Data;
using PopBalloons.Utilities;

namespace PopBalloons.HandTracking
{
    public class HandDetected : MonoBehaviour, IMixedRealitySourceStateHandler/*, IMixedRealityHandJointHandler*/
    {
        #region variables
        //[Tooltip("max distance between pointer and object to understand the collision")]
        //[Range(0, 1)]
        //[SerializeField]
        //private float distance;

        [SerializeField]
        private GameObject HL1_Hand;
        [SerializeField]
        private GameObject HL2_Hands;
        [SerializeField]
        private GameObject PointerHand;


        [Header("Virtual hand settings :")]

        [Tooltip("Maximum distance for displaying virtual hand indicator.")]
        [Range(0, 3)]
        [SerializeField]
        float distanceConsiderNear = 1.2f;


        [Tooltip("Time required before considering user is struggling with hand recognition.")]
        [Range(0, 5f)]
        [SerializeField]
        float timeBeforeDisplayingHand = 3f;

        [Tooltip("Value compare to a scalar product between child -> vector and camera -> forward")]
        [Range(0f, 1f)]
        [SerializeField]
        float seableAngleTolerance = 0.8f;

        [Header("Vocal & Textual Helper :")]
        [SerializeField]
        bool needHelp = false;

        [SerializeField]
        bool isOnLevel = false;

        [SerializeField]
        HelperSystem helpDisplayer;

        private Vector3 position;
        private bool handIsDetected = false;
        private bool handIsDisplay = false;
        public bool balloonExists = false;
        private bool coroutineIsRunning = false;
        private bool registeringData = true;
        private bool lvl1 = true;
        private IMixedRealityInputSystem inputSystem = null;
        private IMixedRealityInputSource source;
        private List<IMixedRealityInputSource> sources;
        private int nbBalloon = 0;
        private Vector3 currentBalloonPosition;
        private float balloonSpawnTime;
        private float timeWhenBalloonWasSeenAndNear;
        private float timeCurrentlyBalloonNotSeen;
        private float timeCurrentlyBalloonIsSeen;
        #endregion

        #region unity functions


        private void Awake()
        {
            sources = new List<IMixedRealityInputSource>();
            position = transform.position;
            PointerHand.SetActive(handIsDisplay);
        }



        public void Start()
        {
            LoadLevels.OnLevelEnd += LevelEnd;
            
          
        }




        private void OnEnable()
        {
            InputSystem?.Register(this.gameObject);
        }

        private void OnDisable()
        {
            InputSystem?.Unregister(this.gameObject);
           
        }


        private void Update()
        {
            if (source != null)
            {

                if (!coroutineIsRunning) StartCoroutine(TrackGesture());
                int i = 1;
                //foreach (IMixedRealityPointer p in source.Pointers)
                //{
                //    //Debug.Log("Pointer: "+p.PointerName +"Position : " + p.Position + " NbPointer : " + i);
                //    //i++; 
                //}

                if(HL1_Hand.activeSelf)
                    HL1_Hand.transform.position = source.Pointers[0].Position; //  we put the virtual hand position on the first pointer


                if (GameManager.Instance.CurrentLevelIndex > 0 && needHelp)
                {
                    BalloonBehaviour.OnBalloonSpawned += BalloonSpawned;

                    if (balloonExists)
                    {
                        timeWhenBalloonWasSeenAndNear = (BalloonIsSeen() && BalloonIsNear()) ? timeWhenBalloonWasSeenAndNear + Time.deltaTime : 0;
                        timeCurrentlyBalloonNotSeen += (BalloonIsSeen()) ? 0 : Time.deltaTime;
                        if (timeCurrentlyBalloonNotSeen > 5f && GameManager.Instance.CurrentGameType == GameManager.GameType.CLASSIC && GameManager.Instance.CurrentState == GameManager.GameState.PLAY)
                        {
                            helpDisplayer.Display(HelperSystem.HelpRequired.TOO_LONG);
                            timeCurrentlyBalloonNotSeen = 0;
                        }
                        ManageHandVisibility();
                    }


                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.CLASSIC && GameManager.Instance.CurrentLevelIndex == 1 && lvl1)
                    {
                        StartCoroutine(WaitForFirstPage());
                        BalloonBehaviour.OnBalloonMissed += BalloonMissed;
                        BalloonBehaviour.OnDestroyBalloon += BalloonDestroyed;
                        lvl1 = false;
                    }

                }

              
               
            }
        }




        private void OnDestroy()
        {
            BalloonBehaviour.OnBalloonSpawned -= BalloonSpawned;
            BalloonBehaviour.OnBalloonMissed -= BalloonMissed;
            BalloonBehaviour.OnDestroyBalloon -= BalloonDestroyed;
            LoadLevels.OnLevelEnd -= LevelEnd;
        }
        #endregion

        #region functions

        private void BalloonDestroyed(float timestamp, float duration, bool isBonus)
        {
            nbBalloon++;
            if(nbBalloon >= 2)
            {
                BalloonBehaviour.OnBalloonMissed -= BalloonMissed;
                BalloonBehaviour.OnDestroyBalloon -= BalloonDestroyed;
                //  needHelp = false;
            }

        }


        private void BalloonMissed(float timeStamp, float duration, bool timeout)
        {

        }
         


        private void BalloonSpawned(BalloonBehaviour balloon)
        {
            if(PointerHand == null)
            {
               BalloonBehaviour.OnBalloonSpawned -= BalloonSpawned;
            }

            timeCurrentlyBalloonNotSeen = 0;
            balloonExists = true;
            currentBalloonPosition = balloon.transform.position;
            balloonSpawnTime = TimerManager.GetTime();
            
        }


        public void LevelEnd()
        {
            registeringData = false;
        }

        private void ManageHandVisibility()
        {
            bool shouldBeDisplayingHand = !handIsDetected && (timeWhenBalloonWasSeenAndNear > timeBeforeDisplayingHand);

            if (shouldBeDisplayingHand != handIsDisplay)
            {
                if (shouldBeDisplayingHand && needHelp)
                {
                    helpDisplayer.Display(HelperSystem.HelpRequired.HAND_PLACEMENT);
                }
                handIsDisplay = shouldBeDisplayingHand;
                PointerHand.SetActive(shouldBeDisplayingHand);
            }
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


        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }

                return inputSystem;
            }
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            //Debug.Log("Source detected : " + eventData.SourceId);
          
            if(eventData.InputSource.SourceType == InputSourceType.Hand)// && distance <= 0.02)
            {
                //Debug.Log("Hand detected : " + eventData.SourceId);
                handIsDetected = true;
                this.SetActiveHands(true);
                source = eventData.InputSource;
                sources.Add(source);
            }
           
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if(eventData.InputSource.SourceType == InputSourceType.Hand)
            {
                sources.Remove(eventData.InputSource);

                if (source != null && eventData.SourceId == source.SourceId)
                {
                
                    if (sources.Count > 0)
                    {
                        source = sources[0];
                    }
                    else
                    {
                        handIsDetected = false;
                        this.SetActiveHands(false);
                        source = null;
                    }
                }
            }
        }


        private void SetActiveHands(bool isActive)
        {
            if (IsHoloLens2())
            {
                HL2_Hands.SetActive(isActive);
                HL1_Hand.SetActive(false);
            }
            else
            {
                HL2_Hands.SetActive(false);
                HL1_Hand.SetActive(isActive);
            }
        }


        private bool BalloonIsNear()
        {
            return (Vector3.Distance(Camera.main.transform.position, currentBalloonPosition) < distanceConsiderNear);
        }



        private bool BalloonIsSeen()
        {
            return (Vector3.Dot((currentBalloonPosition - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized) > seableAngleTolerance);
        }



        IEnumerator TrackGesture()
        {
            coroutineIsRunning = true;
            yield return new WaitForSeconds(0.01f);
            if (registeringData)
            {
                UserDatas data = new UserDatas();

                data.headPos = Camera.main.transform.position;
                data.headRotation = Camera.main.transform.rotation.eulerAngles;
                data.scoreBoardTime = TimerManager.GetTimeStamp();
                data.timeStamp = System.DateTime.Now.ToString("HH-mm-ss.fff");//TimerManager.GetTimeStamp();
                if (DataManager.instance != null)
                    DataManager.instance.AddUsersDatas(data);
            }
            coroutineIsRunning = false;

        }


        IEnumerator WaitForFirstPage()
        {
            yield return new WaitForSeconds(2.2f);
            helpDisplayer.Display(HelperSystem.HelpRequired.INTRODUCTION);
        }
        #endregion

    }
}


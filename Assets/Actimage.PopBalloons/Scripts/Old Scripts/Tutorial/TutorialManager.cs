using PopBalloons.Boundaries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PopBalloons
{
    public class TutorialManager : MonoBehaviour
    {

        #region variables
        public enum TutorialState { INITIALISATION, POSITIONNING, WAITING_FOR_DEMONSTRATION, DEMONSTRATION, DIY, FINISH }
        public delegate void TutorialStateChanged(TutorialState ts);
        public static TutorialStateChanged onTutorialStateChanged;

        [Header("Settings :")]
        [SerializeField]
        private GameObject juliePrefab;
        [SerializeField]
        private GameObject tutorialBalloonPrefab;

        public TutorialState currentState = TutorialState.INITIALISATION;
        private GameObject julie;
        private bool doctorSide = false;

        [Header("Focus :")]
        [SerializeField]
        [Range(0, 5)]
        private float focusTimeRequired;
        [SerializeField]
        [Range(0.5f, 1)]
        private float focusTolerance;
        private float focusTime;


        Animator julieAnimator;
        Camera player;

        #endregion

        #region unity functions
        // Use this for initialization
        void Start()
        {
            if (currentState == TutorialState.INITIALISATION)
            {
                player = Camera.main;
                julie = Instantiate(juliePrefab);
                if (PlaySpace.Instance)
                {
                    GameObject l1, l2;
                    l1 = GameObject.Find("Landmark1");
                    l2 = GameObject.Find("Landmark2");

                    julie.transform.position = (l1 != null && l2 != null)
                        ? l1.transform.position + (l2.transform.position - l1.transform.position) * 0.5f
                        : player.transform.position + new Vector3(0, -1, 4);
                    GameObject target = PlaySpace.Instance.GetCenter();
                    julie.transform.LookAt(new Vector3(target.transform.position.x, julie.transform.position.y, target.transform.position.z), Vector3.up);
                }
                julieAnimator = julie.GetComponent<Animator>();

            }
        }


        void Update()
        {

                UpdateState(currentState);
        }

        #endregion

        #region functions

        void ChangeCurrentState(TutorialState ts)
        {
            currentState = ts;
            CurrentState(ts);
        }


        void CurrentState(TutorialState ts)
        {
            if (onTutorialStateChanged != null)
                onTutorialStateChanged(ts);
        }

        void InitState(TutorialState nextState)
        {

            switch (nextState)
            {
                case TutorialState.POSITIONNING:
                    //Event positionning
                    break;

                case TutorialState.DEMONSTRATION:
                    //RpcLaunchJulie();
                    Invoke("goToDIY", 8);
                    break;
                case TutorialState.DIY:
                    Vector3 pos = Camera.main.transform.position + new Vector3(0, 0, 1f);  /*Camera.main.transform.forward * 2*/;
                    pos.y = Camera.main.transform.position.y;
                    pos.x = Camera.main.transform.position.x;
                    Quaternion rot = Quaternion.Euler(0, 110, 0);  // issue 

                    if(tutorialBalloonPrefab != null) Instantiate(tutorialBalloonPrefab, pos, rot);
                    TutorialBalloon.onTutorialBalloonPopped += GoToFinish;


                    break;
                case TutorialState.FINISH:
                    TutorialBalloon.onTutorialBalloonPopped -= GoToFinish;
                    break;
            }

                   ChangeCurrentState(nextState);
        }

        void GoToDIY()
        {
            InitState(TutorialState.DIY);
        }

        void GoToFinish()
        {
            InitState(TutorialState.FINISH);
        }



        void UpdateState(TutorialState ts)
        {
            switch (ts)
            {
                case TutorialState.INITIALISATION:
                    if (PlaySpace.Instance && CheckLimitArea.isOut)
                    {
                        InitState(TutorialState.POSITIONNING);
                        return;
                    }
                    InitState(TutorialState.WAITING_FOR_DEMONSTRATION);
                    break;

                case TutorialState.POSITIONNING:
                    if (PlaySpace.Instance && CheckLimitArea.isOut)
                        return;
                    InitState(TutorialState.WAITING_FOR_DEMONSTRATION);
                    break;

                case TutorialState.WAITING_FOR_DEMONSTRATION:

                    if (Vector3.Dot(player.transform.forward.normalized, (julie.transform.position - player.transform.position).normalized) > focusTolerance)
                    {
                        focusTime += Time.deltaTime;
                    }

                    if (Vector3.Dot(player.transform.forward.normalized, (julie.transform.position - player.transform.position).normalized) < 0.5)
                    {
                        focusTime = 0;
                    }

                    if (focusTime > focusTimeRequired)
                        InitState(TutorialState.DEMONSTRATION);
                    break;
                default:
                    //Nothing
                    break;
            }
        }
        #endregion
    }
}


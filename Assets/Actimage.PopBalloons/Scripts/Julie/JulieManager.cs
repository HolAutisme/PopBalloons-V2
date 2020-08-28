using PopBalloons;
using PopBalloons.Boundaries;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.Utilities
{

    /// <summary>
    /// Class that will manage the avatars position and animation.
    /// </summary>
    public class JulieManager : MonoBehaviour
    {
        #region variables
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static JulieManager instance;


        [Header("Settings :")]
        [SerializeField]
        private GameObject juliePrefab;

        [SerializeField]
        [Tooltip("Textures correspondants au pull de Julie (Bleu,Rouge,Vert,Jaune)")]
        private List<Texture2D> texs;
        [Header("Walking settings :")]
        [SerializeField]
        private bool autoFollow = false;
        [SerializeField]
        [Range(0, 10)]
        private float speed;
        [SerializeField]
        private Vector3 destination;
        [SerializeField]
        private Transform target;

        [Header("Focus :")]
        [SerializeField]
        [Range(0, 5)]
        private float focusTimeRequired;
        [SerializeField]
        [Range(0.5f, 1)]
        private float focusTolerance;

        private float focusTime;
        private Animator julie;
        private Camera player;
        private JulieState currentState = JulieState.NONE;


        public enum JulieState {NONE,INIT,WALKING,READY}
        public enum JulieAnimation {Dance,Clap,Sad,Angry,Defeat,Jump,Pop,Point_Left,Point_Right,Point_Middle,Setup_Motricity,Demonstrate_Motricity,Disappear, Setup_Cognitive, Demonstrate_Cognitive};

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static JulieManager Instance { get => instance;}
        public JulieState CurrentState { get => currentState; }

        #endregion

        #region Unity functions
        /// <summary>
        /// Singleton implementation
        /// </summary>
        private void Awake()
        {
            if(instance != null)
            {
                DestroyImmediate(this);
            }
            else
            {
                GameManager.OnGameStateChanged += HandleGameStateChanged;
                player = Camera.main;
                instance = this;
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.PLAY && GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE)
            {
                this.Init();
            }
            else if (newState == GameManager.GameState.HOME)
            {
                currentState = JulieState.INIT;
                if (julie != default && (currentState == JulieState.WALKING || currentState == JulieState.READY))
                {
                    this.Play(JulieAnimation.Disappear);
                }
            }
        }

        private void Update()
        {

            if(julie == default)
            {
                return;
            }

            //Test
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    StartCoroutine(Walk(GetPathToBestAngle()));
            //}

            //Check for tolerance
            //if (julie.GetBool("isWalking"))
            //{
            //    if (Vector3.Dot(target.forward.normalized, (julie.transform.parent.position - target.position).normalized)> focusTolerance)
            //    {
            //        StopAllCoroutines();
            //        julie.SetBool("isWalking", false);
            //    }

            //}

            //Gaze at player at all time for now
            if (!julie.GetBool("isWalking"))
            {
                Vector3 center = PlaySpace.Instance.GetCenter().transform.position;
                julie.transform.parent.rotation = Quaternion.Slerp(julie.transform.parent.rotation, Quaternion.LookRotation(new Vector3(center.x, julie.transform.parent.position.y, center.z) - julie.transform.parent.position, Vector3.up), 0.1f);
            }
                //WalkAround for now
                //if (!julie.GetBool("isWalking"))
                //{
                //    List<Vector3> steps = new List<Vector3>();
                //    PlaySpace.Instance.landmarks.ForEach((landmark) => { steps.Add(landmark.transform.position); });


                //    StartCoroutine(Walk(steps.ToArray()));
                //}

            if (autoFollow && target != null)
            {
                destination = new Vector3(target.position.x, julie.transform.parent.position.y, target.position.z);
                FollowTarget();
            }
            
           
        }

        #endregion

        #region Julie Functions
        public Vector3 GetPosition()
        {
            return this.julie.transform.parent.position;
        }

        /// <summary>
        /// Should add eye tracking to this feature
        /// </summary>
        /// <returns></returns>
        public bool IsFocused()
        {
            return Vector3.Dot(target.forward.normalized, (julie.transform.parent.position - target.position).normalized) > focusTolerance;
        }

        /// <summary>
        /// Will stop Julie Walking animation and coroutine when she is focus by the user.
        /// </summary>
        public void StopWhenFocused()
        {
            if (julie.GetBool("isWalking") && Vector3.Dot(target.forward.normalized, (julie.transform.parent.position - target.position).normalized) > focusTolerance)
            {
                StopAllCoroutines();
                julie.SetBool("isWalking", false);               
            }
        }


        /// <summary>
        /// Place julie around the player area
        /// </summary>
        public void Init(bool forceReinit = false)
        {
            if(forceReinit)
            {
                currentState = JulieState.INIT;
            }
            
            if (julie == default)
            {
                currentState = JulieState.INIT;
                julie = Instantiate(juliePrefab, this.transform).GetComponentInChildren<Animator>();
            }

            
            if (PlaySpace.Instance && currentState != JulieState.READY)
            {
                julie.Play("Julie@Appearing");
                GameObject target = PlaySpace.Instance.GetCenter();
                AreaSegment segment = PlaySpace.Instance.GetFacingSegment();
                julie.transform.parent.position = segment.V1.transform.position + (segment.V1.transform.position - target.transform.position).normalized * 0.5f;        
                destination = segment.V2.transform.position - (segment.V2.transform.position - target.transform.position).normalized * 0.25f;

                julie.transform.parent.LookAt(new Vector3(target.transform.position.x, julie.transform.parent.position.y, target.transform.position.z), Vector3.up);
                Invoke("WalkTowardDestination", 3f);// WalkToward(destination);
                currentState = JulieState.WALKING;
                
            }
            //target.position = julie.transform.parent.position;
        }

        /// <summary>
        /// Will try to play the current animation
        /// </summary>
        /// <param name="animation"></param>
        public void Play(JulieAnimation animation)
        {
            if (julie.GetBool("isWalking"))
            {
                //julie.SetBool("isWalking", false);
            }
            julie.SetTrigger(animation.ToString());
        }

        /// <summary>
        /// Iterate one time toward a specific target
        /// </summary>
        public void FollowTarget()
        {

            if (julie.transform.parent.position != destination)
            {
                this.julie.SetBool("isWalking", true);
                Vector3 initialDirection = (destination - julie.transform.parent.position).normalized;
                Vector3 offset = initialDirection * Time.deltaTime * speed;
                julie.transform.parent.LookAt(new Vector3(destination.x, julie.transform.parent.position.y, destination.z), Vector3.up);
                if (offset.sqrMagnitude > (destination - julie.transform.parent.position).sqrMagnitude)
                {
                    julie.transform.parent.position = destination;
                    return;
                }
                julie.transform.parent.position += offset;
            }
            else
            {
                this.julie.SetBool("isWalking", false);
            }
        }

        private void WalkTowardDestination()
        {
            this.WalkToward(destination);
        }

        public void UpdateShirtColor(GameCreator.BalloonColor color)
        {
            if(julie != default)
            {
                julie.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetTexture("_MainTex",GetTextureFromColor(color));
            }
        }

        private Texture2D GetTextureFromColor(GameCreator.BalloonColor color)
        {
            switch (color)
            {
                case GameCreator.BalloonColor.BLUE:
                    return texs[0];
                case GameCreator.BalloonColor.RED:
                    return texs[1];
                case GameCreator.BalloonColor.GREEN:
                    return texs[2];
                case GameCreator.BalloonColor.YELLOW:
                    return texs[3];
                default:
                    return texs[4];
            }
        }


        /// <summary>
        /// Walk toward a specific point in space with a coroutine
        /// </summary>
        /// <param name="target">Intended destination</param>
        private void WalkToward(Vector3 target)
        {
            Vector3[] steps = new Vector3[1];
            steps[0] = target;

            //Debug.Log("Walk toward :" + target);
            if(!julie.GetBool("isWalking"))
                StartCoroutine(Walk(steps));
        }

        /// <summary>
        /// Walk toward a specific point in space with a coroutine
        /// </summary>
        /// <param name="target">Intended destination</param>
        private Vector3[] GetPathToBestAngle()
        {
            Vector3 center = PlaySpace.Instance.GetCenter().transform.position;
            List<Vector3> steps = new List<Vector3>();
            PlaySpace.Instance.Segments.ForEach((segment) => { steps.Add(segment.V1.transform.position); });
            Vector3 playerPos = Camera.main.transform.position;
            bool clockwise = (Vector2.SignedAngle(Camera.main.transform.forward, (julie.transform.parent.position - playerPos).normalized) < 0);
            bool correctOrder = (Vector2.SignedAngle(steps[0] - center,steps[1] -center) < 0);

            float minDistance = Mathf.Infinity;
            Vector3 closestStep = default;
            foreach (Vector3 step in steps)
            {
                float current = Vector3.Distance(step, julie.transform.parent.position);
                if (current < minDistance)
                {
                    closestStep = step;
                    minDistance = current;
                }
            }
            int index = steps.IndexOf(closestStep);
            bool sameDirection = (Vector2.SignedAngle((closestStep - playerPos).normalized, (julie.transform.parent.position - playerPos).normalized) < 0);

            if (clockwise)
            {
                int offset = (sameDirection != clockwise) ? 0 : 1;
                List<Vector3> items = steps.GetRange(0, index+ offset);
                steps.RemoveRange(0, index+ offset);
                items.Reverse();
                steps.InsertRange(0,items);
            }
            else
            {
                int offset = (sameDirection == !clockwise) ? 0 : 1;
                Vector3[] items = steps.GetRange(0, index+ offset).ToArray();
                steps.RemoveRange(0, index+ offset);
                steps.AddRange(items);
                //steps.Reverse();
            }
            Debug.Log("Clockwise : " + clockwise + " SameDirection : " + sameDirection + " Correct Order :" + correctOrder);

            return steps.ToArray();
        }


        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine for walking along a predetermined path.
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        private IEnumerator Walk(Vector3[] steps)
        {

            Vector3 initialPosition = julie.transform.parent.position;
            this.julie.SetBool("isWalking",true);
            foreach(Vector3 step in steps)
            {
                Vector3 initialDirection =  (step - julie.transform.parent.position).normalized;
                while (julie.transform.parent.position != step)
                {
                    if (julie.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Julie@Walking")//!julie.GetBool("isWalking"))
                    {
                        if (!this.julie.GetBool("isWalking"))
                            this.julie.SetBool("isWalking", true);
                        //Debug.Log("Wait for end animation :"+ julie.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                        yield return null;
                        continue;
                        //yield break;
                    }

                    //Look better with this line, but can introduce an error with the threshold getting julie to turn around one of the point for a long period of time.
                    //Vector3 initialDirection = julie.transform.parent.forward.normalized;
                    Vector3 offset = initialDirection * Time.deltaTime * speed;
                    //julie.transform.parent.LookAt(new Vector3(step.x, julie.transform.parent.position.y, step.z), Vector3.up);
                    julie.transform.parent.rotation = Quaternion.Slerp(julie.transform.parent.rotation, Quaternion.LookRotation(new Vector3(step.x, julie.transform.parent.position.y, step.z) - julie.transform.parent.position, Vector3.up), 0.1f);
                    if (offset.sqrMagnitude > (step - julie.transform.parent.position).sqrMagnitude)
                    {
                        julie.transform.parent.position = step;
                        break;
                    }
                    julie.transform.parent.position += offset;
                    yield return null;
                }
                yield return null;
            }

            this.julie.SetBool("isWalking", false);
            currentState = JulieState.READY;
            yield return null;
        }

        #endregion


    }

}
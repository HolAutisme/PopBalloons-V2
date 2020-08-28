using PopBalloons.Boundaries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PopBalloons
{
    public class CheckLimitArea : MonoBehaviour
    {

        public Material limitMat;
        public Color mainColor;
        public Color inGameColor;
        public Color outColor;


        Vector3 userPos;
        public static bool isOut;
        bool initialized = false;

        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            userPos = Camera.main.transform.position;
            int c = SceneManager.sceneCount;
            for(int i = 0; i<c; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene == SceneManager.GetSceneByName("Tutorial") || scene.name.Contains("Level") && initialized)
                {
                    initialized = false;
                }
                else
                {
                    Initialize();
                }

                if (scene == SceneManager.GetSceneByName("Tutorial") || scene.name.Contains("Level"))
                {
                    if (!LimitArea.ContainsPoint(PlaySpace.Instance.GetLandmarksPoint(), new Vector2(userPos.x, userPos.z)) && !isOut)
                    {
                        //Debug.Log("User out of limit");
                        //limitMat.color = outColor;
                        limitMat.SetFloat("_Speed", 5.0f);
                        StartCoroutine(InToOut());
                        isOut = true;
                    }
                    else if (LimitArea.ContainsPoint(PlaySpace.Instance.GetLandmarksPoint(), new Vector2(userPos.x, userPos.z)) && isOut)
                    {
                        //Debug.Log("User inside the play area");
                        //limitMat.color = inGameColor;
                        limitMat.SetFloat("_Speed", 0.2f);
                        StartCoroutine(OutToInt());
                        isOut = false;
                    }
                }
            }
           


        }
        void Initialize()
        {
            limitMat.color = mainColor;
            limitMat.SetFloat("_Speed", 0.2f);
            if (!LimitArea.ContainsPoint(PlaySpace.Instance.GetLandmarksPoint(), new Vector2(userPos.x, userPos.z)))
                isOut = true;
            else
                isOut = false;
            initialized = true;
        }

        // Lerp from inside material to the outside one
        IEnumerator InToOut()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                limitMat.color = Color.Lerp(inGameColor, outColor, elapsedTime);
                //limitMat.SetFloat("_Speed", Mathf.Lerp(0.2f, 5.0f, elapsedTime));
                elapsedTime += Time.deltaTime * 2;
                yield return null;
            }
        }
        // Lerp from outside material to the inside one
        IEnumerator OutToInt()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                limitMat.color = Color.Lerp(outColor, inGameColor, elapsedTime);
                //limitMat.SetFloat("_Speed", Mathf.Lerp(5.0f, 0.2f, elapsedTime));
                elapsedTime += Time.deltaTime * 2;
                yield return null;
            }
        }
    }

}

using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using PopBalloons.Boundaries;

namespace PopBalloons
{
    public class BonusBehaviour : MonoBehaviour
    {

        #region variables
        [SerializeField]
        private float moveFactor;
        public float amplitudeX = 0.10f;
        public float amplitudeY = 0.05f;
        public float omegaX = 1.0f;
        public float omegaY = 5.0f;
        public static event BalloonBehaviour.DestroyAction OnDestroyBonus;
        public GameObject particleBurst;
        public GameObject particleBurstClone;
        public event EventHandler OnTouchHand2;
        public static bool isBonus;
        Color rend;
        float index;
        #endregion


        #region functions
        /// <summary>
        /// Fonction appellée lorsque l'on souhaite supprimer un ballon. Pourrait remplacer l'itégralité des Invoke de l'event OnTouchHand. Mais on préfère conserver le code existant.
        /// </summary>
        /// <param name="pRef">Pas forcément utile, mais doit correspondre à OnTouchHand</param>
        public void disposeBalloon(BonusBehaviour pRef)
        {
            this.PopIt();
        }



        public void PopIt()
        {
            particleBurstClone = Instantiate(particleBurst, gameObject.transform.position, Quaternion.identity);
            particleBurstClone.GetComponent<SoundManager>().PlayConfetti();
            Destroy(particleBurstClone, 3.0f);
            Destroy(this.gameObject);
        }

        private void Start()
        {
            isBonus = true;
            rend = gameObject.GetComponent<Renderer>().material.color;
            
        }

        public void Update()
        {
            index += Time.deltaTime;
            float x = amplitudeX * Mathf.Cos(omegaX * index);

            float y = amplitudeY * Mathf.Cos(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor);
            float z = amplitudeY * Mathf.Cos(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor);
            transform.position = PlaySpace.Instance.GetCenter().transform.TransformPoint(new Vector3(x, y+1, z));
            gameObject.GetComponent<Renderer>().material.color = rend;
            rend.a -= Time.deltaTime * 0.125f;

            if (rend.a <= 0)
            {
               Destroy(this.gameObject);
            }

        }



        public void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.tag == "VirtualHand")
            {
                if (OnDestroyBonus != null) OnDestroyBonus(-1, -1, isBonus);
                disposeBalloon(this);
            }
            if (collision.gameObject.tag == "Balloon")
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            }

        }
    }
    #endregion
}

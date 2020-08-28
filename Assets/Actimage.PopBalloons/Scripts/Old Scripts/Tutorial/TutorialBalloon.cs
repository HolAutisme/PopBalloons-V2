using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons
{
    public class TutorialBalloon : MonoBehaviour
    {

        public delegate void TutorialBalloonPopped();
        public static TutorialBalloonPopped onTutorialBalloonPopped;

        [SerializeField]
        private GameObject particleBurst;

        [SerializeField]
        private bool isWrongBalloon = false;

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "VirtualHand")
            {
                if (onTutorialBalloonPopped != null) onTutorialBalloonPopped();// Détruit les objets et fait apparaitre des particules
                CmdPopIt();
            }
        }

        public void CmdPopIt()
        {
            GameObject particleBurstClone = Instantiate(particleBurst, gameObject.transform.position, Quaternion.identity);
            particleBurstClone.GetComponent<SoundManager>().PlayPopAndConfetti();
            Destroy(particleBurstClone, 3.0f);
            Destroy(this.gameObject);
        }

        public void Start()
        {
           //Quaternion rot = Quaternion.Euler(-90, 0, 0);
           // this.gameObject.transform.rotation = rot;
        }
    }
}


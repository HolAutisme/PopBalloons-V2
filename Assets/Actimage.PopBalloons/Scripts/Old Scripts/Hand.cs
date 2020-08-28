using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons
{
    public class Hand : MonoBehaviour
    {

        public GameObject hand;
        public static Hand instance;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }
    }

}

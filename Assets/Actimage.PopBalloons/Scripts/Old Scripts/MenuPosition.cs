using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons
{
    public class MenuPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
                this.gameObject.transform.position = Camera.main.transform.position + new Vector3(0, 0, 5.0f);
        }

        // Update is called once per frame
        void Update()
        {
           
        }
    }

}

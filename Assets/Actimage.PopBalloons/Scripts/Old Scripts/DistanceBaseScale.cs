using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PopBalloons
{
    public class DistanceBaseScale : MonoBehaviour
    {

        [SerializeField]
        [Range(0.1f, 10f)]
        float baseDistance;
       

        Vector3 initialScale;
        Camera player;

        void Start()
        {
            player = Camera.main;
            initialScale = this.transform.localScale;
        }

        void Update()
        {
            this.transform.localScale = initialScale * GetCurrentScaleFactor();

        }


        private float GetCurrentScaleFactor()
        {
             return (Vector3.Distance(this.transform.position, player.transform.position) / baseDistance);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.Boundaries
{
    /// <summary>
    /// Draw the area for motricity game mode
    /// </summary>
    public class AreaSegment : MonoBehaviour
    {
        [SerializeField]
        private GameObject v1;

        [SerializeField]
        private GameObject v2;

        public GameObject V1 { get => v1;  }
        public GameObject V2 { get => v2;  }


        public Vector3 Center { get => (V1.transform.position + V2.transform.position) / 2f; }

        //We should draw our own mesh in order to prevent scale issue with the shader
        //private MeshRenderer meshRenderer;
        //private MeshRenderer meshFilter;


        /// <summary>
        /// TODO : Temporary Debug Update, will be removed for performance
        /// </summary>
        private void Update()
        {
            this.Redraw();
        }



        /// <summary>
        /// Will change current target end point and trigger a Redraw
        /// </summary>
        /// <param name="beginPoint"></param>
        /// <param name="endPoint"></param>
        public void SetVertices(GameObject beginPoint,GameObject endPoint)
        {
            v1 = beginPoint;
            v2 = endPoint;
            this.Redraw();
        }


        /// <summary>
        /// Will
        /// </summary>
        public void Redraw()
        {
            Vector3 pos, scale;
            if(V1 != null && V2 != null)
            {
               
                Vector3 dir = V2.transform.position - V1.transform.position;
                pos = V1.transform.position +  (dir / 2.0f);
                float dist = dir.magnitude;// Vector3.Distance(V1.transform.position, V2.transform.position);
                pos.y += dist / 2f;
                scale = Vector3.one * dist / 10f; //Because of plane mesh scale
                this.transform.position = pos;
                this.transform.localScale = scale;
                this.transform.rotation = Quaternion.LookRotation(dir.normalized, (Vector3.Cross((dir).normalized, Vector3.up)));
            }
        }

    }
}
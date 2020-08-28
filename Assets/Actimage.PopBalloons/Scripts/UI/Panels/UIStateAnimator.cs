using PopBalloons.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PopBalloons.UI
{
    /// <summary>
    /// Simple way to refresh / Initialize part of the UI when we arrive on a panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIStateAnimator<T> : UIStateElement<T>
    {
        [SerializeField]
        AnimationSettings settings;
        [SerializeField]
        List<RectTransform> anchors;

        [SerializeField]
        private bool AnimateBound = false;

        private Vector3 InitialPosition;
        private bool isRunning;

        
        public void Awake()
        {
            base.Awake();
           
            InitialPosition = this.transform.position;
        }

        public override void HandleChange(T status)
        {
            bool b = mask.Contains(status);
            c.alpha = (b)?1:0;
            c.interactable = b;
            c.blocksRaycasts = b;


            if (b)
            {
                StartCoroutine(AnimateTo(anchors[mask.IndexOf(status)]));
            }
            else
            {
                //We quit current animation if any
                StopAllCoroutines();

                //We reset at default state
                this.transform.position = anchors[0].position;
            }


        }



        /// <summary>
        /// Will move item to anchored position
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public IEnumerator AnimateTo(RectTransform target)
        {
            isRunning = !isRunning;
            bool b = isRunning;

            float t = 0;
            Vector3 iPosition = this.transform.position;
            RectTransform rectTransform =  this.GetComponent<RectTransform>();
            Vector2 iSize = rectTransform.sizeDelta;

            while(t < settings.Duration)
            {
                if (b != isRunning)
                    yield break;

                this.transform.position = Vector3.Lerp(iPosition, target.position, settings.Curve.Evaluate(t / settings.Duration));
                if (AnimateBound)
                    rectTransform.sizeDelta = Vector2.Lerp(iSize, target.sizeDelta, settings.Curve.Evaluate(t / settings.Duration));
                

                t += Time.deltaTime;
                yield return null;
            }
            this.transform.position = target.position;
            if (AnimateBound)
                rectTransform.sizeDelta = target.sizeDelta;
        }
    }
}
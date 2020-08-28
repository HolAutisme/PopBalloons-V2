using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIStateElement<T> : MonoBehaviour
    {
        #region variables
        [SerializeField]
        protected List<T> mask;

        protected CanvasGroup c;
        #endregion

        #region functions
        protected void Awake()
        {
            if (c == null)
                c = this.GetComponent<CanvasGroup>();
        }


        void Start()
        {
            this.Subscribe();
            // ScoreBoard.OnBoardStatusChange += handleChange;
        }

        private void OnDestroy()
        {
            this.UnSubscribe();

        }

        protected abstract void Subscribe();
        protected abstract void UnSubscribe();

        public virtual void HandleChange(T status)
        {
            if (mask.Contains(status))
            {
                c.alpha = 1;
                c.interactable = true;
                c.blocksRaycasts = true;
            }
            else
            {
                c.alpha = 0;
                c.interactable = false;
                c.blocksRaycasts = false;
            }
        }
    }
    #endregion
}

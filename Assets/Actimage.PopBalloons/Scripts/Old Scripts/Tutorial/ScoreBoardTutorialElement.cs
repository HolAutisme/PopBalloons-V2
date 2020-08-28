using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScoreBoardTutorialElement : MonoBehaviour
    {

        #region variables
        [SerializeField]
        private List<TutorialManager.TutorialState> mask;
        private CanvasGroup c;
        #endregion

        #region functions
        void Start()
        {
            TutorialManager.onTutorialStateChanged += handleChange;
            if (c == null)
                c = this.GetComponent<CanvasGroup>();
            
        }


        private void OnDestroy()
        {
            TutorialManager.onTutorialStateChanged -= handleChange;
        }


        private void handleChange(TutorialManager.TutorialState status)
        {
            if (mask.Contains(status))
            {
                //TODO : Animation?
                c.alpha = 1;
                c.interactable = true;
                c.blocksRaycasts = true;
              //  this.gameObject.SetActive(true);
            }
            else
            {
                //TODO : Animation?
                c.alpha = 0;
                c.interactable = false;
                c.blocksRaycasts = false;
               // this.gameObject.SetActive(false);
            }
        }
        #endregion
    }

}

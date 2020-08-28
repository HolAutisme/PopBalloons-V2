using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScoreBoardElement : MonoBehaviour
    {

        #region variables
        [SerializeField]
        private List<ScoreBoard.BoardStatus> mask;
        private CanvasGroup c;
        #endregion


        #region functions
        void Start()
        {
            ScoreBoard.OnBoardStatusChange += handleChange;
            if (c == null)
                c = this.GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            ScoreBoard.OnBoardStatusChange -= handleChange;

        }

        private void handleChange(ScoreBoard.BoardStatus status)
        {
            if (mask.Contains(status))
            {
                //TODO : Animation?
                c.alpha = 1;
                c.interactable = true;
                c.blocksRaycasts = true;
            }
            else
            {
                //TODO : Animation?
                c.alpha = 0;
                c.interactable = false;
                c.blocksRaycasts = false;
            }
        }
    }
    #endregion
}

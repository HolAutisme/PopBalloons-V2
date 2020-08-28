using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{
    /// <summary>
    /// Manage the score display
    /// </summary>
    public class ScoreDisplayer : MonoBehaviour
    {
        TextMeshProUGUI text;
        Animator animator;
        TextMeshProUGUI gain;

        private void Awake()
        {
            animator = this.GetComponentInChildren<Animator>(true);
            gain = animator.GetComponent<TextMeshProUGUI>();
            text = this.GetComponent<TextMeshProUGUI>();
            ScoreManager.onScoreChange += HandleScoreChanged;
        }

        private void HandleScoreChanged(int score, int scoreGain)
        {
            this.text.text = score.ToString();
            if(scoreGain > 0)
            {
                //Debug.Log("Animate");
                gain.text = "+"+ scoreGain.ToString();
                animator.SetTrigger("Animate");
                //TODO:  Animate score
                //animator.Play("ScoreGain");
            }
        }

        private void OnDestroy()
        {

            ScoreManager.onScoreChange -= HandleScoreChanged;
        }
    }

}
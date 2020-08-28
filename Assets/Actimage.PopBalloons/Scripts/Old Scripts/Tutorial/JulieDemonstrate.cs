using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons
{
    public class JulieDemonstrate : MonoBehaviour
    {
        Animator anim;

        // Use this for initialization
        void Start()
        {
            TutorialManager.onTutorialStateChanged += HandleChange;
            anim = this.GetComponent<Animator>();

            TutorialManagerNew.onTutorialStateChanged += HandleChangeNew;
            anim = this.GetComponent<Animator>();
        }

        private void HandleChange(TutorialManager.TutorialState ts)
        {
            if (ts == TutorialManager.TutorialState.DEMONSTRATION && anim != null)
            {
                anim.SetTrigger("Burst");
            }
        }

        private void HandleChangeNew(TutorialManagerNew.TutorialState state)
        {
            if (state == TutorialManagerNew.TutorialState.DEMONSTRATION && anim != null)
            {
                anim.SetTrigger("Burst");
            }
        }
    }
}


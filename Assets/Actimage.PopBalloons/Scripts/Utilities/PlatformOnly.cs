using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.Utilities
{
    public enum Plateform
    {
        DEFAULT = 1<<0,
        EDITOR = 1<<1,
        HOLOLENS = 1<<2
    }


    public class PlatformOnly : MonoBehaviour
    {
        [SerializeField]
        private Plateform plateform;

        private void Awake()
        {
            HandlePlateformDependent();
        }

        private void HandlePlateformDependent()
        {
            Plateform current = Plateform.DEFAULT;
#if UNITY_EDITOR
            current |= Plateform.EDITOR;
#elif UNITY_WSA
            current |= Plateform.HOLOLENS;
#endif

            this.gameObject.SetActive((current & plateform) != 0);
           
        }
    }

}
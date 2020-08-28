using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.Configuration
{

    [CreateAssetMenu(fileName ="New Animation",menuName ="PopBalloons/Animation Settings")]
    public class AnimationSettings : ScriptableObject
    {
        [Header("Animation Settings :")]

        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        [Range(0f,3f)]
        private float duration = 1f;

   
        public AnimationCurve Curve { get => curve;}
        public float Duration { get => duration;}
    }

}
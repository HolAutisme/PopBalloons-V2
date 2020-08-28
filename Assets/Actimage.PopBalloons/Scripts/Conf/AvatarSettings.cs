using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.Configuration
{
    [CreateAssetMenu(fileName = "New avatar settings", menuName = "PopBalloons/AvatarSettings")]
    public class AvatarSettings : ScriptableObject
    {
        [SerializeField]
        private List<AvatarColorOptions> colors;


        [SerializeField]
        private List<AvatarEyeOptions> eyes;


        [SerializeField]
        private List<AvatarAccessoryOptions> accessories;

        public List<AvatarAccessoryOptions> Accessories { get => accessories; }
        public List<AvatarEyeOptions> Eyes { get => eyes;}
        public List<AvatarColorOptions> Colors { get => colors;}
    }
}
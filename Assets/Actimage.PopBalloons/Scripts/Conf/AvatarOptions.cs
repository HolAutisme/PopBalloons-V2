using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.Configuration
{
    public abstract class AvatarOptions<T> : ScriptableObject
    {
        [SerializeField]
        protected T tag;

        [SerializeField]
        protected Sprite sprite;

        [SerializeField]
        protected Sprite icon;

        [SerializeField]
        protected Sprite iconSelected;

        [SerializeField]
        protected Sprite iconHover;

        public T Tag { get => tag;}
        public Sprite Sprite { get => sprite;}
        public Sprite Icon { get => icon;}
        public Sprite SelectedIcon { get => iconSelected; }
        public Sprite HoverIcon { get => iconHover; }
    }

}
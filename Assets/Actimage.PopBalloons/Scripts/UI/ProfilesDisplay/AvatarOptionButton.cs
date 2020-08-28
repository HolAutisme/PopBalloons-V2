using PopBalloons.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{

    public abstract class AvatarOptionButton<T> : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        protected bool isSelected = false;

        Button b;
        protected AvatarOptions<T> option;
        protected ProfileAvatar target;

        public void Awake()
        {
            b = this.GetComponent<Button>();
        }

        public void Hydrate(AvatarOptions<T> option, ProfileAvatar target)
        {
            this.option = option;
            this.target = target;
            if(target != null && target.Data != null)
            {
                this.SetSelected();
            }
            this.Redraw();
        }


        public void Redraw()
        {
            SpriteState state = new SpriteState();
            state.pressedSprite = (isSelected) ?option.Icon: option.SelectedIcon;
            state.disabledSprite = (!isSelected) ? option.Icon : option.SelectedIcon;
            state.highlightedSprite = (isSelected) ? option.SelectedIcon : option.HoverIcon;

            b.spriteState = state;
            (this.b.targetGraphic as Image).sprite =(isSelected)?option.SelectedIcon :option.Icon;
            if(isSelected)
            {
                //Debug.Log("I am selected ");
            }
        }


        public abstract void OnClick();

        public abstract void SetSelected();

    }

}
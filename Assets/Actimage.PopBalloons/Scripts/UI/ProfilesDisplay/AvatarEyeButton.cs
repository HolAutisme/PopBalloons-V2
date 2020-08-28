using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{
    public class AvatarEyeButton : AvatarOptionButton<ProfileAvatar.Eyes>
    {
        public override void OnClick()
        {
            target.SetEyes(this.option.Tag);
        }

        public override void SetSelected()
        {
            this.isSelected = (target.Data.eyeOption == (int) this.option.Tag) ;
            this.Redraw();
        }
    }
}


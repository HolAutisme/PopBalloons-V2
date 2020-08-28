using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class AvatarAccessoryButton : AvatarOptionButton<ProfileAvatar.Accessories>
    {
        public override void OnClick()
        {
            target.ToggleAccessory(this.option.Tag);
        }

        public override void SetSelected()
        {
            this.isSelected = (target.Data.accessoryOption & (int) this.option.Tag) != 0;
            this.Redraw();
        }
    }
}


using PopBalloons.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class AvatarColorButton : AvatarOptionButton<GameCreator.BalloonColor>
    {
        public override void OnClick()
        {
            target.SetColor(this.option.Tag);
            
        }

        public override void SetSelected()
        {
            this.isSelected = (target.Data.colorOption == (int) this.option.Tag);
            this.Redraw();
        }
    }
}


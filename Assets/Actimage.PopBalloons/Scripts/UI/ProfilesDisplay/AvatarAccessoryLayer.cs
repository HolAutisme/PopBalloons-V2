using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PopBalloons.UI.ProfileAvatar;

namespace PopBalloons.UI
{

    public class AvatarAccessoryLayer : UIStateElement<Accessories>
    {
        protected override void Subscribe()
        {
            //Nothing
        }

        protected override void UnSubscribe()
        {
            //Nothing
        }

        public override void HandleChange(Accessories status)
        {
            c.alpha = ((status & mask[0]) != 0) ? 1 : 0;
        }
    }

}
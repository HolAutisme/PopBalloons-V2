using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class ProfilePanelAnimatedElement : UIStateAnimator<ProfileSubState>
    {
        protected override void Subscribe()
        {
            //Nothing here because event is managed by Profile Panel.
            //Debug.Log("Beware, Profile Panel Element does not handle its subscribtion / deletion itself. Check out ProfilePanel");
        }

        protected override void UnSubscribe()
        {
            //Nothing here because event is managed by Profile Panel.
            //Debug.Log("Beware, Profile Panel Element does not handle its subscribtion / deletion itself. Check out ProfilePanel");
        }
    }

}
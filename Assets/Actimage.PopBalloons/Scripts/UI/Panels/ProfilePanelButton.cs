using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    /// <summary>
    /// Handle navigation inside MainPanel
    /// </summary>
    public class ProfilePanelButton : SubPanelButton<ProfileSubState>
    {
        public override void OnClick()
        {
            ProfilePanel.Instance.SetState(destination);
        }
    }

}
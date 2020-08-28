using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    /// <summary>
    /// Handle navigation inside MainPanel
    /// </summary>
    public class MainPanelButton : SubPanelButton<MainPanelState>
    {
        public override void OnClick()
        {
            MainPanel.Instance.SetState(destination);
        }
    }

}
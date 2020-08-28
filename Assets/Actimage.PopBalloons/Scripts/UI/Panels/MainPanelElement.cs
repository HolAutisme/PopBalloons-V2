using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.UI
{

    /// <summary>
    /// Main section of the panel
    /// </summary>
    public class MainPanelElement : UIStateDispatcher<MainPanelState>
    {
        protected override void Subscribe()
        {
            MainPanel.Instance.Subscribe(this.HandleChange);
        }

        protected override void UnSubscribe()
        {
            MainPanel.Instance.Subscribe(this.HandleChange);
        }
    }
}
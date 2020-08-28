using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class CognitivePanelElement : UIStateElement<CognitiveSubState>
    {
        protected override void Subscribe()
        {
            CognitivePanel.Instance.OnStateChanged += this.HandleChange;
        }

        protected override void UnSubscribe()
        {

            CognitivePanel.Instance.OnStateChanged -= this.HandleChange;
        }
    }

}
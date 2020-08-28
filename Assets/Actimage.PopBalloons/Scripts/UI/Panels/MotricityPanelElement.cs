using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class MotricityPanelElement : UIStateElement<MotricitySubState>
    {
        protected override void Subscribe()
        {
            MotricityPanel.Instance.OnStateChanged += this.HandleChange;
        }

        protected override void UnSubscribe()
        {

            MotricityPanel.Instance.OnStateChanged -= this.HandleChange;
        }
    }

}
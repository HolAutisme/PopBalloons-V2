using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.UI
{

    /// <summary>
    /// Handle navigation inside MainPanel
    /// </summary>
    public abstract class SubPanelButton<T> : MonoBehaviour
    {
        [SerializeField]
        protected T destination;

        /// <summary>
        /// Would be interesting to make an abstract link from T to link panel and button automatically
        /// </summary>
        public abstract void OnClick();

    }

}
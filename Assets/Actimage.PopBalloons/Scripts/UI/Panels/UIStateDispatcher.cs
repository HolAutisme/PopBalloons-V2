using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PopBalloons.UI
{
    /// <summary>
    /// Simple way to refresh / Initialize part of the UI when we arrive on a panel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIStateDispatcher<T> : UIStateElement<T>
    {
        [SerializeField]
        private UnityEvent OnStateFocused;


        public override void HandleChange(T status)
        {
            if (mask.Contains(status))
            {
                c.alpha = 1;
                c.interactable = true;
                c.blocksRaycasts = true;
                //We notify our potential listeners
                OnStateFocused?.Invoke();
            }
            else
            {
                c.alpha = 0;
                c.interactable = false;
                c.blocksRaycasts = false;
            }
        }
    }
}
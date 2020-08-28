using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{
    /// <summary>
    /// Handle all the subPanel. Custom implementation of Observer Pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SubPanel<T> : MonoBehaviour
    {
        protected T currentState;

        public delegate void StateChange(T newState);
        public event StateChange OnStateChanged;

        public T GetState()
        {
            return currentState;
        }

        /// <summary>
        /// Update current state status
        /// </summary>
        /// <param name="newState"></param>
        public virtual void SetState(T newState)
        {
            //Debug.Log("New panel state: " + newState);
            currentState = newState;
            this.OnStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// Will subscribe fonction from event delegate
        /// </summary>
        /// <param name="callback">Function to add to observers list</param>
        public void Subscribe(StateChange callback)
        {
            this.OnStateChanged += callback;
        }

        /// <summary>
        /// Will unsubscribe fonction from event delegate
        /// </summary>
        /// <param name="callback">Function to remove from observers list</param>
        public void UnSubscribe(StateChange callback)
        {
            this.OnStateChanged -= callback;
        }

        /// <summary>
        /// We populate observer list from childs component
        /// </summary>
        protected virtual void PopulateWithChildren()
        {
            System.Array.ForEach(this.GetComponentsInChildren<UIStateElement<T>>(), (element) => this.Subscribe(element.HandleChange));
        }

        public abstract void Init();
    }

}
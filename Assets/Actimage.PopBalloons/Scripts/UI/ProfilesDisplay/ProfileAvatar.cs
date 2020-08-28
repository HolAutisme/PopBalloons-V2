using PopBalloons.Data;
using PopBalloons.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{
    /// <summary>
    /// Handle the profile Avatar
    /// </summary>
    public class ProfileAvatar : MonoBehaviour
    {
        /// <summary>
        /// Multiple accessories is allowed
        /// </summary>
        public enum Accessories
        {
            Hat = (1<<0),
            Glasses = (1<<1),
            Mustache = (1<<2),
            Tie = (1<<3)
        }

        public enum Eyes
        {
            Eyes_01,
            Eyes_02,
            Eyes_03,
            Eyes_04
        }

        [SerializeField]
        private Image balloon;

        [SerializeField]
        private Image eye;

        private List<UIStateElement<Accessories>> accessories;

        private Accessories currentAccessories;
        private AvatarData data;

        public AvatarData Data { get => data; }

        public delegate void HandleChange(Accessories accessories);
        public event HandleChange AccessoriesChanged;


        public delegate void Redrawn();
        public event Redrawn OnRedrawnColors;
        public event Redrawn OnRedrawnEyes;
        public event Redrawn OnRedrawnAccessories;

        private void Awake()
        {
            accessories = new List<UIStateElement<Accessories>>(this.GetComponentsInChildren<UIStateElement<Accessories>>());
            foreach(UIStateElement<Accessories> accessory in accessories)
            {
                AccessoriesChanged += accessory.HandleChange;
            }
            
            //Init();
        }

        private void Start()
        {
            //this.Hydrate(Data);
        }

        public void ToggleAccessory(Accessories accessory)
        {
            //If the accessory is already toggled, we remove it, otherwise we add it.
            if((currentAccessories & accessory) != 0)
            {
                currentAccessories &= ~accessory;
            }
            else
            {
                currentAccessories |= accessory;
            }
           
            data.accessoryOption = (int) currentAccessories;
            this.Redraw();
            OnRedrawnAccessories?.Invoke();
        }


        public void SetColor(GameCreator.BalloonColor color)
        {
            data.colorOption = (int) color;
            this.Redraw();
            OnRedrawnColors?.Invoke();
        }

        public void SetEyes(Eyes eyes)
        {
            data.eyeOption = (int)eyes;
            this.Redraw();
            OnRedrawnEyes?.Invoke();
        }


        public void Hydrate(AvatarData data)
        {
            this.data = data;
            this.currentAccessories = (Accessories)data.accessoryOption;
            this.Redraw();
            OnRedrawnAccessories?.Invoke();
            OnRedrawnColors?.Invoke();
            OnRedrawnEyes?.Invoke();
        }

        public void Redraw()
        {
            AccessoriesChanged?.Invoke(this.currentAccessories);
            this.balloon.sprite = ProfilesManager.AvatarSettings.Colors.Find((color) => color.Tag == (GameCreator.BalloonColor)Data.colorOption).Sprite; //.color.Find((option)=>option.Sprite);
            this.eye.sprite = ProfilesManager.AvatarSettings.Eyes.Find((eye) => eye.Tag == (ProfileAvatar.Eyes)Data.eyeOption).Sprite;
        }

        public void Init()
        {
            this.data = new AvatarData();
        }

    }
}
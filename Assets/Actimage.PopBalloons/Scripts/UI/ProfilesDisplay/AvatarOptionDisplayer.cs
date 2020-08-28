using PopBalloons.Configuration;
using PopBalloons.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class AvatarOptionDisplayer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private ProfileAvatar avatarRef;

        [Header("Scroll views")]
        [SerializeField]
        private Transform colorsDisplay;
        [SerializeField]
        private Transform eyesDisplay;
        [SerializeField]
        private Transform accessoriesDisplay;


        [Header("Buttons prefabs")]
        [SerializeField]
        private AvatarColorButton colorButtonPrefab;
        [SerializeField]
        private AvatarEyeButton eyeButtonPrefab;
        [SerializeField]
        private AvatarAccessoryButton accessoryButtonPrefab;


        private List<AvatarColorButton> colors;
        private List<AvatarEyeButton> eyes;
        private List<AvatarAccessoryButton> accessories;

        private void Start()
        {
            colors = new List<AvatarColorButton>();
            eyes = new List<AvatarEyeButton>();
            accessories = new List<AvatarAccessoryButton>();
            avatarRef.OnRedrawnAccessories += RefreshAccessories;
            avatarRef.OnRedrawnEyes += RefreshEyes;
            avatarRef.OnRedrawnColors += RefreshColors;
            this.PopulateLists();
        }

        public void RefreshEyes()
        {
            foreach (AvatarEyeButton e in eyes)
            {
                e.SetSelected();
            }
        }

        public void RefreshColors()
        {
            foreach (AvatarColorButton c in colors)
            {
                c.SetSelected();
            }
        }

        public void RefreshAccessories()
        {
            foreach(AvatarAccessoryButton a in accessories)
            {
                a.SetSelected();
            }
        }


        private void PopulateLists()
        {
            foreach(AvatarColorOptions o in ProfilesManager.AvatarSettings.Colors)
            {
                var it = Instantiate(colorButtonPrefab, colorsDisplay);
                it.Hydrate(o, avatarRef);
                colors.Add(it);
            }

            foreach (AvatarEyeOptions o in ProfilesManager.AvatarSettings.Eyes)
            {
                var it = Instantiate(eyeButtonPrefab, eyesDisplay);
                it.Hydrate(o, avatarRef);
                eyes.Add(it);
            }

            foreach (AvatarAccessoryOptions o in ProfilesManager.AvatarSettings.Accessories)
            {
                var it = Instantiate(accessoryButtonPrefab, accessoriesDisplay);
                it.Hydrate(o, avatarRef);
                accessories.Add(it);
            }
        }
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using PopBalloons.Configuration;
using PopBalloons.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{

    public class ProfileButton : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private AnimationSettings settings;

        [SerializeField]
        private ProfileAvatar avatar;

        [SerializeField]
        private CanvasGroup editButton;

        private Button background;
        private TextMeshProUGUI label;
        private ProfileList parentList;
        private PlayerData infos;

        public PlayerData Infos { get => infos; }


        private RectTransform rectTransform;
        private Vector2 defaultSize;
        private bool isSelected = false;
        #endregion

        #region Unity Functions
        private void Awake()
        {
            this.background = this.GetComponentInChildren<Button>();
            this.label = this.GetComponentInChildren<TextMeshProUGUI>();
            this.rectTransform = this.GetComponent<RectTransform>();
            this.defaultSize = rectTransform.sizeDelta;
            editButton.alpha = 0;
            editButton.interactable = false;
            editButton.blocksRaycasts = false;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Will store the matching level info on this button
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parentList"></param>
        public void Hydrate(PlayerData infos, ProfileList parentList)
        {
            this.parentList = parentList;
            this.infos = infos;
            this.avatar.Hydrate(infos.avatar);
            label.text = this.Infos.username;
            
            //Todo : set avatar item
        }

        public void Edit()
        {
            if(this.parentList != null)
                this.parentList.EditProfile();
        }

        /// <summary>
        /// Will select the current level
        /// </summary>
        public void OnClick()
        {
            this.parentList.Select(this.infos);
        }

        public void SetSelected(bool b)
        {
            label.color = (b) ? new Color(0.19607f, 0.19607f, 0.19607f) : Color.white;


            editButton.alpha = (b)?1:0;
            editButton.interactable = b;
            editButton.blocksRaycasts = b;
            background.image.sprite = (b) ? this.parentList.ActiveBackground : this.parentList.InactiveBackground;
            background.spriteState = (b) ? this.parentList.ActiveSpriteState : this.parentList.InactiveSpriteState;

            if (isSelected != b)
            {
                StartCoroutine(Animate(b));
            }
        }

        #endregion

        #region Coroutines
        private IEnumerator Animate(bool b)
        {
            isSelected = b;
            float t = 0;

            Vector2 initialSize = this.rectTransform.sizeDelta;
            Vector2 targetSize = (b) ? defaultSize * 2f : defaultSize;
            while(t < settings.Duration)
            {
                this.rectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, settings.Curve.Evaluate(t / settings.Duration));
                t += Time.deltaTime;
                yield return null;
            }
            this.rectTransform.sizeDelta = targetSize;
        }

        #endregion
    }
}
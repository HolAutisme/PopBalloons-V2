using PopBalloons.Configuration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{
    public class LevelButton : MonoBehaviour
    {
        #region Variables

        private Image background;
        private TextMeshProUGUI label;
        private LevelList parentList;
        private LevelInfo infos;

        public LevelInfo Infos { get => infos;}

        #endregion

        #region Unity Functions
        private void Awake()
        {
            this.background = this.GetComponentInChildren<Image>();
            this.label = this.GetComponentInChildren<TextMeshProUGUI>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Will store the matching level info on this button
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parentList"></param>
        public void Hydrate(LevelInfo infos, LevelList parentList)
        {
            this.parentList = parentList;
            this.infos = infos;
            label.text = this.Infos.GameIndex.ToString();
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
            background.sprite = (b) ? this.parentList.ActiveBackground : this.parentList.InactiveBackground;
        }
        #endregion
    }
}
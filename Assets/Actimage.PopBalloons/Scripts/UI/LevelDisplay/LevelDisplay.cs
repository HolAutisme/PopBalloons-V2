using System;
using System.Collections;
using System.Collections.Generic;
using PopBalloons.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{

    /// <summary>
    /// Manage the display of a LevelInfos
    /// </summary>
    public class LevelDisplay : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private Image levelIcon;

        [SerializeField]
        private TextMeshProUGUI levelName;

        [SerializeField]
        private TextMeshProUGUI levelDescription;

        #endregion

        #region Functions
        public void Hydrate(LevelInfo currentLevel)
        {
            levelIcon.sprite = currentLevel.Icon;
            levelName.text = currentLevel.Name;
            levelDescription.text = currentLevel.Description;
        }
        #endregion

    }
}

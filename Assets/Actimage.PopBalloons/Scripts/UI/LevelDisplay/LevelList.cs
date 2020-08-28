using PopBalloons.Configuration;
using PopBalloons.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{
    /// <summary>
    /// Will display a list of LevelButton in order to pick a level
    /// </summary>
    public class LevelList : MonoBehaviour
    {
        #region Variables
        [Header("List settings :")]
        [SerializeField]
        private GameManager.GameType levelType;

        [SerializeField]
        private LevelDisplay display;

        [Header("Buttons settings :")]
        [SerializeField]
        private LevelButton levelButtonPrefab;

        [SerializeField]
        private Sprite activeBackground;

        [SerializeField]
        private Sprite inactiveBackground;

        [SerializeField]
        private Sprite hoverBackground;

        private LevelInfo currentLevel;
        private List<LevelButton> levels;


        //TODO: Change this by a stylesheet
        public Sprite HoverBackground { get => hoverBackground;}
        public Sprite InactiveBackground { get => inactiveBackground;}
        public Sprite ActiveBackground { get => activeBackground;}

        #endregion

        #region Unity Functions
        /// <summary>
        /// We only populate the list in here.
        /// </summary>
        private void Start()
        {
            this.PopulateList();
        }

        #endregion

        #region Functions
        /// <summary>
        /// Will initialize the list with the matching levelbuttons.
        /// </summary>
        private void PopulateList()
        {
            levels = new List<LevelButton>();
            foreach(LevelInfo levelInfo in GameManager.Instance.GetAvailableLevels(this.levelType))
            {
                LevelButton item = Instantiate<LevelButton>(levelButtonPrefab, this.transform);
                item.Hydrate(levelInfo,this);
                levels.Add(item);
            }
            if(levels.Count > 0)
            {
                this.Select(levels[0].Infos);
            }
        }

        public void Select(LevelInfo level)
        {
            currentLevel = level;

            //We refresh level item status
            levels.ForEach((button) => button.SetSelected(button.Infos == level));
            display.Hydrate(currentLevel);
        }

        /// <summary>
        /// Will launch the selected level
        /// </summary>
        public void Play()
        {
            if(currentLevel != null)
            {
                GameManager.Instance.NewGame(currentLevel.Type, currentLevel.GameIndex);
            }
        }

        /// <summary>
        /// Will launch the selected level
        /// </summary>
        public void Next()
        {
            GameManager.Instance.NextLevel();
        }

        public void PlayTutorial()
        {
            if(currentLevel != null)
            {
                //Tutorial is index 0 level;
                GameManager.Instance.NewGame(currentLevel.Type, 0);
            }
        }


        #endregion
    }
}
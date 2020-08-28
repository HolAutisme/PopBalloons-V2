using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.Utilities
{
    /// <summary>
    /// Class that manage level loading from menu
    /// </summary>
    public class LoadLevelButton : MonoBehaviour
    {
        [SerializeField]
        private GameManager.GameType type = GameManager.GameType.COGNITIVE;

        [SerializeField]
        private int levelNumber = 1 ;


        public void Load()
        {
            if (type == GameManager.GameType.NONE || levelNumber < 0)
            {
                GameManager.Instance.Home();
            }
            else
            {
                GameManager.Instance.NewGame(type, levelNumber);
            }
                
        }
    }

}
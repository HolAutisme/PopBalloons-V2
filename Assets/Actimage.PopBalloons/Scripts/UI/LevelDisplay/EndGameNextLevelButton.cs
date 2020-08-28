using PopBalloons.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.UI
{
    public class EndGameNextLevelButton : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI label;

        private Action callback;


        private void Start()
        {
            label = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            GameCreator.OnGameStarted += HandleNewGame;
        }

        private void HandleNewGame(GameManager.GameType type)
        {
            if(GameManager.Instance.CurrentLevelIndex == GameManager.Instance.MaxLevelCount)
            {
                this.label.text = "Choix du niveau";
                this.callback = GameManager.Instance.Home;
            }
            else
            {
                this.label.text = "Niveau suivant";
                this.callback = GameManager.Instance.NextLevel;
            }
        }

        public void OnClick()
        {
            callback?.Invoke();
        }
    }

}
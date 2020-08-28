using PopBalloons.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBalloons.Configuration
{

    [CreateAssetMenu(fileName ="New level",menuName ="PopBalloons/Level")]
    public class LevelInfo : ScriptableObject
    {
        [Header("Interal settings :")]
        [SerializeField]
        private GameManager.GameType type;
        [SerializeField]
        private int gameIndex;

        [Header("UI Settings :")]

        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private string name;
        [SerializeField]
        private string description;

        public Sprite Icon { get => icon; }
        public string Name { get => name; }
        public string Description { get => description;  }
        public int GameIndex { get => gameIndex; }
        public GameManager.GameType Type { get => type;}
    }

}
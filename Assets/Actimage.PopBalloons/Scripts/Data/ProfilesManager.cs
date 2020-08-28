using PopBalloons.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;


namespace PopBalloons.Data
{

    /// <summary>
    /// This class serialize game and player's datas.
    /// Saving score, levels, and player personal informations.
    /// </summary>
    public class ProfilesManager : MonoBehaviour
    {


        [SerializeField]
        private AvatarSettings avatarSettings;

        private static ProfilesManager instance;
        private PlayerProfile currentProfile;


        private List<PlayerProfile> playerProfileList;
        private PlayerProfileCollection playerProfileCollection;
        private string localSavePath;

        public delegate void PlayerListUpdate();
        public static event PlayerListUpdate OnPlayerListUpdated;

        public static AvatarSettings AvatarSettings { get => instance.avatarSettings;}
        public static ProfilesManager Instance { get => instance;}
        public PlayerProfile CurrentProfile { get => currentProfile;}


        #region Unity Functions
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
                this.playerProfileList = new List<PlayerProfile>();
            }            
        }


        private void Start()
        {
            //Init global variables
            currentProfile = null;

            localSavePath = Application.persistentDataPath + "/Saves/gameData.json";

            //Load all profile list on start.
            LoadLocalProfiles();
        }

        #endregion

        /// <summary>
        ///  Load all players profiles from json file
        /// </summary>
        public void LoadLocalProfiles()
        {
            string json = null;

            if (File.Exists(localSavePath))
            {
                json = File.ReadAllText(localSavePath);
                playerProfileCollection = JsonUtility.FromJson<PlayerProfileCollection>(json);
            }
            else
            {
                playerProfileCollection = new PlayerProfileCollection(new List<PlayerProfile>());
            }
            playerProfileList = playerProfileCollection.playerProfileList;
            OnPlayerListUpdated?.Invoke();
        }

        /// <summary>
        /// Create a new profile with all data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="avatar"></param>
        public PlayerProfile CreateProfile(string username,AvatarData avatar)
        {
            string playerId = Guid.NewGuid().ToString();

            PlayerProfile playerProfile = new PlayerProfile();// InitPlayerProfile(username);
            PlayerData playerData = new PlayerData();
            playerData.username = username;
            playerData.avatar = avatar;
            playerData.id = playerId;

            playerProfile.id = playerId;
            playerProfile.data = playerData;
            playerProfile.levelsInfo = new List<Level>();

            playerProfileList.Add(playerProfile);
            SaveAll();

            return playerProfile;
        }

        /// <summary>
        /// Create a new profile with all data or retrieve profile and affect data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="avatar"></param>
        public PlayerProfile UpdatePlayerData(PlayerData datas)
        {
            PlayerProfile playerProfile = playerProfileList.Find((profile) => profile.id == datas.id);
            if (playerProfile != null)
            {
                playerProfile.data = datas;
            }
            else
            {
                playerProfile = new PlayerProfile();
                playerProfile.id = datas.id;
                playerProfile.data = datas;
                playerProfile.levelsInfo = new List<Level>();
                playerProfileList.Add(playerProfile);
            }

            
            SaveAll();
            OnPlayerListUpdated?.Invoke();
            return playerProfile;
        }

        /// <summary>
        /// Edit an existing profile in the saved json file
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        public void EditProfileUsername(string userId, string username)
        {

            PlayerProfile profile = playerProfileList.Find((player) => player.id == userId);

            if(profile == default)
            {
                Debug.Log("User Id does not exist");
                return;
            }

            profile.data.username = username;
            this.SaveAll();
        }


        //Delete an existing profile in the saved json file
        public void DeleteProfile(string id)
        {
            PlayerProfile toRemove = playerProfileList.Find((p) => p.id == id);
            if(toRemove != null)
            {
                playerProfileList.Remove(toRemove);
                SaveAll();
            }
        }

        public void SaveAll()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

            string json = JsonUtility.ToJson(playerProfileCollection, true);
            File.WriteAllText(localSavePath, json);
        }


        //Save current player profile data in json
        public void Save(string levelName, int score)
        {
            if(currentProfile == null)
            {
                Debug.Log("Profile is not set, cannot save you may be in guest mode");
                return;
            }

            Level level = currentProfile.levelsInfo.Find((l) => l.name == levelName);

            if(level != null)
            {
                level.score = Mathf.Max(score, level.score);
            }
            else
            {
                level = new Level();
                level.name = levelName;
                level.score = score;
                currentProfile.levelsInfo.Add(level);
            }
            this.SaveAll();
        }

        // Get the active playing profile
        public PlayerProfile GetCurrentProfile()
        {
            return currentProfile;
        }

        //Set current playing profile to active
        public void SetCurrentProfile(string userId)
        {
            currentProfile = this.playerProfileList.Find((profile) => profile.id == userId.ToString());
            //Debug.Log($"Selected id {currentProfile.id} with name {currentProfile.data.username}");
        }

        /// <summary>
        /// Will set currentProfile to a Guest Profile
        /// </summary>
        public void PlayAsGuest()
        {
            currentProfile = new PlayerProfile();
            currentProfile.id = new Guid().ToString(); // Should be 00000-...-00000
            currentProfile.levelsInfo = new List<Level>();
            currentProfile.data = new PlayerData();
            currentProfile.data.username = "Guest";
            currentProfile.data.id = currentProfile.id;
        }

        //Set current playing profile to active
        public void SetCurrentProfile(PlayerProfile user)
        {
            if(playerProfileList.Contains(user))
                currentProfile = user;
        }

        public List<PlayerProfile> GetAvailableProfiles()
        {
            return this.playerProfileList;
        }


    }

    [Serializable]
    public class PlayerProfileCollection
    {
        public List<PlayerProfile> playerProfileList;

        public PlayerProfileCollection(List<PlayerProfile> _playerProfileList)
        {
            playerProfileList = _playerProfileList;
        }
    }

    [Serializable]
    public class PlayerProfile
    {
        public string id;
        public PlayerData data = null;
        public List<Level> levelsInfo = new List<Level>();
    }


    [Serializable]
    public class PlayerData
    {
        public string id;
        public string username;
        public AvatarData avatar;
    }

    [Serializable]
    public class AvatarData
    {
        public int colorOption = 0;
        public int eyeOption = 0;
        public int accessoryOption = 0;
    }


    [Serializable]
    public class Level
    {
        public string name = null;
        public int score = 0;
    }

}
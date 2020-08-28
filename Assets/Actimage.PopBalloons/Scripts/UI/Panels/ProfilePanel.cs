using PopBalloons.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{
    public enum ProfileSubState { PICK, EDIT, SELECTED};

    public class ProfilePanel : SubPanel<ProfileSubState>
    {
        #region Variables
        private static ProfilePanel instance;


        /// <summary>
        /// Singleton instance accessor
        /// </summary>
        public static ProfilePanel Instance { get => instance; }

        [SerializeField]
        /// <summary>
        /// Manager all profile edition
        /// </summary>
        private ProfileEdition editor;

        #endregion Variables


        #region Unity Functions
        /// <summary>
        /// Singleton's implementation.
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Should'nt have two instances of ProfilePanel.");
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
                this.PopulateWithChildren();
            }
        }

        private void Start()
        {
            this.SetState(ProfileSubState.PICK);
        }
        #endregion Unity Functions


    #region Functions
        public void ClearAllSelection()
        {
            //TODO: ProfileManager unset current profile
            this.SetState(ProfileSubState.PICK);
        }


        public void PlayAsGuest()
        {
            ProfilesManager.Instance.PlayAsGuest();
            MainPanel.Instance.SetState(MainPanelState.MODE_PICK);
        }

        public void SelectPlayer(string id)
        {
            ProfilesManager.Instance.SetCurrentProfile(id);
            MainPanel.Instance.SetState(MainPanelState.MODE_PICK);
        }


        public void EditPlayer(PlayerData profile)
        {
            editor.EditProfile(profile);
            this.SetState(ProfileSubState.EDIT);
        }

        public void NewProfile()
        {
            editor.NewProfile();
            this.SetState(ProfileSubState.EDIT);
        }


        public override void Init()
        {
            this.SetState(ProfileSubState.PICK);
        }
        #endregion

    }
}

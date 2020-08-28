using PopBalloons.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PopBalloons.UI
{

    public class ProfileList : MonoBehaviour
    {
        #region Variables
        [Header("Scene settings:")]
        [SerializeField]
        private GameObject noProfileItem;

        [SerializeField]
        private TMPro.TextMeshProUGUI title;


        [Header("Buttons settings :")]
        [SerializeField]
        private ProfileButton profileButtonPrefab;

        [SerializeField]
        private Sprite activeBackground;

        [SerializeField]
        private Sprite inactiveBackground;

        [SerializeField]
        private Sprite hoverBackground;

        [Header("Background sprite state (active):")]
        [SerializeField]
        private SpriteState activeSpriteState;
        [Header("Background sprite state (inactive):")]
        [SerializeField]
        private SpriteState inactiveSpriteState;

        private PlayerData currentProfile;
        private List<ProfileButton> profiles;


        //TODO: Change this by a stylesheet
        public Sprite HoverBackground { get => hoverBackground; }
        public Sprite InactiveBackground { get => inactiveBackground; }
        public Sprite ActiveBackground { get => activeBackground; }

        public SpriteState InactiveSpriteState { get => inactiveSpriteState; }
        public SpriteState ActiveSpriteState { get => activeSpriteState; }

        #endregion

        #region Unity Functions
        /// <summary>
        /// We only populate the list in here.
        /// </summary>
        private void Start()
        {
            ProfilesManager.OnPlayerListUpdated += PopulateList;
            ProfilePanel.Instance.OnStateChanged += HandlePanelStateChange;
            //title.text = "Choisis parmi les profils déjà créés";

            //Waiting for list to be updated
            this.PopulateList();
            this.ClearSelection();
        }

        private void HandlePanelStateChange(ProfileSubState newState)
        {
           //Returning to neutral state
           if(newState == ProfileSubState.PICK)
            {
                this.ClearSelection();
            }
        }

        private void OnDestroy()
        {
            ProfilesManager.OnPlayerListUpdated -= PopulateList;
            if(ProfilePanel.Instance != null)
                ProfilePanel.Instance.OnStateChanged -= HandlePanelStateChange;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Will initialize the list with the current profile list.
        /// </summary>
        private void PopulateList()
        {
            if(profiles == null)
                profiles = new List<ProfileButton>();


            

            List<PlayerProfile> availableProfiles = ProfilesManager.Instance.GetAvailableProfiles();
            int nbButtonRequired = availableProfiles.Count;

            noProfileItem.SetActive(nbButtonRequired == 0);

            while (profiles.Count < availableProfiles.Count)
            {
                profiles.Add(Instantiate<ProfileButton>(profileButtonPrefab, this.transform));
            }
            for(int i = 0; i < profiles.Count;i++)
            {
                if(i >= nbButtonRequired)
                {
                    profiles[i].gameObject.SetActive(false);
                    continue;
                }
                
                ProfileButton item = profiles[i]; 
                item.Hydrate(availableProfiles[i].data, this);
                profiles[i].gameObject.SetActive(true);
                
            }
            
        }

        /// <summary>
        /// Will clear selection
        /// </summary>
        public  void ClearSelection()
        {
            this.Select(null);
        }


        public void Select(PlayerData profile)
        {
            //For usefull when there is no account yet and we launch a quick party
            if(profiles == null)
            {
                //profiles = new List<ProfileButton>();
                Debug.Log("No profile button found");
                currentProfile = profile;
                return;
            } 

            //We refresh level item status
            if (profile == null)
            {
                profiles.ForEach((button) => button.SetSelected(false));
                title.text = "Choisis parmi les profils déjà créés";
                //CancelOffset
            }
            else
            {
                profiles.ForEach((button) => button.SetSelected(button.Infos == profile));
                title.text = "Bonjour " + profile.username + ",";
            }
            
            currentProfile = profile;
            
        }

        public void EditProfile()
        {
            ProfilePanel.Instance.EditPlayer(currentProfile);
        }

        public void OnClick()
        {
            ProfilePanel.Instance.SelectPlayer(currentProfile.id);
        }
        #endregion
    }
}

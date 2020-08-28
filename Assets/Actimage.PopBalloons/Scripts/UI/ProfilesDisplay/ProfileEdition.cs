using PopBalloons.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PopBalloons.UI
{

    public class ProfileEdition : MonoBehaviour
    {
        [SerializeField]
        private ProfileAvatar avatar;

        [SerializeField]
        private TMPro.TMP_InputField name;

        private PlayerData data;

        private bool isEditing = false;

        public void EditProfile(PlayerData data)
        {
            isEditing = true;
            this.data = data;
            avatar.Hydrate(data.avatar);
            name.text = data.username;
        }

        public void NewProfile()
        {
            PlayerData data = new PlayerData();
            data.id = System.Guid.NewGuid().ToString();
            data.avatar = new AvatarData();
            EditProfile(data);
            isEditing = false;
            //avatar.Init();
            //data.avatar = avatar.Data;
            //name.text = "";

        }

        public void OnClick()
        {
            data.username = name.text;

            ProfilesManager.Instance.UpdatePlayerData(data);
            //Set it as current profile
            ProfilesManager.Instance.SetCurrentProfile(data.id);
            //Display mode pick panel
            MainPanel.Instance.SetState(MainPanelState.MODE_PICK);

        }

        public void OnBack()
        {
            if (isEditing)
            {
                //We save changes
                ProfilesManager.Instance.UpdatePlayerData(data);
            }
        }

    }

}
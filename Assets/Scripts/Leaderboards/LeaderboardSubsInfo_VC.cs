using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AdminModels;
using Newtonsoft.Json;

public class LeaderboardSubsInfo_VC : MonoBehaviour
{
    [SerializeField] private TMP_Text rankNumber;
    [SerializeField] private TMP_Text playerLevels;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text rankTitle;
    [SerializeField] private TMP_Text subscribersNumber;

    [SerializeField] private Image avatarHair;
    [SerializeField] private Image avatarHead;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInfo(int rank, int levels, string name, string title, ulong subscribers)
    {
        rankNumber.text = rank.ToString();
        playerLevels.text = levels.ToString ();
        playerName.text = name;
        rankTitle.text = title;
        subscribersNumber.text = subscribers.ToString ();
        GetUserFaceData (name);
    }
    void SetAvatarData (CharacterAvatarAddressedData avatarData)
    {
        avatarHead.sprite = PlayerInventory.Instance.GetHeadItem (avatarData.Head).sprite;
        avatarHair.sprite = PlayerInventory.Instance.GetHairItem (avatarData.Hair).sprite;
    }
    void GetUserFaceData (string name)
    {
        LookupUserAccountInfoRequest request = new LookupUserAccountInfoRequest { TitleDisplayName = name };
        PlayFabAdminAPI.GetUserAccountInfo (request, OnGetUsersFaceData, ErrorGetUsersFaceData);
    }
    void OnGetUsersFaceData (LookupUserAccountInfoResult result)
    {
        Debug.Log (result.UserInfo.PlayFabId);
        PlayFab.ClientModels.GetUserDataRequest request = new PlayFab.ClientModels.GetUserDataRequest
        {
            PlayFabId = result.UserInfo.PlayFabId
        };
        PlayFabClientAPI.GetUserData (request, GetUsersPublicData, ErrorGetUsersFaceData);
    }
    void ErrorGetUsersFaceData (PlayFabError error)
    {
        Debug.LogError (error.ErrorDetails);
    }

    void GetUsersPublicData (PlayFab.ClientModels.GetUserDataResult result)
    {
        PlayFab.ClientModels.UserDataRecord data = result.Data["Avatar"];
        CharacterAvatarAddressedData avatarData = JsonConvert.DeserializeObject<CharacterAvatarAddressedData> (data.Value);
        SetAvatarData (avatarData);
    }
}

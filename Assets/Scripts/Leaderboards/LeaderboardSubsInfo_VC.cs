using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AdminModels;
using Newtonsoft.Json;
using Zenject;

public class LeaderboardSubsInfo_VC : MonoBehaviour
{
    [SerializeField] private TMP_Text rankNumber;
    [SerializeField] private Image rankImage;
    [SerializeField] private TMP_Text playerLevels;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text rankTitle;
    [SerializeField] private TMP_Text subscribersNumber;

    [SerializeField] private Image avatarHair;
    [SerializeField] private Image avatarHead;

 

    public void SetInfo(int rank ,Sprite rankIcon, int levels, string name, string title, ulong subscribers)
    {
        rankNumber.text = rank.ToString();
        if (rankImage != null)
            rankImage.sprite = rankIcon;
        else
            rankImage.gameObject.SetActive (false);
        playerLevels.text = levels.ToString ();
        playerName.text = name;
        rankTitle.text = title;
        subscribersNumber.text = subscribers.ToString ();
        GetUserFaceData (name);
    }
    void SetAvatarData (CharacterAvatarAddressedData avatarData)
    {
        avatarHead.sprite = HeadAssets.Instance.GetHeadSprite(avatarData.Head);
        avatarHair.sprite = HeadAssets.Instance.GetHairSprite(avatarData.Hair);
        if (avatarHair.sprite == null)
            avatarHair.gameObject.SetActive(false);
        else
            avatarHair.gameObject.SetActive(true);
    }
    void GetUserFaceData (string name)
    {
        LookupUserAccountInfoRequest request = new LookupUserAccountInfoRequest { TitleDisplayName = name };
        PlayFabAdminAPI.GetUserAccountInfo (request, OnGetUsersFaceData, ErrorGetUsersFaceData);
    }
    void OnGetUsersFaceData (LookupUserAccountInfoResult result)
    {
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
        if (result.Data.ContainsKey("Avatar"))
        {
            PlayFab.ClientModels.UserDataRecord data = result.Data["Avatar"];
            CharacterAvatarAddressedData avatarData = JsonConvert.DeserializeObject<CharacterAvatarAddressedData> (data.Value);
            SetAvatarData (avatarData);
        } 
    }
}

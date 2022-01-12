using System.Collections;
using System.Collections.Generic;
using Google;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
  public override void InstallBindings()
  {
    SignalBusInstaller.Install(Container);
    
    // Signals
    Container.DeclareSignal<OnPlayFabLoginSuccessesSignal>();
    Container.DeclareSignal<OnLoginFailedSignal>();
    Container.DeclareSignal<OnGoogleSignInFailed>();
    Container.DeclareSignal<OnGoogleSignInSuccessSignal>();
    Container.DeclareSignal<OnFacebookLoginFailedSignal>();
    Container.DeclareSignal<OnFacebookLoginSuccessSignal>();
    Container.DeclareSignal<OnAppleLoginSuccessSignal>();
    Container.DeclareSignal<OnAppleLoginFailedSignal>();
    Container.DeclareSignal<OnPurchaseProductSignal>();
    Container.DeclareSignal<ProcessPurchaseSignal>();
    Container.DeclareSignal<ConfirmPendingPurchaseSignal>();
    Container.DeclareSignal<BuyHouseSignal>();
    

    Container.DeclareSignal<SaveRoomLayoutSignal>();
    Container.DeclareSignal<DiscardRoomLayoutSignal>();
    Container.DeclareSignal<AddSoftCurrencyForExperienceSignal> ();
    Container.DeclareSignal<ChangePlayerSubsSignal> ();
    Container.DeclareSignal<AssetsLoadedSignal>();
    Container.DeclareSignal<RemoteAssetsCheckSignal>();
    
    Container.DeclareSignal<OpenDefaultMessagePopUpSignal> ();
    Container.DeclareSignal<UpdateSoftCurrencySignal> ();
    Container.DeclareSignal<UpdateHardCurrencySignal> ();
    Container.DeclareSignal<UpdateSoftCurrencySignal> ();

     Container.DeclareSignal<SelectThemeSignal>();
    Container.DeclareSignal<StartRecordingSignal>();
    Container.DeclareSignal<VideoStartedSignal>();
    Container.DeclareSignal<StartPublishSignal>();
    Container.DeclareSignal<PublishVideoSignal>();
    Container.DeclareSignal<EndPublishVideoSignal> ();
    Container.DeclareSignal<EnergyValueSignal> ();
    Container.DeclareSignal<AddEnergySignal> ();
    Container.DeclareSignal<ShowVideosStatsSignal> ();
    Container.DeclareSignal<GetMoneyFromVideoSignal> ();
    Container.DeclareSignal<RoomZoomStateChangedSignal>();
    Container.DeclareSignal<UpdateExperienceSignal> ();
    Container.DeclareSignal<OpenVideoManagerSignal> ();
    Container.DeclareSignal<OpenHomePanelSignal> (); 
    Container.DeclareSignal<Recieve3BestLeaderboard> ();
    Container.DeclareSignal<RecieveTop10Leaderboard> ();
    Container.DeclareSignal<RecievePlayerLeaderboardPosition> ();
    Container.DeclareSignal<OnVideosStatsUpdatedSignal> ();
    Container.DeclareSignal<OpenThemeSelectorPopUpSignal> ();
    Container.DeclareSignal<ThemeHeldSignal> ();
    Container.DeclareSignal<ConfirmThemesSignal> ();
    Container.DeclareSignal<CancelVideoRecordingSignal> ();
    Container.DeclareSignal<ChangeUsernameSignal> ();
    Container.DeclareSignal<OpenVideoCreationSignal> ();
    Container.DeclareSignal<CloseVideoCreationSignal> ();
    Container.DeclareSignal<UpdateThemesGraphSignal> ();
    Container.DeclareSignal<OpenSettingPanelSignal> ();
    Container.DeclareSignal<OpenDeleteAccountSignal> ();
    Container.DeclareSignal<OpenLeaderboardsSignal> ();
    Container.DeclareSignal<AddSubsForExperienceSignal> ();
    Container.DeclareSignal<AddViewsForExperienceSignal> ();
    Container.DeclareSignal<LevelUpSignal> ();
    Container.DeclareSignal<UpdateRankSignal> ();
    Container.DeclareSignal<OpenLevelUpPanelSignal> ();
    Container.DeclareSignal<OpenDefaultMessagePopUpSignal> ();
    Container.DeclareSignal<CloseAdsDefaultPopUpSignal> ();
    Container.DeclareSignal<OpenAdsDefaultPopUpSignal> ();
    Container.DeclareSignal<OpenViralPopUpSignal>();
    Container.DeclareSignal<ChangeBackButtonSignal> ();
    Container.DeclareSignal<OpenEnergyInventorySignal> ();
    Container.DeclareSignal<UseEnergyItemSignal> ();
    Container.DeclareSignal<ChangeRestStateSignal> ();
    Container.DeclareSignal<RoomCustomizationVisibilityChanged>();
    Container.DeclareSignal<CharacterCustomizationVisibilityChanged>();

    //Advertisement signals
    Container.DeclareSignal<AdsInitializedSignal> ();
    Container.DeclareSignal<RewardAdLoadedSignal> ();
    Container.DeclareSignal<StartShowingAdSignal> ();
    Container.DeclareSignal<GrantRewardSignal> ();
    Container.DeclareSignal<NotGrantedRewardSignal> ();
    Container.DeclareSignal<FinishedAdVisualitationRewardSignal> ();
    Container.DeclareSignal<OpenSoftCurrencyAdSignal> ();
    Container.DeclareSignal<OpenHardCurrencyAdSignal> ();
    Container.DeclareSignal<OpenThemeBonusAdSignal> ();
    Container.DeclareSignal<OpenEnergyAdSignal> ();
    Container.DeclareSignal<OpenTimeShortenAdSignal> ();
    Container.DeclareSignal<OpenDoubleViewsAdSignal> ();

    //Customizations signals 
    Container.DeclareSignal<OnPlayerEquippedThemeItemChangedSignal>();
    Container.DeclareSignal<TestRoomThemeItemSignal>();
    Container.DeclareSignal<TestRoomVideoQualityITemSignal>();
    Container.DeclareSignal<OnPlayerInventoryFetchedSignal>();
    Container.DeclareSignal<OnCharacterAvatarChanged>();
    Container.DeclareSignal<OnPlayerInventoryFetchedSignal>();

    //VFX
    Container.DeclareSignal<VFX_EnergyChangeSignal>();
    Container.DeclareSignal<VFX_LowEnergyBlinkSignal>();
    Container.DeclareSignal<VFX_NoEnergyParticlesSignal>();

    //Views signals
    Container.DeclareSignal<SnapToNeighborhoodViewSignal>();
    Container.DeclareSignal<OpenRealEstateShopSignal>();

    //Sound signals
    Container.DeclareSignal<BuyItemSoundSignal>();

    //Dependencies
    Container.Bind<PlayerInventory>().FromInstance(PlayerInventory.Instance);

    Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance);
    Container.Bind<IAuthenticator>().To<PlayFabAuthenticator>().AsSingle();
    
    
    
    
    //views
    Container.DeclareSignal<HouseChangedSignal>();
    Container.DeclareSignal<HousePopUp>();

  }
}

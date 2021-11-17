using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class LevelUpPopUp_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ExperienceManager xpManager;

    [SerializeField] private GameObject experienceBarPanel;
    [SerializeField] private GameObject levelReachedPanel;
    [SerializeField] private GameObject experienceChangePanel;
    [SerializeField] private GameObject rewardsPanel;
    [SerializeField] private GameObject closeButtonPanel;

    [SerializeField] private Image fillBarImage;

    [SerializeField] private TMP_Text reachedLevel;
    [SerializeField] private TMP_Text reachedLevel2;
    [SerializeField] private TMP_Text oldXpLimit;
    [SerializeField] private TMP_Text newXpLimit;

    [SerializeField] private Sprite softCurrencyImage;
    [SerializeField] private Sprite hardCurrencyImage;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject rewardsHolder;

    [SerializeField] private List<GameObject> rewardsSpawned = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenLevelUpPanelSignal> (StartPopUpOpenAnimation);
    }

    void StartPopUpOpenAnimation ()
    {
        experienceBarPanel.SetActive(true);
        levelReachedPanel.SetActive (false);
        experienceChangePanel.SetActive (false);
        rewardsPanel.SetActive (false);
        closeButtonPanel.SetActive (false);

        StartCoroutine (PopUpOpening());
    }
    IEnumerator PopUpOpening ()
    {
        Debug.Log ("Start Level Up Animation");
        float fillBar = 0;
        while (fillBar < 1)
        {
            Mathf.Lerp (0, 1, fillBar);
            fillBar += Time.deltaTime;
            fillBarImage.fillAmount = fillBar;
            yield return null;
        }

        yield return new WaitForSeconds (0.5f);
        experienceBarPanel.SetActive (false);
        levelReachedPanel.SetActive (true);

        int playerLevel = xpManager.GetPlayerLevel ();
        reachedLevel.text = $"Level {playerLevel}";
        reachedLevel2.text = $"Level {playerLevel}";
        float fontSize = reachedLevel.fontSize;
        float lerpValue = 0;
        while (lerpValue < 1)
        {
            lerpValue += Time.deltaTime*2;
            reachedLevel.fontSizeMax = Mathf.Lerp(0,fontSize,lerpValue);
            yield return null;
        }

        yield return new WaitForSeconds (0.5f);

        if(playerLevel>0)
        {
            foreach(GameObject reward in rewardsSpawned)
            {
                Destroy (reward);
            }
            rewardsSpawned.Clear ();
            levelReachedPanel.SetActive (false);
            experienceChangePanel.SetActive (true);
            rewardsPanel.SetActive (true);

            oldXpLimit.text = $"{xpManager.GetXpThreshold (playerLevel - 1)}";
            newXpLimit.text = $"{xpManager.GetXpThreshold (playerLevel)}";

            RewardsData rewards = xpManager.GetRewardData (playerLevel);

            if (rewards.softCurrency > 0)
            {
                GameObject softReward = Instantiate (rewardPrefab, rewardsHolder.transform);
                softReward.GetComponentInChildren<Image> ().sprite = softCurrencyImage;
                softReward.GetComponentInChildren<TMP_Text> ().text = $"{rewards.softCurrency}";
                rewardsSpawned.Add (softReward);
            }
            if (rewards.hardCurrency > 0)
            {
                GameObject softReward = Instantiate (rewardPrefab, rewardsHolder.transform);
                softReward.GetComponentInChildren<Image> ().sprite = hardCurrencyImage;
                softReward.GetComponentInChildren<TMP_Text> ().text = $"{rewards.hardCurrency}";
                rewardsSpawned.Add (softReward);
            }
            //Dummy still need to instantiate the items earned by the reward
        }

        closeButtonPanel.SetActive (true);
    }
}
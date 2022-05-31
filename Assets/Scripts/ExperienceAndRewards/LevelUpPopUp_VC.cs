using System.Linq;
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
    [SerializeField] private TMP_Text oldXpLimit;
    [SerializeField] private TMP_Text newXpLimit;

    [SerializeField] private Sprite softCurrencyImage;
    [SerializeField] private Sprite hardCurrencyImage;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject rewardsHolder;

    [SerializeField] private List<GameObject> rewardsSpawned = new List<GameObject>();

    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenLevelUpPanelSignal> (StartPopUpOpenAnimation);
    }

    void StartPopUpOpenAnimation ()
    {
        experienceBarPanel.SetActive(true);
        levelReachedPanel.SetActive (true);
        experienceChangePanel.SetActive (false);
        rewardsPanel.SetActive (false);
        closeButtonPanel.SetActive (false);
        PopulateRewards();
        StartCoroutine (PopUpOpening());
    }
    void PopulateRewards()
    {
        int playerLevel = xpManager.GetPlayerLevel();
        reachedLevel.text = $"Rank {playerLevel}";

        if (playerLevel > 0)
        {
            foreach (GameObject reward in rewardsSpawned)
            {
                Destroy(reward);
            }
            rewardsSpawned.Clear();

            oldXpLimit.text = $"{xpManager.GetXpThreshold(playerLevel - 1)}";
            newXpLimit.text = $"{xpManager.GetXpThreshold(playerLevel)}";

            RewardsData rewards = xpManager.GetRewardData(playerLevel);

            if (rewards.softCurrency > 0)
            {
                GameObject softReward = Instantiate(rewardPrefab, rewardsHolder.transform);
                Image[] allImages = softReward.GetComponentsInChildren<Image>();
                allImages.Where(k => k.gameObject.name == "Icon").FirstOrDefault().sprite = softCurrencyImage;
                softReward.GetComponentInChildren<TMP_Text>().text = $"x{rewards.softCurrency}";
                rewardsSpawned.Add(softReward);
            }
            if (rewards.hardCurrency > 0)
            {
                GameObject softReward = Instantiate(rewardPrefab, rewardsHolder.transform);
                Image[] allImages = softReward.GetComponentsInChildren<Image>();
                allImages.Where(k => k.gameObject.name == "Icon").FirstOrDefault().sprite = hardCurrencyImage;
                softReward.GetComponentInChildren<TMP_Text>().text = $"x{rewards.hardCurrency}";
                rewardsSpawned.Add(softReward);
            }
            //Dummy still need to instantiate the items earned by the reward
        }
    }
    IEnumerator PopUpOpening ()
    {
        anim.Play("Star_LevelUp");
        experienceBarPanel.SetActive(true);
        levelReachedPanel.SetActive(true);
        experienceChangePanel.SetActive(false);
        rewardsPanel.SetActive(false);
        closeButtonPanel.SetActive(false);
        Debug.Log ("Start Level Up Animation");
        float fillBar = 0;
        while (fillBar < 1)
        {
            Mathf.Lerp (0, 1, fillBar);
            fillBar += Time.deltaTime*2;
            fillBarImage.fillAmount = fillBar;
            yield return null;
        }
        //yield return new WaitForSeconds(0.5f);
        anim.Play("Glow_LevelUp");
    }
}

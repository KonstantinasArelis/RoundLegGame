using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// TODO: refactor this to their own controllers
// Fix bug when if level up reward not selected, put the next in Queue
public class MainHudController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private int currentScore;

    private GameObject levelUpPanel;
    private GameObject upgradeItemsPanel;

    private GameObject buildingPanel;

    private PlayerController playerController;

    private BuildSystem buildSystem;
    private TextMeshProUGUI waveTimeText;

    private GameObject lastSelectedBuildingItem = null;

    [SerializeField] private UpgradeData currentUpgrade;
    [SerializeField] private BuildingData[] buildings;
    [SerializeField] private GameObject upgradeItemUIPrefab;
    [SerializeField] private GameObject buildingItemUIPrefab;

    public GameObject[] DamageProgressBar;
    public GameObject[] FireRateProgressBar;
    public GameObject[] PenetrationProgressBar;
    public GameObject[] KnockBackProgressBar;

    public int DamageProgress;
    public int FireRateProgress;
    public int PenetrationProgress;
    public int KnockBackProgress;

    public GameObject gunStatPanel;

    void Awake()
    {
        DamageProgressBar[0].SetActive(false);
        DamageProgressBar[1].SetActive(false);
        DamageProgressBar[2].SetActive(false);
        DamageProgress=0;
        FireRateProgress=0;
        PenetrationProgress=0;
        KnockBackProgress=0;

        scoreText = transform.Find("Score/Amount").GetComponent<TextMeshProUGUI>();
        levelUpPanel = transform.Find("LevelUpPanel").gameObject;
        upgradeItemsPanel = levelUpPanel.transform.Find("UpgradeItems").gameObject;
        waveTimeText = transform.Find("WaveTime").GetComponent<TextMeshProUGUI>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        buildingPanel = transform.Find("BuildingItems").gameObject;
        buildSystem = GameObject.Find("BuildSystem").GetComponent<BuildSystem>();
        waveTimeText.text = "00:00";
        BuildingEnabledChanged(true);
        levelUpPanel.SetActive(false);
    }

    void Start()
    {
        playerController.SelectGun(currentUpgrade);
    }

    void OnEnable()
    {
        buildSystem.BuildingPlacedEvent.AddListener(OnBuildingPlaced);
        buildSystem.BuildingDestroyedEvent.AddListener(OnBuildingDestroyed);
    }

    void OnDisable()
    {
        buildSystem.BuildingPlacedEvent.RemoveListener(OnBuildingPlaced);
        buildSystem.BuildingDestroyedEvent.RemoveListener(OnBuildingDestroyed);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        if (score < 0)
        {
            AnimateSubtractScore(score.ToString());
        }
        else if (score > 0)
        {
            AnimateAddScore();
        }
        scoreText.text = currentScore.ToString();
    }

    private void OnBuildingPlaced(BuildingData buildingData)
    {
        AddScore(buildingData.cost);
    }

    private void OnBuildingDestroyed(int cost)
    {
        AddScore(-cost);
    }

    private void AnimateSubtractScore(string scoreString)
    {
        GameObject costScore = Instantiate(scoreText.gameObject, scoreText.transform.parent);
        costScore.GetComponent<TextMeshProUGUI>().text = scoreString;
        CanvasGroup canvasGroup = costScore.AddComponent<CanvasGroup>();
        var rectTransform = costScore.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0f, -scoreText.GetComponent<RectTransform>().rect.height * 0.5f);
        rectTransform.DOAnchorPos(new Vector2(0f, -100f), 1f).SetEase(Ease.InQuad);
        canvasGroup.DOFade(0f, 1f);
        Destroy(costScore, 2f);
    }

    private void AnimateAddScore()
    {
        var scoreElement = scoreText.gameObject.GetComponent<RectTransform>();
        Tweens.Pop(scoreElement, 1.1f, 0.4f);
    }

    public void OnLevelUp()
    {
        // TODO: don't empty everytime for performance?
        // the panel HAS to be active first because strange things can happen
        // upgrade only when the level is right or when the upgrade is the last
        if (
            currentUpgrade.nextUpgradeLevel > playerController.levelProvider.GetCurrentLevel()
            || currentUpgrade.nextUpgrades.Length == 0)
        {
            return;
        }
        // if the level is right look if the current upgrade was accepted
        levelUpPanel.SetActive(true);
        Utility.DestroyChildren(upgradeItemsPanel);
        DisplayLevelUpItems();
        RenewItems(upgradeItemsPanel, () => {UpgradeClickSetup();});
    }

    public void BuildingEnabledChanged(bool isBuildingEnabled)
    {
        if (isBuildingEnabled) OnBuildingEnabled(); else OnBuildingDisabled();
    }

    public void OnBuildingDisabled()
    {
        buildingPanel.SetActive(false);
    }

    public void OnBuildingEnabled()
    {
        buildingPanel.SetActive(true);
        Utility.DestroyChildren(buildingPanel);
        DisplayBuildingItems();
        RenewItems(buildingPanel, () => {
            BuildingClickSetup();
        });
    }

    private void DisplayBuildingItems()
    {
        for (int i = 0; i < buildings.Length; ++i)
        {
            GameObject buildingItem = Instantiate(buildingItemUIPrefab, buildingPanel.GetComponent<RectTransform>());
            buildingItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = buildings[i].name;
            buildingItem.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = buildings[i].cost.ToString();
            RawImage rawImage = buildingItem.transform.Find("ItemDisplay/ItemImage").GetComponent<RawImage>();
            Texture2D thumbnail = AssetPreview.GetAssetPreview(buildings[i].prefab);
            if (thumbnail != null)
            {
                rawImage.texture = thumbnail;
                rawImage.color = Color.white;
            }
            else
            {
                StartCoroutine(Coroutines.WaitForThumbnailCoroutine(buildings[i].prefab, rawImage));
            }
        }
    }

    private void UpgradeClickSetup()
    {
        for (int i = 0; i < currentUpgrade.nextUpgrades.Length; ++i)
        {
            SetupUpgradeClick(i);
        }
    }

    private void SetupUpgradeClick(int i)
    {
        Button button = upgradeItemsPanel
            .transform.GetChild(i).GetComponentInChildren<Button>();
        button.onClick.AddListener(() => {
            playerController.SelectGun(currentUpgrade.nextUpgrades[i]);
            currentUpgrade = currentUpgrade.nextUpgrades[i];
            levelUpPanel.SetActive(false);
            // it will level up if it has to - prevents unclicked upgrades
            OnLevelUp();
        });
    }

    private void BuildingClickSetup()
    {
        for (int i = 0; i < buildings.Length; ++i)
        {
            SetupBuildingClick(i);
        }
    }

    private void SetupBuildingClick(int i)
    {
        GameObject buildingItemUI = buildingPanel.transform.GetChild(i).gameObject;
        Button button = buildingItemUI.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => {
            Tweens.Pop(buildingItemUI.GetComponent<RectTransform>(), 1.2f, 0.2f);
            if (lastSelectedBuildingItem != null)
            {
                // default color
                lastSelectedBuildingItem.GetComponent<RawImage>().color = buildingItemUIPrefab.GetComponent<RawImage>().color;
                if (buildSystem.currentBuilding.Equals(buildings[i]))
                {
                    buildSystem.currentBuilding = null;
                    lastSelectedBuildingItem = null;
                    return;
                }
            }
            buildSystem.currentBuilding = buildings[i];
            buildingItemUI.GetComponent<RawImage>().color = new Color(1f, 1f, 0f, 0.6f);
            lastSelectedBuildingItem = buildingItemUI;
        });
    }

    private void DisplayLevelUpItems()
    {
        for (int i = 0; i < currentUpgrade.nextUpgrades.Length; ++i)
        {
            GameObject upgradeItem = Instantiate(upgradeItemUIPrefab, upgradeItemsPanel.GetComponent<RectTransform>());
            RawImage rawImage = upgradeItem.GetComponentInChildren<RawImage>();
            GameObject upgradeGun = currentUpgrade.nextUpgrades[i].prefab;
            upgradeItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = currentUpgrade.nextUpgrades[i].name;

            Texture2D thumbnail = AssetPreview.GetAssetPreview(upgradeGun);
            if (thumbnail != null)
            {
                rawImage.texture = thumbnail;
                rawImage.color = Color.white;
            }
            else
            {
                StartCoroutine(Coroutines.WaitForThumbnailCoroutine(upgradeGun, rawImage));
            }
        }
    }

    private void RenewItems(GameObject panel, Action callback)
    {
        StartCoroutine(
            Coroutines
            .DelayedLayoutRebuildCoroutine(panel)
            .WithCallback(callback));
    }



    public void SetWaveTime(int waveTime)
    {
        // TODO: don't do this every 1s
        int minutes = waveTime / 60;
        int seconds = waveTime % 60;
        waveTimeText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public void UpgradeSelectedGun(string statName) // cannot pass more than 1 value, or a enum value, in unity button onClick
    {
        
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>(); // PlayerController resets itself to null if this line is not here. I dont know why.
        playerController.UpgradeSelectedGun(statName);

        switch (statName)
        {
            case "shotCooldownSeconds":
                DamageProgressBar[DamageProgress].SetActive(true);
                DamageProgress++;
                break;
            case "penetration":
                
                break;
            case "knockbackForce":
                
                break;
            case "baseDamage":
                
                break;
        }
        Canvas.ForceUpdateCanvases();

    }
}

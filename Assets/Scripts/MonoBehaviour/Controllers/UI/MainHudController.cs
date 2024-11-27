using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    private int previousLevel = 0;
    public int upgradeLevelInterval;
    [SerializeField] private UpgradeData starterUpgrade;
    [SerializeField] private List<UpgradeData> nextUpgrades;
    [SerializeField] private BuildingData[] buildings;
    [SerializeField] private GameObject upgradeItemUIPrefab;
    [SerializeField] private GameObject buildingItemUIPrefab;

    public int DamageProgress;
    public int FireRateProgress;
    public int PenetrationProgress;
    public int KnockBackProgress;

    [SerializeField] private Transform[] DamageProgressBits;
    [SerializeField] private Transform[] FireRateProgressBits;
    [SerializeField] private Transform[] PenetrationProgressBits;
    [SerializeField] private Transform[] KnockBackProgressBits;

    public GameObject gunStatPanel;
    private Vector3 activatedBitSize;

    private Dictionary<string, GunStatPanelTypeEnum> statNameToEnum;

    private Vector3 initialLevelUpScale;

    void Awake()
    {
        DamageProgress = 0;
        FireRateProgress = 0;
        PenetrationProgress = 0;
        KnockBackProgress = 0;

        statNameToEnum = new()
        {
            { "shotCooldownSeconds", GunStatPanelTypeEnum.ShotCooldownSeconds },
            { "penetration", GunStatPanelTypeEnum.Penetration },
            { "knockbackForce", GunStatPanelTypeEnum.Knockback },
            { "baseDamage", GunStatPanelTypeEnum.BaseDamage }
        };

        activatedBitSize = new Vector3(0.6866899f, 0.3495518f, 1f);

        scoreText = transform.Find("Score/Amount").GetComponent<TextMeshProUGUI>();
        levelUpPanel = transform.Find("LevelUpPanel").gameObject;
        initialLevelUpScale = levelUpPanel.transform.localScale;
        upgradeItemsPanel = levelUpPanel.transform.Find("UpgradeItems").gameObject;
        waveTimeText = transform.Find("WaveTime").GetComponent<TextMeshProUGUI>();
        buildingPanel = transform.Find("BuildingItems").gameObject;
        buildSystem = GameObject.Find("BuildSystem").GetComponent<BuildSystem>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        waveTimeText.text = "10:00";
        BuildingEnabledChanged(true);
        InitiateGunStatPanel();

        levelUpPanel.SetActive(false);
    }

    void Start()
    {
        playerController.SelectUpgrade(starterUpgrade);
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
    public int GetScore()
    {
        return currentScore;
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
        costScore.AddComponent<LayoutElement>().ignoreLayout = true;
        var rectTransform = costScore.GetComponent<RectTransform>();
        var scoreTextSizeDelta = scoreText.gameObject.GetComponent<RectTransform>().sizeDelta;
        rectTransform.sizeDelta = new Vector2(scoreTextSizeDelta.x * 100, scoreTextSizeDelta.y);
        costScore.GetComponent<TextMeshProUGUI>().text = scoreString;
        CanvasGroup canvasGroup = costScore.AddComponent<CanvasGroup>();
        float scoreTextAnchorX = scoreText.GetComponent<RectTransform>().anchoredPosition.x;
        rectTransform.anchoredPosition = new Vector2(scoreTextAnchorX, -scoreText.GetComponent<RectTransform>().rect.height * 0.5f);
        rectTransform.DOAnchorPos(new Vector2(scoreTextAnchorX, -100f), 1f).SetEase(Ease.InQuad);
        canvasGroup.DOFade(0f, 1f);
        Destroy(costScore, 2f);
    }

    private async void AnimateAddScore()
    {
        var scoreElement = scoreText.gameObject.GetComponent<RectTransform>();
        await Tweens.Pop(scoreElement, 1.1f, 0.4f).AsyncWaitForCompletion();
    }

    public void OnLevelUp()
    {
        if (playerController.levelProvider.GetCurrentLevel() - previousLevel >= upgradeLevelInterval
            && nextUpgrades.Count > 0)
        {
            previousLevel += upgradeLevelInterval;
            ActivateLevelUpPanel();
            Utility.DestroyChildren(upgradeItemsPanel);
            DisplayLevelUpItems();
            RenewItems(upgradeItemsPanel, UpgradeClickSetup);
        }
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
        RenewItems(buildingPanel, () => { BuildingClickSetup(); });
    }

    private void DisplayBuildingItems()
    {
        for (int i = 0; i < buildings.Length; ++i)
        {
            GameObject buildingItem = Instantiate(buildingItemUIPrefab, buildingPanel.GetComponent<RectTransform>());
            buildingItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = buildings[i].name;
            buildingItem.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = buildings[i].cost.ToString();
            RawImage rawImage = buildingItem.transform.Find("ItemDisplay/ItemImage").GetComponent<RawImage>();

// #if UNITY_EDITOR
//             Texture2D thumbnail = AssetPreview.GetAssetPreview(buildings[i].prefab);
//             if (thumbnail != null)
//             {
//                 rawImage.texture = thumbnail;
//                 rawImage.color = Color.white;
//             }
//             else
//             {
//                 StartCoroutine(Coroutines.WaitForThumbnailCoroutine(buildings[i].prefab, rawImage));
//             }
// #else
//             Debug.LogWarning("AssetPreview is not available in builds.");
//             rawImage.texture = null;
// #endif
            rawImage.texture = buildings[i].icon;
            rawImage.color = Color.white;
        }
    }

    private void UpgradeClickSetup()
    {
        for (int i = 0; i < nextUpgrades.Count; ++i)
        {
            SetupUpgradeClick(i);
        }
    }

    private void SetupUpgradeClick(int i)
    {
        Button button = upgradeItemsPanel.transform.GetChild(i).GetComponentInChildren<Button>();
        button.onClick.AddListener(async () =>
        {
            playerController.SelectUpgrade(nextUpgrades[i]);
            nextUpgrades[i] = nextUpgrades[i].nextUpgrade;

            if (nextUpgrades[i] == null)
                nextUpgrades.RemoveAt(i);

            await DeactivateLevelUpPanel();
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
        button.onClick.AddListener(async () =>
        {
            await Tweens.Pop(buildingItemUI.GetComponent<RectTransform>(), 1.2f, 0.2f).AsyncWaitForCompletion();
            if (lastSelectedBuildingItem != null)
            {
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
        int currentLevel = playerController.levelProvider.GetCurrentLevel();

        foreach (UpgradeData upgrade in nextUpgrades)
        {
            GameObject upgradeItem = Instantiate(upgradeItemUIPrefab, upgradeItemsPanel.GetComponent<RectTransform>());
            RawImage rawImage = upgradeItem.GetComponentInChildren<RawImage>();
            GameObject upgradeGun = upgrade.prefab;
            upgradeItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgrade.name;

// #if UNITY_EDITOR
//             Texture2D thumbnail = AssetPreview.GetAssetPreview(upgradeGun);
//             if (thumbnail != null)
//             {
//                 rawImage.texture = thumbnail;
//                 rawImage.color = Color.white;
//             }
//             else
//             {
//                 StartCoroutine(Coroutines.WaitForThumbnailCoroutine(upgradeGun, rawImage));
//             }
// #else
//             Debug.LogWarning("AssetPreview is not available in builds.");
//             rawImage.texture = null;
// #endif
            rawImage.texture = upgrade.icon;
            rawImage.color = Color.white;
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
        int minutes = waveTime / 60;
        int seconds = waveTime % 60;
        waveTimeText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    void InitiateGunStatPanel()
    {
        foreach (Transform bit in DamageProgressBits)
        {
            bit.DOScale(Vector3.zero, 0.5f);
        }
        foreach (Transform bit in FireRateProgressBits)
        {
            bit.DOScale(Vector3.zero, 0.5f);
        }
        foreach (Transform bit in PenetrationProgressBits)
        {
            bit.DOScale(Vector3.zero, 0.5f);
        }
        foreach (Transform bit in KnockBackProgressBits)
        {
            bit.DOScale(Vector3.zero, 0.5f);
        }
    }

    public void UpgradeSelectedGun(string statName)
    {
        GunStatPanelTypeEnum stat = statNameToEnum[statName];
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        AnimateStatIncrease(statName);
        playerController.UpgradeSelectedGun(stat);
    }

    public void AnimateStatIncrease(string statName)
    {
        float currentStat;
        switch (statName)
        {
            case "baseDamage":
                currentStat = playerController.selectedGunController.baseDamageUpgradeCount;
                DamageProgressBits[(int)currentStat].DOScale(activatedBitSize, 0.5f).SetEase(Ease.InBounce);
                break;
            case "shotCooldownSeconds":
                currentStat = playerController.selectedGunController.shotCooldownSecondsUpgradeCount;
                FireRateProgressBits[(int)currentStat].DOScale(activatedBitSize, 0.5f).SetEase(Ease.InBounce);
                break;
            case "penetration":
                currentStat = playerController.selectedGunController.penetrationUpgradeCount;
                PenetrationProgressBits[(int)currentStat].DOScale(activatedBitSize, 0.5f).SetEase(Ease.InBounce);
                break;
            case "knockbackForce":
                currentStat = playerController.selectedGunController.knockbackForceUpgradeCount;
                KnockBackProgressBits[(int)currentStat].DOScale(activatedBitSize, 0.5f).SetEase(Ease.InBounce);
                break;
        }
    }

    public void AnimateGunStatPanelUpdate()
    {
        InitiateGunStatPanel();

        for (int i = 0; i < playerController.selectedGunController.baseDamageUpgradeCount; i++)
        {
            DamageProgressBits[i].DOScale(activatedBitSize, 0.5f);
        }
        for (int i = 0; i < playerController.selectedGunController.shotCooldownSecondsUpgradeCount; i++)
        {
            FireRateProgressBits[i].DOScale(activatedBitSize, 0.5f);
        }
        for (int i = 0; i < playerController.selectedGunController.penetrationUpgradeCount; i++)
        {
            PenetrationProgressBits[i].DOScale(activatedBitSize, 0.5f);
        }
        for (int i = 0; i < playerController.selectedGunController.knockbackForceUpgradeCount; i++)
        {
            KnockBackProgressBits[i].DOScale(activatedBitSize, 0.5f);
        }
    }

    private void ActivateLevelUpPanel()
    {
        if (levelUpPanel.activeSelf) return;
        levelUpPanel.transform.localScale = Vector3.zero;
        levelUpPanel.SetActive(true);
        levelUpPanel.transform.DOScale(initialLevelUpScale, 0.5f).SetEase(Ease.OutBack).OnComplete(() => Time.timeScale = 0f);;

        PauseMenuManager.isPaused = true;
    }

    private Task DeactivateLevelUpPanel()
    {
        Time.timeScale = 1f;
        PauseMenuManager.isPaused = false;
        return levelUpPanel.transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack).OnComplete(() => levelUpPanel.SetActive(false))
            .AsyncWaitForCompletion();
    }
}

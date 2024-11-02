using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// TODO: refactor this to their own controllers
// Fix bug when if level up reward not selected, put the next in Queue
public class MainHudController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private string startScoreText;

    private GameObject levelUpPanel;
    private GameObject upgradeItemsPanel;

    private GameObject buildingPanel;

    private PlayerController playerController;

    private BuildSystem buildSystem;
    private TextMeshProUGUI waveTimeText;

    private GameObject lastSelectedBuildingItem = null;

    [SerializeField] private GameObject[] upgradeGuns;
    [SerializeField] private BuildingData[] buildings;
    [SerializeField] private GameObject upgradeItemUIPrefab;
    [SerializeField] private GameObject buildingItemUIPrefab;


    private GunEnum[] upgradeGunsEnum = { GunEnum.Uzi, GunEnum.Shotgun };

    void Awake()
    {
        scoreText = transform.Find("Score/Text").GetComponent<TextMeshProUGUI>();
        startScoreText = scoreText.text;
        scoreText.text = startScoreText + "0";
        levelUpPanel = transform.Find("LevelUpPanel").gameObject;
        upgradeItemsPanel = levelUpPanel.transform.Find("UpgradeItems").gameObject;
        waveTimeText = transform.Find("WaveTime").GetComponent<TextMeshProUGUI>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        buildingPanel = transform.Find("BuildingItems").gameObject;
        buildSystem = GameObject.Find("BuildSystem").GetComponent<BuildSystem>();
        BuildingEnabledChanged(true);
        // levelUpPanel.SetActive(false);
    }

    void OnEnable()
    {
        buildSystem.BuildingPlacedEvent.AddListener(OnBuildingPlaced);
    }

    void OnDisable()
    {
        buildSystem.BuildingPlacedEvent.RemoveListener(OnBuildingPlaced);
    }

    public void AddScore(int score)
    {
        string currentScoreText = scoreText.text[startScoreText.Length..];
        int currentScore = int.Parse(currentScoreText);
        scoreText.text = startScoreText + (currentScore + score).ToString();
    }

    private void OnBuildingPlaced(BuildingData buildingData)
    {
        AddScore(-buildingData.cost);
    }

    public void OnLevelUp()
    {
        // TODO: don't empty everytime for performance?
        // the panel HAS to be active first because strange things can happen
        levelUpPanel.SetActive(true);
        Utility.DestroyChildren(upgradeItemsPanel);
        DisplayLevelUpItems();
        RenewItems(upgradeItemsPanel, () => {
            OnUpgradeClickSetup();
        });
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
            OnBuildingClickSetup();
        });
    }

    private void DisplayBuildingItems()
    {
        for (int i = 0; i < buildings.Length; ++i)
        {
            GameObject buildingItem = Instantiate(buildingItemUIPrefab, buildingPanel.GetComponent<RectTransform>());
            buildingItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = buildings[i].name;
            buildingItem.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = buildings[i].GetCostString();
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

    private void OnUpgradeClickSetup()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            SetupUpgradeClick(i);
        }
    }

    private void SetupUpgradeClick(int i)
    {
        Button button = upgradeItemsPanel
            .transform.GetChild(i).GetComponentInChildren<Button>();
        GunEnum upgradeGunEnum = upgradeGunsEnum[i];
        button.onClick.AddListener(() => {
            playerController.SelectGun(upgradeGunEnum);
            levelUpPanel.SetActive(false);
        });
    }

    private void OnBuildingClickSetup()
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
        GunEnum upgradeGunEnum = upgradeGunsEnum[i];
        button.onClick.AddListener(() => {
            if (lastSelectedBuildingItem != null)
            {
                lastSelectedBuildingItem.GetComponent<RawImage>().color = Color.white;
                if (buildSystem.currentBuilding.Equals(buildings[i]))
                {
                    buildSystem.currentBuilding = null;
                    lastSelectedBuildingItem = null;
                    return;
                }
            }
            buildSystem.currentBuilding = buildings[i];
            buildingItemUI.GetComponent<RawImage>().color = Color.yellow;
            lastSelectedBuildingItem = buildingItemUI;
        });
    }

    private void DisplayLevelUpItems()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            GameObject upgradeItem = Instantiate(upgradeItemUIPrefab, upgradeItemsPanel.GetComponent<RectTransform>());
            RawImage rawImage = upgradeItem.GetComponentInChildren<RawImage>();
            GameObject upgradeGun = upgradeGuns[i];
            upgradeItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgradeGun.name;

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
}

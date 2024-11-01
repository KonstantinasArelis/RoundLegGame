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

    private PlayerController playerController;
    private TextMeshProUGUI waveTimeText;

    [SerializeField] private GameObject[] upgradeGuns;
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private GameObject upgradeItemPrefab;


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
        // levelUpPanel.SetActive(false);
    }

    public void AddScore(int score)
    {
        string currentScoreText = scoreText.text[startScoreText.Length..];
        int currentScore = int.Parse(currentScoreText);
        scoreText.text = startScoreText + (currentScore + score).ToString();
    }

    public void OnLevelUp()
    {
        // TODO: don't empty everytime for performance?
        levelUpPanel.SetActive(true);
        Utility.DestroyChildren(upgradeItemsPanel);
        DisplayLevelUpItems();
        RenewLevelUpItems(() => {
            OnItemClickSetup();
        });
    }

    private void OnItemClickSetup()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            SetupItemClick(i);
        }
    }

    private void SetupItemClick(int i)
    {
        Button button = upgradeItemsPanel
            .transform.GetChild(i).Find("ItemDisplay/ItemImage").GetComponent<Button>();
        GunEnum upgradeGunEnum = upgradeGunsEnum[i];
        button.onClick.AddListener(() => {
            playerController.SelectGun(upgradeGunEnum);
            levelUpPanel.SetActive(false);
        });
    }

    private void DisplayLevelUpItems()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            GameObject upgradeItem = Instantiate(upgradeItemPrefab, upgradeItemsPanel.GetComponent<RectTransform>());
            RawImage rawImage = upgradeItem.GetComponentInChildren<RawImage>();
            GameObject upgradeGun = upgradeGuns[i];
            upgradeItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgradeGun.name;

            Texture2D thumbnail = AssetPreview.GetAssetPreview(upgradeGun);
            if (thumbnail != null)
            {
                rawImage.texture = thumbnail;
                rawImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                StartCoroutine(Coroutines.WaitForThumbnailCoroutine(upgradeGun, rawImage));
            }
        }
    }

    private void RenewLevelUpItems(Action callback)
    {
        StartCoroutine(
            Coroutines
            .DelayedLayoutRebuildCoroutine(upgradeItemsPanel)
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

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
        EmptyUpgradeChoices();
        DisplayLevelUpItems();
        OnItemClickSetup();
        levelUpPanel.SetActive(true);
    }

    private void OnItemClickSetup()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            Button button = upgradeItemsPanel.transform.GetChild(i).GetComponentInChildren<Button>();
            GameObject prefab = upgradeGuns[i];
            GunEnum gunToSelect = upgradeGunsEnum[i];
            button.onClick.AddListener(() =>
            {
                playerController.SelectGun(gunToSelect);
                levelUpPanel.SetActive(false);
            });
        }
    }

    private void DisplayLevelUpItems()
    {
        for (int i = 0; i < upgradeGuns.Length; ++i)
        {
            GameObject upgradeItem = Instantiate(upgradeItemPrefab, upgradeItemsPanel.GetComponent<RectTransform>());
            RawImage rawImage = upgradeItem.GetComponentInChildren<RawImage>();
            GameObject upgradeGun = upgradeGuns[i];
            upgradeItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgradeGun.name;
            if (upgradeGun == null || rawImage == null)
            {
                Debug.LogError("Prefab or RawImage is not assigned.");
                continue;
            }
                // Use AssetPreview.GetAssetPreview instead
            Texture2D thumbnail = AssetPreview.GetAssetPreview(upgradeGun);

            if (thumbnail != null)
            {
                rawImage.texture = thumbnail;
            }
            else
            {
                Debug.Log("Generating thumbnail, please wait...");
                StartCoroutine(WaitForThumbnail(upgradeGun, rawImage));
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeItemsPanel.GetComponent<RectTransform>());
    }

    private void EmptyUpgradeChoices()
    {
        for (int i = 0; i < upgradeItemsPanel.transform.childCount; ++i)
        {
            print("Destroying " + upgradeItemsPanel.transform.GetChild(i).gameObject.name);
            Destroy(upgradeItemsPanel.transform.GetChild(i).gameObject);
        }
    }

    private System.Collections.IEnumerator WaitForThumbnail(Object prefab, RawImage rawImage)
    {
        Texture2D thumbnail;

        while ((thumbnail = AssetPreview.GetAssetPreview(prefab)) == null)
        {
            yield return null;  // Wait until thumbnail is generated
        }
        rawImage.texture = thumbnail;
        rawImage.color = new Color(1, 1, 1, 1);
        Debug.Log("Thumbnail successfully generated.");
    }

    public void SetWaveTime(int waveTime)
    {
        // TODO: don't do this every 1s
        int minutes = waveTime / 60;
        int seconds = waveTime % 60;
        waveTimeText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }
}

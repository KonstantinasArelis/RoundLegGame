using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// TODO: refactor this to their own controllers
public class MainHudController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private string startScoreText;

    private GameObject levelUpPanel;
    private GameObject levelUpItemPanel;

    private PlayerController playerController;
    private TextMeshProUGUI waveTimeText;

    [SerializeField] private GameObject[] upgradeGuns;
    private GunEnum[] upgradeGunsEnum = { GunEnum.Uzi, GunEnum.Shotgun };

    void Awake()
    {
        scoreText = transform.Find("Score/Text").GetComponent<TextMeshProUGUI>();
        startScoreText = scoreText.text;
        scoreText.text = startScoreText + "0";
        levelUpPanel = transform.Find("LevelUpPanel").gameObject;
        levelUpItemPanel = levelUpPanel.transform.Find("Items").gameObject;
        waveTimeText = transform.Find("WaveTime").GetComponent<TextMeshProUGUI>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        DisplayLevelUpItems();
        OnItemClickSetup();
        levelUpPanel.SetActive(false);
    }

    public void AddScore(int score)
    {
        string currentScoreText = scoreText.text[startScoreText.Length..];
        int currentScore = int.Parse(currentScoreText);
        scoreText.text = startScoreText + (currentScore + score).ToString();
    }

    public void OnLevelUp()
    {
        levelUpPanel.SetActive(true);
    }

    private void OnItemClickSetup()
    {
        for (int i = 0; i < levelUpItemPanel.transform.childCount; ++i)
        {
            Button button = levelUpItemPanel.transform.GetChild(i).GetComponent<Button>();
            GameObject prefab = upgradeGuns[i];
            // need to capture i by value for the enclosing scope
            int index = i;
            button.onClick.AddListener(() =>
            {
                playerController.SelectGun(upgradeGunsEnum[index]);
                levelUpPanel.SetActive(false);
            });
        }
    }

    private void DisplayLevelUpItems()
    {
        for (int i = 0; i < levelUpItemPanel.transform.childCount; ++i)
        {
            RawImage rawImage = levelUpItemPanel.transform.GetChild(i).gameObject.GetComponent<RawImage>();
            GameObject prefab = upgradeGuns[i];
            if (prefab != null && rawImage != null)
            {
                // Use AssetPreview.GetAssetPreview instead
                Texture2D thumbnail = AssetPreview.GetAssetPreview(prefab);

                if (thumbnail != null)
                {
                    rawImage.texture = thumbnail;
                }
                else
                {
                    Debug.Log("Generating thumbnail, please wait...");
                    StartCoroutine(WaitForThumbnail(prefab, rawImage));
                }
            }
            else
            {
                Debug.LogError("Prefab or RawImage is not assigned.");
            }
        }
    }

    private System.Collections.IEnumerator WaitForThumbnail(Object prefab, RawImage rawImage)
    {
        Texture2D thumbnail = null;

        while ((thumbnail = AssetPreview.GetAssetPreview(prefab)) == null)
        {
            yield return null;  // Wait until thumbnail is generated
        }

        rawImage.texture = thumbnail;
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

using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Text title;
    private Text subTitle;

    private string titleText;
    private string subTitleText;

    private RectTransform menuTransform;

    void Start()
    {
        menuTransform = transform.Find("MenuPanel").GetComponent<RectTransform>();
        title = menuTransform.Find("Title").GetComponent<Text>();
        subTitle = menuTransform.Find("SubTitle").GetComponent<Text>();
        titleText = title.text;
        subTitleText = subTitle.text;
        title.text = "";
        subTitle.text = "";
        StartCoroutine(AnimateEntranceCoroutine());
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // Replace "GameScene" with the name of your game scene
    }

    public void OpenSettings()
    {
        // Code to open the settings screen or menu
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game (works only in the built version)
    }

    private IEnumerator AnimateEntranceCoroutine()
    {
        float dropY = 1000f;
        menuTransform.position = new (menuTransform.position.x, menuTransform.position.y + dropY, menuTransform.position.z);
        menuTransform.DOMoveY(menuTransform.position.y - dropY, 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        title.DOText(titleText, 0.5f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);
        subTitle.DOText(subTitleText, 0.5f).SetEase(Ease.Linear);
    }
}

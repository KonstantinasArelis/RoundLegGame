using System.Collections;
using System.Linq;
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

    private SceneFadeController sceneFadeController;

    private readonly float dropY = 1000f;

    void Awake()
    {
        sceneFadeController = GameObject.Find("SceneFade").GetComponent<SceneFadeController>();
    }

    void Start()
    {
        menuTransform = transform.Find("MenuPanel").GetComponent<RectTransform>();
        title = menuTransform.Find("Title").GetComponent<Text>();
        subTitle = menuTransform.Find("SubTitle").GetComponent<Text>();
        titleText = title.text;
        subTitleText = subTitle.text;
        title.text = "";
        subTitle.text = "";
        menuTransform.position = new (menuTransform.position.x, menuTransform.position.y + dropY, menuTransform.position.z);
        StartCoroutine(OnStartCoroutine());
    }

    public void StartGame()
    {
        sceneFadeController.FadeOut(() =>
        {
            SceneManager.LoadScene("MainScene"); // Replace "GameScene" with the name of your game scene
        });
    }

    public void StartTutorial()
    {
        sceneFadeController.FadeOut(() =>
        {
            SceneManager.LoadScene("TutorialScene");
        });
    }

    public void LoadCredits()
    {
        sceneFadeController.FadeOut(() =>
        {
            SceneManager.LoadScene("CreditsScene");
        });
    }

    public void OpenSettings()
    {
        // Code to open the settings screen or menu
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game (works only in the built version)
    }

    private IEnumerator OnStartCoroutine()
    {
        sceneFadeController.FadeIn();
        yield return new WaitForSeconds(1f);
        AnimateEntrance();
        MakeButtonsInteractible(true);
    }

    private void AnimateEntrance()
    {
        MakeButtonsInteractible(false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(         
            menuTransform.DOMoveY(menuTransform.position.y - dropY, 1f).SetEase(Ease.OutBounce)
                .OnComplete(() => GetComponentsInChildren<Button>().ToList()
                    .ForEach(button => button.gameObject.AddComponent<HoverExpandController>()))
        );
        sequence.Append(title.DOText(titleText, 0.5f).SetEase(Ease.Linear));
        sequence.Append(subTitle.DOText(subTitleText, 0.5f).SetEase(Ease.Linear));
        sequence.Play().OnComplete(() => MakeButtonsInteractible(true));
    }

    private void MakeButtonsInteractible(bool state)
    {
        GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = state);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class AutoDarkSoulsMainMenu : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Level1";

    [Header("Audio Optional")]
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioClip backgroundMusic;

    private Canvas canvas;
    private CanvasGroup mainGroup;
    private CanvasGroup settingsGroup;
    private CanvasGroup creditsGroup;
    private CanvasGroup fadeOverlay;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private Color gold = new Color32(201, 162, 74, 255);
    private Color hoverGold = new Color32(255, 211, 106, 255);
    private Color darkPanel = new Color32(15, 13, 10, 230);

    private void Start()
    {
        BuildMenu();
    }

    private void BuildMenu()
    {
        CreateAudio();
        CreateCanvas();
        CreateBackground();

        mainGroup = CreatePanel("Main Menu Panel");
        settingsGroup = CreatePanel("Settings Panel");
        creditsGroup = CreatePanel("Credits Panel");

        CreateMainMenu();
        CreateSettingsMenu();
        CreateCreditsMenu();
        CreateFadeOverlay();

        Show(mainGroup);
        Hide(settingsGroup);
        Hide(creditsGroup);

        StartCoroutine(FadeFromBlack());
    }

    private void CreateAudio()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
            musicSource.Play();
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Generated Dark Souls Main Menu");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventObj = new GameObject("EventSystem");
            eventObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void CreateBackground()
    {
        GameObject bg = CreateUIObject("Abyss Background", canvas.transform);
        Image img = bg.AddComponent<Image>();
        img.color = new Color32(4, 4, 4, 255);
        Stretch(img.rectTransform);

        GameObject mist = CreateUIObject("Dark Red Overlay", canvas.transform);
        Image mistImg = mist.AddComponent<Image>();
        mistImg.color = new Color32(45, 5, 5, 140);
        Stretch(mistImg.rectTransform);
    }

    private CanvasGroup CreatePanel(string name)
    {
        GameObject obj = CreateUIObject(name, canvas.transform);

        Image img = obj.AddComponent<Image>();
        img.color = darkPanel;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(700, 720);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        CanvasGroup group = obj.AddComponent<CanvasGroup>();
        return group;
    }

    private void CreateMainMenu()
    {
        CreateText(mainGroup.transform, "SOULS OF THE ABYSS", 64, new Vector2(0, 240), gold);

        CreateButton(mainGroup.transform, "BEGIN JOURNEY", new Vector2(0, 90), StartGame);
        CreateButton(mainGroup.transform, "SETTINGS", new Vector2(0, 10), () => Switch(mainGroup, settingsGroup));
        CreateButton(mainGroup.transform, "CREDITS", new Vector2(0, -70), () => Switch(mainGroup, creditsGroup));
        CreateButton(mainGroup.transform, "QUIT", new Vector2(0, -150), QuitGame);

        CreateText(mainGroup.transform, "Press Start. Enter the forgotten realm.", 24, new Vector2(0, -285), new Color32(170, 150, 110, 255));
    }

    private void CreateSettingsMenu()
    {
        CreateText(settingsGroup.transform, "SETTINGS", 58, new Vector2(0, 250), gold);

        CreateSlider(settingsGroup.transform, "Music Volume", new Vector2(0, 120), "MusicVolume", 0.6f);
        CreateSlider(settingsGroup.transform, "SFX Volume", new Vector2(0, 20), "SFXVolume", 0.7f);
        CreateSlider(settingsGroup.transform, "Camera Sensitivity", new Vector2(0, -80), "CameraSensitivity", 1f);

        CreateButton(settingsGroup.transform, "BACK", new Vector2(0, -230), () => Switch(settingsGroup, mainGroup));
    }

    private void CreateCreditsMenu()
    {
        CreateText(creditsGroup.transform, "CREDITS", 58, new Vector2(0, 240), gold);

        CreateText(
            creditsGroup.transform,
            "Created by your team\nDark fantasy RPG project\nUnity Main Menu System\nInspired by Souls-like atmosphere",
            30,
            new Vector2(0, 40),
            new Color32(210, 190, 150, 255)
        );

        CreateButton(creditsGroup.transform, "BACK", new Vector2(0, -230), () => Switch(creditsGroup, mainGroup));
    }

    private Button CreateButton(Transform parent, string text, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = CreateUIObject(text + " Button", parent);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color32(25, 21, 16, 240);

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            PlayClick();
            action.Invoke();
        });

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420, 58);
        rt.anchoredPosition = pos;

        TMP_Text label = CreateText(btnObj.transform, text, 28, Vector2.zero, gold);
        label.alignment = TextAlignmentOptions.Center;

        DarkMenuButtonHover hover = btnObj.AddComponent<DarkMenuButtonHover>();
        hover.Setup(label, gold, hoverGold, sfxSource, hoverClip);

        return btn;
    }

    private void CreateSlider(Transform parent, string label, Vector2 pos, string key, float defaultValue)
    {
        CreateText(parent, label, 24, pos + new Vector2(0, 35), gold);

        GameObject sliderObj = CreateUIObject(label + " Slider", parent);
        Slider slider = sliderObj.AddComponent<Slider>();

        RectTransform rt = sliderObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420, 25);
        rt.anchoredPosition = pos;

        GameObject bg = CreateUIObject("Background", sliderObj.transform);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color32(40, 35, 28, 255);
        Stretch(bgImg.rectTransform);

        GameObject fillArea = CreateUIObject("Fill Area", sliderObj.transform);
        RectTransform fillAreaRt = fillArea.GetComponent<RectTransform>();
        Stretch(fillAreaRt);
        fillAreaRt.offsetMin = new Vector2(8, 0);
        fillAreaRt.offsetMax = new Vector2(-8, 0);

        GameObject fill = CreateUIObject("Fill", fillArea.transform);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = hoverGold;
        Stretch(fillImg.rectTransform);

        GameObject handle = CreateUIObject("Handle", sliderObj.transform);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = gold;

        RectTransform handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(26, 36);

        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRt;
        slider.targetGraphic = handleImg;

        slider.minValue = 0f;
        slider.maxValue = key == "CameraSensitivity" ? 3f : 1f;
        slider.value = PlayerPrefs.GetFloat(key, defaultValue);

        slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(key, value);

            if (key == "MusicVolume" && musicSource != null)
                musicSource.volume = value;
        });
    }

    private TMP_Text CreateText(Transform parent, string text, int size, Vector2 pos, Color color)
    {
        GameObject obj = CreateUIObject(text + " Text", parent);

        TMP_Text tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(620, 120);
        rt.anchoredPosition = pos;

        return tmp;
    }

    private void CreateFadeOverlay()
    {
        GameObject obj = CreateUIObject("Fade Overlay", canvas.transform);
        Image img = obj.AddComponent<Image>();
        img.color = Color.black;
        Stretch(img.rectTransform);

        fadeOverlay = obj.AddComponent<CanvasGroup>();
        fadeOverlay.alpha = 1f;
        fadeOverlay.blocksRaycasts = true;
    }

    private IEnumerator FadeFromBlack()
    {
        while (fadeOverlay.alpha > 0f)
        {
            fadeOverlay.alpha -= Time.deltaTime * 1.2f;
            yield return null;
        }

        fadeOverlay.blocksRaycasts = false;
    }

    private void StartGame()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        fadeOverlay.blocksRaycasts = true;

        while (fadeOverlay.alpha < 1f)
        {
            fadeOverlay.alpha += Time.deltaTime * 1.5f;
            yield return null;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    private void Switch(CanvasGroup from, CanvasGroup to)
    {
        Hide(from);
        Show(to);
    }

    private void Show(CanvasGroup group)
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void Hide(CanvasGroup group)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private void PlayClick()
    {
        if (sfxSource != null && clickClip != null)
            sfxSource.PlayOneShot(clickClip, PlayerPrefs.GetFloat("SFXVolume", 0.7f));
    }

    private void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}

public class DarkMenuButtonHover : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
{
    private TMP_Text text;
    private Color normal;
    private Color hover;
    private AudioSource audioSource;
    private AudioClip hoverClip;

    private Vector3 originalScale;
    private Vector3 targetScale;

    public void Setup(TMP_Text label, Color normalColor, Color hoverColor, AudioSource source, AudioClip clip)
    {
        text = label;
        normal = normalColor;
        hover = hoverColor;
        audioSource = source;
        hoverClip = clip;
    }

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * 10f);
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        targetScale = originalScale * 1.08f;

        if (text != null)
            text.color = hover;

        if (audioSource != null && hoverClip != null)
            audioSource.PlayOneShot(hoverClip);
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        targetScale = originalScale;

        if (text != null)
            text.color = normal;
    }
}
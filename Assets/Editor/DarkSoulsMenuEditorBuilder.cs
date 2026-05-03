// ============================================================
// DarkSoulsMenuBuilder.cs
// Tools → Create A+ Dark Souls Main Menu
// Builds the menu in the Unity Editor as real objects.
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
public static class DarkSoulsMenuBuilder
{
    static readonly Color32 BG = new Color32(3, 3, 3, 255);
    static readonly Color32 PANEL = new Color32(8, 7, 6, 225);
    static readonly Color32 GOLD = new Color32(214, 170, 72, 255);
    static readonly Color32 GOLD_DIM = new Color32(128, 96, 38, 190);
    static readonly Color32 BUTTON = new Color32(12, 10, 8, 245);
    static readonly Color32 BUTTON_HOVER = new Color32(34, 25, 12, 255);
    static readonly Color32 RED_DARK = new Color32(45, 6, 4, 120);

    [MenuItem("Tools/Create A+ Dark Souls Main Menu")]
    public static void CreateMenu()
    {
        GameObject old = GameObject.Find("MainMenuCanvas");
        if (old != null)
        {
            if (!EditorUtility.DisplayDialog("Replace Menu?", "MainMenuCanvas already exists. Replace it?", "Replace", "Cancel"))
                return;

            Object.DestroyImmediate(old);
        }

        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        GameObject controller = new GameObject("MainMenuController");
        controller.AddComponent<MainMenuController>();

        BuildBackground(canvasObj.transform);
        BuildMainPanel(canvasObj.transform, controller.GetComponent<MainMenuController>());
        BuildSettingsPanel(canvasObj.transform, controller.GetComponent<MainMenuController>());
        BuildCreditsPanel(canvasObj.transform, controller.GetComponent<MainMenuController>());
        BuildFade(canvasObj.transform, controller.GetComponent<MainMenuController>());

        Selection.activeGameObject = canvasObj;
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create A+ Dark Souls Main Menu");

        Debug.Log("A+ Dark Souls Main Menu created. Connect gameSceneName in MainMenuController if needed.");
    }

    static void BuildBackground(Transform parent)
    {
        GameObject bg = Rect("Abyss Background", parent);
        Stretch(bg);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = BG;
        bgImg.raycastTarget = false;

        GameObject red = Rect("Blood Mist Overlay", parent);
        Stretch(red);
        Image redImg = red.AddComponent<Image>();
        redImg.color = RED_DARK;
        redImg.raycastTarget = false;

        GameObject vignette = Rect("Heavy Vignette", parent);
        Stretch(vignette);
        Image vig = vignette.AddComponent<Image>();
        vig.color = new Color32(0, 0, 0, 115);
        vig.raycastTarget = false;

        GameObject embers = Rect("Procedural Embers", parent);
        Stretch(embers);
        Image eImg = embers.AddComponent<Image>();
        eImg.color = Color.clear;
        eImg.raycastTarget = false;
        EmberSystem emberSystem = embers.AddComponent<EmberSystem>();
        emberSystem.particleCount = 75;

        GameObject scan = Rect("Subtle Scanlines", parent);
        Stretch(scan);
        Image sImg = scan.AddComponent<Image>();
        sImg.color = Color.clear;
        sImg.raycastTarget = false;
        scan.AddComponent<ScanlineOverlay>();
    }

    static void BuildMainPanel(Transform parent, MainMenuController controller)
    {
        GameObject panel = Rect("Main Panel", parent);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(820, 860);
        rt.anchoredPosition = new Vector2(-260, 0);

        Image img = panel.AddComponent<Image>();
        img.color = PANEL;
        img.raycastTarget = false;

        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        controller.mainPanel = group;

        Rule(panel.transform, "Top Gold Line", new Vector2(0, 378), new Vector2(720, 2));
        Rule(panel.transform, "Bottom Gold Line", new Vector2(0, -378), new Vector2(720, 2));
        Rule(panel.transform, "Title Separator", new Vector2(0, 205), new Vector2(440, 1));

        Corner(panel.transform, new Vector2(-365, 380));
        Corner(panel.transform, new Vector2(365, 380));
        Corner(panel.transform, new Vector2(-365, -380));
        Corner(panel.transform, new Vector2(365, -380));

        TMP_Text title = Text(panel.transform, "BONECRYPT", 82, new Vector2(0, 292), GOLD);
        title.gameObject.AddComponent<FlickerTitle>();

        Text(panel.transform, "— souls do not rest here —", 24, new Vector2(0, 232), GOLD_DIM);

        Button begin = Button(panel.transform, "BEGIN JOURNEY", new Vector2(0, 105));
        begin.onClick.AddListener(controller.StartGame);

        Button load = Button(panel.transform, "LOAD GAME", new Vector2(0, 25));
        load.onClick.AddListener(controller.StartGame);

        Button settings = Button(panel.transform, "SETTINGS", new Vector2(0, -55));
        settings.onClick.AddListener(controller.OpenSettings);

        Button credits = Button(panel.transform, "CREDITS", new Vector2(0, -135));
        credits.onClick.AddListener(controller.OpenCredits);

        Button quit = Button(panel.transform, "QUIT", new Vector2(0, -215));
        quit.onClick.AddListener(controller.QuitGame);

        Text(panel.transform, "v1.0.0  |  Unity RPG Project", 18, new Vector2(0, -340), GOLD_DIM);
    }

    static void BuildSettingsPanel(Transform parent, MainMenuController controller)
    {
        GameObject panel = Rect("Settings Panel", parent);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(760, 760);
        rt.anchoredPosition = new Vector2(-260, 0);

        Image img = panel.AddComponent<Image>();
        img.color = PANEL;

        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        controller.settingsPanel = group;

        Text(panel.transform, "SETTINGS", 68, new Vector2(0, 270), GOLD);
        Rule(panel.transform, "Settings Line", new Vector2(0, 210), new Vector2(440, 1));

        Slider(panel.transform, "MUSIC VOLUME", new Vector2(0, 105), "MusicVolume", 0.7f, 1f);
        Slider(panel.transform, "SFX VOLUME", new Vector2(0, 0), "SFXVolume", 0.7f, 1f);
        Slider(panel.transform, "CAMERA SENSITIVITY", new Vector2(0, -105), "CameraSensitivity", 1f, 3f);

        Button back = Button(panel.transform, "BACK", new Vector2(0, -260));
        back.onClick.AddListener(controller.BackToMain);
    }

    static void BuildCreditsPanel(Transform parent, MainMenuController controller)
    {
        GameObject panel = Rect("Credits Panel", parent);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(760, 760);
        rt.anchoredPosition = new Vector2(-260, 0);

        Image img = panel.AddComponent<Image>();
        img.color = PANEL;

        CanvasGroup group = panel.AddComponent<CanvasGroup>();
        controller.creditsPanel = group;

        Text(panel.transform, "CREDITS", 68, new Vector2(0, 260), GOLD);
        Rule(panel.transform, "Credits Line", new Vector2(0, 200), new Vector2(440, 1));

        Text(panel.transform,
            "BONECRYPT\n\nDark Fantasy RPG Project\nMain Menu • UI System • Scene Flow\n\nCreated in Unity\nInspired by Souls-like atmosphere",
            30,
            new Vector2(0, 20),
            new Color32(205, 180, 125, 255)
        );

        Button back = Button(panel.transform, "BACK", new Vector2(0, -280));
        back.onClick.AddListener(controller.BackToMain);
    }

    static void BuildFade(Transform parent, MainMenuController controller)
    {
        GameObject fade = Rect("Fade Overlay", parent);
        Stretch(fade);
        Image img = fade.AddComponent<Image>();
        img.color = Color.black;

        CanvasGroup group = fade.AddComponent<CanvasGroup>();
        group.alpha = 1;
        group.blocksRaycasts = true;

        controller.fadeOverlay = group;
    }

    static Button Button(Transform parent, string label, Vector2 pos)
    {
        GameObject obj = Rect(label + " Button", parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 62);
        rt.anchoredPosition = pos;

        Image img = obj.AddComponent<Image>();
        img.color = BUTTON;

        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock colors = btn.colors;
        colors.normalColor = BUTTON;
        colors.highlightedColor = BUTTON_HOVER;
        colors.pressedColor = new Color32(70, 52, 25, 255);
        colors.selectedColor = BUTTON_HOVER;
        colors.fadeDuration = 0.12f;
        btn.colors = colors;

        Border(obj.transform, new Vector2(500, 62));
        Text(obj.transform, label, 28, Vector2.zero, GOLD);

        DarkSoulsButtonFX fx = obj.AddComponent<DarkSoulsButtonFX>();
        fx.borderColor = GOLD_DIM;
        fx.hoverColor = GOLD;

        return btn;
    }

    static void Slider(Transform parent, string label, Vector2 pos, string key, float defaultValue, float maxValue)
    {
        Text(parent, label, 24, pos + new Vector2(0, 42), GOLD);

        GameObject obj = Rect(label + " Slider", parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(470, 28);
        rt.anchoredPosition = pos;

        Slider slider = obj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = maxValue;
        slider.value = PlayerPrefs.GetFloat(key, defaultValue);

        GameObject bg = Rect("Background", obj.transform);
        Stretch(bg);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color32(25, 21, 14, 255);

        GameObject fillArea = Rect("Fill Area", obj.transform);
        Stretch(fillArea);

        GameObject fill = Rect("Fill", fillArea.transform);
        Stretch(fill);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = GOLD;

        GameObject handle = Rect("Handle", obj.transform);
        RectTransform hrt = handle.GetComponent<RectTransform>();
        hrt.sizeDelta = new Vector2(24, 36);
        Image hImg = handle.AddComponent<Image>();
        hImg.color = new Color32(240, 205, 115, 255);

        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = hrt;
        slider.targetGraphic = hImg;

        slider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(key, v));
    }

    static TMP_Text Text(Transform parent, string value, int size, Vector2 pos, Color32 color)
    {
        GameObject obj = Rect(value + " Text", parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(720, 110);
        rt.anchoredPosition = pos;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = value;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;

        TMP_FontAsset font = FindCinzelFont();
        if (font != null)
            tmp.font = font;

        return tmp;
    }

    static TMP_FontAsset FindCinzelFont()
    {
        string[] guids = AssetDatabase.FindAssets("Cinzel t:TMP_FontAsset");
        if (guids.Length == 0)
            guids = AssetDatabase.FindAssets("CinzelDecorative t:TMP_FontAsset");

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        }

        return null;
    }

    static void Border(Transform parent, Vector2 size)
    {
        Rule(parent, "Border Top", new Vector2(0, size.y / 2), new Vector2(size.x, 1));
        Rule(parent, "Border Bottom", new Vector2(0, -size.y / 2), new Vector2(size.x, 1));
        Rule(parent, "Border Left", new Vector2(-size.x / 2, 0), new Vector2(1, size.y));
        Rule(parent, "Border Right", new Vector2(size.x / 2, 0), new Vector2(1, size.y));
    }

    static void Rule(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject obj = Rect(name, parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = obj.AddComponent<Image>();
        img.color = GOLD_DIM;
        img.raycastTarget = false;
    }

    static void Corner(Transform parent, Vector2 pos)
    {
        GameObject obj = Rect("Gold Corner", parent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(12, 12);

        Image img = obj.AddComponent<Image>();
        img.color = GOLD;
        img.raycastTarget = false;
    }

    static GameObject Rect(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    static void Stretch(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "SampleScene";

    [Header("Panels")]
    public CanvasGroup mainPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup creditsPanel;
    public CanvasGroup fadeOverlay;

    private void Start()
    {
        Time.timeScale = 1f;

        Show(mainPanel);
        Hide(settingsPanel);
        Hide(creditsPanel);

        StartCoroutine(FadeIn());
    }

    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    public void OpenSettings()
    {
        Switch(mainPanel, settingsPanel);
    }

    public void OpenCredits()
    {
        Switch(mainPanel, creditsPanel);
    }

    public void BackToMain()
    {
        Switch(settingsPanel, mainPanel);
        Switch(creditsPanel, mainPanel);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.alpha = 1;
        fadeOverlay.blocksRaycasts = true;

        while (fadeOverlay.alpha > 0)
        {
            fadeOverlay.alpha -= Time.deltaTime * 1.2f;
            yield return null;
        }

        fadeOverlay.alpha = 0;
        fadeOverlay.blocksRaycasts = false;
    }

    IEnumerator LoadGame()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.blocksRaycasts = true;

            while (fadeOverlay.alpha < 1)
            {
                fadeOverlay.alpha += Time.deltaTime * 1.6f;
                yield return null;
            }
        }

        SceneManager.LoadScene(gameSceneName);
    }

    void Switch(CanvasGroup from, CanvasGroup to)
    {
        if (from != null) Hide(from);
        if (to != null) Show(to);
    }

    void Show(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    void Hide(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}

public class DarkSoulsButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color32 borderColor = new Color32(120, 90, 40, 255);
    public Color32 hoverColor = new Color32(230, 185, 85, 255);
    public float pulseSpeed = 4f;

    private TextMeshProUGUI label;
    private Image[] borders;
    private bool hovering;
    private float pulse;

    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();

        List<Image> list = new List<Image>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Border"))
                list.Add(child.GetComponent<Image>());
        }

        borders = list.ToArray();
    }

    private void Update()
    {
        if (!hovering || label == null) return;

        pulse += Time.unscaledDeltaTime * pulseSpeed;
        float t = (Mathf.Sin(pulse) + 1f) * 0.5f;

        label.transform.localScale = Vector3.one * (1f + t * 0.035f);

        foreach (Image border in borders)
        {
            if (border != null)
                border.color = Color.Lerp(borderColor, hoverColor, t);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        pulse = 0;

        if (label != null)
            label.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;

        if (label != null)
        {
            label.color = hoverColor;
            label.transform.localScale = Vector3.one;
        }

        foreach (Image border in borders)
        {
            if (border != null)
                border.color = borderColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (label != null)
            StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        Color original = label.color;
        label.color = new Color32(255, 240, 175, 255);
        yield return new WaitForSecondsRealtime(0.07f);
        label.color = original;
    }
}

public class FlickerTitle : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float timer;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (text == null) return;

        timer += Time.unscaledDeltaTime;

        if (timer > 0.08f)
        {
            timer = 0;

            if (Random.value < 0.08f)
            {
                Color c = text.color;
                c.a = Random.Range(0.75f, 1f);
                text.color = c;
            }
            else
            {
                Color c = text.color;
                c.a = 1f;
                text.color = c;
            }
        }
    }
}

public class ScanlineOverlay : MonoBehaviour
{
    public int lineSpacing = 5;
    public float scrollSpeed = 8f;

    private readonly List<RectTransform> lines = new List<RectTransform>();
    private RectTransform rect;
    private float offset;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        for (int i = 0; i < 260; i++)
        {
            GameObject line = new GameObject("Scanline");
            line.transform.SetParent(transform, false);

            RectTransform rt = line.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.sizeDelta = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(0, i * lineSpacing);

            Image img = line.AddComponent<Image>();
            img.color = new Color32(0, 0, 0, 20);
            img.raycastTarget = false;

            lines.Add(rt);
        }
    }

    private void Update()
    {
        offset = (offset + Time.unscaledDeltaTime * scrollSpeed) % lineSpacing;

        float height = rect != null ? rect.rect.height : 1080;

        for (int i = 0; i < lines.Count; i++)
        {
            float y = (i * lineSpacing - offset) % height;
            lines[i].anchoredPosition = new Vector2(0, y);
        }
    }
}

public class EmberSystem : MonoBehaviour
{
    public int particleCount = 75;

    private RectTransform rect;
    private readonly List<RectTransform> embers = new List<RectTransform>();

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        for (int i = 0; i < particleCount; i++)
            CreateEmber(Random.Range(-540f, 540f));
    }

    private void Update()
    {
        foreach (RectTransform ember in embers)
        {
            if (ember == null) continue;

            ember.anchoredPosition += new Vector2(
                Mathf.Sin(Time.unscaledTime * 2f + ember.GetInstanceID()) * 0.12f,
                Time.unscaledDeltaTime * Random.Range(15f, 35f)
            );

            if (ember.anchoredPosition.y > 560)
                ember.anchoredPosition = new Vector2(Random.Range(-900f, 900f), -560f);
        }
    }

    void CreateEmber(float startY)
    {
        GameObject obj = new GameObject("Ember");
        obj.transform.SetParent(transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = Vector2.one * Random.Range(2f, 5f);
        rt.anchoredPosition = new Vector2(Random.Range(-900f, 900f), startY);

        Image img = obj.AddComponent<Image>();
        img.color = new Color32(255, (byte)Random.Range(80, 170), 20, (byte)Random.Range(120, 210));
        img.raycastTarget = false;

        embers.Add(rt);
    }
}
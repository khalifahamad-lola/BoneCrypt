using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class DarkFantasyMainMenuBuilder : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "SampleScene";

    [Header("Images")]
    public Sprite backgroundImage;
    public Sprite selectionArrow;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip menuMusic;

    [Header("Text")]
    public string gameTitle = "BONE CRYPT";
    public string subtitle = "RISE FROM THE DARKNESS";

    private Canvas canvas;
    private RectTransform selectionGlow;
    private CanvasGroup fadeGroup;
    private CanvasGroup creditsGroup;
    private Coroutine glowMoveRoutine;

    private readonly string[] menuItems =
    {
        "New Game",
        "Credits",
        "Exit"
    };

    private void Start()
    {
        BuildMenu();
        PlayMusic();
        StartCoroutine(FadeFromBlack());
    }

    private void BuildMenu()
    {
        CreateEventSystem();
        CreateCanvas();
        CreateBackground();
        CreateSoftLeftFade();
        CreateTitle();
        CreateMenuButtons();
        CreateFooter();
        CreateCreditsPanel();
        CreateFadePanel();
    }

    private void PlayMusic()
    {
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.volume = 0.35f;
            audioSource.Play();
        }
    }

    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas_MainMenu");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void CreateBackground()
    {
        GameObject obj = CreateUIObject("Background_Image", canvas.transform);
        Image img = obj.AddComponent<Image>();

        if (backgroundImage != null)
            img.sprite = backgroundImage;

        img.color = Color.white;
        img.preserveAspect = false;
        Stretch(obj.GetComponent<RectTransform>());
    }

    private void CreateSoftLeftFade()
    {
        GameObject leftDark = CreateUIObject("Left_Dark_Area", canvas.transform);
        Image leftImg = leftDark.AddComponent<Image>();
        leftImg.color = new Color(0f, 0f, 0f, 0.85f);

        RectTransform leftRt = leftDark.GetComponent<RectTransform>();
        leftRt.anchorMin = new Vector2(0f, 0f);
        leftRt.anchorMax = new Vector2(0.38f, 1f);
        leftRt.offsetMin = Vector2.zero;
        leftRt.offsetMax = Vector2.zero;

        int strips = 24;

        for (int i = 0; i < strips; i++)
        {
            float t = i / (float)(strips - 1);
            float alpha = Mathf.Lerp(0.45f, 0f, t);

            GameObject strip = CreateUIObject("Fade_Strip_" + i, canvas.transform);
            Image img = strip.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, alpha);

            RectTransform rt = strip.GetComponent<RectTransform>();
            float start = Mathf.Lerp(0.34f, 0.62f, t);
            float end = Mathf.Lerp(0.37f, 0.65f, t);

            rt.anchorMin = new Vector2(start, 0f);
            rt.anchorMax = new Vector2(end, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }

    private void CreateTitle()
    {
        TextMeshProUGUI title = CreateText("Game_Title", canvas.transform, gameTitle, 88, new Color32(216, 177, 90, 255));
        RectTransform rt = title.GetComponent<RectTransform>();

        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(105, -120);
        rt.sizeDelta = new Vector2(850, 120);

        title.alignment = TextAlignmentOptions.Left;
        title.fontStyle = FontStyles.SmallCaps | FontStyles.Bold;
        title.characterSpacing = 2;
        title.outlineWidth = 0.12f;
        title.outlineColor = new Color32(30, 20, 8, 255);

        title.enableVertexGradient = true;
        title.colorGradient = new VertexGradient(
            new Color32(255, 230, 160, 255),
            new Color32(230, 180, 90, 255),
            new Color32(120, 80, 30, 255),
            new Color32(185, 130, 50, 255)
        );

        TextMeshProUGUI sub = CreateText("Subtitle", canvas.transform, subtitle, 28, new Color32(205, 195, 170, 255));
        RectTransform srt = sub.GetComponent<RectTransform>();

        srt.anchorMin = new Vector2(0f, 1f);
        srt.anchorMax = new Vector2(0f, 1f);
        srt.pivot = new Vector2(0f, 1f);
        srt.anchoredPosition = new Vector2(215, -225);
        srt.sizeDelta = new Vector2(650, 50);

        sub.characterSpacing = 8;
        sub.alignment = TextAlignmentOptions.Left;
        sub.fontStyle = FontStyles.SmallCaps;
        sub.outlineWidth = 0.08f;
        sub.outlineColor = new Color32(20, 15, 10, 255);
    }

    private void CreateMenuButtons()
    {
        GameObject panel = CreateUIObject("MenuPanel", canvas.transform);
        RectTransform prt = panel.GetComponent<RectTransform>();

        prt.anchorMin = new Vector2(0f, 0.5f);
        prt.anchorMax = new Vector2(0f, 0.5f);
        prt.pivot = new Vector2(0f, 0.5f);
        prt.anchoredPosition = new Vector2(135, -20);
        prt.sizeDelta = new Vector2(600, 320);

        selectionGlow = CreateGlow(panel.transform);

        for (int i = 0; i < menuItems.Length; i++)
        {
            string itemName = menuItems[i];

            GameObject btnObj = CreateUIObject(itemName + "_Button", panel.transform);
            Image bg = btnObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0f);

            Button btn = btnObj.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(0, -i * 85);
            rt.sizeDelta = new Vector2(520, 68);

            TextMeshProUGUI txt = CreateText(itemName + "_Text", btnObj.transform, itemName, 42, new Color32(215, 210, 195, 255));
            RectTransform trt = txt.GetComponent<RectTransform>();
            Stretch(trt);
            trt.offsetMin = new Vector2(90, 0);
            trt.offsetMax = Vector2.zero;

            txt.alignment = TextAlignmentOptions.MidlineLeft;
            txt.fontStyle = FontStyles.SmallCaps;
            txt.characterSpacing = 4;
            txt.outlineWidth = 0.08f;
            txt.outlineColor = new Color32(10, 8, 5, 255);

            int index = i;

            EventTrigger trigger = btnObj.AddComponent<EventTrigger>();

            AddEvent(trigger, EventTriggerType.PointerEnter, () =>
            {
                MoveGlow(rt);
                txt.color = new Color32(255, 205, 95, 255);
                txt.fontStyle = FontStyles.SmallCaps | FontStyles.Bold;
                StartCoroutine(ScaleButton(btnObj.transform, new Vector3(1.05f, 1.05f, 1f)));
                PlaySound(hoverSound);
            });

            AddEvent(trigger, EventTriggerType.PointerExit, () =>
            {
                txt.color = new Color32(215, 210, 195, 255);
                txt.fontStyle = FontStyles.SmallCaps;
                StartCoroutine(ScaleButton(btnObj.transform, Vector3.one));
            });

            btn.onClick.AddListener(() =>
            {
                PlaySound(clickSound);
                HandleMenuClick(index);
            });
        }
    }

    private RectTransform CreateGlow(Transform parent)
    {
        GameObject glow = CreateUIObject("Selection_Glow", parent);
        Image img = glow.AddComponent<Image>();
        img.color = new Color(1f, 0.65f, 0.18f, 0.03f);

        RectTransform rt = glow.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(500, 50);

        GameObject arrow = CreateUIObject("Selection_Arrow", glow.transform);
        Image arrowImg = arrow.AddComponent<Image>();

        if (selectionArrow != null)
            arrowImg.sprite = selectionArrow;

        arrowImg.color = new Color32(255, 200, 80, 255);
        arrowImg.preserveAspect = true;
        arrowImg.raycastTarget = false;

        RectTransform art = arrow.GetComponent<RectTransform>();
        art.anchorMin = new Vector2(0f, 0.5f);
        art.anchorMax = new Vector2(0f, 0.5f);
        art.pivot = new Vector2(0.5f, 0.5f);
        art.anchoredPosition = new Vector2(22, 0);
        art.sizeDelta = new Vector2(78, 78);

        GameObject line = CreateUIObject("Selection_Line", glow.transform);
        Image lineImg = line.AddComponent<Image>();
        lineImg.color = new Color(1f, 0.72f, 0.25f, 0.18f);

        RectTransform lrt = line.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0f, 0.5f);
        lrt.anchorMax = new Vector2(1f, 0.5f);
        lrt.pivot = new Vector2(0f, 0.5f);
        lrt.anchoredPosition = new Vector2(85, -22);
        lrt.sizeDelta = new Vector2(-90, 1.5f);

        return rt;
    }

    private void HandleMenuClick(int index)
    {
        string item = menuItems[index];

        if (item == "New Game")
            StartCoroutine(LoadGameScene());

        else if (item == "Credits")
            ShowCreditsPanel();

        else if (item == "Exit")
            QuitGame();
    }

    private void CreateCreditsPanel()
    {
        GameObject panel = CreateUIObject("CreditsPanel", canvas.transform);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.86f);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(760, 560);

        creditsGroup = panel.AddComponent<CanvasGroup>();
        creditsGroup.alpha = 0f;
        creditsGroup.interactable = false;
        creditsGroup.blocksRaycasts = false;

        TextMeshProUGUI title = CreateText("Credits_Title", panel.transform, "CREDITS", 62, new Color32(230, 180, 90, 255));
        RectTransform titleRt = title.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 1f);
        titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0, -55);
        titleRt.sizeDelta = new Vector2(650, 90);
        title.alignment = TextAlignmentOptions.Center;
        title.fontStyle = FontStyles.SmallCaps | FontStyles.Bold;

        TextMeshProUGUI body = CreateText(
            "Credits_Body",
            panel.transform,
            "BONE CRYPT\n\nCreated by:\n---- \n\nCourse: IT8101 Game Development\n\nMain Menu System\nScene Transition • Credits Panel • Exit Function",
            30,
            new Color32(220, 215, 200, 255)
        );

        RectTransform bodyRt = body.GetComponent<RectTransform>();
        bodyRt.anchorMin = new Vector2(0.5f, 0.5f);
        bodyRt.anchorMax = new Vector2(0.5f, 0.5f);
        bodyRt.pivot = new Vector2(0.5f, 0.5f);
        bodyRt.anchoredPosition = new Vector2(0, -20);
        bodyRt.sizeDelta = new Vector2(650, 300);
        body.alignment = TextAlignmentOptions.Center;

        GameObject backObj = CreateUIObject("Credits_Back_Button", panel.transform);
        Image backBg = backObj.AddComponent<Image>();
        backBg.color = new Color(0.35f, 0.22f, 0.05f, 0.65f);

        Button backBtn = backObj.AddComponent<Button>();
        backBtn.transition = Selectable.Transition.None;

        RectTransform backRt = backObj.GetComponent<RectTransform>();
        backRt.anchorMin = new Vector2(0.5f, 0f);
        backRt.anchorMax = new Vector2(0.5f, 0f);
        backRt.pivot = new Vector2(0.5f, 0f);
        backRt.anchoredPosition = new Vector2(0, 45);
        backRt.sizeDelta = new Vector2(260, 65);

        TextMeshProUGUI backTxt = CreateText("Credits_Back_Text", backObj.transform, "BACK", 34, new Color32(230, 220, 190, 255));
        Stretch(backTxt.GetComponent<RectTransform>());
        backTxt.alignment = TextAlignmentOptions.Center;
        backTxt.fontStyle = FontStyles.SmallCaps | FontStyles.Bold;

        backBtn.onClick.AddListener(HideCreditsPanel);
    }

    private void ShowCreditsPanel()
    {
        StartCoroutine(FadeCanvasGroup(creditsGroup, true));
    }

    private void HideCreditsPanel()
    {
        StartCoroutine(FadeCanvasGroup(creditsGroup, false));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, bool show)
    {
        group.blocksRaycasts = show;
        group.interactable = show;

        float start = group.alpha;
        float end = show ? 1f : 0f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 7f;
            group.alpha = Mathf.Lerp(start, end, EaseOut(t));
            yield return null;
        }

        group.alpha = end;
    }

    private void CreateFooter()
    {
        TextMeshProUGUI version = CreateText("Version_Text", canvas.transform, "v1.0.0", 20, new Color32(190, 190, 180, 255));
        RectTransform vrt = version.GetComponent<RectTransform>();
        vrt.anchorMin = new Vector2(0f, 0f);
        vrt.anchorMax = new Vector2(0f, 0f);
        vrt.pivot = new Vector2(0f, 0f);
        vrt.anchoredPosition = new Vector2(55, 35);
        vrt.sizeDelta = new Vector2(250, 40);
        version.alignment = TextAlignmentOptions.Left;

        TextMeshProUGUI footer = CreateText("Footer_Text", canvas.transform, "© 2026 Bone Crypt Team. All Rights Reserved.", 20, new Color32(190, 185, 175, 255));
        RectTransform frt = footer.GetComponent<RectTransform>();
        frt.anchorMin = new Vector2(1f, 0f);
        frt.anchorMax = new Vector2(1f, 0f);
        frt.pivot = new Vector2(1f, 0f);
        frt.anchoredPosition = new Vector2(-250, 35);
        frt.sizeDelta = new Vector2(700, 40);
        footer.alignment = TextAlignmentOptions.Right;
    }

    private void CreateFadePanel()
    {
        GameObject obj = CreateUIObject("FadePanel", canvas.transform);
        Image img = obj.AddComponent<Image>();
        img.color = Color.black;

        fadeGroup = obj.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 1f;
        fadeGroup.blocksRaycasts = true;

        Stretch(obj.GetComponent<RectTransform>());
    }

    private IEnumerator LoadGameScene()
    {
        yield return FadeToBlack();
        SceneManager.LoadScene(gameSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Exit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator FadeFromBlack()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * 0.8f;
            fadeGroup.alpha = t;
            yield return null;
        }

        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeToBlack()
    {
        fadeGroup.blocksRaycasts = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 1.2f;
            fadeGroup.alpha = t;
            yield return null;
        }

        fadeGroup.alpha = 1f;
    }

    private void MoveGlow(RectTransform target)
    {
        if (glowMoveRoutine != null)
            StopCoroutine(glowMoveRoutine);

        glowMoveRoutine = StartCoroutine(MoveGlowSmooth(target.anchoredPosition));
    }

    private IEnumerator MoveGlowSmooth(Vector2 targetPos)
    {
        Vector2 start = selectionGlow.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 12f;
            selectionGlow.anchoredPosition = Vector2.Lerp(start, targetPos, EaseOut(t));
            yield return null;
        }

        selectionGlow.anchoredPosition = targetPos;
    }

    private IEnumerator ScaleButton(Transform target, Vector3 targetScale)
    {
        Vector3 start = target.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 10f;
            target.localScale = Vector3.Lerp(start, targetScale, EaseOut(t));
            yield return null;
        }

        target.localScale = targetScale;
    }

    private float EaseOut(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, Color color)
    {
        GameObject obj = CreateUIObject(name, parent);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.raycastTarget = false;
        return tmp;
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

    private void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void CreateEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
    }
}
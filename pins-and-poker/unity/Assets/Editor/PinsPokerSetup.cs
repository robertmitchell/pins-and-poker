using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UIAnimatrix;

/// <summary>
/// Run individual steps from Tools -> Pins & Poker Setup, or run them all at once.
/// Open the Gameplay scene before running any command.
/// </summary>
public static class PinsPokerSetup
{
    const string MENU = "Tools/Pins & Poker Setup/";

    // ─────────────────────────────────────────────────────────────────────────
    // RUN ALL
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "Run All Setup")]
    public static void RunAll()
    {
        CreateInfoScreens();
        AddAgreementToggles();
        AddProfileStats();
        AddLeagueFields();
        CreateDisputeFormScreen();
        AddHomeInfoButtons();
        AddMyLeaguesDescription();
        AddStartDateToLeaguePrefab();
        MarkDirty();
        Debug.Log("[PinsPoker] ✓ All setup complete. Save the scene (Ctrl+S) and check Console for any warnings.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 1. INFO SCREENS
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "1 - Create Info Screens")]
    public static void CreateInfoScreens()
    {
        var template = FindInScene("Privacy Policy Screen");
        if (template == null) { Debug.LogError("[PinsPoker] Cannot find 'Privacy Policy Screen' to use as template."); return; }

        Transform parent = template.transform.parent;

        BuildInfoScreen<HowItWorksScreen>(template, parent, "How It Works Screen");
        BuildInfoScreen<CapabilityListScreen>(template, parent, "Capability List Screen");
        BuildInfoScreen<HandRankingsScreen>(template, parent, "Hand Rankings Screen");
        BuildInfoScreen<HowToCreateGameScreen>(template, parent, "How To Create Game Screen");
        BuildInfoScreen<ButtonGuideScreen>(template, parent, "Button Guide Screen");

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ Info screens created. UIManager will auto-register them at runtime.");
    }

    static void BuildInfoScreen<T>(GameObject template, Transform parent, string screenName) where T : UIScreenBase
    {
        if (FindInScene(screenName) != null)
        {
            Debug.LogWarning($"[PinsPoker] '{screenName}' already exists — skipped.");
            return;
        }

        GameObject copy = Object.Instantiate(template, parent);
        Undo.RegisterCreatedObjectUndo(copy, $"Create {screenName}");
        copy.name = screenName;

        // Harvest references from the old script before removing it
        var old = copy.GetComponent<PrivacyPolicyScreen>();
        AnimatrixButton backBtn        = old != null ? old.backBtn          : null;
        TMP_Text        contentTxt     = old != null ? old.contentTxt       : null;
        ScrollRect      scrollRect     = old != null ? old.contentScrollRect : null;

        if (old != null) Undo.DestroyObjectImmediate(old);

        var newScript = Undo.AddComponent<T>(copy);

        var so = new SerializedObject(newScript);
        SetObjRef(so, "backBtn",          backBtn);
        SetObjRef(so, "contentTxt",       contentTxt);
        SetObjRef(so, "contentScrollRect", scrollRect);
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 2. AGREEMENT TOGGLES (Privacy Policy + Terms & Conditions)
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "2 - Add Agreement Toggles")]
    public static void AddAgreementToggles()
    {
        AddTogglesToScreen("Privacy Policy Screen",       typeof(PrivacyPolicyScreen));
        AddTogglesToScreen("Terms And Conditions Screen", typeof(TermsAndConditionsScreen));
        MarkDirty();
        Debug.Log("[PinsPoker] ✓ Agreement toggles added to Privacy Policy and Terms & Conditions screens.");
    }

    static void AddTogglesToScreen(string screenName, System.Type scriptType)
    {
        var screen = FindInScene(screenName);
        if (screen == null) { Debug.LogError($"[PinsPoker] Cannot find '{screenName}'."); return; }

        const float AGREEMENT_HEIGHT = 150f;

        // Find the ScrollRect — the Agreement Panel must be a sibling so they share a coordinate space
        var scrollRect = screen.GetComponentInChildren<ScrollRect>(true);
        Transform panelParent = scrollRect != null ? scrollRect.transform.parent : screen.transform;

        // Pull the scroll rect's bottom edge up to make room
        if (scrollRect != null)
        {
            var srt = scrollRect.GetComponent<RectTransform>();
            srt.offsetMin = new Vector2(srt.offsetMin.x, AGREEMENT_HEIGHT);
        }

        // Agreement Panel — sibling of scroll rect, pinned to the bottom of that same parent
        var existingPanel = panelParent.Find("Agreement Panel");
        Transform agreementPanel;
        if (existingPanel == null)
        {
            var panel = new GameObject("Agreement Panel", typeof(RectTransform), typeof(VerticalLayoutGroup));
            Undo.RegisterCreatedObjectUndo(panel, "Create Agreement Panel");
            GameObjectUtility.SetParentAndAlign(panel, panelParent.gameObject);

            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 0f);
            rt.anchorMax        = new Vector2(1f, 0f);
            rt.pivot            = new Vector2(0.5f, 0f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta        = new Vector2(0f, AGREEMENT_HEIGHT);

            // Center children, don't stretch them — lets items stay naturally sized and centred
            var vlg = panel.GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment         = TextAnchor.MiddleCenter;
            vlg.spacing                = 14f;
            vlg.padding                = new RectOffset(20, 20, 14, 14);
            vlg.childControlHeight     = false;
            vlg.childControlWidth      = false;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth  = false;

            agreementPanel = panel.transform;
        }
        else
        {
            agreementPanel = existingPanel;
        }

        // Toggle — built manually so the checkmark graphic is explicitly wired
        Toggle toggle = null;
        var existingToggle = agreementPanel.Find("Agree Toggle");
        if (existingToggle == null)
            toggle = BuildToggle(agreementPanel, "Agree Toggle", "I have read and understand the above.");
        else
            toggle = existingToggle.GetComponent<Toggle>();

        // Agree Button
        AnimatrixButton agreeBtn = null;
        var existingBtn = agreementPanel.Find("Agree Button");
        if (existingBtn == null)
        {
            var btnGO = CreateAnimatrixButton(agreementPanel, "Agree Button", "I Agree");
            btnGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 46f);
            agreeBtn = btnGO.GetComponent<AnimatrixButton>();
        }
        else
        {
            agreeBtn = existingBtn.GetComponent<AnimatrixButton>();
        }

        // Wire script fields
        var script = screen.GetComponent(scriptType) as MonoBehaviour;
        if (script == null) return;
        var so = new SerializedObject(script);
        SetObjRef(so, "agreeToggle", toggle);
        SetObjRef(so, "agreeBtn",    agreeBtn);
        so.ApplyModifiedProperties();
    }

    /// Builds a Toggle whose checkmark is explicitly wired and clearly visible.
    static Toggle BuildToggle(Transform parent, string name, string labelText)
    {
        var root = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(root, $"Create {name}");
        GameObjectUtility.SetParentAndAlign(root, parent.gameObject);
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 28f);

        var toggle = root.AddComponent<Toggle>();
        toggle.transition = Selectable.Transition.ColorTint;
        toggle.isOn = false;

        // Checkbox background
        var bgGO = new GameObject("Background", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(bgGO, root);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin        = new Vector2(0f, 0.5f);
        bgRT.anchorMax        = new Vector2(0f, 0.5f);
        bgRT.sizeDelta        = new Vector2(24f, 24f);
        bgRT.anchoredPosition = new Vector2(12f, 0f);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.22f, 0.22f, 0.22f, 1f);
        toggle.targetGraphic = bgImg;

        var colors = ColorBlock.defaultColorBlock;
        colors.normalColor      = new Color(0.22f, 0.22f, 0.22f, 1f);
        colors.highlightedColor = new Color(0.32f, 0.32f, 0.32f, 1f);
        colors.pressedColor     = new Color(0.12f, 0.12f, 0.12f, 1f);
        toggle.colors = colors;

        // Checkmark — green fill, explicitly assigned as toggle.graphic so Unity shows/hides it
        var checkGO = new GameObject("Checkmark", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(checkGO, bgGO);
        var checkRT = checkGO.GetComponent<RectTransform>();
        checkRT.anchorMin        = new Vector2(0.12f, 0.12f);
        checkRT.anchorMax        = new Vector2(0.88f, 0.88f);
        checkRT.sizeDelta        = Vector2.zero;
        checkRT.anchoredPosition = Vector2.zero;
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color  = new Color(0.18f, 0.78f, 0.35f, 1f);
        checkImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        toggle.graphic  = checkImg;

        // Label
        var labelGO = new GameObject("Label", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(labelGO, root);
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = new Vector2(32f, 0f);
        labelRT.offsetMax = Vector2.zero;
        var txt = labelGO.AddComponent<Text>();
        txt.text      = labelText;
        txt.fontSize  = 13;
        txt.color     = Color.white;
        txt.alignment = TextAnchor.MiddleLeft;

        return toggle;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 3. PROFILE STATS
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "3 - Add Profile Stats")]
    public static void AddProfileStats()
    {
        var screen = FindInScene("Profile Screen");
        if (screen == null) { Debug.LogError("[PinsPoker] Cannot find 'Profile Screen'."); return; }

        // Stats Panel
        var statsPanel = screen.transform.Find("Stats Panel");
        if (statsPanel == null)
        {
            var panel = new GameObject("Stats Panel", typeof(RectTransform), typeof(VerticalLayoutGroup));
            Undo.RegisterCreatedObjectUndo(panel, "Create Stats Panel");
            GameObjectUtility.SetParentAndAlign(panel, screen);

            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.05f, 0.2f);
            rt.anchorMax        = new Vector2(0.95f, 0.55f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta        = Vector2.zero;

            var vlg = panel.GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment       = TextAnchor.UpperLeft;
            vlg.spacing              = 8f;
            vlg.childControlHeight   = true;
            vlg.childControlWidth    = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth  = true;

            statsPanel = panel.transform;
        }

        var gamesCountText  = GetOrCreateTMPText(statsPanel, "Games Count Text",   "Games Played: 0");
        var winRecordText   = GetOrCreateTMPText(statsPanel, "Win Record Text",     "Win Record: 0W / 0L");
        var pointsText      = GetOrCreateTMPText(statsPanel, "Points Text",         "Points: 0");
        var moneyEarnedText = GetOrCreateTMPText(statsPanel, "Money Earned Text",   "Money Earned: $0.00");

        // QR Code placeholder
        var qrTransform = screen.transform.Find("QR Code Image");
        RawImage qrImage = null;
        if (qrTransform == null)
        {
            var qrGO = new GameObject("QR Code Image", typeof(RectTransform), typeof(RawImage));
            Undo.RegisterCreatedObjectUndo(qrGO, "Create QR Code Image");
            GameObjectUtility.SetParentAndAlign(qrGO, screen);

            var rt = qrGO.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.25f, 0.03f);
            rt.anchorMax        = new Vector2(0.75f, 0.2f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta        = Vector2.zero;

            qrImage = qrGO.GetComponent<RawImage>();
        }
        else
        {
            qrImage = qrTransform.GetComponent<RawImage>();
        }

        // Wire
        var profileScript = screen.GetComponent<ProfileScreen>();
        if (profileScript != null)
        {
            var so = new SerializedObject(profileScript);
            SetObjRef(so, "gamesCountText",  gamesCountText);
            SetObjRef(so, "winRecordText",   winRecordText);
            SetObjRef(so, "pointsText",      pointsText);
            SetObjRef(so, "moneyEarnedText", moneyEarnedText);
            SetObjRef(so, "qrCodeImage",     qrImage);
            so.ApplyModifiedProperties();
        }

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ Profile stats fields added and wired.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 4. LEAGUE FIELDS (Start Date + Frequency Dropdown)
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "4 - Add League Date + Frequency")]
    public static void AddLeagueFields()
    {
        var screen = FindInScene("Create/Edit League Screen (Moderator)");
        if (screen == null) { Debug.LogError("[PinsPoker] Cannot find 'Create/Edit League Screen (Moderator)'."); return; }

        // Best parent: Enter League Info Panel > Middle Panel, falling back to screen root
        Transform formParent = FindDeepChild(screen.transform, "Enter League Info Panel")
                            ?? FindDeepChild(screen.transform, "Middle Panel")
                            ?? screen.transform;

        // Start Date InputField
        InputField startDateField = null;
        var existingDate = FindDeepChild(screen.transform, "Start Date InputField");
        if (existingDate == null)
        {
            var inputGO = DefaultControls.CreateInputField(new DefaultControls.Resources());
            Undo.RegisterCreatedObjectUndo(inputGO, "Create Start Date InputField");
            inputGO.name = "Start Date InputField";
            GameObjectUtility.SetParentAndAlign(inputGO, formParent.gameObject);

            var ph = inputGO.transform.Find("Placeholder")?.GetComponent<Text>();
            if (ph) ph.text = "Start Date (MM/DD/YYYY)...";

            startDateField = inputGO.GetComponent<InputField>();
        }
        else
        {
            startDateField = existingDate.GetComponent<InputField>();
        }

        // Frequency TMP_Dropdown — fully built out
        TMP_Dropdown frequencyDropdown = null;
        var existingDrop = FindDeepChild(screen.transform, "Frequency Dropdown");
        if (existingDrop == null)
            frequencyDropdown = BuildTMPDropdown(formParent, "Frequency Dropdown");
        else
            frequencyDropdown = existingDrop.GetComponent<TMP_Dropdown>();

        // Wire
        var leagueScript = screen.GetComponent<CreateEditLeagueScreen>();
        if (leagueScript != null)
        {
            var so = new SerializedObject(leagueScript);
            SetObjRef(so, "startDateField",    startDateField);
            SetObjRef(so, "frequencyDropdown", frequencyDropdown);
            so.ApplyModifiedProperties();
        }

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ League start date and frequency fields added.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 5. SUBMIT DISPUTE SCREEN
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "5 - Create Dispute Form Screen")]
    public static void CreateDisputeFormScreen()
    {
        if (FindInScene("Submit Dispute Screen") != null)
        {
            Debug.LogWarning("[PinsPoker] 'Submit Dispute Screen' already exists — skipped.");
            return;
        }

        var template = FindInScene("Privacy Policy Screen");
        if (template == null) { Debug.LogError("[PinsPoker] Cannot find 'Privacy Policy Screen' template."); return; }

        var copy = Object.Instantiate(template, template.transform.parent);
        Undo.RegisterCreatedObjectUndo(copy, "Create Submit Dispute Screen");
        copy.name = "Submit Dispute Screen";

        // Harvest back button from old script then swap
        var old     = copy.GetComponent<PrivacyPolicyScreen>();
        var backBtn = old != null ? old.backBtn : null;
        if (old != null) Undo.DestroyObjectImmediate(old);

        var disputeScript = Undo.AddComponent<SubmitDisputeScreen>(copy);

        // Content parent: Middle Panel or root
        var contentParent = FindDeepChild(copy.transform, "Middle Panel") ?? copy.transform;

        // Subject field
        var subjectGO = DefaultControls.CreateInputField(new DefaultControls.Resources());
        Undo.RegisterCreatedObjectUndo(subjectGO, "Create Subject Field");
        subjectGO.name = "Subject InputField";
        GameObjectUtility.SetParentAndAlign(subjectGO, contentParent.gameObject);
        var subPH = subjectGO.transform.Find("Placeholder")?.GetComponent<Text>();
        if (subPH) subPH.text = "Subject...";
        var subjectField = subjectGO.GetComponent<InputField>();

        // Description field (multi-line)
        var descGO = DefaultControls.CreateInputField(new DefaultControls.Resources());
        Undo.RegisterCreatedObjectUndo(descGO, "Create Description Field");
        descGO.name = "Description InputField";
        GameObjectUtility.SetParentAndAlign(descGO, contentParent.gameObject);
        var descPH = descGO.transform.Find("Placeholder")?.GetComponent<Text>();
        if (descPH) descPH.text = "Describe your dispute...";
        var descField = descGO.GetComponent<InputField>();
        descField.lineType = InputField.LineType.MultiLineNewline;
        descGO.GetComponent<RectTransform>().sizeDelta = new Vector2(400f, 120f);

        // Submit button
        var submitBtnGO = CreateAnimatrixButton(contentParent, "Submit Button", "Submit Dispute");
        submitBtnGO.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 50f);
        var submitBtn = submitBtnGO.GetComponent<AnimatrixButton>();

        // Status text
        var statusText = GetOrCreateTMPText(contentParent, "Status Text", "");

        // Wire
        var so = new SerializedObject(disputeScript);
        SetObjRef(so, "backBtn",         backBtn);
        SetObjRef(so, "submitBtn",       submitBtn);
        SetObjRef(so, "subjectField",    subjectField);
        SetObjRef(so, "descriptionField", descField);
        SetObjRef(so, "statusText",      statusText);
        so.ApplyModifiedProperties();

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ Submit Dispute Screen created and wired.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 6. HOME INFO BUTTONS
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "6 - Add Home Info Buttons")]
    public static void AddHomeInfoButtons()
    {
        var screen = FindInScene("Home Screen");
        if (screen == null) { Debug.LogError("[PinsPoker] Cannot find 'Home Screen'."); return; }

        // Create or find an Info Buttons Panel near the bottom of the screen
        var infoPanel = screen.transform.Find("Info Buttons Panel");
        if (infoPanel == null)
        {
            var panel = new GameObject("Info Buttons Panel", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            Undo.RegisterCreatedObjectUndo(panel, "Create Info Buttons Panel");
            GameObjectUtility.SetParentAndAlign(panel, screen);

            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 0f);
            rt.anchorMax        = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(0f, 55f);
            rt.sizeDelta        = new Vector2(-20f, 48f);

            var hlg = panel.GetComponent<HorizontalLayoutGroup>();
            hlg.childAlignment       = TextAnchor.MiddleCenter;
            hlg.spacing              = 6f;
            hlg.childControlWidth    = true;
            hlg.childForceExpandWidth  = true;
            hlg.childForceExpandHeight = false;

            infoPanel = panel.transform;
        }

        var howItWorksGO     = CreateAnimatrixButton(infoPanel, "How It Works Button",     "How It Works");
        var capabilityGO     = CreateAnimatrixButton(infoPanel, "Capability List Button",   "Capabilities");
        var handRankingsGO   = CreateAnimatrixButton(infoPanel, "Hand Rankings Button",     "Hand Rankings");
        var howToCreateGO    = CreateAnimatrixButton(infoPanel, "How To Create Button",     "Create Game");
        var buttonGuideGO    = CreateAnimatrixButton(infoPanel, "Button Guide Button",      "Controls");

        var homeScript = screen.GetComponent<HomeScreen>();
        if (homeScript != null)
        {
            var so = new SerializedObject(homeScript);
            SetObjRef(so, "howItWorksBtn",     howItWorksGO.GetComponent<AnimatrixButton>());
            SetObjRef(so, "capabilityListBtn", capabilityGO.GetComponent<AnimatrixButton>());
            SetObjRef(so, "handRankingsBtn",   handRankingsGO.GetComponent<AnimatrixButton>());
            SetObjRef(so, "howToCreateGameBtn", howToCreateGO.GetComponent<AnimatrixButton>());
            SetObjRef(so, "buttonGuideBtn",    buttonGuideGO.GetComponent<AnimatrixButton>());
            so.ApplyModifiedProperties();
        }

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ Home info buttons created and wired.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 7. MY LEAGUES DESCRIPTION TEXT
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "7 - Add My Leagues Description")]
    public static void AddMyLeaguesDescription()
    {
        var screen = FindInScene("My Leagues Screen (Player)");
        if (screen == null) { Debug.LogError("[PinsPoker] Cannot find 'My Leagues Screen (Player)'."); return; }

        // Place the description at the top, inside the screen
        var upperPanel = FindDeepChild(screen.transform, "Upper Panel") ?? screen.transform;
        var descTxt = GetOrCreateTMPText(upperPanel, "Description Text",
            "Leagues are organised groups of players who compete on a set schedule. " +
            "Search for a league on the home screen to join one.");

        var descRT = descTxt.GetComponent<RectTransform>();
        descRT.sizeDelta = new Vector2(0f, 60f);
        descTxt.fontSize     = 13f;
        descTxt.fontStyle    = FontStyles.Normal;
        descTxt.alignment    = TextAlignmentOptions.Center;
        descTxt.enableWordWrapping = true;

        var leagueScript = screen.GetComponent<MyLeaguesScreen>();
        if (leagueScript != null)
        {
            var so = new SerializedObject(leagueScript);
            SetObjRef(so, "descriptionText", descTxt);
            so.ApplyModifiedProperties();
        }

        MarkDirty();
        Debug.Log("[PinsPoker] ✓ My Leagues description text added.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 8. START DATE TEXT IN MY LEAGUE PREFAB
    // ─────────────────────────────────────────────────────────────────────────

    [MenuItem(MENU + "8 - Add Start Date to League Prefab")]
    public static void AddStartDateToLeaguePrefab()
    {
        const string prefabPath = "Assets/Prefabs/Player/My League Request Prefab.prefab";
        var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset == null)
        {
            Debug.LogError($"[PinsPoker] Cannot find prefab at '{prefabPath}'. Check the path.");
            return;
        }

        using (var scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var root = scope.prefabContentsRoot;

            // Find MyLeaguePrefab script
            var script = root.GetComponent<MyLeaguePrefab>();
            if (script == null)
            {
                Debug.LogError("[PinsPoker] MyLeaguePrefab script not found on prefab root.");
                return;
            }

            // Look for an existing start time text to mirror its parent
            var startTimeTxtObj = root.GetComponentsInChildren<TMP_Text>(true);
            Transform textParent = startTimeTxtObj.Length > 0
                ? startTimeTxtObj[0].transform.parent
                : root.transform;

            // Don't create if already exists
            var existingDate = FindDeepChild(root.transform, "Start Date Text");
            TMP_Text startDateTxt;
            if (existingDate == null)
            {
                startDateTxt = GetOrCreateTMPText(textParent, "Start Date Text", "Start Date: N/A");
                startDateTxt.fontSize = 12f;
            }
            else
            {
                startDateTxt = existingDate.GetComponent<TMP_Text>();
            }

            // Wire the field on the script
            var so = new SerializedObject(script);
            SetObjRef(so, "startDateTxt", startDateTxt);
            so.ApplyModifiedProperties();
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[PinsPoker] ✓ Start Date text added to My League Request Prefab.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SHARED UTILITIES
    // ─────────────────────────────────────────────────────────────────────────

    /// Find a scene GameObject by name, including inactive objects.
    static GameObject FindInScene(string name)
    {
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            if (go.scene.IsValid() && go.name == name) return go;
        return null;
    }

    /// Depth-first child search.
    static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    /// Create or find a TMP_Text child.
    static TMP_Text GetOrCreateTMPText(Transform parent, string name, string defaultText)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.GetComponent<TMP_Text>();

        var go = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);

        go.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 28f);

        var text = go.AddComponent<TextMeshProUGUI>();
        text.text    = defaultText;
        text.fontSize = 14f;
        text.color   = Color.white;
        return text;
    }

    /// Create an AnimatrixButton with a background Image and a Text label child.
    static GameObject CreateAnimatrixButton(Transform parent, string name, string label)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing.gameObject;

        var btnGO = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(btnGO, $"Create {name}");
        GameObjectUtility.SetParentAndAlign(btnGO, parent.gameObject);

        var bg  = btnGO.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.18f, 0.18f);

        var btn = btnGO.AddComponent<AnimatrixButton>();
        btn.targetGraphic = bg;

        // Label child
        var labelGO = new GameObject("Text", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(labelGO, btnGO);

        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin        = Vector2.zero;
        labelRT.anchorMax        = Vector2.one;
        labelRT.sizeDelta        = Vector2.zero;
        labelRT.anchoredPosition = Vector2.zero;

        var text = labelGO.AddComponent<Text>();
        text.text      = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color     = Color.white;
        text.fontSize  = 13;

        btnGO.GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 42f);
        return btnGO;
    }

    /// Build a fully functional TMP_Dropdown with Template, Viewport, Content, and Item hierarchy.
    static TMP_Dropdown BuildTMPDropdown(Transform parent, string name)
    {
        var root = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(root, $"Create {name}");
        GameObjectUtility.SetParentAndAlign(root, parent.gameObject);
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 40f);

        var rootBG = root.AddComponent<Image>();
        rootBG.color = new Color(0.15f, 0.15f, 0.15f);

        var dropdown = root.AddComponent<TMP_Dropdown>();
        dropdown.targetGraphic = rootBG;

        // Caption label
        var labelGO = new GameObject("Label", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(labelGO, root);
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin        = Vector2.zero;
        labelRT.anchorMax        = Vector2.one;
        labelRT.offsetMin        = new Vector2(10f, 2f);
        labelRT.offsetMax        = new Vector2(-28f, -2f);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text      = "Every week";
        labelTMP.fontSize  = 14f;
        labelTMP.color     = Color.white;
        labelTMP.alignment = TextAlignmentOptions.Left;
        dropdown.captionText = labelTMP;

        // Arrow indicator
        var arrowGO = new GameObject("Arrow", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(arrowGO, root);
        var arrowRT = arrowGO.GetComponent<RectTransform>();
        arrowRT.anchorMin        = new Vector2(1f, 0.5f);
        arrowRT.anchorMax        = new Vector2(1f, 0.5f);
        arrowRT.sizeDelta        = new Vector2(20f, 20f);
        arrowRT.anchoredPosition = new Vector2(-15f, 0f);
        arrowGO.AddComponent<Image>().color = Color.white;

        // Template (hidden scrollable panel)
        var templateGO = new GameObject("Template", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(templateGO, root);
        var templateRT = templateGO.GetComponent<RectTransform>();
        templateRT.anchorMin        = new Vector2(0f, 0f);
        templateRT.anchorMax        = new Vector2(1f, 0f);
        templateRT.pivot            = new Vector2(0.5f, 1f);
        templateRT.anchoredPosition = new Vector2(0f, 2f);
        templateRT.sizeDelta        = new Vector2(0f, 150f);
        templateGO.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f);
        var scrollRect = templateGO.AddComponent<ScrollRect>();
        scrollRect.horizontal      = false;
        scrollRect.movementType    = ScrollRect.MovementType.Clamped;
        templateGO.SetActive(false);
        dropdown.template = templateRT;

        // Viewport
        var viewportGO = new GameObject("Viewport", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(viewportGO, templateGO);
        var viewportRT = viewportGO.GetComponent<RectTransform>();
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.sizeDelta = new Vector2(-18f, 0f);
        viewportRT.pivot     = new Vector2(0f, 1f);
        viewportGO.AddComponent<Image>();
        viewportGO.AddComponent<Mask>().showMaskGraphic = false;

        // Content
        var contentGO = new GameObject("Content", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(contentGO, viewportGO);
        var contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin        = new Vector2(0f, 1f);
        contentRT.anchorMax        = new Vector2(1f, 1f);
        contentRT.pivot            = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta        = new Vector2(0f, 28f);
        scrollRect.content  = contentRT;
        scrollRect.viewport = viewportRT;

        // Item template
        var itemGO = new GameObject("Item", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(itemGO, contentGO);
        var itemRT = itemGO.GetComponent<RectTransform>();
        itemRT.anchorMin = new Vector2(0f, 0.5f);
        itemRT.anchorMax = new Vector2(1f, 0.5f);
        itemRT.sizeDelta = new Vector2(0f, 28f);
        var itemToggle = itemGO.AddComponent<Toggle>();

        var itemBGGO = new GameObject("Item Background", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(itemBGGO, itemGO);
        var itemBGRT = itemBGGO.GetComponent<RectTransform>();
        itemBGRT.anchorMin = Vector2.zero;
        itemBGRT.anchorMax = Vector2.one;
        itemBGRT.sizeDelta = Vector2.zero;
        var itemBGImg = itemBGGO.AddComponent<Image>();
        itemBGImg.color = new Color(0.2f, 0.2f, 0.2f);
        itemToggle.targetGraphic = itemBGImg;

        var checkGO = new GameObject("Item Checkmark", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(checkGO, itemGO);
        var checkRT = checkGO.GetComponent<RectTransform>();
        checkRT.anchorMin        = new Vector2(0f, 0.5f);
        checkRT.anchorMax        = new Vector2(0f, 0.5f);
        checkRT.sizeDelta        = new Vector2(20f, 20f);
        checkRT.anchoredPosition = new Vector2(12f, 0f);
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.3f, 0.8f, 0.3f);
        itemToggle.graphic = checkImg;

        var itemLabelGO = new GameObject("Item Label", typeof(RectTransform));
        GameObjectUtility.SetParentAndAlign(itemLabelGO, itemGO);
        var itemLabelRT = itemLabelGO.GetComponent<RectTransform>();
        itemLabelRT.anchorMin = Vector2.zero;
        itemLabelRT.anchorMax = Vector2.one;
        itemLabelRT.offsetMin = new Vector2(28f, 1f);
        itemLabelRT.offsetMax = new Vector2(-5f, -2f);
        var itemLabelTMP = itemLabelGO.AddComponent<TextMeshProUGUI>();
        itemLabelTMP.text      = "Option";
        itemLabelTMP.fontSize  = 14f;
        itemLabelTMP.color     = Color.white;
        dropdown.itemText = itemLabelTMP;

        return dropdown;
    }

    /// Safely set an object reference on a SerializedProperty.
    static void SetObjRef(SerializedObject so, string propName, Object value)
    {
        var prop = so.FindProperty(propName);
        if (prop != null) prop.objectReferenceValue = value;
        else Debug.LogWarning($"[PinsPoker] Property '{propName}' not found on {so.targetObject.GetType().Name}");
    }

    static void MarkDirty()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}

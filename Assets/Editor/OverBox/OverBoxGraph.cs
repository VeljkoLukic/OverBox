using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class OverBoxGraph : EditorWindow
{
    private OverBoxGraphView graphView;

    private static EditorWindow window;

    [MenuItem("OverBox/Graph")]
    public static void OpenWindow()
    {
        window = GetWindow<OverBoxGraph>();
        window.titleContent = new GUIContent("OverBox");
        window.autoRepaintOnSceneChange = true;
    }

    void OnGUI()
    {
        if (window != null)
            logo.style.left = (window.position.width / 2) - 50;

        if (graphView == null)
            return;

        foreach (OverBoxNode node in graphView.nodes)
            node.Save();
    }

    public void OnEnable()
    {
        InitializeGraph();
        GenerateToolbar();
    }

    public void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    public void InitializeGraph()
    {
        graphView = new OverBoxGraphView(this)
        {
            name = "OverBox Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
    Image logo;
    Toolbar toolbar;
    private void GenerateToolbar()
    {
        toolbar = new Toolbar();
        toolbar.style.height = 40;

        logo = new Image();
        logo.image = Resources.Load<Texture>("OverBox/overbox_logo");
        logo.style.width = 100;
        logo.style.height = 50;
        logo.style.bottom = 5;

        toolbar.Add(logo);

        rootVisualElement.Add(toolbar);
    }
}

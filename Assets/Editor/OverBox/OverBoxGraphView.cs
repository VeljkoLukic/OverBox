using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class OverBoxGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new(300, 300);

    public OverBoxGraphData graphContainer;

    private OverBoxGraph editorWindow;

    public OverBoxGraphView(OverBoxGraph editorWindow)
    {
        graphViewChanged += OnGraphViewChanged;
        this.editorWindow = editorWindow;

        styleSheets.Add(Resources.Load<StyleSheet>("OverBoxGraph"));
        graphContainer = Resources.Load<OverBoxGraphData>("GraphContainer");

        SetupZoom(ContentZoomer.DefaultMinScale, 1.5f);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateNodeContextualMenu());

        GridBackground grid = new();
        Insert(0, grid);
        grid.StretchToParentSize();

        LoadNode();
    }

    private GraphViewChange OnGraphViewChanged (GraphViewChange changes)
    {
        if (changes.elementsToRemove != null)
        {
            changes.elementsToRemove.ForEach(element =>
            {
                OverBoxNode node = element as OverBoxNode;
                if (node != null)
                {
                    UnityEngine.Object[] data = Resources.LoadAll<OverBoxNodeData>("OverBox/Data/");

                    foreach (OverBoxNodeData dataFile in data)
                    {
                        if (dataFile.name == node.ID)
                        {
                            AssetDatabase.DeleteAsset("Assets/Resources/OverBox/Data/" + dataFile.name + ".asset");
                            foreach (Edge e in edges)
                            {
                                if (e.output.node == node)
                                    RemoveElement(e);
                            }
                        }
                    }
                }
            });
        }
        return changes;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new();
        ports.ForEach(port =>
        {
            if ((startPort != port) && (startPort.node != port.node))
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port GeneratePort(OverBoxNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port newPort = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        newPort.portName = String.Empty;
        return newPort;
    }

    public void AddNodeToGraph(Vector2 position)
    {
        OverBoxNodeData data = CreateNodeData();
        data.SetupData(data.name, position);
        OverBoxNode newNode = CreateNewNode(data);
        AddElement(newNode);
    }

    public void LoadNode()
    {
        UnityEngine.Object[] data = Resources.LoadAll<OverBoxNodeData>("OverBox/Data/");

        foreach (OverBoxNodeData dataFile in data)
        {
            OverBoxNode node = CreateNewNode(dataFile);
            AddElement(node);
        }

        foreach (OverBoxNode currentNode in nodes)
        {
            foreach (OverBoxNode nextNode in nodes)
            {
                for (int i = 0; i < currentNode.data.choices.Length; i++)
                {
                    if (currentNode.data.choices[i].nextDialogue == nextNode.data)
                    {
                        var tempEdge = new Edge
                        {
                            output = currentNode.ports[i],
                            input = nextNode.inputPort
                        };
                        tempEdge.input.Connect(tempEdge);
                        tempEdge.output.Connect(tempEdge);
                        Add(tempEdge);
                    }
                }
            }
        }
    }

    public OverBoxNode CreateNewNode(OverBoxNodeData data)
    {
        OverBoxNode newNode = new()
        {
            data = data,
            character = data.character,
            choices = data.choices,
            ID = data.name
        };
      
        Port inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
        newNode.inputContainer.Add(inputPort);
        inputPort.SetEnabled(false);
        newNode.inputPort = inputPort;

        OverBoxStyler.SetNodeTitleStyle(newNode, (int)newNode.character);

        EnumField characterEnumField = OverBoxStyler.GenerateCharacterEnumField(newNode);

        newNode.titleContainer.Clear();
        newNode.titleContainer.Add(characterEnumField);

        characterEnumField.RegisterValueChangedCallback(evt =>
        {
            OverBoxStyler.SetNodeTitleStyle(newNode, (Convert.ToInt32(evt.newValue)));
            newNode.character = (CharacterType)evt.newValue;
        });

        newNode.mainContainer.style.borderBottomWidth = newNode.mainContainer.style.borderLeftWidth = newNode.mainContainer.style.borderRightWidth = 1f;
        newNode.mainContainer.style.borderTopLeftRadius = newNode.mainContainer.style.borderTopRightRadius = newNode.mainContainer.style.borderBottomLeftRadius = newNode.mainContainer.style.borderBottomRightRadius = 4;

        AddChoiceSection(newNode, 0);
        AddChoiceSection(newNode, 1);
        AddChoiceSection(newNode, 2);

        newNode.RefreshExpandedState();
        newNode.RefreshPorts();
        newNode.SetPosition(new Rect(data.nodePosition, defaultNodeSize));

        return newNode;
    }

    private void AddChoiceSection(OverBoxNode node, int index)
    {   
        Port newPort = GeneratePort(node, Direction.Output);

        Box allChoicesBox = OverBoxStyler.GenerateAllChoicesBox();

        TextField bubbleLineTextField = OverBoxStyler.GenerateBubbleLineTextField();

        Toggle immediateCont = new();

        immediateCont.RegisterValueChangedCallback(evt =>
        {
            node.data.choices[index].immediateContinuation = evt.newValue;
        });

        TextField longLineTextField = OverBoxStyler.GenerateLongLineTextField();

        node.ports[index] = newPort;
        immediateCont.value = node.data.choices[index].immediateContinuation;
        bubbleLineTextField.value = node.data.choices[index].bubbleLine;
        longLineTextField.value = node.data.choices[index].longLine;
        
        PopupField<string> popupField = new();
        popupField.style.left = 1;

        popupField = new(CommandListUtility.GetMethodList(), node.data.choices[index].commandName);
        popupField.RegisterValueChangedCallback(evt => { node.data.choices[index].commandName = evt.newValue; });

        ObjectField voiceClip = new ();
        voiceClip.objectType = typeof(AudioClip);
        voiceClip.style.left = 1;

        Box firstRowFirstColumnBox = new();
        firstRowFirstColumnBox.contentContainer.Add(bubbleLineTextField);
        firstRowFirstColumnBox.style.flexDirection = FlexDirection.Row;    

        Box firstRowFull = new();
        firstRowFull.contentContainer.Add(firstRowFirstColumnBox);
        firstRowFull.style.flexDirection = FlexDirection.Row;

        Box singleChoiceBox = new();
        singleChoiceBox.contentContainer.Add(firstRowFull);
        singleChoiceBox.style.flexDirection = FlexDirection.Column;

        longLineTextField.RegisterValueChangedCallback(evt =>
        {
            node.data.choices[index].longLine = evt.newValue;        
        });

        bubbleLineTextField.RegisterValueChangedCallback(evt =>
        {
            node.data.choices[index].bubbleLine = evt.newValue;

            if (evt.newValue == String.Empty)
            {
                node.choices[index].nextDialogue = null;
                node.data.choices[index].nextDialogue = null;
                foreach (Edge e in edges)
                {
                    OverBoxNode edgeOutputNode = (OverBoxNode)e.output.node;
                    if (edgeOutputNode == node && edgeOutputNode.ports[index].connections.Contains(e))
                        RemoveElement(e);
                }
                node.ports[index].DisconnectAll();
            }    

            if (evt.newValue == String.Empty)
            {
                singleChoiceBox.contentContainer.Remove(longLineTextField);
                singleChoiceBox.contentContainer.Remove(popupField);
                singleChoiceBox.contentContainer.Remove(voiceClip);
                firstRowFull.contentContainer.Remove(newPort);
                firstRowFirstColumnBox.contentContainer.Remove(immediateCont);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
            else
            {
                singleChoiceBox.contentContainer.Add(longLineTextField);
                singleChoiceBox.contentContainer.Add(popupField);
                singleChoiceBox.contentContainer.Add(voiceClip);
                firstRowFull.contentContainer.Add(newPort);
                firstRowFirstColumnBox.contentContainer.Add(immediateCont);

                node.RefreshPorts();
                node.RefreshExpandedState();
            }
        });

        OverBoxStyler.SetBorder(singleChoiceBox, OvergrownColors.GetBorderColor(0.5f), 1f, 4);

        OverBoxStyler.SetTextFieldStyle(bubbleLineTextField);
        OverBoxStyler.SetTextFieldStyle(immediateCont);
        OverBoxStyler.SetTextFieldStyle(longLineTextField);

        allChoicesBox.contentContainer.Add(singleChoiceBox);

        if (bubbleLineTextField.value != null)
        {
            if (bubbleLineTextField.value == String.Empty)
            {
                if (singleChoiceBox.contentContainer.Contains(popupField))
                    singleChoiceBox.contentContainer.Remove(popupField);
                if (singleChoiceBox.contentContainer.Contains(voiceClip))
                    singleChoiceBox.contentContainer.Remove(voiceClip);
                if (firstRowFull.contentContainer.Contains(newPort))
                    firstRowFull.contentContainer.Remove(newPort);
                if (firstRowFirstColumnBox.contentContainer.Contains(immediateCont))
                    firstRowFirstColumnBox.contentContainer.Remove(immediateCont);
                if (firstRowFirstColumnBox.contentContainer.Contains(longLineTextField))
                    firstRowFirstColumnBox.contentContainer.Remove(longLineTextField);
            }
            else
            {
                singleChoiceBox.contentContainer.Add(longLineTextField);
                singleChoiceBox.contentContainer.Add(popupField);
                singleChoiceBox.contentContainer.Add(voiceClip);
                firstRowFull.contentContainer.Add(newPort);
                firstRowFirstColumnBox.contentContainer.Add(immediateCont);
            }
        }
        node.outputContainer.Add(allChoicesBox);
        node.outputContainer.style.minWidth = 200;
        node.outputContainer.style.backgroundColor = OvergrownColors.GetDarkGrey();
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private OverBoxNodeData CreateNodeData()
    {
        OverBoxNodeData nodeData = ScriptableObject.CreateInstance<OverBoxNodeData>();

        nodeData.nodePosition = new Vector2(50, 50);

        graphContainer.NodeCount++;
        EditorUtility.SetDirty(graphContainer);
        AssetDatabase.CreateAsset(nodeData, "Assets/Resources/OverBox/Data/" + graphContainer.NodeCount + ".asset");

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = nodeData;

        return nodeData;
    }

    private IManipulator CreateNodeContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new(menuEvent =>
        {
            Vector2 localMousePos = menuEvent.localMousePosition;
            Vector2 actualGraphPosition = viewTransform.matrix.inverse.MultiplyPoint(localMousePos);

            for (int i = 0; i < menuEvent.menu.MenuItems ().Count; i++)
                menuEvent.menu.RemoveItemAt(i);

            menuEvent.menu.InsertAction(0, "Create Node", actionEvent => AddNodeToGraph(actualGraphPosition));

            for (int i = 1; i < menuEvent.menu.MenuItems().Count; i++)
                menuEvent.menu.RemoveItemAt(i);
        });

        return contextualMenuManipulator;
    }
}

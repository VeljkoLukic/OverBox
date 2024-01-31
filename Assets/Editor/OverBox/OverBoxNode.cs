using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;

public class OverBoxNode : Node
{
    public string ID;

    public CharacterType character;

    public DialogueChoice [] choices = new DialogueChoice[3];

    public OverBoxNodeData data;

    public Port inputPort;
    public Port[] ports = new Port[3];

    public OverBoxNode ()
    {
        this.AddManipulator(CreateNodeContextualMenu());
    }

    private IManipulator CreateNodeContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new(menuEvent =>
        {
            for (int i = 0; i < menuEvent.menu.MenuItems().Count; i++)
                menuEvent.menu.RemoveItemAt(i);
        });

        return contextualMenuManipulator;
    }

    public void Save()
    {
        for (int i = 0; i < data.choices.Length; i++)
            data.choices[i] = choices[i];

        data.character = character;

        data.nodePosition.x = GetPosition().x;
        data.nodePosition.y = GetPosition().y;

        OverBoxNode[] portConnections = new OverBoxNode[3];

        for (int i = 0; i < ports.Length; i++)
        {
            if (ports[i].connections.Count() > 0)
                portConnections[i] = (OverBoxNode)ports[i].connections.First().input.node;
        }

        for (int i = 0; i < portConnections.Length; i++)
        {
            if (portConnections[i] != null)
            {
                data.choices[i].nextDialogue = portConnections[i].data;
                data.choices[i].commandName = choices[i].commandName;
            }
            else
                data.choices[i].nextDialogue = null;
        }

        EditorUtility.SetDirty(data);
    }
}

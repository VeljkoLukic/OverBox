using System;

[Serializable]
public class DialogueChoice
{
    public string bubbleLine, longLine;

    public OverBoxNodeData nextDialogue;

    public bool immediateContinuation;

    public string commandName = "None";

    public DialogueChoice ()
    {
        this.bubbleLine = "";
        this.longLine = "";
        this.nextDialogue = null;
        this.immediateContinuation = false;
    }

    public void InvokeCommand ()
    {
        if (commandName != string.Empty)
            Type.GetType(nameof(DialogueCommands)).GetMethod(commandName.Substring(commandName.IndexOf('/') + 1)).Invoke(null, null);        
    }
}

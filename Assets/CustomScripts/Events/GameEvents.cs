using System;

public static class GameEvents
{
    public static Action OnDialogueShown;

    public static Action<CharacterType, string> OnBubbleShown;

    public static void DialogueShown() => OnDialogueShown?.Invoke();

    public static void BubbleShown(CharacterType character, string longLine) => OnBubbleShown?.Invoke(character, longLine);
}

using System;
using UnityEngine;

public static partial class DialogueCommands
{
    public static Action OnHeatherFollowPlayer;

    public static Action OnPlayerFollowHeather;

    public static void None() { }

    [CommandCategory ("Intro")]
    public static void HeatherFollowPlayer() => OnHeatherFollowPlayer?.Invoke();

    [CommandCategory("Intro")]
    public static void PlayerFollowHeather() => OnPlayerFollowHeather?.Invoke();

    [CommandCategory("Bar Scene")]
    public static void MoveCharactersToPosition() => Debug.LogError("Move Characters To Position"); 
}

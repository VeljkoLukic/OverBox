public class DialogueEvent
{
    private bool fired;

    public void FireEvent()
    {
        if (!fired)
        {
            GameEvents.DialogueShown();
            fired = true;
        }
    }
}
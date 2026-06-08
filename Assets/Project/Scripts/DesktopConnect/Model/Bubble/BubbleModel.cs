public static class BubbleModel
{
    private static BubbleManager _bubbleManager;
    public static BubbleManager bubbleManager => _bubbleManager;

    static BubbleModel()
    {
        _bubbleManager = new BubbleManager();   
    }
}

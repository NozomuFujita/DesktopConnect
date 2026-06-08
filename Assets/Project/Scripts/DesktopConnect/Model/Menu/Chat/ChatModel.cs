public static class ChatModel
{
    private static ChatManager _chatManager;
    public static ChatManager chatManager => _chatManager;

    static ChatModel()
    {
        _chatManager = new ChatManager();
    }
}

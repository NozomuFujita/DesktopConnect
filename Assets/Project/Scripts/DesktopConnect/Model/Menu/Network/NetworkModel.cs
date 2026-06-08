public static class NetworkModel
{
    private static ConnectManager _connectManager;
    public static ConnectManager connectManager => _connectManager;

    static NetworkModel()
    {
        _connectManager = new ConnectManager();
    }
}

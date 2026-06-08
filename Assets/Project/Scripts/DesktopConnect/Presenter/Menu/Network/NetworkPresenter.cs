using UniRx;

public class NetworkPresenter
{
    public NetworkPresenter()
    {
        
    }

    public void ConnectWebSocket(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;
        NetworkModel.connectManager.ConnectWebSocket(url);
    }

    public void CloseWebSocket()
    {
        NetworkModel.connectManager.CloseWebSocket();
    }
}

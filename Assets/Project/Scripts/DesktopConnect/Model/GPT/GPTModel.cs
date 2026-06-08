using Unity2GPT;

public static class GPTModel
{
    private static GPTConnection _gptConnection;
    public static GPTConnection gptConnection => _gptConnection;

    static GPTModel()
    {
        _gptConnection = new GPTConnection();
    }
}

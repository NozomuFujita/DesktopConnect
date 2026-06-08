public class MenuModel
{
    private static ExternalInteration _externalInteration;
    public static ExternalInteration externalInteration => _externalInteration;

    static MenuModel()
    {
        _externalInteration = new ExternalInteration();
    }
}

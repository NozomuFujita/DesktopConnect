using UnityEngine;

public class CharacterModel
{
    private static JsonReader _jsonReader;
    public static JsonReader jsonReader => _jsonReader;
    private static ObjectPoint _objectPoint;
    public static ObjectPoint objectPoint => _objectPoint;

    private static AnimationSender _animationSender;
    public static AnimationSender animationSender => _animationSender;

    static CharacterModel()
    {
        _jsonReader = new JsonReader();
        _objectPoint = new ObjectPoint();
        _animationSender = new AnimationSender();
    }
}

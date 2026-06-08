using UnityEngine;

public class DisplayChecker
{
    private float width;
    private float height;

    public DisplayChecker(float width, float height)
    {
        // 引数のwidthは横幅全体を取るため、半分にする
        this.width = width;
        this.height = height;
    }

    public bool CheckDisplayIn(Vector3 position)
    {
        return -width < position.x && position.x < width && 0.0f < position.y && position.y < height;
    }
}

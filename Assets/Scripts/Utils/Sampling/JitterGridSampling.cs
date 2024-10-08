using UnityEngine;

public class JitterGridSampling
{
    float spacing;
    float strength;
    Vector2 globalPos;
    float width;
    float height;
    System.Random random;

    public List<Vector2> fill()
    {
        List<Vector2> points = new List<Vector2>();
        float xOffset = globalPos.x % spacing;
        float yOffset = globalPos.y % spacing;
        for (float x = -width; x < width; x += spacing)
        {
            for (float y = 0; y < height; y += spacing)
            {
                points.Add(new Vector2(x + globalPos.x + (float)random.NextDouble() * strength, y + globalPos.y + (float)random.NextDouble() * strength));
            }
        }
        return points;
    }
}

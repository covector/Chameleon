using UnityEngine;

public class SmallTreeGeneration : GenericTreeGeneration<SmallTreeGeneration>
{
    public SmallTreeGeneration(): base(
        new Vector2Int(10, 14),
        new Vector2(0.03f, 0.08f),
        0.3f, 0.3f,
        50f, 10f,
        new Vector2(0.6f, 1f), new Vector2(0.6f, 0.6f),
        0, Vector2.zero, Vector3.zero
    ) { }

    public override int PreGenCount() { return 20; }
}

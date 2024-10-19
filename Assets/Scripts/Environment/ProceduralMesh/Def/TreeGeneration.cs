using UnityEngine;

public class TreeGeneration : GenericTreeGeneration<TreeGeneration>
{
    public TreeGeneration() : base(
        new Vector2Int(10, 10),
        new Vector2(0.05f, 0.4f),
        0.1f, 0.4f,
        30f, 10f,
        new Vector2(1f, 6f), new Vector2(1f, 1f),
        0, Vector2.zero, Vector3.zero
    )
    { }

    public override int PreGenCount() { return 50; }

    public override bool ItemSpawnCheck() { return true; }
    public override bool CollisionCheck() { return true; }
}

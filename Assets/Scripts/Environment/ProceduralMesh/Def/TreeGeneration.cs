using UnityEngine;

public class TreeGeneration : GenericTreeGeneration<TreeGeneration>
{
    public TreeGeneration() : base(
        depth: new Vector2Int(10, 10),
        radius: new Vector2(0.05f, 0.4f),
        cylinderStep: 9,
        trunkSplitChance: 0.1f, splitChance: 0.4f,
        splitRotate: 30f, nonSplitRotate: 10f,
        trunkHeight: new Vector2(1f, 6f), branchLength: new Vector2(1f, 1f),
        leavesCount: 4, nonEndLeafChance: 0.1f,
        crossRenderLeaves: false,
        leavesDim: new Vector2(1f, 3f), leavesScale: new Vector2(0.35f, 1f),
        leavesRotationRange: new Vector3(45f, 90f, 180f), leavesRotationOffset: Vector3.zero
    )
    { }

    public override int PreGenCount() { return 50; }

    public override bool ItemSpawnCheck() { return true; }
    public override bool CollisionCheck() { return true; }
}

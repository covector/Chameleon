using UnityEngine;

public class GradientController
{
    private float lastY = 0f;
    private TerrainGeneration tgen;
    public GradientController(TerrainGeneration tgen)
    {
        this.tgen = tgen;
    }
    public Vector3 GetAdjustedPosition(Transform transform, float xDel, float zDel, int levels = 1, float yOffset = 0f)
    {
        float y = tgen.GetGroudLevel(transform.position.x + xDel, transform.position.z + zDel, levels);
        float gradient = Mathf.Max((y - lastY) / Time.deltaTime + 3f, 2.5f) / 3f;
        lastY = y;
        return new Vector3(
            transform.position.x + xDel / gradient,
            tgen.GetGroudLevel(transform.position.x + xDel / gradient, transform.position.z + zDel / gradient, levels) + yOffset,
            transform.position.z + zDel / gradient
        );
    }
}

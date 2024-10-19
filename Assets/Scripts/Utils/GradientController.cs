using UnityEngine;

public class GradientController
{
    private float lastY = 0f;
    public Vector3 GetAdjustedPosition(Transform transform, float xDel, float zDel, int levels = 1, float yOffset = 0f)
    {
        float y = ChunkGeneration.GetGroudLevel(transform.position.x + xDel, transform.position.z + zDel, levels);
        float gradient = Mathf.Max((y - lastY) / Time.deltaTime + 3f, 2.5f) / 3f;
        lastY = y;
        return Utils.ProjectToGround(transform.position.x + xDel / gradient, transform.position.z + zDel / gradient, levels) + Vector3.up * yOffset;
    }
}

using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class RadarDisplay : MonoBehaviour
{
    Material screenMaterial;
    static string[] varNames = new string[] {
        "_Dot1", "_Dot2", "_Dot3", "_Dot4"
    };
    public Transform items;
    public Transform cam;
    public RadarScanner rs;
    private const float aspectRatio = 0.675f;
    private float timePassedSinceRender = float.PositiveInfinity;
    float halfWidth;
    float halfHeight;
    float maxDiag;

    void Start()
    {
        screenMaterial = GetComponent<MeshRenderer>().materials[1];
        halfWidth = ItemSpawning.spacing;
        halfHeight = halfWidth * aspectRatio;
        maxDiag = Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
    }

    void Update()
    {
        if (rs.isActiveAndEnabled && (timePassedSinceRender += Time.deltaTime) > 0.1f)
        {
            RenderDisplay(GetDots());
            timePassedSinceRender = 0f;
        }
    }

    List<Vector2> GetDots()
    {
        List<Vector2> dots = new List<Vector2>();
        foreach (Transform item in items)
        {
            if (item.GetComponent<ItemPickUp>().pickedUp) { continue; }
            float diffX = item.position.x - cam.position.x;
            float diffZ = item.position.z - cam.position.z;
            if (Mathf.Abs(diffX) > maxDiag || Mathf.Abs(diffZ) > maxDiag) { continue; }
            Vector3 diff = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0f, -cam.eulerAngles.y, 0f))).MultiplyVector(new Vector3(diffX, 0f, diffZ));
            if (Mathf.Abs(diff.x) < halfWidth && Mathf.Abs(diff.z) < halfHeight)
            {
                dots.Add(new Vector2(diff.x / (2f * halfWidth) + 0.5f, diff.z / (2f * halfHeight) + 0.5f));
            }
            if (dots.Count == 4) { break; }
        }
        return dots;
    }

    void RenderDisplay(List<Vector2> dots)
    {
        for (int i = 0; i < varNames.Length; i++)
        {
            Vector2 pos = dots.Count > i ? dots[i] : new Vector2(-1, -1);
            screenMaterial.SetVector(varNames[i], pos);
        }
    }

    public void MaterialScreenOn()
    {
        screenMaterial.SetInt("_ScreenOn", 1);
    }

    public void MaterialScreenOff()
    {
        screenMaterial.SetInt("_ScreenOn", 0);
    }
}


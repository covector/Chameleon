using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TreeGeneration : MonoBehaviour
{
    public void Generate(int seed)
    {
        MeshBuilder meshBuilder = new MeshBuilder(1, seed);
        meshBuilder.AddCylinder(0.2f, 10, 16, 0, Matrix4x4.identity);
        GetComponent<MeshFilter>().mesh = meshBuilder.Build();
    }
}

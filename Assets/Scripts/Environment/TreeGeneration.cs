using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TreeGeneration : MonoBehaviour
{
    void Start()
    {
        MeshBuilder meshBuilder = new MeshBuilder(1);

        //meshBuilder.AddQuad(new Vector3(-0.5f, 0, -0.5f), Vector3.right, Vector3.forward, 0, Matrix4x4.Translate(new Vector3(0, 2, 0)) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)));
        //meshBuilder.AddQuad(new Vector3(-0.5f, 0, -0.5f), Vector3.right, Vector3.forward, 1);
        meshBuilder.AddCylinder(0.2f, 10, 16, 0, Matrix4x4.identity);
        GetComponent<MeshFilter>().mesh = meshBuilder.Build();
    }
}

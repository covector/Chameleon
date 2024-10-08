using UnityEngine;
using static MeshBuilder;

public class RockGeneration : ProceduralAsset
{
    protected override void Edit(MeshBuilder meshBuilder)
    {
        //TempMesh plane = CreatePlane(Vector3.up, Vector3.left, Vector3.up, 5, 5);
        //TempMesh plane = TransformMesh(CreateCube(2f, 3), Matrix4x4.Translate(new Vector3(0f, 3f, 0f)));
        TempMesh plane = TransformMesh(CreateCubeSphere(2f, 3), Matrix4x4.Translate(new Vector3(0f, 3f, 0f)));
        // TempMesh plane = TransformMesh(UNIT_CUBE, Matrix4x4.Scale(new Vector3(2f, 2f, 2f)));

        meshBuilder.AddMesh(plane, 0);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

//

public class Main : MonoBehaviour
{
    public Mesh mesh;
    public MeshFilter filter;

    NativeArray<Matrix4x4> output;

    struct RenderJob : IJobParallelFor
    {
        [ReadOnly]
        public float time;

        [WriteOnly]
        public NativeArray<Matrix4x4> output;

        [ReadOnly]
        public float scale;

        public void Execute(int i)
        {
            var pos = new Vector3(Mathf.Sin(time + (float)i / 100f), Mathf.Cos(time + (float)i / 100f), (float)i / 100f) * scale;
            var rot = Quaternion.AngleAxis(time * 10 + i, pos);
            output[i] = Matrix4x4.TRS(pos, rot, Vector3.one);
        }
    }
    
    IEnumerator Compute()
    {
        while (true)
        {
            var time = Time.time;
            output = new NativeArray<Matrix4x4>(computeSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            Debug.Log(time);
            var job = new RenderJob()
            {
                time = time,
                output = output,
                scale = scale
            };
            var handleCalculate = job.Schedule(computeSize, batchSize);

            yield return new WaitWhile(() => handleCalculate.IsCompleted);

            handleCalculate.Complete();

            Graphics.DrawMeshInstanced(mesh, 0, mat, output.ToArray(), 1023);
            output.Dispose();
            yield return null;
        }
    }

    void OnEnable()
    {
       // StartCoroutine(Compute());
    }

    private void Update()
    {
        //ExecJob();
    }
    
    [ContextMenu("Animate")]
    public void Animate(){
        StartCoroutine(Compute());
    }

    [ContextMenu("EXEC")]
    public void ExecJob()
    {
        // Allocate mesh data for one mesh.
        var dataArray = Mesh.AllocateWritableMeshData(1);
        var data = dataArray[0];

        // Tetrahedron vertices with positions and normals.
        // 4 faces with 3 unique vertices in each -- the faces
        // don't share the vertices since normals have to be
        // different for each face.
        data.SetVertexBufferParams(
            12,
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));

        // Four tetrahedron vertex positions:
        var sqrt075 = Mathf.Sqrt(0.75f);
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(0.5f, 0, sqrt075);
        var p3 = new Vector3(0.5f, sqrt075, sqrt075 / 3);

        // The first vertex buffer data stream is just positions;
        // fill them in.
        var pos = data.GetVertexData<Vector3>();
        pos[0] = p0;
        pos[1] = p1;
        pos[2] = p2;
        pos[3] = p0;
        pos[4] = p2;
        pos[5] = p3;
        pos[6] = p2;
        pos[7] = p1;
        pos[8] = p3;
        pos[9] = p0;
        pos[10] = p3;
        pos[11] = p1;

        // Note: normals will be calculated later in RecalculateNormals.
        // Tetrahedron index buffer: 4 triangles, 3 indices per triangle.
        // All vertices are unique so the index buffer is just a
        // 0,1,2,...,11 sequence.
        data.SetIndexBufferParams(12, IndexFormat.UInt16);
        var ib = data.GetIndexData<ushort>();
        for (ushort i = 0; i < ib.Length; ++i)
            ib[i] = i;

        // One sub-mesh with all the indices.
        data.subMeshCount = 1;
        data.SetSubMesh(0, new SubMeshDescriptor(0, ib.Length));

        // Create the mesh and apply data to it:
        var mesh = new Mesh();
        mesh.name = "Tetrahedron";
        Mesh.ApplyAndDisposeWritableMeshData(dataArray, mesh);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Graphics.DrawMeshInstanced(
            mesh,
            0,
            mat,
            new[]
            {
                Matrix4x4.identity
            });
    }

    public int computeSize = 1000000,
            batchSize = 100;

    public float scale = 3;
    public Material mat;
}
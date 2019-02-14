// <copyright>
//   Copyright (c) 2012 Codefarts
//   All rights reserved.
//   contact@codefarts.com
//   http://www.codefarts.com
// </copyright>

// Source -> http://wiki.unity3d.com/index.php/SkinMeshCombineUtility

/*IMPORTANT: READ !!!!!!
@Autor: Gabriel Santos
@Description Class that Combine the Meshes and create SubMeshes for Meshes which uses different material.
@IMPORTANT: This script is used by CombineSkinnedMeshes script!
This script was based on the MeshCombineUtility provided by Unity, I have just modified in order to create new submeshes for meshes which uses different materials
PS: It was tested with FBX files exported from 3D MAX*/
 
namespace Codefarts.GeneralTools.Utilities
{
    using UnityEngine;

    public class SkinMeshCombineUtility
    {

        public struct MeshInstance
        {
            public Mesh mesh;
            public int subMeshIndex;
            public Matrix4x4 transform;
        }

        public static Mesh Combine(MeshInstance[] combines)
        {
            var vertexCount = 0;
            var triangleCount = 0;

            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    vertexCount += combine.mesh.vertexCount;
                }
            }

            // Precomputed how many triangles we need instead

            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                }
            }

            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var tangents = new Vector4[vertexCount];
            var uv = new Vector2[vertexCount];
            var uv2 = new Vector2[vertexCount];

            int offset;

            offset = 0;
            foreach (var combine in combines)
            {
                if (combine.mesh)
                    Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
            }

            offset = 0;
            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    Matrix4x4 invTranspose = combine.transform;
                    invTranspose = invTranspose.inverse.transpose;
                    CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
                }

            }

            offset = 0;
            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    Matrix4x4 invTranspose = combine.transform;
                    invTranspose = invTranspose.inverse.transpose;
                    CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
                }

            }

            offset = 0;
            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
                }
            }

            offset = 0;
            foreach (var combine in combines)
            {
                if (combine.mesh)
                {
                    Copy(combine.mesh.vertexCount, combine.mesh.uv2, uv2, ref offset);
                }
            }

            var triangleOffset = 0;
            var vertexOffset = 0;

            var j = 0;

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.uv2 = uv2;
            mesh.tangents = tangents;

            //Setting SubMeshes
            mesh.subMeshCount = combines.Length;

            foreach (var combine in combines)
            {
                var inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                var trianglesx = new int[inputtriangles.Length];
                for (var i = 0; i < inputtriangles.Length; i++)
                {
                    //triangles[i+triangleOffset] = inputtriangles[i] + vertexOffset;
                    trianglesx[i] = inputtriangles[i] + vertexOffset;
                }

                triangleOffset += inputtriangles.Length;
                mesh.SetTriangles(trianglesx, j++);

                vertexOffset += combine.mesh.vertexCount;
            }

            mesh.name = "Combined Mesh";

            return mesh;
        }

        static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dst[i + offset] = transform.MultiplyPoint(src[i]);
            }

            offset += vertexcount;
        }

        static void CopyBoneWei(int vertexcount, BoneWeight[] src, BoneWeight[] dst, ref int offset, Matrix4x4 transform)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dst[i + offset] = src[i];
            }

            offset += vertexcount;
        }

        static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
            }
            
            offset += vertexcount;
        }

        static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
        {
            for (var i = 0; i < src.Length; i++)
            {
                dst[i + offset] = src[i];
            }
           
            offset += vertexcount;
        }

        static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
        {
            for (var i = 0; i < src.Length; i++)
            {
                var p4 = src[i];
                var p = new Vector3(p4.x, p4.y, p4.z);
                p = transform.MultiplyVector(p).normalized;
                dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
            }

            offset += vertexcount;
        }
    }
}
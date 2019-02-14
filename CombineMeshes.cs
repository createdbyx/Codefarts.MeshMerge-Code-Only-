// <copyright>
//   Copyright (c) 2012 Codefarts
//   All rights reserved.
//   contact@codefarts.com
//   http://www.codefarts.com
// </copyright>

namespace Codefarts.GeneralTools.Utilities
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class CombineMeshes
    {
        public static void Combine(MeshRenderer[] meshRenders, MeshFilter[] meshFilters, out Mesh mesh, out Material[] materials)
        {
            var meshInstanceList = new List<SkinMeshCombineUtility.MeshInstance>();
            var materialList = new List<Material>();

            // validate arguments
            if (meshFilters == null)
            {
                throw new ArgumentNullException("meshFilters");
            }

            if (meshRenders== null)
            {
                throw new ArgumentNullException("meshRenders");
            }
            
            if (meshRenders.Length != meshFilters.Length)
            {
                throw new ArgumentException("'meshRenders' & 'meshFilters' must be the same length.");
            }

            // build a list of MeshInstance types
            for (var index = 0; index < meshRenders.Length; index++)
            {
                var meshRenderer = meshRenders[index];
                
                // Getting one by one
                var meshFilter = meshFilters[index];

                // Making changes to the Skinned Renderer
                var instance = new SkinMeshCombineUtility.MeshInstance { mesh = meshFilter.sharedMesh };

                // Getting All Materials
                for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    materialList.Add(meshRenderer.sharedMaterials[i]);
                }

                if (meshRenderer != null && instance.mesh != null)
                {
                    // Getting subMesh
                    for (var i = 0; i < meshFilter.sharedMesh.subMeshCount; i++)
                    {
                        instance.subMeshIndex = i;
                        meshInstanceList.Add(instance);
                    }
                }
            }

            // combine meshes
            var newMesh = SkinMeshCombineUtility.Combine(meshInstanceList.ToArray());
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            mesh = newMesh;

            // Setting Materials
            materials = materialList.ToArray();
        }
    }
}
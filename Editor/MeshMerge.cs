namespace Codefarts.GeneralTools.Utilities
{
    using UnityEditor;

    using UnityEngine;

    public class MeshMerge
    {
        /// <summary>
        /// Provides unity menu item to open the window.
        /// </summary>
        [MenuItem("Window/Codefarts/General Utilities/Merge Selected Meshes")]
        private static void MergeSelectedMeshes()
        {
            Material[] materials;
            Mesh mesh;
            var renderers = new MeshRenderer[2];
            var meshFilters = new MeshFilter[2];

            var index = 0;

            foreach (var item in Selection.transforms)
            {
                var f = item.GetComponent<MeshFilter>();
                var r = item.GetComponent<MeshRenderer>();
                if (f != null && r != null)
                {
                    renderers[index] = r;
                    meshFilters[index] = f;
                    index++;
                }

                if (index == 2)
                {
                    break;
                }
            }

            if (index == 2)
            {
                var newObject = new GameObject("Result");
                var filter = newObject.AddComponent<MeshFilter>();
                var renderer = newObject.AddComponent<MeshRenderer>();
                CombineMeshes.Combine(renderers, meshFilters, out mesh, out materials);
                filter.sharedMesh = mesh;
                renderer.sharedMaterials = materials;
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Information",
                    "Must select 2 game objects that have a MeshRenderer & MeshFilter components on them!",
                    "Accept");
            }
        }
    }
}

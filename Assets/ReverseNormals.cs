using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReverseNormals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		#if !UNITY_EDITOR
		FlipNormals();
		#endif
	}

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
			FlipNormals();
        }
    }

    public void FlipNormals()
    {
		MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (filter != null)
		{
			Mesh mesh = filter.sharedMesh;

			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; i++)
				normals[i] = -normals[i];
			mesh.normals = normals;

			for (int m = 0; m < mesh.subMeshCount; m++)
			{
				int[] triangles = mesh.GetTriangles(m);
				for (int i = 0; i < triangles.Length; i += 3)
				{
					int temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]

public class MeshQuad : MonoBehaviour {

	public Color[] colorList = new Color[4]
	{
		new Color(1,1,1,1),
		new Color(1,1,1,1),
		new Color(1,1,1,1),
		new Color(1,1,1,1),
	};
	public Vector2[] uvList = new Vector2[4]
	{
		new Vector2(0,1),
		new Vector2(1,1),
		new Vector2(0,0),
		new Vector2(1,0),
	};

	// Use this for initialization
	void Awake ()
	{
		initMesh();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void initMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3(-0.5f,  0.5f, 0f),
			new Vector3( 0.5f,  0.5f, 0f),
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3( 0.5f, -0.5f, 0f),
		};

		mesh.colors = colorList;
		mesh.uv = uvList;

		mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };

		GetComponent<MeshFilter>().mesh = mesh;
	}

	public void updateUv(Vector2[] uvs)
	{
		Mesh mesh;
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.uv = uvs;
	}

	public void updateColor(Color[] colors)
	{
		Mesh mesh;
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.colors = colors;
	}
}

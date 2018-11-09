using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteCubeFaceRender : MonoBehaviour {

	public GameObject renderTexture;
	public Camera renderCamera;
	public GameObject target;

	public Material cameraMat;

	// Use this for initialization
	void Start () {
		if (cameraMat.mainTexture == null)
		{
			cameraMat.mainTexture = new RenderTexture(Screen.width, Screen.height, 24);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Dot(Camera.main.transform.forward, target.transform.forward) > 0)
		{
			renderCamera.targetTexture = (RenderTexture)cameraMat.mainTexture;
			renderCamera.enabled = true;
		}
		else
		{
			renderCamera.targetTexture = null;
			renderCamera.enabled = false;
		}

		renderCamera.transform.position = Camera.main.transform.position;
		renderCamera.transform.rotation = Camera.main.transform.rotation;
	}
}

using UnityEngine;
using System.Collections;

public class ffcsharp : MonoBehaviour {
	float scrollSpeed_X = 0f;
	float scrollSpeed_Y = 0.5f;
	Renderer render;

	void Start(){
		render = GetComponent<Renderer>();
	}
	
	void Update() {
		float offsetX = Time.time * scrollSpeed_X;
		float offsetY = Time.time * scrollSpeed_Y;
		render.material.mainTextureOffset = new Vector2 (offsetX,offsetY);
	}
}

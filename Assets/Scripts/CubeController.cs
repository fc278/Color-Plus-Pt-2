using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {
	public int cubePositionX, cubePositionY;
	GameController myGameController;
	public bool active = false;
//	public int x, y;

	// Use this for initialization
	void Start () {
		myGameController = GameObject.Find ("GameControllerObject").GetComponent<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown () {
		myGameController.ProcessClick (gameObject, cubePositionX, cubePositionY, gameObject.GetComponent<Renderer>().material.color, active);
	}
}

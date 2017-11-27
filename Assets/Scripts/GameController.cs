using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public GameObject cubePrefab, nextCubePrefab;
	float gameLength = 60;
	Vector3 cubePosition;
	Vector3 nextCubePosition = new Vector3 (7, 11, 0);
	int gridX = 8;
	int gridY = 5;
	GameObject[ , ] grid;
	GameObject nextCube;
	float turnLength = 2;
	int numOfTurns = 0;
	Color[] randomCubeColors = { Color.red, Color.blue, Color.green, Color.magenta, Color.yellow };
	int score = 0;
	GameObject activeCube = null;


	// Use this for initialization
	void Start () {
		CreateGrid ();
	}

	void CreateGrid () {
		// create 8 x 5 grid
		grid = new GameObject[gridX, gridY];

		for (int x = 0; x < gridX; x++) {
			for (int y = 0; y < gridY; y++) {
				cubePosition = new Vector3 (x * 2, y * 2, 0);
				grid[x,y] = Instantiate (cubePrefab, cubePosition, Quaternion.identity);
				grid[x,y].GetComponent<CubeController> ().cubePositionX = x;
				grid[x,y].GetComponent<CubeController> ().cubePositionY = y;

			}
		}

	}

	void SpawnNextCube () {
		nextCube = Instantiate (nextCubePrefab, nextCubePosition, Quaternion.identity);
		nextCube.GetComponent<Renderer> ().material.color = randomCubeColors [ Random.Range(0, randomCubeColors.Length) ];
	}

	void EndGame(bool win) {
		if (win) {
			print("You win!");
		} else {
			print("You lose... please try again.");
		}
	}

	GameObject PickWhiteCube (List<GameObject> whiteCubes){
		// no white cubes in row
		if (whiteCubes.Count == 0) {
			// error value
			return null;
		}

		// pick random white cube
		return whiteCubes [Random.Range (0, whiteCubes.Count)];

	}
		


	GameObject FindAvailableCube (int y) {
		List<GameObject> whiteCubes = new List<GameObject> ();

		// makes a list of white cubes
		for (int x = 0; x < gridX; x++) {
			if (grid [x, y].GetComponent<Renderer> ().material.color == Color.white) {
				whiteCubes.Add (grid [x, y]);
			}
		}

		return PickWhiteCube (whiteCubes);	
	}


	GameObject FindAvailableCube () {
		List<GameObject> whiteCubes = new List<GameObject> ();

		// makes a list of white cubes
		for (int y = 0; y < gridY; y++) {
			for (int x = 0; x < gridX; x++) {
				if (grid [x, y].GetComponent<Renderer> ().material.color == Color.white) {
					whiteCubes.Add (grid [x, y]);
				}
			}
		}

		return PickWhiteCube (whiteCubes);
	}
			

	void SetCubeColor (GameObject myCube, Color color) {
		// no available cube in row
		if (myCube == null) {
			EndGame (false);
		} else {
			// make chosen cube to be the nextCube's color
			myCube.GetComponent<Renderer> ().material.color = color;
			Destroy (nextCube);
			nextCube = null;
		}
	}


	void PlaceNextCube (int y) {
		GameObject whiteCube = FindAvailableCube (y);
		SetCubeColor (whiteCube, nextCube.GetComponent<Renderer>().material.color);
	}

	void AddRandomBlackCube() {
		GameObject whiteCube = FindAvailableCube ();
		SetCubeColor (whiteCube, Color.black);
	}

	void ProcessKeyboardInput () {
		int numKeyPressed = 0;
		// tracks number input of 1-5
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
			numKeyPressed = 1;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
			numKeyPressed = 2;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
			numKeyPressed = 3;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
			numKeyPressed = 4;
		}
		if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
			numKeyPressed = 5;
		}

		// if there's still a next cube and player pressed a valid num key
		if (nextCube != null && numKeyPressed != 0) {
			
			// subtract 1 because grid array has a 0-based index
			// place it in specified row
			PlaceNextCube (numKeyPressed-1);
		}

		// if row is full, end the game

	}

	public void ProcessClick (GameObject clickedCube, int x, int y, Color cubeColor, bool active) {
		if (cubeColor != Color.white && cubeColor != Color.black) {

			if (active) {
				// if there was an active cube, deactivate it
				clickedCube.transform.localScale /= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = false;
				activeCube = null;

			} else {
				// deactivate previous active cube(s)
				if (activeCube != null) {
					activeCube.transform.localScale /= 1.5f;
					activeCube.GetComponent<CubeController> ().active = false;

				}

				// activate new clickedCube
				clickedCube.transform.localScale *= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = true;
				activeCube = clickedCube;
			}

		} else if (cubeColor == Color.white) {

			int distanceX = clickedCube.GetComponent<CubeController> ().cubePositionX - activeCube.GetComponent<CubeController> ().cubePositionX;
			int distanceY = clickedCube.GetComponent<CubeController> ().cubePositionY - activeCube.GetComponent<CubeController> ().cubePositionY;

			// if we are within 1 unit - includes diagonals
			if (Mathf.Abs(distanceY) <= 1 && Mathf.Abs(distanceX) <= 1) {
				// activate clicked cube
				clickedCube.GetComponent<Renderer> ().material.color = activeCube.GetComponent<Renderer> ().material.color;
				clickedCube.transform.localScale *= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = true;


				// deactivate previous active cube and make it white
				activeCube.GetComponent<Renderer> ().material.color = Color.white;
				activeCube.transform.localScale /= 1.5f;
				activeCube.GetComponent<CubeController> ().active = false;

				// keep track of new active cube
				activeCube = clickedCube;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		ProcessKeyboardInput();

		// as time increases by 2 seconds, number of turns increases by 1
		if (Time.time > turnLength * numOfTurns) {
			numOfTurns++;

			// if nextCube still exists, 
			if (nextCube != null) {
				score -= 1;
				AddRandomBlackCube ();
			}

			SpawnNextCube ();

			}
		}

		
	}


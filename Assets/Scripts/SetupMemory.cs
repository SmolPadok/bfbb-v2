using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMemory : MonoBehaviour {

	public bool[] playerActive = new bool[4];
	SetupConfiguration setup;

	// Use this for initialization
	void Start () {
		
		DontDestroyOnLoad(gameObject);
		setup = GameObject.FindGameObjectWithTag("Setup").GetComponent<SetupConfiguration>();

	}
	
	// Update is called once per frame
	public void UpdateSetup () {

	gameObject.tag = "OldSetupMemory";
	for (int i = 0; i < playerActive.Length; i++)
	{
		playerActive[i] = setup.playerActive[i];
	}
		
	}
}

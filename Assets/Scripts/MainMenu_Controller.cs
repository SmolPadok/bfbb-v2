using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour {

	GameObject transition;
	GameObject cam;
	SetupMemory setupMem;

	void Awake(){

		transition = GameObject.FindGameObjectWithTag("Transition");
		cam = GameObject.Find("Main Camera");
		setupMem = GameObject.FindGameObjectWithTag("SetupMemory").GetComponent<SetupMemory>();
		GameObject oldSetupMem = GameObject.FindGameObjectWithTag("OldSetupMemory");
		if(oldSetupMem != null){
			Destroy(oldSetupMem);
		}


	}
	public void LoadScene(string sceneName){

		cam.GetComponent<Animator>().SetTrigger("GameView");
		setupMem.UpdateSetup();
		transition.GetComponent<Transition_Controller>().LoadSceneOnTransition(sceneName);

	}
}

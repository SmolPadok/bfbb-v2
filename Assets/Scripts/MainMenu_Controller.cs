using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour {

	GameObject canvas;
	GameObject transition;
	GameObject cam;
	GameObject changelog;
	GameObject logo;
	GameObject betaSetup;
	GameObject playButton;
	SetupMemory setupMem;

	void Awake(){

		canvas = GameObject.Find("Canvas");
		transition = GameObject.FindGameObjectWithTag("Transition");
		cam = GameObject.Find("Main Camera");
		betaSetup = GameObject.FindGameObjectWithTag("Setup");
		setupMem = GameObject.FindGameObjectWithTag("SetupMemory").GetComponent<SetupMemory>();
		changelog = canvas.transform.Find("Changelog").gameObject;
		logo = canvas.transform.Find("Logo").gameObject;
		playButton = canvas.transform.Find("PlayButton").gameObject;
		setupMem.setup = betaSetup.GetComponent<SetupConfiguration>();
		GameObject oldSetupMem = GameObject.FindGameObjectWithTag("OldSetupMemory");
		if(oldSetupMem != null){
			Destroy(oldSetupMem);
		}
		
		betaSetup.SetActive(false);


	}
	public void BetaSetup(){
		changelog.GetComponent<Animator>().SetTrigger("End");
		logo.GetComponent<Animator>().SetTrigger("End");
		betaSetup.SetActive(true);
		playButton.SetActive(false);
	}

	public void LoadScene(string sceneName){

		cam.GetComponent<Animator>().SetTrigger("GameView");
		setupMem.UpdateSetup();
		transition.GetComponent<Transition_Controller>().LoadSceneOnTransition(sceneName);

	}
}

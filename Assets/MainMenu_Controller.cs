using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour {

	GameObject transition;
	GameObject cam;

	void Awake(){

		transition = GameObject.FindGameObjectWithTag("Transition");
		cam = GameObject.Find("Main Camera");

	}
	public void LoadScene(string sceneName){

		cam.GetComponent<Animator>().SetTrigger("GameView");
		transition.GetComponent<Transition_Controller>().LoadSceneOnTransition(sceneName);

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

	[Header("Active Players")]

	public bool player1;
	public bool player2, player3, player4;

	[Header("Override Disability (for debug)")]

	public bool player1Or;
	public bool player2Or, player3Or, player4Or;

	public Camera_Controller cam;
	GameObject play1, play2, play3, play4;
	GameObject ind1, ind2, ind3, ind4;
	GameObject stat1, stat2, stat3, stat4;
	Player_Controller p1, p2, p3, p4;
	GameObject canvas, playerStats;
	TextMeshProUGUI countdown;
	GameObject transition;


	// Use this for initialization
	void Start () {
		
		cam = GameObject.Find("Main Camera").GetComponent<Camera_Controller>();
		play1 = GameObject.Find("Player 1");
		play2 = GameObject.Find("Player 2");
		play3 = GameObject.Find("Player 3");
		play4 = GameObject.Find("Player 4");
		p1 = play1.GetComponent<Player_Controller>();
		p2 = play2.GetComponent<Player_Controller>();
		p3 = play3.GetComponent<Player_Controller>();
		p4 = play4.GetComponent<Player_Controller>();
		ind1 = GameObject.Find("Indicator1");
		ind2 = GameObject.Find("Indicator2");
		ind3 = GameObject.Find("Indicator3");
		ind4 = GameObject.Find("Indicator4");
		stat1 = GameObject.Find("Stats1");
		stat2 = GameObject.Find("Stats2");
		stat3 = GameObject.Find("Stats3");
		stat4 = GameObject.Find("Stats4");
		canvas = GameObject.Find("Canvas");
		playerStats = canvas.transform.Find("Stats").gameObject;
		countdown = canvas.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
		transition = GameObject.FindGameObjectWithTag("Transition");

		CheckPlayers();
		
		StartRound();

	}

	void Update(){

		if(Input.GetKeyDown(KeyCode.Escape)){

			LoadScene("MainMenuScene");

		}

	}
	void LoadScene(string sceneName){

		transition.GetComponent<Transition_Controller>().LoadSceneOnTransition(sceneName);

	}

	void StartRound(){
	
	playerStats.SetActive(false);
	p1.enabled = false;
	p2.enabled = false;
	p3.enabled = false;
	p4.enabled = false;
	cam.targets.Clear();

	StartCoroutine(CountdownRound());

	}

	IEnumerator CountdownRound(){

		FocusFirstPlayer();
		countdown.text = "3";

		yield return new WaitForSeconds(1f);

		countdown.text = "2";

		yield return new WaitForSeconds(1f);
		
		countdown.text = "1";
		FocusAllPlayers();
		
		yield return new WaitForSeconds(1f);

		countdown.text = "GO!";
		playerStats.SetActive(true);
		EnablePlayers();

		yield return new WaitForSeconds(1f);

		countdown.gameObject.SetActive(false);

	}

	void CheckPlayers(){

		if(!player1){
			play1.SetActive(false);
			ind1.SetActive(false);
			stat1.SetActive(false);
		}
		if(!player2){
			play2.SetActive(false);
			ind2.SetActive(false);
			stat2.SetActive(false);
		}
		if(!player3){
			play3.SetActive(false);
			ind3.SetActive(false);
			stat3.SetActive(false);
		}
		if(!player4){
			play4.SetActive(false);
			ind4.SetActive(false);
			stat4.SetActive(false);
		}

	}

	void FocusFirstPlayer(){

		if(player1){
		cam.targets.Add(play1.transform);
		}else if(player2){
		cam.targets.Add(play2.transform);
		}else if(player3){
		cam.targets.Add(play2.transform);
		}else if(player4){
		cam.targets.Add(play2.transform);
		}else{
		Debug.LogError("Wait a minute, no active players!");
		return;
		}

	}

	void FocusAllPlayers(){
		if(player1){
		cam.targets.Add(play1.transform);
		}
		if(player2){
		cam.targets.Add(play2.transform);
		}
		if(player3){
		cam.targets.Add(play3.transform);
		}
		if(player4){
		cam.targets.Add(play4.transform);
		}
	}

	void EnablePlayers(){
		if(player1){
			p1.enabled = true;
			if(player1Or){
				p1.debugDisable = true;
			}
		}
		if(player2){
			p2.enabled = true;
			if(player2Or){
				p2.debugDisable = true;
			}
		}
		if(player3){
			p3.enabled = true;
			if(player3Or){
				p3.debugDisable = true;
			}
		}
		if(player4){
			p4.enabled = true;
			if(player4Or){
				p4.debugDisable = true;
			}
		}
	}
}

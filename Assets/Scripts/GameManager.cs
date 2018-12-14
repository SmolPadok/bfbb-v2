using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
	
	public Camera_Controller cam;
	public GameObject miniProfiler;

	[Header("Arena Bounds")]
	public float boundsUp;
	public float boundsDown, boundsLeft, boundsRight;

	[Header("Active Players")]

	public bool player1Active;
	public bool player2Active, player3Active, player4Active;

	[Header("Override Disability (for debug)")]

	public bool player1Or;
	public bool player2Or, player3Or, player4Or;


	[Header("Data")]

	public float player1Damage = 0f;
	public float player2Damage = 0f, player3Damage = 0f, player4Damage = 0f;

	[Space]

	public float lerpTime;
	
	float player1LerpDamage = 0f, player2LerpDamage = 0f, player3LerpDamage = 0f, player4LerpDamage = 0f;

	float lerpVel1, lerpVel2, lerpVel3, lerpVel4;

	GameObject player1, player2, player3, player4;
	GameObject ind1, ind2, ind3, ind4;
	GameObject stat1, stat2, stat3, stat4;
	GameObject statDamage1, statDamage2, statDamage3, statDamage4;
	Player_Controller p1, p2, p3, p4;
	GameObject canvas, playerStats;
	TextMeshProUGUI countdown;
	GameObject transition, parent;

	//[Header("Inputs")]

	//public string[] p1Inputs = new string[];




	// Use this for initialization
	void Start () {
		
		cam = GameObject.Find("Main Camera").GetComponent<Camera_Controller>();
		parent = GameObject.Find("GameObjects");
		player1 = GameObject.Find("Player 1");
		player2 = GameObject.Find("Player 2");
		player3 = GameObject.Find("Player 3");
		player4 = GameObject.Find("Player 4");
		p1 = player1.GetComponent<Player_Controller>();
		p2 = player2.GetComponent<Player_Controller>();
		p3 = player3.GetComponent<Player_Controller>();
		p4 = player4.GetComponent<Player_Controller>();
		ind1 = GameObject.Find("Indicator1");
		ind2 = GameObject.Find("Indicator2");
		ind3 = GameObject.Find("Indicator3");
		ind4 = GameObject.Find("Indicator4");
		stat1 = GameObject.Find("Stats1");
		stat2 = GameObject.Find("Stats2");
		stat3 = GameObject.Find("Stats3");
		stat4 = GameObject.Find("Stats4");
		statDamage1 = stat1.transform.Find("Damage").gameObject;
		statDamage2 = stat2.transform.Find("Damage").gameObject;
		statDamage3 = stat3.transform.Find("Damage").gameObject;
		statDamage4 = stat4.transform.Find("Damage").gameObject;
		canvas = GameObject.Find("Canvas");
		playerStats = canvas.transform.Find("Stats").gameObject;
		countdown = canvas.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
		transition = GameObject.FindGameObjectWithTag("Transition");

		InitialCheckPlayers();
		
		StartRound();

	}

	void Update(){

		if(Input.GetKeyDown(KeyCode.Escape)){

			LoadScene("MainMenuScene");

		}
		if(Input.GetKeyDown(KeyCode.M)){

			if(!miniProfiler.activeSelf){
				miniProfiler.SetActive(true);
			}else{
				miniProfiler.SetActive(false);
			}

		}

		lerpDamageStat();
		waitForKill();

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

	void InitialCheckPlayers(){

		if(!player1Active){
			player1.SetActive(false);
			ind1.SetActive(false);
			stat1.SetActive(false);
		}
		if(!player2Active){
			player2.SetActive(false);
			ind2.SetActive(false);
			stat2.SetActive(false);
		}
		if(!player3Active){
			player3.SetActive(false);
			ind3.SetActive(false);
			stat3.SetActive(false);
		}
		if(!player4Active){
			player4.SetActive(false);
			ind4.SetActive(false);
			stat4.SetActive(false);
		}

	}

	void FocusFirstPlayer(){

		if(player1Active){
		cam.targets.Add(player1.transform);
		}else if(player2Active){
		cam.targets.Add(player2.transform);
		}else if(player3Active){
		cam.targets.Add(player3.transform);
		}else if(player4Active){
		cam.targets.Add(player4.transform);
		}else{
		Debug.LogError("Wait a minute, no active players!");
		return;
		}

	}

	void FocusAllPlayers(){
		if(player1Active && !cam.targets.Contains(player1.transform)){
		cam.targets.Add(player1.transform);
		}
		if(player2Active && !cam.targets.Contains(player2.transform)){
		cam.targets.Add(player2.transform);
		}
		if(player3Active && !cam.targets.Contains(player3.transform)){
		cam.targets.Add(player3.transform);
		}
		if(player4Active && !cam.targets.Contains(player4.transform)){
		cam.targets.Add(player4.transform);
		}
	}

	void EnablePlayers(){
		if(player1Active){
			p1.enabled = true;
			if(player1Or){
				p1.debugDisable = true;
			}
		}
		if(player2Active){
			p2.enabled = true;
			if(player2Or){
				p2.debugDisable = true;
			}
		}
		if(player3Active){
			p3.enabled = true;
			if(player3Or){
				p3.debugDisable = true;
			}
		}
		if(player4Active){
			p4.enabled = true;
			if(player4Or){
				p4.debugDisable = true;
			}
		}
	}

	void EliminatePlayer(Player_Controller refPlayer){
		
            GameObject smashDeath = Instantiate(refPlayer.deathConfetti, refPlayer.transform.position, Quaternion.identity, refPlayer.effectStash);
            Vector3 relativePos = parent.transform.position - smashDeath.transform.position;
            smashDeath.transform.rotation = Quaternion.LookRotation(relativePos);
            smashDeath.SetActive(true);

			refPlayer.stat.SetActive(false);
			refPlayer.indicator.SetActive(false);

			StartCoroutine(SecondsOfSilence(refPlayer));

            refPlayer.gameObject.SetActive(false);

	}

	IEnumerator SecondsOfSilence(Player_Controller refPlayer){

		yield return new WaitForSeconds(0.5f);

		RemoveCamTarget(refPlayer.transform);

	}

	public void RemoveCamTarget(Transform target){

		cam.targets.Remove(target);

	}

	void lerpDamageStat(){
		if(player1Active){
		player1LerpDamage = Mathf.SmoothDamp(player1LerpDamage, player1Damage, ref lerpVel1, lerpTime);
		player1LerpDamage = Mathf.Round(player1LerpDamage * 10) / 10;
		statDamage1.GetComponent<TextMeshProUGUI>().text = (player1LerpDamage).ToString() + "%";
		}
		if(player2Active){
		player2LerpDamage = Mathf.SmoothDamp(player2LerpDamage, player2Damage, ref lerpVel2, lerpTime);
		player2LerpDamage = Mathf.Round(player2LerpDamage * 10) / 10;
		statDamage2.GetComponent<TextMeshProUGUI>().text = player2LerpDamage.ToString() + "%";
		}
		if(player3Active){
		player3LerpDamage = Mathf.SmoothDamp(player3LerpDamage, player3Damage, ref lerpVel3, lerpTime);
		player3LerpDamage = Mathf.Round(player3LerpDamage * 10) / 10;
		statDamage3.GetComponent<TextMeshProUGUI>().text = player3LerpDamage.ToString() + "%";
		}
		if(player4Active){
		player4LerpDamage = Mathf.SmoothDamp(player4LerpDamage, player4Damage, ref lerpVel4, lerpTime);
		player4LerpDamage = Mathf.Round(player4LerpDamage * 10) / 10;
		statDamage4.GetComponent<TextMeshProUGUI>().text = player4LerpDamage.ToString() + "%";
		}
	}

	void waitForKill(){
		if(player1.activeSelf){
        if(p1.transform.position.x > boundsRight || p1.transform.position.x < boundsLeft
		|| p1.transform.position.y > boundsUp || p1.transform.position.y < boundsDown){
            EliminatePlayer(p1);
        	}
		}
		if(player2.activeSelf){
        if(p2.transform.position.x > boundsRight || p2.transform.position.x < boundsLeft
		|| p2.transform.position.y > boundsUp || p2.transform.position.y < boundsDown){
            EliminatePlayer(p2);
			}
		}
		if(player3.activeSelf){
        if(p3.transform.position.x > boundsRight || p3.transform.position.x < boundsLeft
		|| p3.transform.position.y > boundsUp || p3.transform.position.y < boundsDown){
            EliminatePlayer(p3);
			}
		}
		if(player4.activeSelf){
        if(p4.transform.position.x > boundsRight || p4.transform.position.x < boundsLeft
		|| p4.transform.position.y > boundsUp || p4.transform.position.y < boundsDown){
            EliminatePlayer(p4);
			}
		}
	}

	public void HazardDeath(Player_Controller refPlayer){
		EliminatePlayer(refPlayer);
	}

	public void UpdateDamage(int playerIndex){
		
		if(playerIndex == 1){
			player1Damage = Mathf.Round(player1.GetComponent<Player_Controller>().damage * 10) / 10;
			statDamage1.GetComponent<Animator>().SetTrigger("Hit");
			statDamage1.GetComponent<Animator>().SetFloat("Damage", player1Damage);
		}
		if(playerIndex == 2){
			player2Damage = Mathf.Round(player2.GetComponent<Player_Controller>().damage * 10) / 10;
			statDamage2.GetComponent<Animator>().SetTrigger("Hit");
			statDamage2.GetComponent<Animator>().SetFloat("Damage", player2Damage);
		}
		if(playerIndex == 3){
			player3Damage = Mathf.Round(player3.GetComponent<Player_Controller>().damage * 10) / 10;
			statDamage3.GetComponent<Animator>().SetTrigger("Hit");
			statDamage3.GetComponent<Animator>().SetFloat("Damage", player3Damage);
		}
		if(playerIndex == 4){
			player4Damage = Mathf.Round(player4.GetComponent<Player_Controller>().damage * 10) / 10;
			statDamage4.GetComponent<Animator>().SetTrigger("Hit");
			statDamage4.GetComponent<Animator>().SetFloat("Damage", player4Damage);
		}
	}
}

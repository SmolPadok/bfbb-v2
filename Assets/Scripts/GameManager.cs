using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public BattleSceneID battleSceneID;
	public Camera_Controller cam;
	public GameObject miniProfiler;
	SetupMemory setup;

	[Header("Arena Bounds")]
	public float boundsUp;
	public float boundsDown, boundsLeft, boundsRight;

	[Header("Active Players")]

	public bool[] playerActive = new bool[4];

	[Header("Override Disability (for debug)")]

	public bool[] playerOr = new bool[4];


	[Header("Data")]

	public float[] playerDamage = new float[4];

	[Space]

	public float lerpTime;

	[Space]

	public int setLives;

	float[] playerLerpDamage = new float[4];
	float[] velRef = new float[4];

	GameObject[] players;
	GameObject[] indicators;
	GameObject[] stats;
	int[] playerLives = {0, 0, 0, 0};
	GameObject[] statDamage = new GameObject[4];
	GameObject[] statLives = new GameObject[4];
	Player_Controller[] playerControl = new Player_Controller[4];
	GameObject canvas, playerStats, commentary, betaEnd;
	TextMeshProUGUI countdown;
	GameObject transition, parent;
	GameObject lifeSprite;
	bool[] statRecovery = new bool[4];

	// Use this for initialization
	void Start () {

		GameObject checkSetup = GameObject.FindGameObjectWithTag("OldSetupMemory");
		if(checkSetup != null){
		Destroy(checkSetup);
		setup = GameObject.FindGameObjectWithTag("OldSetupMemory").GetComponent<SetupMemory>();
		}
		cam = GameObject.Find("Main Camera").GetComponent<Camera_Controller>();
		parent = GameObject.Find("GameObjects");
		players = GameObject.FindGameObjectsWithTag("Player").OrderBy(go=>go.name).ToArray();
		indicators = GameObject.FindGameObjectsWithTag("Indicator").OrderBy(go=>go.name).ToArray();
		stats = GameObject.FindGameObjectsWithTag("Stats").OrderBy(go=>go.name).ToArray();
		for (int c = 0; c < players.Length; c++)
		{
			playerControl[c] = players[c].GetComponent<Player_Controller>();
			statDamage[c] = stats[c].transform.Find("Damage").gameObject;
			statLives[c] = stats[c].transform.Find("Lives").gameObject;
		}
		canvas = GameObject.Find("Canvas");
		playerStats = canvas.transform.Find("Stats").gameObject;
		countdown = canvas.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
		transition = GameObject.FindGameObjectWithTag("Transition");
		lifeSprite = canvas.transform.Find("LifeSprite").gameObject;
		commentary = canvas.transform.Find("CommentaryPanel").gameObject;
		betaEnd = canvas.transform.Find("BetaEnd").gameObject;

		RunSetup();

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
	
	void LoadBattleScene(){

		GameObject battleScene = Instantiate(battleSceneID.asset, transform.position, Quaternion.identity, parent.transform);
		for (int i = 0; i < players.Length; i++)
		{
			players[i].transform.position = battleSceneID.playerPosition[i];
			playerLives[i] = setLives;
		}
		UpdateLives();

	}

	void StartRound(){
	
	betaEnd.SetActive(false);
	playerStats.SetActive(false);
	for (int i = 0; i < playerControl.Length; i++)
	{
		playerControl[i].enabled = false;
	}
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

		commentary.GetComponent<Animator>().SetTrigger("End");
		countdown.gameObject.GetComponent<Animator>().SetTrigger("End");

	}

	void RunSetup(){

		if(setup != null){
		for (int i = 0; i < players.Length; i++)
		{
			playerActive[i] = setup.playerActive[i];
		}
		}else{
		Debug.Log("No setup memory found. Using pre-assigned settings.");
		}

		LoadBattleScene();

	}

	void InitialCheckPlayers(){
		for (int i = 0; i < players.Length; i++)
		{
			if(!playerActive[i]){
				
			players[i].SetActive(false);
			indicators[i].SetActive(false);
			stats[i].SetActive(false);
			}
		}
	}

	void FocusFirstPlayer(){

		for (int i = 0; i < players.Length; i++)
		{
			if(playerActive[i]){
				cam.targets.Add(players[i].transform);
				break;
			}
		}
		/*
		if(player1Active){
		cam.targets.Add(players[0].transform);
		}else if(player2Active){
		cam.targets.Add(players[1].transform);
		}else if(player3Active){
		cam.targets.Add(players[2].transform);
		}else if(player4Active){
		cam.targets.Add(players[3].transform);
		}else{
		Debug.LogError("Wait a minute, no active players!");
		return;
		}
		*/
	}

	void FocusAllPlayers(){
		for (int i = 0; i < players.Length; i++)
		{
			if(playerActive[i] && !cam.targets.Contains(players[i].transform)){
			cam.targets.Add(players[i].transform);
			}
		}
	}

	void EnablePlayers(){
		for (int i = 0; i < players.Length; i++)
		{
			if(playerActive[i]){
				playerControl[i].enabled = true;
				if(playerOr[i]){
					playerControl[i].debugDisable = true;
				}
			}
		}
	}

	void KillPlayer(Player_Controller refPlayer){
		
		for (int i = 0; i < players.Length; i++)
		{
			if(refPlayer.playerNumber == (i + 1)){

				Debug.Log("Killed Player " + playerControl[i].playerNumber);
            	GameObject smashDeath = Instantiate(playerControl[i].deathConfetti, playerControl[i].transform.position, Quaternion.identity, playerControl[i].effectStash);
            	Vector3 relativePos = parent.transform.position - smashDeath.transform.position;
            	smashDeath.transform.rotation = Quaternion.LookRotation(relativePos);
            	smashDeath.SetActive(true);
				cam.DeathShake();

				playerControl[i].indicator.SetActive(false);
            	players[i].SetActive(false);
				statRecovery[i] = true;
				statDamage[i].GetComponent<Animator>().SetTrigger("Recovery");

				playerLives[i] -= 1;
				UpdateLives();
				
				if(playerLives[i] > 0){
				StartCoroutine(RespawnPlayer(i));
				StartCoroutine(FancyRecovery(i));
				}else{
				StartCoroutine(SecondsOfSilence(i));
				}
				break;

			}
		}

	}

	IEnumerator RespawnPlayer(int index){

		yield return new WaitForSeconds(1f);

		playerDamage[index] = 0;
		playerControl[index].damage = 0;
		players[index].transform.position = new Vector3(Random.Range(-25f,25f), 30f, 0f);
		players[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
		players[index].SetActive(true);
		indicators[index].SetActive(true);
		statRecovery[index] = false;

	}
	//Pay respects to the dead player
	IEnumerator SecondsOfSilence(int index){

		statDamage[index].GetComponent<TextMeshProUGUI>().text = "ERROR";
		yield return new WaitForSeconds(0.05f);
		statDamage[index].GetComponent<TextMeshProUGUI>().text = "Connection lost";
		yield return new WaitForSeconds(0.05f);
		statDamage[index].GetComponent<TextMeshProUGUI>().text = "Recovery failed";

		yield return new WaitForSeconds(0.4f);

	    stats[index].SetActive(false);
		RemoveCamTarget(players[index].transform);
		if(cam.targets.Count == 1){

			StartCoroutine(EndGame());

		}

	}

	IEnumerator FancyRecovery(int index){

		TextMeshProUGUI damage = statDamage[index].GetComponent<TextMeshProUGUI>();

		damage.text = "ERROR";
		yield return new WaitForSeconds(0.05f);
		damage.text = "Connection lost";
		yield return new WaitForSeconds(0.1f);
		damage.text = "Executing recovery.";
		yield return new WaitForSeconds(0.2f);
		damage.text = "Executing recovery..";
		yield return new WaitForSeconds(0.2f);
		damage.text = "Executing recovery...";
		yield return new WaitForSeconds(0.4f);
		damage.text = "Online";

	}

	public void RemoveCamTarget(Transform target){

		cam.targets.Remove(target);

	}

	void lerpDamageStat(){
		for (int i = 0; i < players.Length; i++)
		{
			if(playerActive[i]){
				if(!statRecovery[i]){
				playerLerpDamage[i] = Mathf.SmoothDamp(playerLerpDamage[i], playerDamage[i], ref velRef[i], lerpTime);
				playerLerpDamage[i] = Mathf.Round(playerLerpDamage[i] * 10) / 10;
				statDamage[i].GetComponent<TextMeshProUGUI>().text = (playerLerpDamage[i]).ToString() + "%";
				}
			}
		}
	}

	void waitForKill(){

		for (int i = 0; i < players.Length; i++)
		{
		if(players[i].activeSelf){
        if(playerControl[i].transform.position.x > boundsRight || playerControl[i].transform.position.x < boundsLeft
		|| playerControl[i].transform.position.y > boundsUp || playerControl[i].transform.position.y < boundsDown){
            KillPlayer(playerControl[i]);
        	}
		}
		}
	}

	void UpdateLives(){
		for (int i = 0; i < playerLives.Length; i++)
		{
			for (int x = 0; x < statLives[i].transform.childCount; x++)
			{
				Destroy(statLives[i].transform.GetChild(x).gameObject);
			}
			for (int y = 0; y < playerLives[i]; y++)
			{
				GameObject liveSprite = Instantiate(lifeSprite, Vector3.zero, Quaternion.identity, statLives[i].transform);
				liveSprite.GetComponent<Image>().sprite = playerControl[i].characterID.icon;
				liveSprite.GetComponent<Image>().preserveAspect = true;
				liveSprite.SetActive(true);
			}
		}
		Debug.Log("Removed legacy life sprites");
	}

	public void HazardDeath(Player_Controller refPlayer){
		KillPlayer(refPlayer);
	}

	public void UpdateDamage(int playerIndex){
		for (int i = 0; i < players.Length; i++)
		{
			if(playerIndex == (i + 1)){
				playerDamage[i] = Mathf.Round(playerControl[i].damage * 10) / 10;
				statDamage[i].GetComponent<Animator>().SetTrigger("Hit");
				statDamage[i].GetComponent<Animator>().SetFloat("Damage", playerDamage[i]);
				break;
			}
		}
	}

	IEnumerator EndGame(){

		countdown.text = "GAME";
		countdown.gameObject.GetComponent<Animator>().SetTrigger("Start");
		commentary.GetComponent<Animator>().SetTrigger("Start");
		cam.targets[0].GetComponent<Player_Controller>().enabled = false;
		
		yield return new WaitForSeconds(1f);

		betaEnd.SetActive(true);

	}
}

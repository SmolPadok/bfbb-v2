using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
	Player_Controller[] playerControl = new Player_Controller[4];
	GameObject canvas, playerStats;
	TextMeshProUGUI countdown;
	GameObject transition, parent;

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
		}
		canvas = GameObject.Find("Canvas");
		playerStats = canvas.transform.Find("Stats").gameObject;
		countdown = canvas.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
		transition = GameObject.FindGameObjectWithTag("Transition");

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

	}

	void StartRound(){
	
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

		countdown.gameObject.SetActive(false);

	}

	void RunSetup(){

		if(setup != null){
		for (int i = 0; i < playerActive.Length; i++)
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

	void EliminatePlayer(Player_Controller refPlayer){
		
            GameObject smashDeath = Instantiate(refPlayer.deathConfetti, refPlayer.transform.position, Quaternion.identity, refPlayer.effectStash);
            Vector3 relativePos = parent.transform.position - smashDeath.transform.position;
            smashDeath.transform.rotation = Quaternion.LookRotation(relativePos);
            smashDeath.SetActive(true);
			cam.DeathShake();

			refPlayer.stat.SetActive(false);
			refPlayer.indicator.SetActive(false);

			StartCoroutine(SecondsOfSilence(refPlayer));

            refPlayer.gameObject.SetActive(false);

	}
	//Pay respects to the dead player
	IEnumerator SecondsOfSilence(Player_Controller refPlayer){

		yield return new WaitForSeconds(0.5f);

		RemoveCamTarget(refPlayer.transform);

	}

	public void RemoveCamTarget(Transform target){

		cam.targets.Remove(target);

	}

	void lerpDamageStat(){
		for (int i = 0; i < players.Length; i++)
		{
			if(playerActive[i]){
				playerLerpDamage[i] = Mathf.SmoothDamp(playerLerpDamage[i], playerDamage[i], ref velRef[i], lerpTime);
				playerLerpDamage[i] = Mathf.Round(playerLerpDamage[i] * 10) / 10;
				statDamage[i].GetComponent<TextMeshProUGUI>().text = (playerLerpDamage[i]).ToString() + "%";
			}
		}
	}

	void waitForKill(){

		for (int i = 0; i < players.Length; i++)
		{
		if(players[i].activeSelf){
        if(playerControl[i].transform.position.x > boundsRight || playerControl[i].transform.position.x < boundsLeft
		|| playerControl[i].transform.position.y > boundsUp || playerControl[i].transform.position.y < boundsDown){
            EliminatePlayer(playerControl[i]);
        	}
		}
		}
	}

	void UpdateLives(){

	}

	public void HazardDeath(Player_Controller refPlayer){
		EliminatePlayer(refPlayer);
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
}

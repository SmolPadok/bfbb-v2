using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "BFBB/Character ID")]
public class Character_ID : ScriptableObject {

	public string characterName = "New Character";

	[Header("Physics")]
	public PhysicMaterial playerPhysic;
	public float mass;
	public float drag;

	[Header("Movement")]
	public float speed = 0f;
	public float jumpPower = 0f;
	public float dashPower = 0f;
	public float dashPeriod = 0f;
	public float divePower = 0f;

	[Header("Damage")]
	public float damage = 0f;

	[Header("Attack Trigger")]
	public Vector3 TriggerPosition;

	[Header("Animations")]
	public RuntimeAnimatorController animatorController = null;

}

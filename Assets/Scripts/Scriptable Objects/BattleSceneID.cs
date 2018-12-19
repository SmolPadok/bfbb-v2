using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Scene", menuName = "BFBB/Battle Scene ID")]
public class BattleSceneID : ScriptableObject
{
    public string sceneName = "Untitled Battle Scene";
    public GameObject asset = null;

    public Vector3[] playerPosition = new Vector3[3];
    public float gravity = -36;
}

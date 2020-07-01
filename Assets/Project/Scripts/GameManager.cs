using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	GameObject enemyObj;
	EnemyBehavior enemy;
	PlayerManager player;
	public bool autoAttack;
	[SerializeField]
	int enemyLevel = 1, swordLevel = 10;

	void Start()
	{
		enemyObj = Resources.Load("Assets/Enemy") as GameObject;
	}

	void Update () {
		if (!enemy)
		{
			/*Vector3 position;
			if (EditorWindow.focusedWindow.maximized) position = new Vector3(0, 1 / 477.3685f * 60, 0);
			else position = new Vector3(0, 1 / 333.6842f * 60, 0);*/

			enemy = Instantiate(enemyObj, GameObject.Find("UICanvas").transform.GetChild(3)).GetComponent<EnemyBehavior>();
			//enemy.transform.position = enemy.transform.parent.position + position;
			print("New Enemy");
		}
		CallInputs();
	}

	public void SetPlayer(PlayerManager pm)
	{
		player = pm;
		print("Player Set");
	}

	public PlayerManager GetPlayer()
	{
		return player;
	}

	public int ExpToLevel(uint exp)
	{
		//print($"Exp: {exp}, Level: {(int)Mathf.Floor(Mathf.Sqrt(Mathf.Sqrt(exp)))}");
		return (int)Mathf.Floor(Mathf.Sqrt(Mathf.Sqrt(exp)));
	}

	public uint LevelToExp(int level)
	{
		//print($"Level: {level}, Exp: {(uint)Mathf.Pow(level, 4)}");
		return (uint)Mathf.Pow(level, 4);
	}

	public int GetEnemyLevel()
	{
		return enemyLevel;
	}

	public void IncrementEnemyLevel()
	{
		enemyLevel += 1;
	}

	public void SetSwordLevel(int level)
	{
		swordLevel += level;
		print(swordLevel);
	}

	public int GetSwordLevel()
	{
		return swordLevel;
	}

	void CallInputs()
	{
		if (Input.GetKeyDown(KeyCode.A) && !Input.GetKey(KeyCode.LeftShift))
		{
			enemy.GetComponent<EnemyBehavior>().RecieveDamage(player.GetSword().GetWeaponDamage() * player.GetStrength() / 10);
		}
		else if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftShift))
		{
			enemy.GetComponent<EnemyBehavior>().RecieveDamage(player.GetSword().GetWeaponDamage() * player.GetStrength());
		}
		if (Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.LeftShift))
		{
			player.ExpUp(1000);
		}
		else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
		{
			player.ExpUp(1000000);
		}
	}

	public void ToggleAutoAttack()
	{
		autoAttack = !autoAttack;
	}
}

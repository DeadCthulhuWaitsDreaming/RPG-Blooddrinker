using UnityEngine;
using UnityEngine.UI;

public class BloodDrinker : MonoBehaviour
{
	GameObject nextPhase;
	Slider[] slider;
	bool upgrade = false;

	[SerializeField]
	uint weaponExp;
	[SerializeField]
	int weaponLevel, weaponDamage, maxLevel, baseDamage;

	PlayerManager owner;
	GameManager gm;

	Text weaponExpText, weaponLevelText;
	
	// Use this for initialization
	void Start () {
		gm = FindObjectOfType<GameManager>();
		owner = gm.GetPlayer();
		weaponExp = 1;
		weaponLevel = 1;
		if (name == "Phase1(Clone)")
		{
			nextPhase = Resources.Load("Assets/Phase2") as GameObject;
			maxLevel = 50;
			baseDamage = 10;
			weaponDamage = baseDamage + weaponLevel * 10;
			slider = transform.GetChild(0).GetComponent<Canvas>().GetComponentsInChildren<Slider>();
			owner.UpdateDamage();
		}
		else if (name == "Phase2(Clone)")
		{
			nextPhase = Resources.Load("Assets/Phase3") as GameObject;
			maxLevel = 150;
			baseDamage = gm.GetSwordLevel();
			weaponDamage = baseDamage + weaponLevel * 25;
			slider = transform.GetChild(0).GetComponent<Canvas>().GetComponentsInChildren<Slider>();
			owner.IncreaseAttackModifier(.25f + (baseDamage - 25.0f) / 100 - .001f);
			owner.UpdateDamage();
		}
		else
		{
			maxLevel = 250;
			baseDamage = gm.GetSwordLevel() * 3;
			weaponDamage = baseDamage + weaponLevel * 50;
			owner.IncreaseAttackModifier(.5f + (gm.GetSwordLevel() - 100.0f) / 100 - .001f);
			owner.UpdateDamage();
		}

		weaponExpText = GameObject.Find("Weapon Exp").GetComponent<Text>();
		weaponLevelText = GameObject.Find("Weapon Level").GetComponent<Text>();
		weaponExpText.text = "Weapon Exp: " + weaponExp;
		weaponLevelText.text = "Weapon Level: " + weaponLevel;
		owner = gm.GetPlayer();
	}

	public void ExpUp (int exp) {
		if (weaponLevel != maxLevel)
		{
			switch (name)
			{
				case "Phase1(Clone)":
					weaponExp += (uint)(exp * .5);
					weaponLevel = gm.ExpToLevel(weaponExp);
					weaponDamage = baseDamage + weaponLevel * 10;
					if (weaponLevel >= 25 && !upgrade)
					{
						upgrade = true;
					}
					else
					{
						for (int x = 0; x < slider.Length; x++)
						{
							slider[x].value = (float)(weaponLevel - 1) / 25;
						}
					}
					break;
				case "Phase2(Clone)":
					weaponExp += (uint)(exp * .3);
					weaponLevel = gm.ExpToLevel(weaponExp);
					weaponDamage = baseDamage + weaponLevel * 25;
					if (weaponLevel >= 100 && !upgrade)
					{
						upgrade = true;
					}
					else
					{
						for (int x = 0; x < slider.Length; x++)
						{
							slider[x].value = (float)(weaponLevel - 1) / 100;
						}
					}
					break;
				case "Phase3(Clone)":
					weaponExp += (uint)(exp * .1);
					weaponLevel = gm.ExpToLevel(weaponExp);
					weaponDamage = baseDamage + weaponLevel * 50;
					break;
			}
		}
		owner.UpdateDamage();
		weaponExpText.text = "Weapon Exp: " + weaponExp;
		weaponLevelText.text = "Weapon Level: " + weaponLevel;
	}

	public bool IsUpgradable()
	{
		return upgrade;
	}

	public Vector3 GetPosition()
    {
		return transform.position;
    }

	public int GetWeaponDamage()
	{
		return weaponDamage;
	}

	public GameObject GetNextPhase()
	{
		return nextPhase;
	}

	public int GetWeaponLevel()
	{
		return weaponLevel;
	}

	public void SetOwnership(PlayerManager player)
	{
		owner = player;
	}
}

#define DEBUGGING
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
	GameManager gm;
	[SerializeField]
	int hitPoints, level, baseExp, defense;
	Text hitpointsText, levelText, damageText;

	void Start () {
		gm = FindObjectOfType<GameManager>();
		hitpointsText = GameObject.Find("Enemy Hitpoints").GetComponent<Text>();
		levelText = GameObject.Find("Enemy Level").GetComponent<Text>();
		damageText = GameObject.Find("Damage Done").GetComponent<Text>();
		level = gm.GetEnemyLevel();
		if (level < 50)
		{
			hitPoints = 50 * level;
			baseExp = 10 * level;
		}
		else if (level >= 50 && level < 150)
		{
			hitPoints = 100 * level;
			baseExp = 20 * level;
		}
		else
		{
			hitPoints = 250 * level;
			baseExp = 50 * level;
		}
		defense = hitPoints / 25;
		hitpointsText.text = "Enemy Hitpoints: " + hitPoints;
		levelText.text = "Enemy Level: " + level;
	}

#if DEBUGGING
	void Update()
    {
        if(gm.autoAttack)
        {
			RecieveDamage(gm.GetPlayer().GetDamage());
		}
    }
#endif

	public void RecieveDamage(int damage) {
		if (damage - defense > 0)
		{
			hitPoints -= damage - defense;
			hitpointsText.text = "Enemy Hitpoints: " + hitPoints;
			damageText.text = "Damage Done: " + (damage - defense);
			if (hitPoints <= 0)
			{
				EnemyDied();
			}
		}
		else
		{
			damageText.text = "Damage Done: 0";
		}
	}

	void EnemyDied()
	{
		gm.GetPlayer().GetSword().ExpUp(baseExp);  //Add exp modifier for buffs
		gm.GetPlayer().ExpUp(baseExp);
		gm.GetPlayer().AddKilltoPlayer();
		if (gm.GetPlayer().GetLevel() > level - 2) gm.IncrementEnemyLevel();
#if DEBUGGING
		Start();
#else
		Destroy(gameObject);
#endif       
	}
}

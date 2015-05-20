using UnityEngine;
using System.Collections;

public class AI_Assassin : MonoBehaviour 
{
	public int spentPoints;

	public int Strength;
	public int Dexterity;
	public int Vitality;
	public int Magic;
	
	public float HP;
	public float MP;
	public float hpRegen;
	public float mpRegen;

	float maxHP;
	float maxMP;
	
	public bool State_Attack = false;
	public bool State_Heal = false;
	public bool State_Find = false;
	public bool State_Flee = false;

	bool isBehaviourSet = false;

	int[] AttributeArray;

	string myEnemiesTag = "Templars";
	
	AI_Behaviour AssassinBehaviour = new AI_Behaviour();

	void Start ()
	{
		AttributeArray = Skill_Spend_Assassin ();
	}

	void Update () 
	{
		setBehaviour ();
		checkVitals ();
		makeDecision ();

	}

	int[] Skill_Spend_Assassin()
	{
		Strength = Random.Range (2, 11);
		Dexterity = Random.Range (2, 11);
		Vitality = Random.Range (2, 11);
		Magic = Random.Range (2, 11);
		
		spentPoints = Strength + Dexterity + Vitality + Magic;
		
		Strength = Strength * 12 / spentPoints;
		Dexterity = Dexterity * 12/ spentPoints;
		Vitality = Vitality * 12 / spentPoints;
		Magic = Magic * 12 / spentPoints;
		
		spentPoints = Strength + Dexterity + Vitality + Magic;
		
		if (spentPoints < 10) 
		{
			Magic = Magic + (10 - spentPoints);
		}
		else if (spentPoints > 10) 
		{
			Magic = Magic - (spentPoints - 10);
		}
		
		spentPoints = Strength + Dexterity + Vitality + Magic;

		Strength++;Dexterity++;Vitality++;Magic++;
		
		HP = 10 * Vitality + 5 * Strength;
		MP = 10 * Magic + 5 * Dexterity;
		hpRegen = 110 / HP;
		mpRegen = 110 / MP;

		int[] AttributeArray = {Strength,Dexterity,Vitality,Magic};
		return AttributeArray;

	}
	void takeDamage(float damageToTake)
	{
		HP = HP - damageToTake;
	}
	void healDamage(float damageToHeal)
	{
		if (HP < maxHP) 
		{
			HP = HP + damageToHeal;
			MP = MP - damageToHeal;
		}
	}

	void checkVitals()
	{
		if (HP <= 0) 
		{
			Destroy(gameObject);
		}
		
		if (HP < maxHP)
		{
			HP = HP + hpRegen;
		}
		
		if (MP < maxMP) 
		{
			MP = MP + mpRegen;
		}
	}
	void setBehaviour()
	{
		if (isBehaviourSet == false) 
		{
			AssassinBehaviour.getAttributes(AttributeArray,myEnemiesTag,gameObject);
			maxHP = HP;
			maxMP = MP;
			isBehaviourSet = true;
		}
	}
	void makeDecision()
	{
		if (HP / maxHP * 100 < 30)
		{
			State_Heal = true;
			State_Attack = false;
			State_Flee = true;
			State_Find = false;
		}
		else
		{
			State_Heal = false;
			State_Attack = true;
			State_Flee = false;
			State_Find = true;
		}

		if (State_Find == true)
		{
			AssassinBehaviour.Find_Target_State();
		}

		if (State_Attack == true) 
		{
			AssassinBehaviour.Attacking_State();
		}

		if (State_Heal == true && MP > 0) 
		{
			AssassinBehaviour.Healing_State();
		}

		if (State_Flee == true) 
		{
			AssassinBehaviour.Flee_Target_State();
		}
	}

}

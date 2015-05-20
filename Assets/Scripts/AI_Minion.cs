using UnityEngine;
using System.Collections;

public class AI_Minion : MonoBehaviour 
{
	public int spentPoints;

	public int Strength;
	public int Dexterity;
	public int Vitality;
	public int Magic;
	
	public float HP;
	public float MP;

	float maxHP;
	float maxMP;
	
	public bool State_Attack = false;
	public bool State_Find = false;
	public bool State_Flee = false;

	bool isBehaviourSet = false;

	int[] AttributeArray;

	string myEnemiesTag = "Assassins";

	AI_Behaviour MinionBehaviour = new AI_Behaviour();

	void Start () 
	{
		AttributeArray = Skill_Spend_Minion ();
	}


	void Update () 
	{
		setBehaviour ();
		checkVitals ();
		makeDecision ();
	
	}

	int[] Skill_Spend_Minion()
	{
		int rnd;
		
		bool  strFlag = false
			, dexFlag = false
				, vitFlag = false
				, magFlag = false;
		
		
		while(spentPoints != 4)
		{
			rnd = Random.Range (1, 101);
			if (rnd % 4 == 0 && strFlag == false)
			{
				Strength = 2;
				strFlag = true;
			}
			
			if (rnd % 4 == 1 && dexFlag ==false)
			{
				Dexterity = 2;
				dexFlag = true;
			}
			
			if (rnd % 4 == 2 && vitFlag == false)
			{
				Vitality = 2;
				vitFlag = true;
			}
			
			if (rnd % 4 == 3 && magFlag == false) 
			{
				Magic = 2;
				magFlag = true;
			}
			spentPoints = Strength + Dexterity + Vitality + Magic;
		}
		
		if(strFlag == false)
			Strength++;
		if(dexFlag == false)
			Dexterity++;
		if(vitFlag == false)
			Vitality++;
		if(magFlag == false)
			Magic++;
		
		spentPoints = Strength + Dexterity + Vitality + Magic;
		
		HP = 10 * Vitality + 5 * Strength;
		MP = 10 * Magic + 5 * Dexterity;

		int[] AttributeArray = {Strength,Dexterity,Vitality,Magic};
		return AttributeArray;
	}
	void takeDamage(float damageToTake)
	{
		HP = HP - damageToTake;
	}
	void checkVitals()
	{
		if (HP <= 0) 
		{
			Destroy(gameObject);
		}
	}
	void setBehaviour()
	{
		if (isBehaviourSet == false) 
		{
			MinionBehaviour.getAttributes(AttributeArray,myEnemiesTag,gameObject);
			maxHP = HP;
			maxMP = MP;
			isBehaviourSet = true;
		}
	}
	void makeDecision()
	{
		if (HP / maxHP * 100 < 30)
		{
			State_Attack = false;
			State_Flee = true;
			State_Find = false;
		}
		else
		{
			State_Attack = true;
			State_Flee = false;
			State_Find = true;
		}
		
		if (State_Find == true)
		{
			MinionBehaviour.Find_Target_State();
		}
		
		if (State_Attack == true) 
		{
			MinionBehaviour.Attacking_State();
		}

		if (State_Flee == true) 
		{
			MinionBehaviour.Flee_Target_State();
		}
	}

}

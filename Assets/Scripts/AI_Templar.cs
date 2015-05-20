using UnityEngine;
using System.Collections;

public class AI_Templar : MonoBehaviour 
{
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

	AI_Behaviour TemplarBehaviour = new AI_Behaviour();
	
	void Start () 
	{
		AttributeArray = Skill_Spend_Templar ();
	}

	void Update () 
	{
		setBehaviour ();
		checkVitals ();
		makeDecision ();

	}

	int[] Skill_Spend_Templar()
	{
		Strength = 3;
		Dexterity = 3;
		Vitality = 3;
		Magic = 3;		
		HP = 10 * Vitality + 5 * Strength;
		MP = 10 * Magic + 5 * Dexterity;
									//0			1		2		3
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
			TemplarBehaviour.getAttributes(AttributeArray,myEnemiesTag,gameObject);
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
			TemplarBehaviour.Find_Target_State();
		}
		
		if (State_Attack == true) 
		{
			TemplarBehaviour.Attacking_State();
		}

		if (State_Flee == true) 
		{
			TemplarBehaviour.Flee_Target_State();
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class AI_Behaviour : MonoBehaviour 
{
	Astar A = new Astar();

	Vector3 targetVector;
	Vector3 previousPosition;
	Vector3 astarNextTarget;

	GameObject EnemyAI = null;
	GameObject AI = null;

	Thread myThread;
	bool isSameThread;
	bool isStuck;
	bool canMove = true;
	bool kickStartAstar = false;
	string searchTag = null;

	int Strength;
	int Dexterity;
	int Vitality;
	int Magic;
	int moveSpeed = 4;
	int rotateSpeed = 90;
	int astarIndex = 0;
	int[,] mapStructure;

	float cooldown;
	float damageSword;
	float damageStaff;
	float damageBow;
	float damageMagic;
	float castingMaxDistance = 20;
	float castingMinDistance = 3;
	float checkTime;


	void Move_State()
	{
		if (A.isAstarComplete == true && isStuck == false && canMove == true)
		{
			AI.transform.Translate (Vector3.forward * Time.deltaTime * moveSpeed, Space.Self);
		} 
		else if (isStuck == true && canMove == true) 
		{
			AI.transform.Translate (Vector3.back * Time.deltaTime * moveSpeed, Space.Self);
		}

		if(checkTime < Time.time)
		{
			if((previousPosition - AI.transform.position).magnitude < 3f)
			{
				isStuck = true;
			}
			else
			{
				isStuck = false;
				previousPosition = AI.transform.position;
				checkTime = Time.time + 1.5f;
			}

		}
	}

	void Rotate_State()
	{
		A.globalStart = AI.transform.position;
		A.globalEnd = targetVector;

		if (A.mapStructure == null) 
		{
			mapStructure = GameObject.Find ("Map Generator").GetComponent<MapGenerator>().getMapStructure();
			A.mapStructure = mapStructure;
		}

		if ((A.isAstarRunning == false && astarIndex == A.AstarPath.Count) || kickStartAstar == true) 
		{
			myThread = new Thread (new ThreadStart (A.calculatePath));
			myThread.Start ();
			isSameThread = true;
			kickStartAstar = false;
			A.AstarPath.Clear();
			Debug.Log ("Thread Started");
		}

		if (A.isAstarRunning == false && A.isAstarComplete == false)
		{
			myThread.Abort();
			Debug.Log("Thread Aborted");
			astarIndex = 0;
		}
		
		if (A.isAstarComplete == true)
		{
			if(isSameThread == true)
			{
				Debug.Log("Thread Finished");
				astarIndex = 0;
				canMove = true;
			}
			isSameThread = false;
			myThread.Join ();
			astarNextTarget.x = A.AstarPath [astarIndex].x;
			astarNextTarget.y = 0.5f;
			astarNextTarget.z = A.AstarPath [astarIndex].z;

			
			if ((AI.transform.position - astarNextTarget).magnitude < 1.5f && astarIndex != A.AstarPath.Count) 
			{
				astarIndex++;
				astarNextTarget = A.AstarPath [astarIndex];
			}

			AI.transform.rotation = 
				Quaternion.RotateTowards(AI.transform.rotation,Quaternion.LookRotation(astarNextTarget - AI.transform.position), rotateSpeed * Time.deltaTime);
		}

	}

	public void Find_Target_State()
	{
		if (EnemyAI == null) 
		{
			float minDistance = Mathf.Infinity;
			Collider[] Colliders;
			Colliders = Physics.OverlapSphere (AI.transform.position, 100);
			Vector3 targetPosition;
			int posInArray = 0;
			for (int i = 0; i < Colliders.Length; i++) 
			{
				if(Colliders[i].tag == searchTag)
				{
					targetPosition = Colliders[i].transform.position;
					
					if((targetPosition - AI.transform.position).sqrMagnitude < minDistance)
					{
						minDistance = (targetPosition - AI.transform.position).sqrMagnitude;
						posInArray = i;
					}
				}
			}
			EnemyAI = Colliders [posInArray].gameObject;

		}
		else
		{
			if(astarIndex == A.AstarPath.Count)
			{
				canMove = false;
			}
			targetVector = EnemyAI.transform.position;
			Move_State();
			Rotate_State();
		}
	}

	public void Attacking_State()
	{
		if ((AI.transform.position - EnemyAI.transform.position).magnitude <= castingMinDistance) 
		{				//Attack via Staff or Sword
			if(cooldown <= Time.time)
			{
				if(damageStaff >= damageSword)
				{
					EnemyAI.SendMessage ("takeDamage", damageStaff);
					cooldown = Time.time + 1;
					Debug.Log("Attack via Staff: " + damageStaff);
				}
				else if(damageStaff <= damageSword)
				{
					EnemyAI.SendMessage ("takeDamage", damageSword);
					cooldown = Time.time + 1;
					Debug.Log("Attack via Sword: " + damageSword);
				}
			}
		} 
		else if ((AI.transform.position - EnemyAI.transform.position).magnitude <= castingMaxDistance)
		{
			//Attack via Bow or Magic
			if(cooldown <= Time.time)
			{
				if(damageBow >= damageMagic)
				{
					EnemyAI.SendMessage ("takeDamage", damageBow);
					cooldown = Time.time + 1;
					Debug.Log("Attack via Bow: " + damageBow);
				}
				else if(damageStaff <= damageSword)
				{
					EnemyAI.SendMessage ("takeDamage", damageMagic);
					cooldown = Time.time + 1;
					Debug.Log("Attack via Magic: " + damageMagic);
				}
			}
		}
	}

	public void Healing_State()
	{
		if (cooldown <= Time.time) 
		{
			AI.SendMessage ("healDamage", 3 * Magic + Vitality + 1);
			cooldown = Time.time + 1;
			Debug.Log("Healed for " + (3 * Magic  + Vitality + 1));
		}


	}
	public void Flee_Target_State ()
	{
		if (EnemyAI == null) 
		{
			float minDistance = Mathf.Infinity;
			Collider[] Colliders;
			Colliders = Physics.OverlapSphere (AI.transform.position, 100);
			Vector3 targetPosition;
			int posInArray = 0;
			for (int i = 0; i < Colliders.Length; i++) {
				if (Colliders [i].tag == searchTag) {
					targetPosition = Colliders [i].transform.position;
					
					if ((targetPosition - AI.transform.position).sqrMagnitude < minDistance) {
						minDistance = (targetPosition - AI.transform.position).sqrMagnitude;
						posInArray = i;
					}
				}
			}
			EnemyAI = Colliders [posInArray].gameObject;
			
		}
		else 
		{
			Vector3 fleeHere;

			do
			{
				fleeHere.x = Random.Range(2,50);
				fleeHere.y = 0.5f;
				fleeHere.z = Random.Range(2,90);

			}while(A.isObstacle(fleeHere) == true);
			targetVector = fleeHere;
			Move_State();
			Rotate_State();
		}
	}


	public void getAttributes(int[] AttributeArray, string enemyTag, GameObject myAI)
	{
		AI = myAI;

		Strength = AttributeArray [0];
		Dexterity = AttributeArray [1];
		Vitality = AttributeArray [2];
		Magic = AttributeArray [3];


		damageSword = 3 * Strength + Dexterity + 1;
		damageStaff = 2 * Strength + 2 * Dexterity + 1;
		damageBow = 3 * Dexterity + Strength + 1;
		damageMagic = 3 * Magic + Dexterity + 1;

		searchTag = enemyTag;
	}

}

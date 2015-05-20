using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour 
{
	public bool StartGame = false;	
	public int howManyMinions = 5;


	void Update()
	{
		if (StartGame == true || Input.GetKeyUp (KeyCode.Space)) 
		{
			bool AssassinInit = false;
			bool TemplarInit = false;
			int MinionInit = 0;

			Vector3 SpawnPos_Assassin = new Vector3(0,0.5f,0);
			Vector3 SpawnPos_Templar = new Vector3(0,0.5f,0);
			Vector3 SpawnPos_Minions = new Vector3(0,0.5f,0);

			while(AssassinInit == false || TemplarInit == false || MinionInit <= howManyMinions)
			{
				Collider[] Colliders;


				SpawnPos_Assassin.x = Random.Range(2,98);
				SpawnPos_Assassin.z = Random.Range(2,32);

				SpawnPos_Templar.x = Random.Range(62,98);
				SpawnPos_Templar.z = Random.Range(62,98);

				SpawnPos_Minions.x = Random.Range(2,98);
				SpawnPos_Minions.z = Random.Range(62,98);


				Colliders = Physics.OverlapSphere (SpawnPos_Assassin, 1); 
				if(Colliders.Length <= 1  && AssassinInit == false)
				{
					GameObject.Instantiate (Resources.Load ("AI_Assassin"), SpawnPos_Assassin, Quaternion.Euler (0, 90, 0));
					Debug.Log ("Instantiated Assassin.");
					AssassinInit = true;
				}

				Colliders = Physics.OverlapSphere (SpawnPos_Templar, 1); 
				if(Colliders.Length <=1 && TemplarInit == false)
				{
					GameObject.Instantiate(Resources.Load("AI_Templar"), SpawnPos_Templar,Quaternion.Euler(0,270,0));
					Debug.Log("Instantiated Templar.");
					TemplarInit = true;
				}

				Colliders = Physics.OverlapSphere (SpawnPos_Minions, 1); 
				if(Colliders.Length <=1 && MinionInit <= howManyMinions)
				{
					GameObject.Instantiate(Resources.Load("AI_Minion"), SpawnPos_Minions,Quaternion.Euler(0,270,0));
					Debug.Log("Instantiated Minion: " + MinionInit);
					MinionInit++;
				}


			}


			StartGame = false;
		}
	}
}

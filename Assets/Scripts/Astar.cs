using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Astar : MonoBehaviour
{
	public bool calculate = false;
	public bool isAstarRunning = false;
	public bool isAstarComplete = false;

	public List<Vector3> AstarPath = new List<Vector3>();
	public Vector3 globalStart;
	public Vector3 globalEnd;

	List<VectorRecord> openList = new List<VectorRecord> ();
	List<VectorRecord> closedList = new List<VectorRecord> ();
	List<Vector3> adjacentList = new List<Vector3> ();

	VectorRecord currentVector = new VectorRecord ();
	VectorRecord startVector = new VectorRecord();
	VectorRecord endVector = new VectorRecord ();
	VectorRecord tempRecord = new VectorRecord ();
	AdjacentVectors adjacents = new AdjacentVectors();
	public int[,] mapStructure = null;

	float failOnLoopCount;

	public void calculatePath()
	{
		isAstarRunning = true;
		isAstarComplete = false;

		Vector3 start = globalStart;
		Vector3 end = globalEnd;

		int Index = -1;
		bool addOpenList = false;
		startVector.setInfo (start, null, 0, (start - end).magnitude);
		endVector.setInfo (end, null, (start - end).magnitude, 0);
		openList.Add (startVector);
	
		currentVector = startVector;
		AstarPath.Clear ();
		failOnLoopCount = 0;
		while (openList.Count > 0) 
		{
			failOnLoopCount++;

			if(failOnLoopCount > 5000)
			{
				isAstarComplete = false;
				isAstarRunning = false;
				break;
			}

				Index = LowestCost();
				currentVector = openList[Index];


			if(currentVector.getVector() == endVector.getVector())
			{
				tempRecord = currentVector;
				while(tempRecord.getParent() != null)
				{
					AstarPath.Add (tempRecord.getVector());
					tempRecord = tempRecord.getParent();
				}
				AstarPath.Add (start);
				AstarPath.Reverse();

				isAstarComplete = true;
				isAstarRunning = false;
				break;
			}
			else
			{
				openList.Remove(currentVector);
				closedList.Add(currentVector);
				adjacentList.Clear();
				adjacentList = adjacents.getAdjacents(currentVector);

				if(adjacentList.Count == 8)
				{
					for(int i = 0; i < 8; i++)
					{
						if(isObstacle(adjacentList[i]) == false)
						{
							addOpenList = true;

							if(isInOpenList(adjacentList[i]) == false)
							{
								addOpenList = true;

								if(isInClosedList(adjacentList[i]) == false)
								{
									addOpenList = true;
								}
								else if(isInClosedList(adjacentList[i]) == true)
								{
									addOpenList = false;
								}
							}
							else if(isInOpenList(adjacentList[i]) == true)
							{
								addOpenList = false;
							}
						}
						else if(isObstacle(adjacentList[i]) == true)
						{
							addOpenList = false;
						}

						if(addOpenList == true)
						{
							VectorRecord adjacentVector = new VectorRecord();
							adjacentVector.setInfo(adjacentList[i]
							                       ,currentVector
							                       ,currentVector.getCostSoFar() + Mathf.Sqrt(2)
							                       ,(adjacentList[i] - endVector.getVector()).magnitude);
							openList.Add (adjacentVector);
						}
					}
				}
				else
				{
					Debug.Log("Failed to get Adjacent List, astar failed.");
					isAstarRunning = false;
					isAstarComplete =false;
					break;
				}
			}
		}
		if (openList.Count == 0) 
		{
			Debug.Log("OpenList is empty, astar failed.");
			isAstarRunning = false;
			isAstarComplete =false;
		}
	}
	int LowestCost()
	{
		float maxValue = Mathf.Infinity;
		int Index = -1;


		if (openList.Count != 0) 
		{
			for (int i = 0; i < openList.Count; i++) 
			{
				if (openList [i].getEstimatedCost () < maxValue) 
				{
					maxValue = openList [i].getEstimatedCost ();
					Index = i;
				}
			}
			return Index;
		} 
		else 
		{
			Debug.Log("Lowest Cost Failed.");
			return Index;
		}

	}
	public bool isObstacle(Vector3 myVector)
	{
		int x = (int)myVector.x;
		int z = (int)myVector.z;

		if (mapStructure == null)
		{
			mapStructure = GameObject.Find ("Map Generator").GetComponent<MapGenerator>().getMapStructure();
		}

		if (x >= 0 && z >= 0 && x < 103 && z < 103) 
		{
			if (mapStructure [x, z] == 1) 
			{
				return true;
			}
			return false;
		}
		return true;
	}
	bool isInOpenList(Vector3 myVector)
	{
		for (int i = 0; i < openList.Count; i++) 
		{
			if(openList[i].getVector() == myVector)
			{
				return true;
			}
		}
		return false;
	}
	bool isInClosedList(Vector3 myVector)
	{
		for (int i = 0; i < closedList.Count; i++) 
		{
			if(closedList[i].getVector() == myVector)
			{
				return true;
			}
		}
		return false;
	}
	void testAstar()
	{
		List<Vector3> testPath = null;

		calculatePath ();
		testPath = AstarPath;

		if (testPath != null) 
		{
			for(int i = 0; i < testPath.Count; i++)
			{
				GameObject.Instantiate (GameObject.CreatePrimitive(PrimitiveType.Sphere), testPath[i],Quaternion.identity);
				Debug.Log(testPath[i].ToString());
			}
		}
	}
	void Update()
	{
		if (calculate == true) 
		{
			testAstar();
			calculate = false;
		}

	}
}

public class VectorRecord
{
	Vector3 theVector;
	VectorRecord myParent;
	float CostSoFar;
	float Heuristic;
	float EstimatedCost;

	public void setInfo(Vector3 myVector,VectorRecord myParentVector,float CostSOfar,float heuristic)
	{
		theVector.x = (int)myVector.x;
		theVector.y = (int)myVector.y;
		theVector.z = (int)myVector.z;

		myParent = myParentVector;

		CostSoFar = CostSOfar;
		Heuristic = heuristic;
	}
	public float getEstimatedCost()
	{
		EstimatedCost = CostSoFar + Heuristic;
		return EstimatedCost;
	}
	public Vector3 getVector()
	{
		return theVector;
	}
	public float getCostSoFar()
	{
		return CostSoFar;
	}
	public VectorRecord getParent()
	{
		return myParent;
	}
}
public class AdjacentVectors
{
	List<Vector3> adjacentList = new List<Vector3> ();
	Vector3 v11;
	Vector3 v12;
	Vector3 v13;
	Vector3 v21;
	Vector3 v23;
	Vector3 v31;
	Vector3 v32;
	Vector3 v33;

	public List<Vector3> getAdjacents(VectorRecord myVector)
	{
		v11 = myVector.getVector();
		v11.x = (int)v11.x + 1;
		v11.z = (int)v11.z + 1;

		v12 = myVector.getVector ();
		v12.x = (int)v12.x + 1;

		v13 = myVector.getVector();
		v13.x = (int)v13.x + 1;
		v13.z = (int)v13.z - 1;


		v21 = myVector.getVector();
		v21.z = (int)v21.z + 1;

		v23 = myVector.getVector();
		v23.z = (int)v23.z - 1;

		v31 = myVector.getVector();
		v31.x = (int)v31.x - 1;
		v31.z = (int)v31.z + 1;

		v32 = myVector.getVector();
		v32.x = (int)v32.x - 1;

		v33 = myVector.getVector();
		v33.x = (int)v33.x - 1;
		v33.z = (int)v33.z - 1;

		adjacentList.Add (v11);
		adjacentList.Add (v12);
		adjacentList.Add (v13);

		adjacentList.Add (v21);

		adjacentList.Add (v23);

		adjacentList.Add (v31);
		adjacentList.Add (v32);
		adjacentList.Add (v33);

		return adjacentList;
	}
}

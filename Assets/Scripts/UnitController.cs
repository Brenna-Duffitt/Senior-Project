using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UnitController : UEBStatistics
{
	public PlayerControl player;

	public string s_Action;

	public GameObject abilityPrefab;
	public Transform showSelected;

	public enum state { idle = 0, moving = 1, attacking = 2, building = 3, gatheringWood = 4, gatheringWater = 5, gatheringFood = 6, placingBuilding = 7 };
	public string[] stateText = { "Idle", "Moving", "Attacking", "Building", "Gathering Wood", "Gathering Water", "Gathering Food", "Placing Building" };
	public int currentState = (int)state.idle;
	public int secondaryState = (int)state.idle;
	public BuildingController toBuild;

	private float startTime;
	private float originalDistance;
	private float currentDistance;

	private Vector3 endPoint;
	private Vector3 startPoint;

	float currentTime;
	float gatherTime = 1.0F;

	public UEBStatistics mostRecentTarget;
	float cooldownTime;

	#region START / UPDATE

	void Start()
	{
		player = GameObject.FindObjectOfType<PlayerControl>();
		currentTime = Time.time;
	}

	// Update is called once per frame
	void Update()
	{
		if (currentHealth <= 0)
		{
			Destroy(gameObject);

		}

		changedCurrentState();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="other"></param>
	void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
		{
			currentState = (int)state.idle;

			if (other.gameObject.GetComponent<BuildingController>() && other.gameObject.GetComponent<BuildingController>().tag == "FriendlyBuilding")
			{
				mostRecentTarget = other.gameObject.GetComponent<UEBStatistics>();
				currentState = (int)state.building;
				secondaryState = (int)state.idle;

				StartBuilding();
			}
		}

		changedCurrentState();
	}

	#endregion

	#region STATE MANIPULATION

	/// <summary>
	/// 
	/// </summary>
	void changedCurrentState()
	{
		s_Action = (string)stateText[(int)currentState];

		if (currentState == (int)state.moving)
		{
			transform.rotation = Quaternion.LookRotation(endPoint - transform.position);

			if (currentDistance >= 0.5F)
			{
				float percentComplete = ((Time.time - startTime) * 4) / originalDistance;
				transform.position = Vector3.Lerp(startPoint, endPoint, percentComplete);
				currentDistance = Vector3.Distance(transform.position, endPoint);
			}
			else
			{
				AssignStates();
			}
		}
	}



	/// <summary>
	/// 
	/// </summary>
	public void AssignStates()
	{
		if (secondaryState == (int)state.gatheringFood)
		{
			currentState = (int)state.gatheringFood;
			StartGathering();
		}
		else if (secondaryState == (int)state.gatheringWood)
		{
			currentState = (int)state.gatheringWood;
			StartGathering();
		}
		else if (secondaryState == (int)state.gatheringWater)
		{
			currentState = (int)state.gatheringWater;
			StartGathering();
		}
		else
			currentState = (int)state.idle;

		secondaryState = (int)state.idle;
	}

	#endregion

	#region MISC. UNIT MOVEMENT / UPGRADES
	/// <summary>
	/// 
	/// </summary>
	/// <param name="point"></param>
	public void Move(Vector3 point)
	{
		StopAllCoroutines();
		//change the state
		currentState = (int)state.moving;

		//location to move to
		endPoint = point;
		endPoint.y = 2;

		startTime = Time.time;
		startPoint = transform.position;
		originalDistance = Vector3.Distance(startPoint, endPoint);
		currentDistance = Vector3.Distance(transform.position, endPoint);

	}
	 	
	/// <summary>
	/// 
	/// </summary>
	public void LevelUp()
	{
		maxHealth += 5;
		currentHealth = maxHealth;
		defense += .1;
		attack += .1;
	}

	#endregion

	#region RESOURCE COLLECTION

	/// <summary>
	/// 
	/// </summary>
	/// <param name="point"></param>
	/// <param name="type"></param>
	public void CollectResources(Vector3 point, int type)
	{
		Move(point);

		switch (type)
		{
			case 1:	//Food
				secondaryState = (int)state.gatheringFood;
				break;
			case 2: //water
				secondaryState = (int)state.gatheringWater;
				break;
			case 3: //wood
				secondaryState = (int)state.gatheringWood;
				break;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void StartGathering()
	{
		currentTime = Time.time;

		StartCoroutine(Gathering());
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IEnumerator Gathering()
	{
		while (currentState == (int)state.gatheringWater || currentState == (int)state.gatheringFood || currentState == (int)state.gatheringWood)
		{
			if (Time.time - currentTime < gatherTime)
			{
				switch (currentState)
				{
					case (int)state.gatheringFood:	//Food
						player.r_Food += 1;
						break;
					case (int)state.gatheringWater:    //water
						player.r_Water += 1;
						break;
					case (int)state.gatheringWood:	   //wood
						player.r_Wood += 1;
						break;
				}
				yield return new WaitForSeconds(gatherTime);
			}
			currentTime = Time.time;
		}
	}

	#endregion

	#region COMBAT

	/// <summary>
	/// 
	/// </summary>
	/// <param name="target"></param>
	public void Combat(UEBStatistics target)
	{
		if (currentState != (int)state.moving)
		{
			mostRecentTarget = target;
			currentState = (int)state.attacking;

			StopAllCoroutines();
			StartCoroutine(CombatAction());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public IEnumerator CombatAction()
	{
		while (mostRecentTarget != null)
		{
			if (Time.time > cooldownTime)
			{
				Quaternion targetRotation = Quaternion.LookRotation(mostRecentTarget.transform.position - transform.position);
				transform.rotation = targetRotation;

				GameObject clone = (GameObject)Instantiate(abilityPrefab, transform.position, transform.rotation);
				AbilityController ac = clone.GetComponent<AbilityController>();
				ac.SetTarget(mostRecentTarget, this.GetComponent<UEBStatistics>());

				if (mostRecentTarget == null)
					currentState = (int)state.idle;

				cooldownTime = Time.time + 3;
			}

			yield return new WaitForSeconds(1);

		}
	}

	#endregion
	   
	#region BUILDING

	/// <summary>
	/// 
	/// </summary>
	public void StartBuilding()
	{
		StopAllCoroutines();
		StartCoroutine(Build());
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public IEnumerator Build()
	{
		while (currentState == (int)state.building)
		{
			mostRecentTarget.currentHealth += 2;

			if (mostRecentTarget.currentHealth >= mostRecentTarget.maxHealth)
				currentState = (int)state.idle;

			yield return new WaitForSeconds(0.5f);
		}
	}

	#endregion
}


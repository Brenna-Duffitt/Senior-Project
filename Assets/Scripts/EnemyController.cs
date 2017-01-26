using UnityEngine;
using System.Collections;

public class EnemyController : UEBStatistics
{
	public PlayerControl Player;

	public UEBStatistics mostRecentTarget;
	public GameObject abilityPrefab;
	public float distanceFromTarget;

	public bool inCombat = false;
	float cooldownTime;

	bool collide = false;

	UnitController closestUnit_Combat = null;

	public bool noTargets = false;

	#region START / UPDATE

	void Start()
	{
		Player = FindObjectOfType<PlayerControl>();
	}

	void Update()
	{
		if (currentHealth <= 0)
		{
			Destroy(gameObject);
		}

		mostRecentTarget = GetClosestTarget();

		if (mostRecentTarget != null)
		{
			distanceFromTarget = Vector3.Distance(transform.position, mostRecentTarget.transform.position);

			ChangedState();
		}
	}
	 	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="other"></param>
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "FriendlyBuilding")
		{
			collide = true;
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public void ChangedState()
	{
		if (collide || (distanceFromTarget < 10 && !inCombat))
		{
			inCombat = true;

			AttackUnit(closestUnit_Combat);
			StopAllCoroutines();
			StartCoroutine(CombatAction());
		}
		else if (distanceFromTarget >= 5)
		{
			inCombat = false;
			Move();
		}
	}

	#endregion

	#region ACTIONS

	/// <summary>
	/// 
	/// </summary>
	public void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, mostRecentTarget.transform.position, 3 * Time.deltaTime);
	}


	#region COMBAT

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public UEBStatistics GetClosestTarget()
	{
		UEBStatistics closestBuilding = null;
		UEBStatistics closestUnit = null;

		UEBStatistics returnObject = null;

		if (Player.buildings != null)
		{
			foreach (GameObject b in Player.buildings)
			{
				if (closestBuilding == null)
				{
					closestBuilding = b.GetComponent<UEBStatistics>();
				}
				else if (closestBuilding != null)
				{
					if (Vector3.Distance(transform.position, b.transform.position) <= Vector3.Distance(transform.position, closestBuilding.transform.position))
					{
						closestBuilding = b.GetComponent<UEBStatistics>();
					}
				}
			}
		}

		if (Player.units != null)
		{
			foreach (GameObject u in Player.units)
			{
				if (closestUnit == null)
				{
					closestUnit = u.GetComponent<UEBStatistics>();
					closestUnit_Combat = u.GetComponent<UnitController>();
				}
				else if (closestUnit != null)
				{
					if (Vector3.Distance(transform.position, u.transform.position) <= Vector3.Distance(transform.position, closestUnit.transform.position))
					{
						closestUnit = u.GetComponent<UEBStatistics>();

						closestUnit_Combat = u.GetComponent<UnitController>();
					}
				}
			}
		}

		if (closestBuilding != null && closestUnit != null)
		{
			if (Vector3.Distance(transform.position, closestBuilding.transform.position) < Vector3.Distance(transform.position, closestUnit.transform.position))
			{
				returnObject = closestBuilding;
			}
			else
			{
				returnObject = closestUnit;
			}
		}
		else if (closestBuilding != null && closestUnit == null)
		{
			returnObject = closestBuilding;
		}
		else if (closestBuilding == null && closestUnit != null)
		{
			returnObject = closestUnit;
		}
		else
			noTargets = true;

		return returnObject;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="unit"></param>
	void AttackUnit(UnitController unit)
	{
		unit.Combat(this);
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

				cooldownTime = Time.time + 5;
			}

			yield return null;
		}
	}

	#endregion

	#endregion
}

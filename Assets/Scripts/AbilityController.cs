using UnityEngine;
using System.Collections;

public class AbilityController : MonoBehaviour
{
	public UEBStatistics targetEnemy;
	public UEBStatistics castingUnit;

	// Use this for initialization
	void Start()
	{	}

	// Update is called once per frame
	void Update()
	{
		if (targetEnemy == null)
		{
			Destroy(gameObject);
		}
		else
		{
			Move();

			if (transform.position == targetEnemy.transform.position)
			{
				Hit();
			}

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, 5 * Time.deltaTime);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="target"></param>
	/// <param name="castingFrom"></param>
	public void SetTarget(UEBStatistics target, UEBStatistics castingFrom)
	{
		targetEnemy = target;
		castingUnit = castingFrom;

		//cachedEnemyStats = targetEnemy.GetComponent<EnemyController>();

	}

	/// <summary>
	/// 
	/// </summary>
	public void Hit()
	{
		double damage = Random.Range(1, 15) + castingUnit.attack - targetEnemy.defense;

		if (damage > 0)
		{
			targetEnemy.currentHealth -= damage;
		}

		Destroy(gameObject);
	}
}

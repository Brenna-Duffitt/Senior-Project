using UnityEngine;
using System.Collections;

public class BuildingController : UEBStatistics
{
	public string s_Action;

	public GameObject spawnPoint;

	void Start()
	{ }

	void Update()
	{
		if (currentHealth <= 0)
		{
			Destroy(gameObject);
		}

		if (currentHealth > maxHealth)
			currentHealth = maxHealth;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyBehavior : MonoBehaviour
{
	public GameObject[] enemyType;                // The enemy prefab to be spawned.
	public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
	public GameObject[] enemies;		   //I don't know why this is needed twice O.o
	public GameBehavior gameBehavior;
	Transform target;	

	public Text SpawnTimerDisplayText;
	public Text WaveCountText;
	public Text enemyPopulationText;

	public int waveCount = 1;
	public int enemyPopulation = 0;

	float spawnTime = 20f;            // How long between each spawn.
	float initialSpawnTime = 180f;
	float timeToSpawn;

	public float getTimeToSpawn
	{
		get { return timeToSpawn; }
		set { timeToSpawn = value; }
	}


	public bool firstSpawn = true;
	public bool gameOver = false;

	#region START / UPDATE

	void Start()
	{
		firstSpawn = true;
		timeToSpawn = Time.time + initialSpawnTime;
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		SpawnTimer();

		InvokeRepeating("Spawn", initialSpawnTime, spawnTime);
	}

	void Update()
	{
		enemies = GameObject.FindGameObjectsWithTag("Enemy");

		if (enemyPopulation != enemies.Length)
		{
			enemyPopulation = enemies.Length;
			enemyPopulationText.text = (enemyPopulation + " Attacking Enemies");
		}

		checkGameOver();

		if (gameOver)
		{
			CancelInvoke();
			SpawnTimerDisplayText.text = "";
			WaveCountText.text = "Game Over";
			Time.timeScale = 0.0F;
		}
		else
			SpawnTimer();
	}

	#endregion



	#region SPAWNING

	/// <summary>
	/// 
	/// </summary>
	void SpawnTimer()
	{
		if (Time.time > timeToSpawn)
			timeToSpawn = Time.time + spawnTime;

		string minutes = Mathf.Floor((timeToSpawn - Time.time) / 60).ToString("00");
		string seconds = ((timeToSpawn - Time.time) % 60).ToString("00");

		string displayTimer = string.Format("{0:00}:{1:00}", minutes, seconds);
		SpawnTimerDisplayText.text = (displayTimer);

	}

	/// <summary>
	/// 
	/// </summary>
	void Spawn()
	{
		if (!gameBehavior.PlayerGamePaused && !gameBehavior.SystemGamePaused)
		{
			float temp = (float)((waveCount * 0.1F) + 1);
			int enemyCount = (int)Math.Floor(temp);	 //every time waves one more enemy willl spawn 1-9 = 1, 10 - 19 = 2

			for (int i = 0; i < enemyCount; i++)
			{
				// Find a random index between zero and one less than the number of spawn points.
				int spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
				int enemyIndex = UnityEngine.Random.Range(0, enemyType.Length);
				// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
				Instantiate(enemyType[enemyIndex], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
			}

			if (firstSpawn)
				firstSpawn = false;

			++waveCount;
			WaveCountText.text = ("Wave: " + waveCount + "    In");
		}
	}

	#endregion

	#region GAME OVER / LOAD SAVED GAME

	/// <summary>
	/// 
	/// </summary>
	public void DestroyAllEnemies()
	{
		foreach(GameObject e in enemies)
		{
			Destroy(e);
		}

		enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}

	/// <summary>
	/// 
	/// </summary>
	void checkGameOver()
	{
		if (!firstSpawn)
		{
			foreach (GameObject e in enemies)
			{
				if (e.GetComponent<EnemyController>().noTargets)
				{
					gameOver = true;
				}
			}
		}
	}
	#endregion
}



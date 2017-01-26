using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GameSerializable
{

	[Serializable]
	public struct GameInformationSerializable
	{
		public bool unitCreationCooldown;
		public float currentCooldown;
		public bool savedGame;
	}

	[Serializable]
	public struct PlayerInformationSerializable
	{
		public int playerWater;
		public int playerWood;
		public int playFood;

		public int unitCap;
		public int currentUnits;

	}

	[Serializable]
	public struct EnemyInformationSerializable
	{
		public int waveCount;
		public int enemyPopulation;

		public float timeToSpawn;

		public bool firstSpawn;
		public bool gameOver;

	}

	[Serializable]
	public struct UEBStatisticsSerializable
	{
		public string type;
		public double currentHealth;
		public double maxHealth;
		public double defense;
		public double attack;


		public float locationX;
		public float locationY;
		public float locationZ;
	}

	public GameInformationSerializable gameInfo = new GameInformationSerializable();
	public PlayerInformationSerializable playerInfo = new PlayerInformationSerializable();
	public EnemyInformationSerializable enemyInfo = new EnemyInformationSerializable();
	public UEBStatisticsSerializable[] uebStats = new UEBStatisticsSerializable[150];


}

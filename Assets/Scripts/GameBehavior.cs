using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameBehavior : MonoBehaviour
{
	#region GAMEOBJECTS

	public GameObject InGameMainMenu;
	public HUDBehavior HUD_Menu;
	public PlayerControl Player;
	public Text cooldownText;
	public EnemyBehavior enemyBehavior;
	#endregion

	#region PREFABS

	public GameObject houseElf;
	public GameObject witch;
	public GameObject wizard;
	public GameObject wall;
	public GameObject baseCamp;
	public GameObject potionsTent;
	//public GameObject[] enemy;

	#endregion

	#region RESOURCE COST FOR BUILDINGS
	int baseCampCost_wood = 200;
	int baseCampCost_food = 15;

	int potionsTentCost_wood = 10;
	int potionsTentCost_food = 25;
	int potionsTentCost_water = 75;

	int wallCost_wood = 20;
	int wallCost_water = 10;
	#endregion

	#region	  RESOURCE COST FOR UNITS
	int wizardCost_food = 50;
	int wizardCost_wood = 10;

	int witchCost_food = 50;
	int witchCost_water = 10;

	int houseElfCost_food = 25;
	#endregion

	#region RESOURCE COST TO UPGRADE UNITS
	int upgradeUnitCost_food = 10;
	int upgradeUnitCost_water = 10;
	int upgradeUnitCost_wood = 10;
	#endregion

	public bool SystemGamePaused = false;	  //The sytem can pause the game 
	public bool PlayerGamePaused = false;	  //The player can pause the game
	bool unitCreationCooldown = false;
	const float cooldown = 10F;

	public float currentCooldown = 10F;

	#region GAME SAVING VARIABLES

	static GameSerializable gameSerializable;
	static SaveLoad saveLoad;
	bool savedGame = false;

	#endregion

	#region START/LOAD

	/// <summary>
	/// This happens first
	/// </summary>
	public void start()
	{
		//Game has started, make sure time is going
		Time.timeScale = 1;
		Time.fixedDeltaTime = 1;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="scene"></param>
	public void LoadScene(int scene)
	{
		Application.LoadLevel(scene);
		Time.timeScale = 1.0F;
	}

	#endregion

	#region UPDATE FUNCTIONS

	/// <summary>
	/// 
	/// </summary>
	public void Update()
	{
		//update the Resouce Panel first (top left)
		HUD_Menu.UpdateResources();

		//Update player unit and building list & count to account for death, destruction, or spawning of new units.
		Player.UpdateUnitsAndBuildings();

		//Now update the Panels off the updated buildings
		HUD_Menu.UpdatePanels();

		//closes or opens the in game main menu
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OpenOrCloseMenu();
			PauseGame();
		}
		//pauses and unpauses the game
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (!SystemGamePaused)
			{
				if (PlayerGamePaused)
					PlayerGamePaused = false;
				else
					PlayerGamePaused = true;

				PauseGame();
			}
		}

		//Mouse button 1 == Right Click
		if (Input.GetMouseButtonDown(1))
		{
			if (Player.selectedUnit && Player.selectedUnit.currentState != (int)UnitController.state.placingBuilding)
			{
				//movement, select action
				RightClick();

			}
		}
		//mouse button 2 == Left Click
		else if (Input.GetMouseButtonDown(0))
		{
			//select unit / building / enemy / resource
			LeftClick();
		}

		//update cooldowns for	unit creation
		updateCooldownTimer();
	}

	/// <summary>
	/// Actions to handle Player movements and menu displays after a left click
	/// </summary>
	public void LeftClick()
	{
		RaycastHit targetInfo = new RaycastHit();

		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out targetInfo);

		Player.LeftClickAction(targetInfo);
		HUD_Menu.ManageStatisticsPanel(targetInfo.collider.gameObject.tag/*selected object tag*/
				, targetInfo.transform.name.ToString());/*selected object name*/
	}


	/// <summary>
	/// Actions to handle Player movements and menu displays after a right click
	/// </summary>
	public void RightClick()
	{
		RaycastHit target = new RaycastHit();

		Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out target);

		if (target.collider != null)
		{
			Player.RightClickAction(target);

		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void OpenOrCloseMenu()
	{
		//if main menu (game hasn't started) is open, or if the in game main menu is open, don't reopen
		if (!InGameMainMenu.activeSelf)
		{
			InGameMainMenu.SetActive(true);
			SystemGamePaused = true;
		}
		//if the in game main menu is open close it
		else if (InGameMainMenu.activeSelf)
		{
			InGameMainMenu.SetActive(false);
			SystemGamePaused = false;
		}
	}

	/// <summary>
	/// Pauses or unpasues the game, based on the bool GamePaused
	/// </summary>
	public void PauseGame()
	{
		//pause the game
		if (SystemGamePaused || PlayerGamePaused)
		{
			Time.timeScale = 0;
			Time.fixedDeltaTime = 0;
		}
		//unpause the game

		if (!SystemGamePaused && !PlayerGamePaused)
		{
			Time.timeScale = 1;
			Time.fixedDeltaTime = 1;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public void updateCooldownTimer()
	{
		if (Player.currentUnits < Player.unitCap)
		{
			if (((currentCooldown - Time.time) % 60) >= 0 && (Mathf.Floor((currentCooldown - Time.time) / 60)) >= 0)
			{
				string minutes = Mathf.Floor((currentCooldown - Time.time) / 60).ToString("00");
				string seconds = ((currentCooldown - Time.time) % 60).ToString("00");
				string displayTimer = string.Format("{0:00}:{1:00}", minutes, seconds);

				cooldownText.text = (displayTimer);
			}
			else
				cooldownText.text = ("");
		}
		else
		{
			cooldownText.text = "Population Limit";
		}
	}

	#endregion


	#region CREATE BUILDING/UNIT
	/// <summary>
	/// Creates specified object if the player meets required criteria (cooldowns - resources)
	/// </summary>
	/// <param name="newObject">Object to be created</param>
	public void CreateObject(GameObject newObject)
	{
		HUD_Menu.ResourceLackingText.text = "";
		HUD_Menu.ResourceLackingText_UNIT.text = "";

		if (newObject.tag.ToString() == "FriendlyBuilding")
		{
			if (BuildingResourceCheck(newObject.GetComponent<BuildingController>().type))
			{
				InstantiateObject(newObject.GetComponent<BuildingController>());
			}
			else
			{
				HUD_Menu.ResourceLackingText.text = "Not enough resources";
			}
		}
		else if (newObject.tag.ToString() == "Friendly")
		{
			if (UnitResourceCheck(newObject.GetComponent<UnitController>().type))
			{
				unitCooldown();

				if (!unitCreationCooldown)
				{
					InstantiateUnit(newObject);
				}
			}
			else
			{
				HUD_Menu.ResourceLackingText_UNIT.text = "Not enough resources";
			}
		}
	}

	#region RESOURCE CHECK
	/// <summary>
	/// checks to see if the player has enough resources to creates the specified unit
	/// </summary>
	/// <param name="newObjectName">name of unit the player wants to create</param>
	/// <returns>boolean - true if the unit will be spanwed, false if not</returns>
	bool UnitResourceCheck(string newObjectName)
	{
		bool canSpawn = false;

		if (newObjectName == "Witch")
		{
			if ((Player.r_Food - witchCost_food) >= 0 && (Player.r_Water - witchCost_water) >= 0)
			{
				Player.r_Food -= witchCost_food;
				Player.r_Water -= witchCost_water;
				canSpawn = true;
			}
		}
		else if (newObjectName == "Wizard")
		{
			if ((Player.r_Food - wizardCost_food) >= 0 && (Player.r_Wood - wizardCost_wood) >= 0)
			{
				Player.r_Food -= wizardCost_food;
				Player.r_Water -= wizardCost_wood;
				canSpawn = true;
			}
		}
		else if (newObjectName == "HouseElf")
		{
			if ((Player.r_Food - houseElfCost_food) >= 0)
			{
				Player.r_Food -= houseElfCost_food;
				canSpawn = true;
			}
		}

		return canSpawn;

	}

	/// <summary>
	/// Checks to see if the Player has enough resources to build the requested building
	/// This need to be fixed
	/// </summary>
	/// <param name="newObjectName">the Name of the building the player wants to create</param>
	/// <returns>boolean - true if okay to build, false if not okay to build</returns>
	bool BuildingResourceCheck(string newObjectName)
	{
		bool canBuild = false;

		if (newObjectName == "BaseCamp")
		{
			if ((Player.r_Wood - baseCampCost_wood) >= 0 && (Player.r_Food - baseCampCost_food) >= 0)
			{
				Player.r_Wood -= baseCampCost_wood;
				Player.r_Food -= baseCampCost_food;

				canBuild = true;
			}
		}
		else if (newObjectName == "PotionsTent")
		{
			if ((Player.r_Wood - potionsTentCost_wood) >= 0 && (Player.r_Food - potionsTentCost_food) >= 0 && (Player.r_Water - potionsTentCost_water) >= 0)
			{
				Player.r_Wood -= potionsTentCost_wood;
				Player.r_Food -= potionsTentCost_food;
				Player.r_Water -= potionsTentCost_water;

				canBuild = true;
			}
		}
		else if (newObjectName == "Wall")
		{
			if ((Player.r_Wood - wallCost_wood) >= 0 && (Player.r_Water - wallCost_water) >= 0)
			{
				Player.r_Wood -= wallCost_wood;
				Player.r_Water -= wallCost_water;

				canBuild = true;
			}
		}

		return canBuild;
	}

	#endregion

	/// <summary>
	/// Checks the cooldown timer for units, if it is at 0, the player can create a new unit
	/// </summary>
	public void unitCooldown()
	{
		if (Player.currentUnits < Player.unitCap)
		{
			if (Time.time > currentCooldown)
			{
				currentCooldown = Time.time + cooldown;
				unitCreationCooldown = false;
			}
			else
				unitCreationCooldown = true;
		}
	}

	/// <summary>
	/// Creates a building
	/// </summary>
	/// <param name="newObject"></param>
	public void InstantiateObject(BuildingController newObject)
	{

		Player.selectedUnit.currentState = 7;
		Player.selectedUnit.toBuild = newObject;

	}

	/// <summary>
	///  creates a unit
	/// </summary>
	/// <param name="newObject"></param>
	public void InstantiateUnit(GameObject newObject)
	{
		if (Player.currentUnits < Player.unitCap)
		{
			Instantiate(newObject, Player.selectedBuilding.spawnPoint.transform.position, Quaternion.identity);
		}
	}

	#endregion


	#region GAME SAVING/LOADING PROCESS

	#region SAVE GAME

	/// <summary>
	/// 
	/// </summary>
	public void startGameSave()
	{
		saveLoad = new SaveLoad();
		gameSerializable = new GameSerializable();

		savedGame = true;

		UEBStatistics[] ueb = FindObjectsOfType<UEBStatistics>();

		gameSerializable.gameInfo = new GameSerializable.GameInformationSerializable();
		gameSerializable.playerInfo = new GameSerializable.PlayerInformationSerializable();
		gameSerializable.enemyInfo = new GameSerializable.EnemyInformationSerializable();
		gameSerializable.uebStats = new GameSerializable.UEBStatisticsSerializable[ueb.Length];

		gameSerializable.playerInfo = SavePlayerInfo();
		gameSerializable.gameInfo = SaveGameInfo();
		gameSerializable.enemyInfo = SaveEnemyInfo();
		gameSerializable.uebStats = SaveUEBStats(ueb);

		saveLoad.Save(gameSerializable);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameSerializable.PlayerInformationSerializable SavePlayerInfo()
	{
		GameSerializable.PlayerInformationSerializable pi = new GameSerializable.PlayerInformationSerializable();

		pi.playerWater = Player.r_Water;
		pi.playerWood = Player.r_Wood;
		pi.playFood = Player.r_Food;

		pi.unitCap = Player.unitCap;
		pi.currentUnits = Player.currentUnits;

		return pi;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameSerializable.GameInformationSerializable SaveGameInfo()
	{
		GameSerializable.GameInformationSerializable gi = new GameSerializable.GameInformationSerializable();

		gi.currentCooldown = currentCooldown;
		gi.savedGame = savedGame;
		gi.unitCreationCooldown = unitCreationCooldown;

		return gi;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="ueb"></param>
	/// <returns></returns>
	public GameSerializable.UEBStatisticsSerializable[] SaveUEBStats(UEBStatistics[] ueb)
	{
		GameSerializable.UEBStatisticsSerializable[] uebStats = new GameSerializable.UEBStatisticsSerializable[ueb.Length];

		foreach (UEBStatistics u in ueb)
		{
			u.SetLocation();
		}

		for (int i = 0; i < ueb.Length; ++i)
		{
			uebStats[i].attack = ueb[i].attack;
			uebStats[i].defense = ueb[i].defense;
			uebStats[i].currentHealth = ueb[i].currentHealth;
			uebStats[i].maxHealth = ueb[i].maxHealth;
			uebStats[i].attack = ueb[i].attack;
			uebStats[i].type = ueb[i].type;
			uebStats[i].locationX = ueb[i].locationX;
			uebStats[i].locationY = ueb[i].locationY;
			uebStats[i].locationZ = ueb[i].locationZ;
		}

		return uebStats;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameSerializable.EnemyInformationSerializable SaveEnemyInfo()
	{
		GameSerializable.EnemyInformationSerializable enemyInfo = new GameSerializable.EnemyInformationSerializable();

		enemyInfo.enemyPopulation = enemyBehavior.enemyPopulation;
		enemyInfo.firstSpawn = enemyBehavior.firstSpawn;
		enemyInfo.gameOver = enemyBehavior.gameOver;
		enemyInfo.timeToSpawn = enemyBehavior.getTimeToSpawn;
		enemyInfo.waveCount = enemyBehavior.waveCount;

		return enemyInfo;
	}

	#endregion

	#region LOAD SAVED GAME

	/// <summary>
	/// 
	/// </summary>
	public void StartLoadGame()
	{
		saveLoad = new SaveLoad();
		gameSerializable = new GameSerializable();

		gameSerializable = saveLoad.Load();

		Player.DestroyAllUnitsAndBuildings();
		enemyBehavior.DestroyAllEnemies();

		SetGameInfo(gameSerializable.gameInfo);
		SetPlayerInfo(gameSerializable.playerInfo);
		SetEnemyInfo(gameSerializable.enemyInfo);
		SetUEBStats(gameSerializable.uebStats);

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="from"></param>
	public void SetGameInfo(GameSerializable.GameInformationSerializable from)
	{
		currentCooldown = from.currentCooldown;
		savedGame = from.savedGame;
		unitCreationCooldown = from.unitCreationCooldown;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="from"></param>
	public void SetPlayerInfo(GameSerializable.PlayerInformationSerializable from)
	{
		Player.r_Water = from.playerWater;
		Player.r_Wood = from.playerWood;
		Player.r_Food = from.playFood;

		Player.unitCap = from.unitCap;
		Player.currentUnits = from.currentUnits;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="from"></param>
	public void SetEnemyInfo(GameSerializable.EnemyInformationSerializable from)
	{
		enemyBehavior.enemyPopulation = from.enemyPopulation;
		enemyBehavior.firstSpawn = from.firstSpawn;
		enemyBehavior.gameOver = from.gameOver;
		enemyBehavior.getTimeToSpawn = from.timeToSpawn;
		enemyBehavior.waveCount = from.waveCount;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="from"></param>
	public void SetUEBStats(GameSerializable.UEBStatisticsSerializable[] from)
	{
		GameObject temp = new GameObject();
		Vector3 location = new Vector3();

		for (int i = 0; i < from.Length; ++i)
		{
			location = new Vector3(from[i].locationX, from[i].locationY, from[i].locationZ);

			switch (from[i].type)
			{
				case "HouseElf":
					temp = (GameObject)Instantiate(houseElf, location, Quaternion.identity);
					break;

				case "Witch":
					temp = (GameObject)Instantiate(witch, location, Quaternion.identity);

					break;

				case "Wizard":
					temp = (GameObject)Instantiate(wizard, location, Quaternion.identity);

					break;

				case "PotionsTent":
					temp = (GameObject)Instantiate(potionsTent, location, Quaternion.identity);

					break;

				case "BaseCamp":
					temp = (GameObject)Instantiate(baseCamp, location, Quaternion.identity);

					break;

				case "Wall":
					temp = (GameObject)Instantiate(wall, location, Quaternion.identity);

					break;

				//Enemies aren't specified as to what type of enemy they are; this is becuase it does not matter at all. The only difference will be the size.
				case "Enemy":
					temp = (GameObject)Instantiate(enemyBehavior.enemyType[UnityEngine.Random.Range(0, enemyBehavior.enemyType.Length)], location, Quaternion.identity);

					break;

				default:
					//We should NEVER EVER end up in here. but just in case....
					break;
			}

			temp.GetComponent<UEBStatistics>().setStatistics(from[i].currentHealth, from[i].maxHealth, from[i].defense, from[i].attack, from[i].type, from[i].locationX, from[i].locationY, from[i].locationZ);
		}
	}

	#endregion

	#endregion

	/// <summary>
	/// Units will be upgraded based the type the player chose. It costs 10 of every resource to upgrade a unit, the function will stop updgrading once either:
	/// A) The player does not have enough resources 
	/// B) All units of the specified type have been upgraded (in THIS cycle)
	/// 
	/// ----This needs to be completely redone.----
	///   -----**I hate the way this works**-----
	/// </summary>
	/// <param name="unit"></param>
	public void UpgradeUnit(GameObject unit)
	{
		foreach (GameObject u in Player.units)
		{
			if (unit.GetComponent<UnitController>().type == u.GetComponent<UnitController>().type)
			{
				if (Player.r_Food - upgradeUnitCost_food >= 0 &&
					Player.r_Water - upgradeUnitCost_water >= 0 &&
					Player.r_Wood - upgradeUnitCost_wood >= 0)
				{
					Player.r_Food -= upgradeUnitCost_food;
					Player.r_Water -= upgradeUnitCost_water;
					Player.r_Wood -= upgradeUnitCost_wood;

					u.GetComponent<UnitController>().LevelUp();
				}
			}
		}
	}

}


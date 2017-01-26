using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	public int unitCap = 10;
	public int currentUnits = 0;
	public UnitController selectedUnit = null;
	public BuildingController selectedBuilding = null;
	public ResourceController selectedResource = null;
	public EnemyBehavior enemyBehavior;

	public GameObject[] units;
	public GameObject[] buildings;

	#region RESOURCES

	public int r_Water = 0;
	public int r_Wood = 0;
	public int r_Food = 0;

	#endregion

	#region START / UPDATES

	void Start()
	{
		Initialize();

		UpdateUnitsAndBuildings();
	}

	/// <summary>
	/// 
	/// </summary>
	public void Initialize()
	{
		r_Food = 200;
		r_Water = 200;
		r_Wood = 200;

		Time.timeScale = 1;
	}

	void Update()
	{ }

	/// <summary>
	/// The player maintains a list of their own buildings and units
	/// </summary>
	public void UpdateUnitsAndBuildings()
	{
		units = GameObject.FindGameObjectsWithTag("Friendly");
		buildings = GameObject.FindGameObjectsWithTag("FriendlyBuilding");

		currentUnits = units.Length;
	}

	#endregion

	#region MOUSE ACTIONS

	/// <summary>
	/// Originally this was RightClick. Moved the mouse functionality portion to GameBehavior, to handles menus and such.
	/// </summary>
	/// <param name="target">Location of the right click</param>
	public void RightClickAction(RaycastHit target)
	{
		//moving to new location
		if (target.collider.gameObject.tag == "Map")
		{
			selectedUnit.Move(target.point);
		}
		//selected a building to build or repair
		else if (target.collider.gameObject.tag == "FriendlyBuilding")
		{
			selectedUnit.Move(target.point);
		}
		//selected a resource to collect
		else if (target.collider.gameObject.tag == "Resource")
		{
			if (target.collider.gameObject.name == "Food")
			{
				selectedUnit.CollectResources(target.point, 1);

			}
			else if (target.collider.gameObject.name == "Water")
			{
				selectedUnit.CollectResources(target.point, 2);

			}
			else if (target.collider.gameObject.name == "Tree")
			{
				selectedUnit.CollectResources(target.point, 3);

			}
		}
		//selected an enemy unit or building to attack
		else if (target.collider.gameObject.tag == "Enemy")
		{
			selectedUnit.Combat(target.collider.gameObject.GetComponent<UEBStatistics>());
		}
	}

	/// <summary>
	/// Originally this was LeftClick. Moved the mouse functionality portion to GameBehavior, to handles menus and such.
	/// </summary>
	/// <param name="targetInfo">The location of the left click</param>
	public void LeftClickAction(RaycastHit targetInfo)
	{
		string tag = targetInfo.collider.gameObject.tag;
		string name = targetInfo.transform.name.ToString();

		if (selectedUnit && selectedUnit.currentState == 7)		//Building...
		{

			RaycastHit info = new RaycastHit();
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info);

			Instantiate(selectedUnit.toBuild, new Vector3(info.point.x, 2, info.point.z), Quaternion.identity);
			selectedUnit.currentState = 0;
		}
		else
		{
			if (selectedUnit)
				selectedUnit.showSelected.gameObject.SetActive(true);

			if (tag == "FriendlyBuilding")
			{
				if (selectedUnit)
					selectedUnit.showSelected.gameObject.SetActive(false);

				selectedBuilding = targetInfo.collider.gameObject.GetComponent<BuildingController>();

			}
			else if (name != "Map : Plane")
			//Map plane is specified here because if a unit or building was selected, when clicking on the menu 
			//- the click was ACTUALLY on the map, causing the menu to close and the unit to not be selected anymore
			{
				if (selectedUnit)
					selectedUnit.showSelected.gameObject.SetActive(false);

				selectedBuilding = null;
			}

			if (tag == "Resource")
			{
				if (selectedUnit)
					selectedUnit.showSelected.gameObject.SetActive(false);

				selectedResource = targetInfo.collider.gameObject.GetComponent<ResourceController>();

			}
			else
			{
				selectedResource = null;

			}

			if (tag == "Friendly")
			{
				if (selectedUnit)
					selectedUnit.showSelected.gameObject.SetActive(false);

				selectedUnit = targetInfo.collider.gameObject.GetComponent<UnitController>();

				selectedUnit.showSelected.gameObject.SetActive(true);

			}
			else if (name != "Map : Plane")
			// ^^See explaination above^^
			{
				selectedUnit = null;
			}
		}
	}

	#endregion

	/// <summary>
	/// 
	/// </summary>
	public void DestroyAllUnitsAndBuildings()
	{
		if(selectedBuilding)
			selectedBuilding = null;

		if(selectedResource)
			selectedResource = null;

		if (selectedUnit)
		{
			selectedUnit.showSelected.gameObject.SetActive(false);
			selectedUnit = null;
		}

		foreach (GameObject u in units)
		{
			Destroy(u);
		}

		foreach(GameObject b in buildings)
		{
			Destroy(b);
		}

		UpdateUnitsAndBuildings();
	}	
}
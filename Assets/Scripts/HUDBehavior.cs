using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDBehavior : MonoBehaviour
{
	#region GAMEOBJECTS

	public PlayerControl Player;

	#region PANELS

	public GameObject UnitStatisticsPanel;
	public GameObject BuildingStatisticsPanel;
	public GameObject ResourceDisplayPanel;
	public GameObject UnitMenuPanel;
	public GameObject BaseCampMenuPanel;
	public GameObject PotionsTentMenuPanel;

	#endregion

	#region UI TEXT

	public Text[] UnitStatisticsPanelText;
	public Text[] ResourcePanelText;
	public Text[] ResourceTypeDisplayPanelText;
	public Text[] UnitMenuPanelText;
	public Text[] BuildingStatisticsPanelText;
	public Text PopulationCapText;
	public Text ResourceLackingText;
	public Text ResourceLackingText_UNIT;

	#endregion

	#endregion

	#region START/UPDATES

	void Start()
	{

		PopulationCapText.text = ("Population: " + Player.currentUnits + "/" + Player.unitCap);

	}

	void Update()
	{
		UpdateResources();
		PopulationCapText.text = ("Population: " + Player.currentUnits + "/" + Player.unitCap);
	}

	#region UPDATE PANELS

	/// <summary>
	///  Updates panels depending on what object is selected
	/// </summary>
	public void UpdatePanels()
	{
		if (Player.selectedUnit != null && Player.selectedUnit.currentHealth > 0)
			UpdateUnitStatisticsPanel();

		else if (Player.selectedBuilding != null && Player.selectedBuilding.currentHealth > 0)
			UpdateBuildingStatisticsPanel();

		else if (Player.selectedResource != null)
			UpdateResourceDisplayPanel();

		else
		{
			ToggleAllPanelsOff();
		}
	}

	/// <summary>
	/// Updates the resources on the top left of the screen - LIVE
	/// </summary>
	public void UpdateResources()
	{
		ResourcePanelText[1].text = "Water: " + Player.r_Water;
		ResourcePanelText[0].text = "Food: " + Player.r_Food;
		ResourcePanelText[2].text = "Wood: " + Player.r_Wood;
	}

	/// <summary>
	/// 
	/// </summary>
	void UpdateUnitStatisticsPanel()
	{
		//UnitStatisticsPanelText = UnitStatisticsPanel.GetComponentsInChildren<Text>();

		UnitStatisticsPanelText[0].text = Player.selectedUnit.type;
		UnitStatisticsPanelText[1].text = Player.selectedUnit.currentHealth + " / " + Player.selectedUnit.maxHealth;
		UnitStatisticsPanelText[2].text = (string)Player.selectedUnit.s_Action;
		UnitStatisticsPanelText[3].text = "Defense: " + Player.selectedUnit.defense;
		UnitStatisticsPanelText[4].text = "Attack: " + Player.selectedUnit.attack;

		if (Player.selectedUnit.type == "HouseElf")
		{
			MainMenuBehavior.ToggleMenuOn(UnitMenuPanel);
			MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);
			MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);

		}
		else
		{
			MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);
			MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);
			MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);

		}
	}

	/// <summary>
	/// 
	/// </summary>
	void UpdateBuildingStatisticsPanel()
	{
		//BuildingStatisticsPanelText = BuildingStatisticsPanel.GetComponentsInChildren<Text>();

		BuildingStatisticsPanelText[0].text = Player.selectedBuilding.type;
		BuildingStatisticsPanelText[1].text = Player.selectedBuilding.currentHealth + " / " + Player.selectedBuilding.maxHealth;
		BuildingStatisticsPanelText[2].text = "Action: " + Player.selectedBuilding.s_Action;
		BuildingStatisticsPanelText[3].text = "Defense: " + Player.selectedBuilding.defense;

		if (Player.selectedBuilding.type == "BaseCamp")
		{
			MainMenuBehavior.ToggleMenuOn(BaseCampMenuPanel);
			MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);
			MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);

		}
		else if (Player.selectedBuilding.type == "PotionsTent")
		{
			MainMenuBehavior.ToggleMenuOn(PotionsTentMenuPanel);
			MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);
			MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);
		}
		else
		{
			MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);
			MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);
			MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void UpdateResourceDisplayPanel()
	{
		ResourceTypeDisplayPanelText[0].text = "Resource";

		ResourceTypeDisplayPanelText[1].text = Player.selectedResource.s_ResourceType;

		MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);
		MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);
		MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);
	}



	#endregion

	#endregion

	/// <summary>
	///  Toggles all panels off
	/// </summary>
	void ToggleAllPanelsOff()
	{
		MainMenuBehavior.ToggleMenuOff(UnitMenuPanel);
		MainMenuBehavior.ToggleMenuOff(BaseCampMenuPanel);
		MainMenuBehavior.ToggleMenuOff(PotionsTentMenuPanel);
		MainMenuBehavior.ToggleMenuOff(UnitStatisticsPanel);
		MainMenuBehavior.ToggleMenuOff(BuildingStatisticsPanel);
		MainMenuBehavior.ToggleMenuOff(ResourceDisplayPanel);
	}

	/// <summary>
	/// Manages stat panels; opens and closes depending on the selected object
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="name"></param>
	public void ManageStatisticsPanel(string tag, string name)
	{
		if (tag == "Friendly")
		{
			MainMenuBehavior.ToggleMenuOn(UnitStatisticsPanel);		//turn on unit panel
		}
		else if (name != "Map : Plane")
		//Map plane is specified here because if a unit or building was selected, when clicking on the menu 
		//- the click was ACTUALLY on the map, causing the menu to close and the unit to not be selected anymore
		{
			MainMenuBehavior.ToggleMenuOff(UnitStatisticsPanel);
		}

		if (tag == "FriendlyBuilding")
		{
			MainMenuBehavior.ToggleMenuOn(BuildingStatisticsPanel);

		}
		else if (name != "Map : Plane")
		//^^See explanation above^^
		{
			MainMenuBehavior.ToggleMenuOff(BuildingStatisticsPanel);
		}

		if (tag == "Resource")
		{
			MainMenuBehavior.ToggleMenuOn(ResourceDisplayPanel);
		}
		else
		{
			MainMenuBehavior.ToggleMenuOff(ResourceDisplayPanel);
		}
	}
}

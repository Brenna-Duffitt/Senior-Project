using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class MainMenuBehavior : MonoBehaviour
{
	public GameObject MainMenu;
	public GameBehavior gameBehavior;

	const string Address = "http://twitter.com/intent/tweet";

	/// <summary>
	/// 
	/// </summary>
	/// <param name="NewMenu"></param>
	public static void ToggleMenuOn(GameObject NewMenu)
	{
		if (NewMenu.activeSelf == false)
			NewMenu.SetActive(true);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="OldMenu"></param>
	public static void ToggleMenuOff(GameObject OldMenu)
	{
		if (OldMenu.activeSelf == true)
			OldMenu.SetActive(false);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="NewMenu"></param>
	public void ToggleMenuOnFromMain(GameObject NewMenu)
	{
		if (NewMenu.activeSelf == false)
			NewMenu.SetActive(true);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="OldMenu"></param>
	public void ToggleMenuOffFromMain(GameObject OldMenu)
	{
		if (OldMenu.activeSelf == true)
			OldMenu.SetActive(false);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="scene"></param>
	public void LoadScene(int scene)
	{
		Application.LoadLevel(scene);
	}

	/// <summary>
	/// 
	/// </summary>
	public void newGame()
	{
		Application.LoadLevel(1);
	}

	/// <summary>
	/// 
	/// </summary>
	public void SaveGame()
	{
		gameBehavior.startGameSave();
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadSavedGame()
	{
		gameBehavior.StartLoadGame();
	}

	/// <summary>
	/// Closes the game
	/// </summary>
	public void QuitApplication()
	{
		Application.Quit();
		Debug.Log("QuitApplication");
	}

	/// <summary>
	/// -----Networking Component-----
	/// -------Share to Twitter-------
	/// </summary>
	public void Share()
	{
		string info = ("I've survived " + (gameBehavior.enemyBehavior.waveCount - 1) + " waves in Morsemordre for PC by Brenna Duffitt");

		Application.OpenURL(Address +
			"?text=" + WWW.EscapeURL(info) +
			"&amp;lang=" + WWW.EscapeURL("en"));
	}
}


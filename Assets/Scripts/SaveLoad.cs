using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{

	public List<GameSerializable> savedGames = new List<GameSerializable>();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="current"></param>
	public void Save(GameSerializable current)
	{
		savedGames.Add(current);
		BinaryFormatter bf = new BinaryFormatter();
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		FileStream file = File.Create(Application.persistentDataPath + "/savegame.gd"); //you can call it anything you want

		bf.Serialize(file, savedGames);
		file.Close();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameSerializable Load()
	{
		if (File.Exists(Application.persistentDataPath + "/savegame.gd"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savegame.gd", FileMode.Open);
			savedGames = (List<GameSerializable>)bf.Deserialize(file);
			file.Close();
		}

		Debug.Log("SAVED GAME:" + savedGames[0].uebStats[0].type);
		return savedGames[0];
	}


}
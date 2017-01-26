using UnityEngine;
using System.Collections;

public class UEBStatistics : MonoBehaviour
{
	public double currentHealth;
	public double maxHealth;
	public double defense;
	public double attack;
	public string type;

	public float locationY;
	public float locationX;
	public float locationZ;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="_currentHealth"></param>
	/// <param name="_maxHealth"></param>
	/// <param name="_defense"></param>
	/// <param name="_attack"></param>
	/// <param name="_type"></param>
	/// <param name="_locationX"></param>
	/// <param name="_locationY"></param>
	/// <param name="_locationZ"></param>
	public void setStatistics(double _currentHealth, double _maxHealth, double _defense, double _attack, string _type, float _locationX, float _locationY, float _locationZ)
	{
		currentHealth = _currentHealth;
		maxHealth = _maxHealth;
		defense = _defense;
		attack = _attack;
		type = _type;
		locationX = _locationX;
		locationY = _locationY;
		locationZ = _locationZ;
	}

	//public void CreateUnit()
	//{
	//	Vector3 location = {locationX, locationY, locationZ};
	//	Instantiate(houseElf, location, Quaternian.identity);
	//}

	/// <summary>
	/// only used for saving and loading a saved game
	/// </summary>
	public void SetLocation()
	{
		locationX = this.transform.localPosition.x;
		locationY = this.transform.localPosition.y;
		locationZ = this.transform.localPosition.z;

		//scale = this.transform.localScale;
	}
}

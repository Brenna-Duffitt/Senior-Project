using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject InGameMainMenu;

	//The Map
	public GameObject Plane;

	//How far the camera can scroll to each side
	private float ScrollMaxVertical;
	private float scrollMaxHorizontal;

	//because the magnitude is larger than how far we need to scroll we're adding this random floating point
	//this makes the camera lock in a position, so the map (plane) stays in view
	private const float pixelScalarVertical = 0.3F;
	private const float pixelScalarHorizontal = 0.23F;

	//How many pixels to move the camera at once
	private const int scrollDistance = 5;
	
	//How fast the camera scrolls
	private const float scrollSpeed = 60;

	void Awake()
	{

	}	

	void Start()
	{
		//this calculation changes the camera's maximum scroll distance, and scales off the size of the plane.
		//THE PLANE MUST BE A SQUARE FOR THIS TO WORK
		ScrollMaxVertical = (Plane.GetComponent<Renderer>().bounds.size).magnitude * pixelScalarVertical;
		scrollMaxHorizontal = (Plane.GetComponent<Renderer>().bounds.size).magnitude * pixelScalarHorizontal;

	}
									 	
	// Update is called once per frame
	void Update () 
	{
		 //edge scrolling variables
		float mousePosX = Input.mousePosition.x;
		float mousePosY = Input.mousePosition.y;

		//edge scrolling code
		if(!InGameMainMenu.activeSelf)
		{
			if (mousePosX < scrollDistance)
			{
				//left
				if (transform.position.x > -scrollMaxHorizontal)
					transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime);
			}

			if (mousePosX >= Screen.width - scrollDistance)
			{
				//right
				if (transform.position.x < scrollMaxHorizontal)
					transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);
			}

			if (mousePosY < scrollDistance)
			{
				//down
				if (transform.position.z > -ScrollMaxVertical)
					transform.Translate(transform.forward * scrollSpeed * Time.deltaTime);
			}

			if (mousePosY >= Screen.height - scrollDistance)
			{
				//up
				if (transform.position.z < ScrollMaxVertical)
					transform.Translate(transform.forward * -scrollSpeed * Time.deltaTime);
			
			}
		}
	}
}

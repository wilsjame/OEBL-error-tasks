/* Keep alive and store task settings
 * here. Then pass to the next scene for
 * configuration. */
using UnityEngine;
using System.Collections;

public class TaskConfig : MonoBehaviour
{

	//TODO bug: selections do not update after coming back from a scene
	//try moving up tree or own do not destroy on load
	
	// A task has either small, medium, or large cubes.
	// It is made up of three trials. 
	// The trials are random, 1, 3, and 5 taps. 
	// A task's cube size and trial order are stored and shared here.
	public string size;
	public string trial_1;   
	public string trial_2;
	public string trial_3;
	//public string trial_4;
	
	void Start()
	{
		// defauit radial selections
		size = "small";
		trial_1 = "random";
		trial_2 = "random";
		trial_3 = "random";
		//trial_4 = "random";
	}

	// size 
	public void setSize(string s)
	{
		size = s;
	}

	// trial error
	public void setTrial1Error(string e)
	{
		trial_1 = e;
	}

	public void setTrial2Error(string e)
	{
		trial_2 = e;
	}

	public void setTrial3Error(string e)
	{
		trial_3 = e;
	}

    /*
	public void setTrial4Error(string e)
	{
		trial_4 = e;
	}
    */

}

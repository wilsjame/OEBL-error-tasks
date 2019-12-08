/* Keep alive and store task settings
 * here. Then pass to the next scene for
 * configuration. */
using UnityEngine;
using System.Collections;
using HoloToolkit.Examples.InteractiveElements;

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

    public string pointing_error;
    public string number_of_trials;

    public float sliderValueMainMenu_SphereSize;
    public float sliderValueMainMenu_SphereCollectionSize;

    void Start()
    {
        // defauit radial selections

        // Pointing, cube, cube random plane
        number_of_trials = "1";

        // cube tasks
        size = "small";
        trial_1 = "random";
        trial_2 = "random";
        trial_3 = "random";
        //trial_4 = "random";

        // pointing tasks
        pointing_error = "random";

        // default if slider not moved
        if (sliderValueMainMenu_SphereCollectionSize == 0)
        {
            sliderValueMainMenu_SphereCollectionSize = 0.5f;
        }

    }

    // Pointing, cube, cube random plane
    public void setNumberOfTrials(string n)
    {
        number_of_trials = n;
    }

    // cube tasks
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

    // pointing tasks
    public void setPointingError(string s)
    {
        pointing_error = s;
    }

    public void getSlider_SphereSize()
    {
        GameObject slider = GameObject.Find("Sphere_Size_Slider"); // Grab sphere size slider from scene
        SliderGestureControl sliderScript = slider.GetComponent<SliderGestureControl>(); // Grab script off of slider
        sliderValueMainMenu_SphereSize = sliderScript.GetSliderValue();
    }

    public void getSlider_SphereCollectionSize()
    {
        GameObject slider = GameObject.Find("Collection_Size_Slider"); // Grab sphere size slider from scene
        SliderGestureControl sliderScript = slider.GetComponent<SliderGestureControl>(); // Grab script off of slider
        sliderValueMainMenu_SphereCollectionSize = sliderScript.GetSliderValue();
    }
}
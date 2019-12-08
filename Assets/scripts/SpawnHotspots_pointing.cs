// For use with single plane pointing task.
// TODO fix infinite NullReferenceException after task completion.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading;


public class SpawnHotspots_pointing : MonoBehaviour {

	/* Parent gameObject to hold generated hotspot collection */
	GameObject parentObject;

    // move these if sphere size collection radius is large enough
    private GameObject counter_label;
    private GameObject slider_manager;

	/* Prefabs */
	public Transform static_point;
	public Transform trigger_point;
    public Transform trial_counter;
	private GameObject camera;

	/* Encapsulated trial counter coordinates */ 
	public struct CoOrds
	{
		public float x, y, z;
		public string plane;

		// Constructor to initiliaze x, y, and z coordinates
		public CoOrds(float x_coOrd, float y_coOrd, float z_coOrd, string p)
		{
			x = x_coOrd;
			y = y_coOrd;
			z = z_coOrd;
			plane = p;
		}
	}

	List<List<CoOrds>> coOrds_collection = new List<List<CoOrds>> ();	/* Entire point collection */
	List<CoOrds> coOrds_collection_2 = new List<CoOrds> (); /* z = 0.3 frame points */
    List<CoOrds> counter_collection = new List<CoOrds>();   /* Trial counter coordinates */
    public int[] order = {0};				/* Plane spawn order */
	public int itr = 0;					/* Keep track of list iterations */
	public int trial = 0;					/* Keep track of completed trials */
    private int total_trials; // number of trials selected from the task set up menu 

	public string fileName = "pointing_task_time_";
	public Stopwatch stopwatch = new Stopwatch();
	public string path;

    // Settings
    private TaskConfig config;
    private float radius; // set from main menu slider
    
	/* Use this for initialization */
	void Start () {

		// Position camera
		camera = GameObject.Find("MixedRealityCameraParent");
		camera.transform.position = new Vector3(0.0f, 0.0f, -2.75f);

		// Create unique out file 
		fileName = fileName + System.DateTime.Now + ".txt";
		fileName = fileName.Replace("/","-");
		fileName = fileName.Replace(":",";");
		path = Path.Combine(Application.persistentDataPath, fileName);
        //Test outfile
        //File.WriteAllText(@path, "trace");

        // Write task set up to results file
        // Error : R, 1, 3, 5
        // # Trials : 1, 2, 3
        config = GameObject.Find("TaskConfig").GetComponent<TaskConfig>();
        //UnityEngine.Debug.Log("pointing_error: " + config.pointing_error);
        //UnityEngine.Debug.Log("number_of_trials: " + config.number_of_trials);
        File.AppendAllText(@path, "Trials  : " + config.number_of_trials);
        File.AppendAllText(@path, "\r\n"); 
        File.AppendAllText(@path, "Error   : " + config.pointing_error);
        File.AppendAllText(@path, "\r\n");

        // convert # trials from string to int
        //TODO change to generic trials?
        switch (config.number_of_trials)
        {
            case "1":
                total_trials = 1;
                break;
            case "2":
                total_trials = 2;
                break;
            case "3":
                total_trials = 3;
                break;
            default:
                //
                break;
        }

        // set radius from main menu preset
        radius = config.sliderValueMainMenu_SphereCollectionSize;

        // move counter and sliders depending on how large the radius is
        counter_label = GameObject.Find("counter label");
        slider_manager = GameObject.Find("Slider_Manager");

        if (radius >= 0.75)
        {
            counter_label.transform.position = new Vector3(1.415f, 0.6f, 0f);
            slider_manager.transform.position = new Vector3(2.4f, 0.35f, 0f);

            CoOrds counter_1 = new CoOrds(0.71f + 0.6f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_1);
            CoOrds counter_2 = new CoOrds(0.81f + 0.6f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_2);
            CoOrds counter_3 = new CoOrds(0.91f + 0.6f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_3);
        }
        else if (radius >= 0.70f)
        {
            counter_label.transform.position = new Vector3(1.265f, 0.6f, 0f);
            slider_manager.transform.position = new Vector3(2.25f, 0.35f, 0f);

            CoOrds counter_1 = new CoOrds(0.71f + 0.45f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_1);
            CoOrds counter_2 = new CoOrds(0.81f + 0.45f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_2);
            CoOrds counter_3 = new CoOrds(0.91f + 0.45f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_3);
        }
        else if (radius >= 0.65)
        {
            counter_label.transform.position = new Vector3(1.115f, 0.6f, 0f);
            slider_manager.transform.position = new Vector3(2.1f, 0.35f, 0f);

            CoOrds counter_1 = new CoOrds(0.71f + 0.3f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_1);
            CoOrds counter_2 = new CoOrds(0.81f + 0.3f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_2);
            CoOrds counter_3 = new CoOrds(0.91f + 0.3f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_3);
        }
        else
        {           
            CoOrds counter_1 = new CoOrds(0.71f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_1);
            CoOrds counter_2 = new CoOrds(0.81f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_2);
            CoOrds counter_3 = new CoOrds(0.91f, 0.5f, 0.0f, null);
            counter_collection.Add(counter_3);
        }

            /* Generate */
            initializeCoordinates(ref order, ref coOrds_collection, ref coOrds_collection_2);

		/* Call function once on startup to create initial hotspot */
		HotSpotTriggerInstantiate ();
	
	}

	/* Generate circular arrays of static points */ 
	public void initializeCoordinates (ref int[] order, ref List<List<CoOrds>> coOrds_collection, ref List<CoOrds> coOrds_collection_2)
	{
		int i;
		int temp;
		CoOrds temp_vector;
		int random_placeholder;
		int numberOfObjects = 18;

		/* z = 0.3 frame */
		for (i = 0; i < numberOfObjects; i++) {
			float angle = i * Mathf.PI * 2 / numberOfObjects;
			CoOrds pos = new CoOrds(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.3f, "middle");
			coOrds_collection_2.Add(pos);
		}

		/* Shuffle */
		for (i = 0; i < numberOfObjects; i++) {
			random_placeholder = i + Random.Range (0, numberOfObjects - i);

			/* Swap */
			temp_vector = coOrds_collection_2[i];
			coOrds_collection_2[i] = coOrds_collection_2[random_placeholder];
			coOrds_collection_2[random_placeholder] = temp_vector;
		}

		/* Add plane to entire collection */
		coOrds_collection.Add(coOrds_collection_2);

        /* Trial counters */
        /*
        CoOrds counter_1 = new CoOrds(0.71f, 0.5f, 0.0f, null);
        counter_collection.Add(counter_1);
        CoOrds counter_2 = new CoOrds(0.81f, 0.5f, 0.0f, null);
        counter_collection.Add(counter_2);
        CoOrds counter_3 = new CoOrds(0.91f, 0.5f, 0.0f, null);
        counter_collection.Add(counter_3);
        */

        /* Spawn initial static points */
        for (i = 0; i < numberOfObjects; i++) { 
			temp_vector = coOrds_collection[order[trial]] [i];
			Transform static_pt = Instantiate(static_point, new Vector3 (temp_vector.x, temp_vector.y, temp_vector.z), Quaternion.identity, this.transform); // Make this gameObject the parent
			
		}

	}

	/* Spawn trigger points until 1 plane is completed */
	public void HotSpotTriggerInstantiate ()
	{
		/* check if user has tapped first point */
		if (itr == 1) {
            UnityEngine.Debug.Log("Trial " + (trial + 1) + " timing start");
            // Begin timing
            stopwatch.Start();
		}

		CoOrds coords_temp = new CoOrds ();

		/* Spawn trigger points */ 
		if ( itr < coOrds_collection[order[0]].Count) { // hard set order[0], previously order[trial], to account for variable # trials
			coords_temp = coOrds_collection[order[0]] [itr]; // hard set order[0], previously order[trial], to account for variable # trials
            Transform trigger = Instantiate (trigger_point, new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity, this.transform); // Make this gameObject the parent
			trigger.localPosition = new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z); // Spawn position relative to parent
			itr++;
		}

		/* Trial is completed */
		else {

            // Spawn trial counter
            trial++;
            UnityEngine.Debug.Log("Trial " + trial + " completed!");
            coords_temp = counter_collection[trial - 1];
            Instantiate(trial_counter, new Vector3(coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity);

            // Stop timing
            System.TimeSpan ts = stopwatch.Elapsed;
			stopwatch.Stop();
			UnityEngine.Debug.Log("Time elapsed: " + ts);
			stopwatch.Reset();

			// Write time to file
            File.AppendAllText(@path, "Trial " + trial + " : ");
            File.AppendAllText(@path, ts.ToString());
            File.AppendAllText(@path, "\r\n");

            if (trial < total_trials)
            {
                UnityEngine.Debug.Log("Trial " + trial + " completed!");
                // reset
                itr = 0;

                // shuffle and spawn first trigger point for next trial
                int numberOfObjects = 18;
                int random_placeholder;

                for (int i = 0; i < numberOfObjects; i++)
                {
                    random_placeholder = i + Random.Range(0, numberOfObjects - i);

                    /* Swap */
                    coords_temp = coOrds_collection_2[i];
                    coOrds_collection_2[i] = coOrds_collection_2[random_placeholder];
                    coOrds_collection_2[random_placeholder] = coords_temp;
                }

                coords_temp = coOrds_collection[order[0]][itr]; // hard set order[0], previously order[trial], to account for variable # trials
                Transform trigger = Instantiate(trigger_point, new Vector3(coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity, this.transform); // Make this gameObject the parent
                trigger.localPosition = new Vector3(coords_temp.x, coords_temp.y, coords_temp.z); // Spawn position relative to parent
                itr++;
            }
            else
            {
                UnityEngine.Debug.Log("All trials completed!");
                //TODO
                // spawn "completed!" text
            }

		}

	}

}

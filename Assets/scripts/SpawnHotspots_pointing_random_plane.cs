﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Threading;


public class SpawnHotspots_pointing_random_plane : MonoBehaviour {

	/* Parent gameObject to hold generated hotspot collection */
	GameObject parentObject;

    // move these if sphere size collection radius is large enough
    private GameObject counter_label;
    private GameObject slider_manager;

    /* Prefabs */
    public Transform static_point;
	public Transform trigger_point;
	public Transform trial_counter;
    public Transform finish_label;
    private GameObject camera;

	/* Encapsulated trial counter coordinates */ 
	public struct CoOrds
	{
		public float x, y, z;
		public string plane;

		/* Constructor to initiliaze x, y, and z coordinates */
		public CoOrds(float x_coOrd, float y_coOrd, float z_coOrd, string p)
		{
			x = x_coOrd;
			y = y_coOrd;
			z = z_coOrd;
			plane = p;
		}
	}

	List<List<CoOrds>> coOrds_collection = new List<List<CoOrds>> ();	/* Entire point collection */
	List<CoOrds> coOrds_collection_1 = new List<CoOrds> (); /* z = 0.0 frame points */
	List<CoOrds> coOrds_collection_2 = new List<CoOrds> (); /* z = 0.3 frame points */
	List<CoOrds> coOrds_collection_3 = new List<CoOrds> (); /* z = 0.6 frame points */
	List<CoOrds> counter_collection = new List<CoOrds> (); 	/* Trial counter coordinates */ 
	public int[] order = {0, 1, 2};				/* Plane spawn order */
	public int itr = 0;					/* Keep track of list iterations */
	public int plane = 0;					/* Keep track of completed planes */
	public int trial = 0;					/* Keep track of completed trials */

	public string fileName = "pointing_random_plane_task_time_";
	public string path;

	public Stopwatch trial_stopwatch = new Stopwatch();
	public Stopwatch plane_stopwatch = new Stopwatch();
	public System.TimeSpan trial_time;
	public System.TimeSpan plane_time;

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
		UnityEngine.Debug.Log(fileName);
		UnityEngine.Debug.Log(Application.persistentDataPath);

        // Write task set up to results file
        // Error : R, 1, 3, 5
        // # Trials : 1, 2, 3
        config = GameObject.Find("TaskConfig").GetComponent<TaskConfig>();
        //UnityEngine.Debug.Log("pointing_error: " + config.pointing_error);
        //UnityEngine.Debug.Log("pointing_trials: " + config.pointing_trials);
        /*
        File.AppendAllText(@path, "Trials  : " + config.pointing_trials);
        File.AppendAllText(@path, "\r\n");
        */
        File.AppendAllText(@path, "Error   : " + config.pointing_error);
        File.AppendAllText(@path, "\r\n");

        // convert # trials from string to int
        /*
        switch (config.pointing_trials)
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
        */

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
        initializeCoordinates (ref order, ref coOrds_collection, ref coOrds_collection_1, ref coOrds_collection_2, ref coOrds_collection_3);

		/* Call function once on startup to create initial hotspot */
		HotSpotTriggerInstantiate ();
	}

	/* Generate circular arrays of static points */ 
	public void initializeCoordinates (ref int[] order, ref List<List<CoOrds>> coOrds_collection, ref List<CoOrds> coOrds_collection_1, ref List<CoOrds> coOrds_collection_2, ref List<CoOrds> coOrds_collection_3)
	{
		int i;
		int temp;
		CoOrds temp_vector;
		int random_placeholder;
		int numberOfObjects = 18;

		/* z = 0 frame */
		for (i = 0; i < numberOfObjects; i++) {
			float angle = i * Mathf.PI * 2 / numberOfObjects;
			CoOrds pos = new CoOrds(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0, "back");
			coOrds_collection_1.Add(pos);
		}

		/* Shuffle */
		for (i = 0; i < numberOfObjects; i++) {
			random_placeholder = i + Random.Range (0, numberOfObjects - i);

			/* Swap */
			temp_vector = coOrds_collection_1[i];
			coOrds_collection_1[i] = coOrds_collection_1[random_placeholder];
			coOrds_collection_1[random_placeholder] = temp_vector;
		}

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

		/* z = 0.6 frame */
		for (i = 0; i < numberOfObjects; i++) {
			float angle = i * Mathf.PI * 2 / numberOfObjects;
			CoOrds pos = new CoOrds(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.6f, "front");
			coOrds_collection_3.Add(pos);
		}

		/* Shuffle */
		for (i = 0; i < numberOfObjects; i++) {
			random_placeholder = i + Random.Range (0, numberOfObjects - i);

			/* Swap */
			temp_vector = coOrds_collection_3[i];
			coOrds_collection_3[i] = coOrds_collection_3[random_placeholder];
			coOrds_collection_3[random_placeholder] = temp_vector;
		}

		/* Add planes to entire collection */
		coOrds_collection.Add(coOrds_collection_1);
		coOrds_collection.Add(coOrds_collection_2);
		coOrds_collection.Add(coOrds_collection_3);

		/* Shuffle plane order */ 
		//UnityEngine.Debug.Log("Plane order before shuffle: " + order[0] + order[1] + order[2]);
		
		for (i = 0; i < 3; i++) {
			random_placeholder = i + Random.Range (0, 3 - i);

			/* Swap */
			temp = order[i];
			order[i] = order[random_placeholder];
			order[random_placeholder] = temp;
		}
		
		//UnityEngine.Debug.Log("Plane order after shuffle: " + order[0] + order[1] + order[2]);

		/* Trial counters */ 
        /*
		CoOrds counter_1 = new CoOrds (0.71f, 0.5f, 0.0f, null);
		counter_collection.Add (counter_1);
		CoOrds counter_2 = new CoOrds (0.81f, 0.5f, 0.0f, null);
		counter_collection.Add (counter_2);
		CoOrds counter_3 = new CoOrds (0.91f, 0.5f, 0.0f, null);
		counter_collection.Add (counter_3);	
        */

		/* Spawn initial static points */ 
		for (i = 0; i < numberOfObjects; i++) {
			temp_vector = coOrds_collection[order[trial]] [i];
			Transform static_pt = Instantiate(static_point, new Vector3 (temp_vector.x, temp_vector.y, temp_vector.z), Quaternion.identity, this.transform); // Make this gameObject the parent

			switch (temp_vector.plane) {
				case "front":
					static_pt.GetComponent<StaticSpot>().plane = "front";
					break;
				case "middle":
					static_pt.GetComponent<StaticSpot>().plane = "middle";
					break;
				case "back":
					static_pt.GetComponent<StaticSpot>().plane = "back";
					break;
			}

		}

	}

	/* Destroy finished plane and spawn a new one */ 
	public void newPlane ()
	{
		CoOrds coords_temp = new CoOrds ();
		itr = 0;

		/* Destroy completed plane */ 
		GameObject[] completed = GameObject.FindGameObjectsWithTag ("static_sphere");

		for (var i = 0; i < completed.Length; i++) {
			Destroy(completed[i]);
		}

		/* Spawn new plane's static points */
		for (int i = 0; i < coOrds_collection[order[plane]].Count; i++) {
			coords_temp = coOrds_collection[order[plane]] [i];
			Transform static_pt = Instantiate (static_point, new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity, this.transform); // Make this gameObject the parent

			switch (coords_temp.plane) {
				case "front":
					static_pt.GetComponent<StaticSpot>().plane = "front";
					break;
				case "middle":
					static_pt.GetComponent<StaticSpot>().plane = "middle";
					break;
				case "back":
					static_pt.GetComponent<StaticSpot>().plane = "back";
					break;
			}

			static_pt.localPosition = new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z); // Spawn position relative to parent
		}

		/* Spawn new plane's intial trigger point */
		coords_temp = coOrds_collection[order[plane]] [itr];
		Transform trigger = Instantiate (trigger_point, new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity, this.transform); // Make this gameObject the parent
		trigger.localPosition = new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z); // Spawn position relative to parent
		itr++;
	}

	/* Spawn trigger points until 3 trials are completed */
	public void HotSpotTriggerInstantiate ()
	{
		int i;
		int temp;
		int random_placeholder;
		CoOrds coords_temp = new CoOrds ();

		/* Check if user has tapped first point */
		if (itr == 1 && plane == 0) {

			// Start timers
			trial_stopwatch.Start();
			plane_stopwatch.Start();
		}
		else if (itr == 1) {

			// Start each plane timer when the plane's first trigger point is activated
			plane_stopwatch.Start();
		}
		
		/* Begin spawning */
		if (plane < 3 && itr != coOrds_collection[order[plane]].Count) {

			/* Spawn the trigger point */ 
			coords_temp = coOrds_collection[order[plane]] [itr];
			Transform trigger = Instantiate (trigger_point, new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity, this.transform); // Make this gameObject the parent
			trigger.localPosition = new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z); // Spawn position relative to parent
			itr++;
		}
		/* Spawn new plane */
		else if (++plane < 3) {
			newPlane();

			// Stop plane timing
			plane_time = plane_stopwatch.Elapsed;
			plane_stopwatch.Stop();
			UnityEngine.Debug.Log("Plane " + plane + " : " + plane_time + " " + GameObject.Find("static_point(Clone)").GetComponent<StaticSpot>().plane);
			plane_stopwatch.Reset();

			// Write plane time to file
			File.AppendAllText(@path, "Plane " + plane + " : ");
			File.AppendAllText(@path, plane_time.ToString() + " " + GameObject.Find("static_point(Clone)").GetComponent<StaticSpot>().plane);
			File.AppendAllText(@path, "\r\n");
		}
		/* Start new trial and spawn counter */
		else if (trial < 3) {

			// Stop plane timing
			System.TimeSpan plane_time = plane_stopwatch.Elapsed;
			plane_stopwatch.Stop();
			UnityEngine.Debug.Log("Plane " + plane + " : " + plane_time + " " + GameObject.Find("static_point(Clone)").GetComponent<StaticSpot>().plane);
			plane_stopwatch.Reset();

			// Write plane time to file
			File.AppendAllText(@path, "Plane " + plane + " : ");
			File.AppendAllText(@path, plane_time.ToString() + " " + GameObject.Find("static_point(Clone)").GetComponent<StaticSpot>().plane);
			File.AppendAllText(@path, "\r\n");

			// Spawn trial counter
			trial++;
			UnityEngine.Debug.Log("Trial " + trial + " completed!");
			coords_temp = counter_collection [trial - 1];
			Instantiate (trial_counter, new Vector3 (coords_temp.x, coords_temp.y, coords_temp.z), Quaternion.identity);

			// Stop trial timing
			System.TimeSpan trial_time = trial_stopwatch.Elapsed;
			trial_stopwatch.Stop();
			UnityEngine.Debug.Log("Trial " + trial + " : " + trial_time);
			trial_stopwatch.Reset();

			// Write trial time to file
			File.AppendAllText(@path, "Trial " + trial + " : ");
			File.AppendAllText(@path, trial_time.ToString());
			File.AppendAllText(@path, "\r\n");

			// Do not time after third trial
			if (trial < 3) {

				// Shuffle plane order before new trial 
				for (i = 0; i < 3; i++) {
					random_placeholder = i + Random.Range (0, 3 - i);

					/* Swap */
					temp = order[i];
					order[i] = order[random_placeholder];
					order[random_placeholder] = temp;
				}
				
				plane = 0;
				newPlane();
			}
			else {
				UnityEngine.Debug.Log("Finish");
                Transform finish = Instantiate(finish_label, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform); // Make this gameobject the parent
                finish.localPosition = new Vector3(0f, 0f, 0f); // Spawn position relative to parent
            }

		}

	}

}


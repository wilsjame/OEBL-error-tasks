/* Attach to game objects intended to be trigger points (hotspots). 
 * This script works in conjuction with HotSpotColorChanger.cs. 
 * Now with added error */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Hotspot : MonoBehaviour
{
    public delegate void HotspotEntered(GameObject someCube);
    public static event HotspotEntered OnEntered;

    public delegate void HotspotExited(GameObject someCube);
    public static event HotspotExited OnExited;

    private Material cachedMaterial;

    Scene m_Scene;

    // Introduced error 
    public Transform small; // cube size
    public Transform medium;
    public Transform large;
    public int n;
    public int cubes_placed;
    Transform local_cube;

    private void Start()
    {

	// Return the current active scene in order to get the current scene's name
	m_Scene = SceneManager.GetActiveScene();

	/* Color hotspot according plane. */
	cachedMaterial = GetComponent<Renderer>().material;

	switch (gameObject.tag) {
		case "front":
			cachedMaterial.SetColor("_Color", Color.yellow);
			break;
		case "middle":
			cachedMaterial.SetColor("_Color", Color.green);
			break;
		case "back":
			cachedMaterial.SetColor("_Color", new Color(.12f, .56f, 1.0f, 1f));
			break;
	}

	/* Introduced error. Spawns a random cube size for each new trigger point. */
	if (m_Scene.name == "cube")
	{
		cubes_placed = GameObject.Find ("SpawnHotSpots").GetComponent<SpawnHotspots_cube> ().itr;
	}
	else if (m_Scene.name == "cube random plane")
	{
		cubes_placed = GameObject.Find ("SpawnHotSpots").GetComponent<SpawnHotspots_cube_random_plane> ().itr;
	}

	if (cubes_placed < 28)
	{
		n = Random.Range(0, 2); // # available cube sizes 

		switch (n)
		{
			case 0:
				local_cube = Instantiate (small, new Vector3 (-0.3f, 0.3f, 0.3f), Quaternion.identity, GameObject.Find ("SpawnHotSpots").transform); 
				break;
			case 1:
				local_cube = Instantiate (medium, new Vector3 (-0.3f, 0.3f, 0.3f), Quaternion.identity, GameObject.Find ("SpawnHotSpots").transform);
				break;
			case 2:
				local_cube = Instantiate (large, new Vector3 (-0.3f, 0.3f, 0.3f), Quaternion.identity, GameObject.Find ("SpawnHotSpots").transform);
				break;
		}

		local_cube.localPosition = new Vector3 (-0.3f, 0.3f, 0.3f); // spawn position relative to parent to correct slider affects
	}

    }

    private void OnTriggerEnter(Collider other)
    {
  
        if (OnEntered != null)
        {
            OnEntered(other.gameObject);

			/* Spawn new trigger point */
	    		if (m_Scene.name == "cube")
			{
				GameObject.Find ("SpawnHotSpots").GetComponent<SpawnHotspots_cube> ().HotSpotTriggerInstantiate ();
			}
			else if (m_Scene.name == "cube random plane")
			{
				GameObject.Find ("SpawnHotSpots").GetComponent<SpawnHotspots_cube_random_plane> ().HotSpotTriggerInstantiate ();
			}

			/* Remove this hotspot when triggered */
			Destroy (this.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (OnExited != null)
        {
            OnExited(other.gameObject);
        }

    }

}

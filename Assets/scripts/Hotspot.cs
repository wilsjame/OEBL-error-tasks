/* Attach to game objects intended to be trigger points (hotspots). 
 * This script works in conjuction with HotSpotColorChanger.cs. */

using UnityEngine;
using UnityEngine.SceneManagement;

public class Hotspot : MonoBehaviour
{
    public delegate void HotspotEntered(GameObject someCube);
    public static event HotspotEntered OnEntered;

    public delegate void HotspotExited(GameObject someCube);
    public static event HotspotExited OnExited;

    private Material cachedMaterial;

    Scene m_Scene;

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

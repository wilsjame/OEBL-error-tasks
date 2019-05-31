/* Attach to main scene that loads MRTK parent objects:
 * MixedRealityCameraParent and InputManager */
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    void Start()
    {
        // Immediatly load menu scene. 
	// MRTK parent objects are inherited.
	SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }
}

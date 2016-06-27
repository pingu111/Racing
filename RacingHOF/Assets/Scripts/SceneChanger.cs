using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Script that change the scenes 
public class SceneChanger : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
    }
	
    // Change the scene and launch the upgrade scene
	public void goToUpgradeScene()
    {
        SceneManager.LoadScene("UpgradeScene");
    }

    // Change the scene and launch the level selection
    public void goToSelectionScene()
    {
        SceneManager.LoadScene("SelectLevel");
    }
}

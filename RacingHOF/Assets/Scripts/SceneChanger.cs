using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that change the scenes 
/// Is'n destroy through the scene navigation
/// </summary>
public class SceneChanger : MonoBehaviour {

    /// <summary>
    /// The number of the Racing level that need to be loaded
    /// </summary>
    public int nbLevelToLoad;

    /// <summary>
    /// Inititialisation
    /// </summary>
    void Start ()
    {
        // We don't want the scene changer to be destroy if we change of scene
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Change the scene and launch the upgrade scene
    /// </summary>
    public void goToUpgradeScene()
    {
        SceneManager.LoadScene("UpgradeScene");
    }

    /// <summary>
    /// Change the scene and launch the level selection
    /// </summary>
    public void goToSelectionScene()
    {
        SceneManager.LoadScene("SelectLevel");
    }

    /// <summary>
    /// Change the scene and launch the main menu
    /// </summary>
    public void goToMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Change the scene and launch the main menu
    /// </summary>
    /// <param name="nbLevel"> The id of the level that we need to launch</param>
    public void goToLevel(int nbLevel)
    {
        nbLevelToLoad = nbLevel;
        // TODO Generate the correct levels with theirs IDs
        SceneManager.LoadScene("MainRacing");
    }
}

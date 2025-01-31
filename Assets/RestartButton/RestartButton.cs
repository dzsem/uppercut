using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartGame() {
        Debug.Log("RestartGame called");
        SceneManager.LoadScene(sceneToLoad.name);
    }

    public SceneAsset sceneToLoad;
}

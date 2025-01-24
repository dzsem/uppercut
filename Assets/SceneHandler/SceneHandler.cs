using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadSceneAsync("TestLevel2", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);

        SceneManager.SetActiveScene(scene);
        GameObject root = SceneManager.GetActiveScene().GetRootGameObjects()[0];
        Debug.Log(root);
        if (root != null) {
            root.transform.position = new Vector3(40f, 0f, 0f);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<string> GenerateMap() {
        List<string> map = new List<string>();

        for (int i = 0; i < numberOfMapParts; ++i) {
            // todo: get a random part and append to the list
        }

        return null; // todo
    }

    public PlayerMover player;
    public List<string> scenes;
    public int numberOfMapParts = 10;
    public string scenesPath = "Assets/Scenes";
    private List<string> _loadedScenes;
    private List<string> _unloadedScenes;
}

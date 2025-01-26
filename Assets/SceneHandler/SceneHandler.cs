using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;

public class SceneHandler : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Debug.Log("Scenes:");
        Debug.Log($"Start scene: {startScene}, end scene: {endScene}");
        foreach (string scene in mapScenes) {
            Debug.Log(scene);
        }

        _map = GenerateMap();
        for (int i = 0; i < _map.Count; ++i) {
            _unloadedScenes.Add(i);
        }
        LoadScenes();

        // SceneManager.LoadSceneAsync("TestLevel2", LoadSceneMode.Additive);
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Debug.Log("OnSceneLoaded: " + scene.name);
        // Debug.Log(mode);

        // SceneManager.SetActiveScene(scene);
        // GameObject root = SceneManager.GetActiveScene().GetRootGameObjects()[0];
        // Debug.Log(root);
        // if (root != null) {
        //     root.transform.position = new Vector3(40f, 0f, 0f);
        // }

        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update() {
        
    }

    private List<string> GenerateMap() {
        List<string> map = new List<string>();
        map.Add(startScene);
        for (int i = 0; i < numberOfMapParts; ++i) {
            int sceneIndex = UnityEngine.Random.Range(0, mapScenes.Count);
            Debug.Log(sceneIndex);
            map.Add(mapScenes.ElementAt(sceneIndex));
        }
        map.Add(endScene);

        return map;
    }

    private void LoadScenes() {
        _loadedScenes.Clear();
        foreach (int sceneIndex in _unloadedScenes) {
            float sceneStartX = (sceneIndex * sceneWidth) - (sceneWidth / 2);
            float sceneEndX = (sceneIndex * sceneWidth) + (sceneWidth / 2);
            Debug.Log($"{_map.ElementAt(sceneIndex)}: {sceneStartX}, {sceneEndX}");

            if (Math.Abs(player.transform.position.x - sceneStartX) < sceneWidth || Math.Abs(player.transform.position.x - sceneEndX) < sceneWidth) {
                _loadedScenes.Add(sceneIndex);
                Debug.Log($"{_map.ElementAt(sceneIndex)} is in load range");
            }
        }
    }

    private void UnloadScenes() {

    }

    public PlayerMover player;
    public List<string> mapScenes;
    public string startScene;
    public string endScene;
    public int numberOfMapParts = 10;
    public float sceneWidth = 30f;
    private List<string> _map;
    private List<int> _loadedScenes = new List<int>();
    private List<int> _unloadedScenes = new List<int>();
}

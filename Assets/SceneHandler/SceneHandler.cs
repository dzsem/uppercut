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

    // Update is called once per frame
    void Update() {
        LoadScenes();
    }

    /**
    *   Legenerálja a mapot a meghatározott scenekből és paraméterekből, amiket az asset menuben meg lehet adni.
    */
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

    /**
    *   Karbantartja a _loadedScenes-t és betölti a megfelelő sceneket
    */
    private void LoadScenes() {
        _loadedScenes.Clear();
        foreach (int sceneIndex in _unloadedScenes) {
            float sceneStartX = (sceneIndex * sceneWidth) - (sceneWidth / 2);
            float sceneEndX = (sceneIndex * sceneWidth) + (sceneWidth / 2);
            Debug.Log($"{_map.ElementAt(sceneIndex)}: {sceneStartX}, {sceneEndX}");

            bool isPlayerInLoadRange = Math.Abs(player.transform.position.x - sceneStartX) < sceneWidth || Math.Abs(player.transform.position.x - sceneEndX) < sceneWidth; // Ez a formula, ami alapján kiszámolom, hogy betöltési rangeben van-e vagy sem. Mindig három scenet tölt be: amiben a player áll, az előtte lévőt és az utána lévőt (ha vannak ilyenek)
            if (isPlayerInLoadRange) {
                Debug.Log($"{_map.ElementAt(sceneIndex)} is in load range");
                _loadedScenes.Add(sceneIndex);
                _unloadedScenes.Remove(sceneIndex);
                SceneManager.LoadSceneAsync(_map.ElementAt(sceneIndex), LoadSceneMode.Additive).completed += _ => {
                    OnSceneLoaded(SceneManager.GetSceneByName(_map.ElementAt(sceneIndex)), sceneIndex, LoadSceneMode.Additive); // idk ezt a delegációs faszságot a chatgőt főzte, mert nem tudtam a SceneManager.sceneLoaded eventtel megoldani normálisan
                };
            }
        }
    }

    /**
    *   Ez a callback, akkor hívódik meg, amikor egy scene betöltése befejeződik és csak a jó helyre pozíciónálja a scenet.
    */
    private void OnSceneLoaded(Scene scene, int sceneIndex, LoadSceneMode mode) {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        SceneManager.SetActiveScene(scene);
        GameObject root = SceneManager.GetActiveScene().GetRootGameObjects()[0];
        Debug.Log(root);

        if (root == null) {
            return; // ha nincs root, akkor szar a scene setup és nem csinál semmit
        }

        float sceneCenterX = sceneIndex * sceneWidth; // ez a formula ami alapján a scenek helye ki van számolva
        Debug.Log($"{sceneIndex}: {sceneCenterX}");
        root.transform.position = new Vector3(sceneCenterX, 0f, 0f); // elmozdítja a scenet a jó helyre
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

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;

public class LevelSystem : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        // Debug.Log("Scenes:");
        // Debug.Log($"Start scene: {startScenePrefab}, end scene: {endScenePrefab}");
        // foreach (GameObject prefab in scenePrefabs) {
        //     Debug.Log(prefab);
        // }

        _map = GenerateMap();
        for (int i = 0; i < _map.Count; ++i) {
            _unloadedPrefabs.Add(i); // csak az index alapján trackeljük a betöltött és be nem töltött prefabeket
        }
        LoadPrefabs();

        // SceneManager.LoadSceneAsync("TestLevel2", LoadSceneMode.Additive);
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update() {
        _checkTimer += Time.deltaTime;

        if (_checkTimer >= checkInterval) {
            _checkTimer = 0f;
            LoadPrefabs();
        }
    }

    /**
    *   Legenerálja a mapot a meghatározott scenekből és paraméterekből, amiket az asset menuben meg lehet adni.
    */
    private List<GameObject> GenerateMap() {
        List<GameObject> map = new List<GameObject>();
        map.Add(startScenePrefab);
        for (int i = 0; i < mapPartCount; ++i) {
            int sceneIndex = UnityEngine.Random.Range(0, scenePrefabs.Count);
            Debug.Log(sceneIndex);
            map.Add(scenePrefabs.ElementAt(sceneIndex));
        }
        map.Add(endScenePrefab);

        return map;
    }

    /**
    *   Karbantartja a _loadedScenes-t és betölti a megfelelő sceneket
    */
    private void LoadPrefabs() {
        if (_unloadedPrefabs.Count == 0) {
            return;
        }

        foreach (int prefabIndex in _unloadedPrefabs) {
            float sceneStartX = (prefabIndex * prefabWidth) - (prefabWidth / 2);
            float sceneEndX = (prefabIndex * prefabWidth) + (prefabWidth / 2);
            Debug.Log($"{_map.ElementAt(prefabIndex)}: {sceneStartX}, {sceneEndX}");

            bool isPlayerInLoadRange = Math.Abs(player.transform.position.x - sceneStartX) < prefabWidth || Math.Abs(player.transform.position.x - sceneEndX) < prefabWidth; // Ez a formula, ami alapján kiszámolom, hogy betöltési rangeben van-e vagy sem. Mindig három scenet tölt be: amiben a player áll, az előtte lévőt és az utána lévőt (ha vannak ilyenek)
            if (isPlayerInLoadRange) {
                Debug.Log($"{_map.ElementAt(prefabIndex)} is in load range");
                _loadedPrefabs.Add(prefabIndex);
                float prefabCenterX = prefabIndex * prefabWidth; // ez a formula ami alapján a scenek helye ki van számolva
                Vector3 prefabPosition = new Vector3(prefabCenterX, 0f, 0f);
                Instantiate(_map.ElementAt(prefabIndex), prefabPosition, Quaternion.identity);
            }
        }

        _unloadedPrefabs.RemoveAll(prefab => _loadedPrefabs.Contains(prefab)); // kiszedjuk az unloaded listából, hogy többször ne legyen betöltve semmi sem
    }

    private void UnloadPrefabs() {

    }

    public PlayerMover player;
    public List<GameObject> scenePrefabs;
    public GameObject startScenePrefab;
    public GameObject endScenePrefab;
    public int mapPartCount = 10;
    public float prefabWidth = 30f;
    public float checkInterval = 4f;
    private float _checkTimer = 0f;
    private List<GameObject> _map;
    private List<int> _loadedPrefabs = new List<int>();
    private List<int> _unloadedPrefabs = new List<int>();
}

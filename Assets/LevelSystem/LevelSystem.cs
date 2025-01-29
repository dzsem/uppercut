using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelSystem : MonoBehaviour {
    void Start() {
        _map = GenerateMap();
        for (int i = 0; i < _map.Count; ++i) {
            _unloadedPrefabs.Add(i); // csak az index alapján trackeljük a betöltött és be nem töltött prefabeket
        }
        LoadPrefabs();
    }

    void Update() {
        _checkTimer += Time.deltaTime;

        if (_checkTimer >= checkInterval) {
            _checkTimer = 0f;
            UnloadPrefabs();
            LoadPrefabs();
        }
    }

    /**
    *   Legenerálja a mapot a meghatározott prefabekből és paraméterekből, amiket az asset menuben meg lehet adni.
    */
    private List<GameObject> GenerateMap() {
        List<GameObject> map = new List<GameObject>();
        GameObject startPrefabInstance = Instantiate(startScenePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        startPrefabInstance.SetActive(false);
        map.Add(startPrefabInstance);
        int i = 0;
        for (; i < mapPartCount; i++) {
            int prefabIndex = UnityEngine.Random.Range(0, scenePrefabs.Count);
            Debug.Log(prefabIndex);
            float prefabCenterX = (i + 1) * prefabWidth; // ez a formula ami alapján a scenek helye ki van számolva
            Vector3 prefabPosition = new Vector3(prefabCenterX, 0f, 0f);
            GameObject prefabInstance = Instantiate(scenePrefabs.ElementAt(prefabIndex), prefabPosition, Quaternion.identity);
            prefabInstance.SetActive(false);
            map.Add(prefabInstance);
        }
        GameObject endPrefabInstance = Instantiate(endScenePrefab, new Vector3((i + 1) * prefabWidth, 0f, 0f), Quaternion.identity);
        endPrefabInstance.SetActive(false);
        map.Add(endPrefabInstance);

        return map;
    }

    /**
    *   Karbantartja a _loadedScenes-t és betölti a megfelelő prefabeket
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
                _map.ElementAt(prefabIndex).SetActive(true);
            }
        }

        _unloadedPrefabs.RemoveAll(prefab => _loadedPrefabs.Contains(prefab)); // kiszedjuk az unloaded listából, hogy többször ne legyen betöltve semmi sem
    }

    /**
    *   Karbantartja az _unloadedScenes-t és kitölti a megfelelő prefabeket
    */
    private void UnloadPrefabs() {
        if (_loadedPrefabs.Count == 0) {
            return;
        }

        foreach (int prefabIndex in _loadedPrefabs) {
            float sceneStartX = (prefabIndex * prefabWidth) - (prefabWidth / 2);
            float sceneEndX = (prefabIndex * prefabWidth) + (prefabWidth / 2);
            Debug.Log($"{_map.ElementAt(prefabIndex)}: {sceneStartX}, {sceneEndX}");

            bool isPlayerInLoadRange = Math.Abs(player.transform.position.x - sceneStartX) < prefabWidth || Math.Abs(player.transform.position.x - sceneEndX) < prefabWidth; // Ez a formula, ami alapján kiszámolom, hogy betöltési rangeben van-e vagy sem. Mindig három scenet tölt be: amiben a player áll, az előtte lévőt és az utána lévőt (ha vannak ilyenek)
            if (!isPlayerInLoadRange) {
                Debug.Log($"{_map.ElementAt(prefabIndex)} is not in load range");
                _unloadedPrefabs.Add(prefabIndex);
                _map.ElementAt(prefabIndex).SetActive(false);
            }
        }

        _loadedPrefabs.RemoveAll(prefab => _unloadedPrefabs.Contains(prefab)); // kiszedjuk az unloaded listából, hogy többször ne legyen betöltve semmi sem
    }

    public PlayerMover player; // player referencia a távolság számításhoz
    public List<GameObject> scenePrefabs; // milyen prefabekből legyen összepakolva a pálya
    public GameObject startScenePrefab; // start room
    public GameObject endScenePrefab; // boss room
    public int mapPartCount = 10; // hány részből áll a map
    public float prefabWidth = 30f; // prefabek előre meghatározott szélesek legyenek, hogy könnyen számítható legyen az elhelyezkedésük és a player távolsága
    public float checkInterval = 1f; // intervallum a betöltések ellenőrzésére, hogy ne minden egyes updatenél legyen sok aritmetika
    private float _checkTimer = 0f;
    private List<GameObject> _map;
    private List<int> _loadedPrefabs = new List<int>();
    private List<int> _unloadedPrefabs = new List<int>();
}

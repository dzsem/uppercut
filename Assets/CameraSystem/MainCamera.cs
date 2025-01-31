using System;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        _fallTimer = 0f;
        _cameraOffsetX = offsetDistanceX;
        _cameraOffsetY = offsetDistanceY;
        _targetPoint = new Vector3(player.transform.position.x + _cameraOffsetX, player.transform.position.y + _cameraOffsetY, -10f);
        transform.position = _targetPoint;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {   
        // Ezekkel kicsit eltolom abba az irányba a kamerát, amerre a karakter néz, hogy több látszódjon a pályából abba az irányba.
        if (player.transform.rotation.y == 0) { // facing right
            // _cameraOffsetX = Mathf.Lerp(_cameraOffsetX, offsetDistance, offsetSmoothing * Time.deltaTime);
            _cameraOffsetX = offsetDistanceX;
        } else { // facing left
            // _cameraOffsetX = Mathf.Lerp(_cameraOffsetX, -offsetDistance, offsetSmoothing * Time.deltaTime);
            _cameraOffsetX = -offsetDistanceX;
        }

        // Esésnél kicsit lentebb néz a kamera, hogy a játékos jobban lássa, hogy hova fog érkezni
        // Ez még nem működik olyan jól szerintem
        if (player.rb.linearVelocityY < 0) { // falling
            _fallTimer += Time.deltaTime;
            if (_fallTimer >= fallDelay) {
                // _cameraOffsetY = Mathf.Lerp(_cameraOffsetY, -2 * cameraOffsetDistance, cameraOffsetSpeed * Time.deltaTime);
                _cameraOffsetY = -1.3f * offsetDistanceY;
            }
        } else {
            _fallTimer = 0f;
            _cameraOffsetY = offsetDistanceY;
        }

        // A kamera ugrásnál csak akkor vált pozíciót, ha a játékos már leérkezett, így könnyebb ugrani szerintem
        // if (player.touchesGround || _fallTimer >= fallDelay) { // platform lock
        //     _targetPoint.y = player.transform.position.y + _cameraOffsetY;
        // }

        Vector3 shakeOffset = Vector3.zero;
        if (_shakeTimer > 0)
        {
            shakeOffset = new Vector3(
                UnityEngine.Random.Range(-_shakeIntensity, _shakeIntensity), 
                UnityEngine.Random.Range(-_shakeIntensity, _shakeIntensity), 
                0f
            );
            _shakeTimer -= Time.deltaTime;
        }

        _targetPoint.y = player.transform.position.y + _cameraOffsetY;
        _targetPoint.x = player.transform.position.x + _cameraOffsetX;

        // Smoothing
        transform.position = Vector3.Lerp(transform.position, _targetPoint + shakeOffset, offsetSmoothing * Time.deltaTime);
    }

    public void ShakeCamera(float duration, float intensity)
    {
        _shakeTimer = duration;
        _shakeIntensity = intensity;
    }

    public PlayerMover player;
    public float offsetDistanceX = 3.5f;
    public float offsetDistanceY = 3f;
    public float offsetSmoothing = 5f;
    public float fallDelay = 0.5f;
    private float _fallTimer;
    private Vector3 _targetPoint;
    private float _cameraOffsetX;
    private float _cameraOffsetY;
    private float _shakeTimer = 0f;
    private float _shakeIntensity = 0f;
}

using System;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraOffsetY = 3;
        _targetPoint = new Vector3(player.transform.position.x + _cameraOffsetX, player.transform.position.y + _cameraOffsetY, -10f);
        transform.position = _targetPoint;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Ezekkel kicsit eltolom abba az irányba a kamerát, amerre a karakter néz, hogy több látszódjon a pályából abba az irányba.
        if (player.rb.linearVelocityX > 0)
        { // facing right
            _cameraOffsetX = Mathf.Lerp(_cameraOffsetX, cameraOffsetDistance, cameraOffsetSpeed * Time.deltaTime);
        }
        else if (player.rb.linearVelocityX < 0)
        { // facing left
            _cameraOffsetX = Mathf.Lerp(_cameraOffsetX, -cameraOffsetDistance, cameraOffsetSpeed * Time.deltaTime);
        }

        // Esésnél kicsit lentebb néz a kamera, hogy a játékos jobban lássa, hogy hova fog érkezni
        // Ez még nem működik olyan jól szerintem
        if (player.rb.linearVelocityY < 0)
        { // falling
            _cameraOffsetY = Mathf.Lerp(_cameraOffsetY, -2 * cameraOffsetDistance, cameraOffsetSpeed * Time.deltaTime);
        }
        else
        {
            _cameraOffsetY = 0;
        }

        // A kamera ugrásnál csak akkor vált pozíciót, ha a játékos már leérkezett, így könnyebb ugrani szerintem
        if (player.TouchesGround || player.rb.linearVelocityY < 0)
        { // platform lock
            _targetPoint.y = player.transform.position.y + _cameraOffsetY;
        }

        _targetPoint.x = player.transform.position.x + _cameraOffsetX;

        // Smoothing
        transform.position = Vector3.Lerp(transform.position, _targetPoint, cameraSpeed * Time.deltaTime);
        // Vector3 velocity = Vector3.zero;
        // transform.position = Vector3.SmoothDamp(transform.position, _targetPoint, ref velocity, cameraSpeed);
    }

    public PlayerMover player;
    public float cameraSpeed;
    public float cameraOffsetDistance;
    public float cameraOffsetSpeed;
    private Vector3 _targetPoint;
    private float _cameraOffsetX;
    private float _cameraOffsetY;
}

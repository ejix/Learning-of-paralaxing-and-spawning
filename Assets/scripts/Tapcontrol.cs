using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tapcontrol : MonoBehaviour{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapFroce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;
    GameManager game;

    Rigidbody2D rb2d;
    Quaternion downRotation;
    Quaternion forwardRotation;

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }
    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    void OnGameStarted()
    {
        rb2d.velocity = Vector3.zero;
        rb2d.simulated = true;
        transform.localPosition = startPos;

        game.gameOver = false;
    }
    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
    }

    private void Update()
    {
        if (game.GameOver) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            transform.rotation = forwardRotation;
            rb2d.velocity = Vector3.zero;
            rb2d.AddForce(Vector2.up * tapFroce, ForceMode2D.Force);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SCOREZONE"))
        {
            OnPlayerScored();
        }
        if (collision.gameObject.CompareTag("DEADZONE"))
        {
            rb2d.simulated = false;
            OnPlayerDied();
        }
    }


}
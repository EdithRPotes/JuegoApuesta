using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class BombProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float multiplierIncreaseRate = 0.2f;
    [SerializeField] private float minTimeToExplode = 1f;
    [SerializeField] private float maxTimeToExplode = 30f;
    


    private float currentMultiplier = 1f;
    private float explosionTimer;
    private Rigidbody2D bombprojectileRb;
    private GameManager gameManager;
    private bool hasExploded = false;
    private float launchTime; // Almacena el tiempo de lanzamiento de la bomba

    public GameObject explosionPrefab;


    private void Awake()
    {
        launchTime = Time.time; // Captura el tiempo de lanzamiento de la Bomba al inicio
        bombprojectileRb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        explosionTimer = Random.Range(minTimeToExplode, maxTimeToExplode); // Establece el  temporizador con un valor aleatorio 
    }

    private void Start()
    {



        StartCoroutine(IncreaseMultiplierOverTime());

    }

    private void Update()
    {
        if (!hasExploded)
        {

            // Disminuye el temporizador y estalla la bomba cuando llegue a 0
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0)
            {
                ExplodeBomb();
            }
        }

    }


    private IEnumerator IncreaseMultiplierOverTime()
    {
        while (true)
        {
            currentMultiplier += multiplierIncreaseRate * Time.deltaTime;

            yield return null;
        }
    }

    public void LaunchBombProjectile(Vector2 direction)
    {
        launchTime = Time.time; // Captura el tiempo de lanzamiento al lanzar la bomba
        currentMultiplier = 1f;
        float launchBetAmount = gameManager.GetCurrentBetAmount();
        StartCoroutine(gameManager.CalculateProgressiveWinnings(launchBetAmount));
        bombprojectileRb.velocity = direction * speed * currentMultiplier;
        
        // Activa la c�mara de Cinemachine para que siga al proyectil
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.gameObject.SetActive(true);
            virtualCamera.Follow = transform; // Sigue al proyectil
        }

       
    }

    public float GetCurrentMultiplier()
    {
        return currentMultiplier;
    }

    private void OnCollisionEnter2D()
    {
        ExplodeBomb();
    }

    private void ExplodeBomb()
    {
        // Calcula el tiempo transcurrido desde el lanzamiento hasta la explosi�n
        float timeElapsed = Time.time - launchTime;

        gameManager.bombExploded = true;

        // Registra el tiempo de la explosi�n
        //Debug.Log("La bomba explot� en el segundo: " + timeElapsed);

       

        if (!gameManager.HasWithdrawnBet())
        {
            float currentMultiplier = GetCurrentMultiplier();
            float betAmount = gameManager.GetCurrentBetAmount();
            float winnings = gameManager.CalculateWinnings(currentMultiplier, betAmount);

            // Mostrar los resultados despu�s de la explosi�n con las ganancias obtenidas
            gameManager.ShowResults(winnings);
        }
        else
        {
            Debug.Log("Has retirado tu apuesta antes de la explosi�n.");

            gameManager.ShowResults(0);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Destruye el objeto de explosi�n despu�s de 2 segundos
            Destroy(gameObject);
           
        }
        else
        {
            Destroy(gameObject);
        }

       

    }
}








using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private Cannon cannon;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button playAgainButton;

    public BombProjectile bombProjectilePrefab;
    public TextMeshProUGUI multiplierTextBomb;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI launchInstructions; // Referencia al objeto de texto que muestra las instrucciones de lanzamiento


    private float currentBetAmount; // Almacena el valor de la apuesta actual
    private bool hasWithdrawnBet = false;
    private float launchTime; // Almacena el tiempo de lanzamiento de la bomba
    private float withdrawnWinnings = 0; // Almacena las ganancias retiradas antes de la explosión
    


    public bool bombExploded { get; set; } = false;
    

    void Start()
    {
        betInputField = GameObject.Find("Canvas/UserInterface/BetInputField").GetComponent<TMP_InputField>();
        // Desahabilita panel de resultados y botón de playAgain 
        resultPanel.SetActive(false);
        playAgainButton.interactable = false;
        cannon = FindObjectOfType<Cannon>();
    }

    public void PlaceBet()
    {
        if (float.TryParse(betInputField.text, out float betAmount))
        {
            launchInstructions.text = "¡Apunta el cañon y da clic en la pantalla!";
            cannon.enabled = true;
            currentBetAmount = betAmount; // Almacena el valor de la apuesta actual
            Debug.Log(currentBetAmount);
            hasWithdrawnBet = false; // Reinicia el estado de retiro de apuesta 
            launchTime = Time.time; // Captura el tiempo de lanzamiento de la Bomba 
            // Invoca la función para borrar el mensaje después de 2 segundos
            Invoke("ClearInstructions", 2f);
        }
        else
        {
            
            Debug.LogWarning("El valor de la apuesta no es válido.");
        }
    }

   
    private void ClearInstructions()
    {
        launchInstructions.text = "";
    }

    public IEnumerator CalculateProgressiveWinnings(float betAmount)
    {
        float initialMultiplier = bombProjectilePrefab.GetCurrentMultiplier();
        float currentMultiplier = initialMultiplier;
        
        while (!bombExploded) //Si la bomba estalla y el jugador no ha retirado su apuesta, perderá el total de lo apostado.
        {
            float timeElapsed = Time.time - launchTime; // Utiliza el mismo tiempo de lanzamiento
            float adjustedMultiplier = initialMultiplier + (0.2f * timeElapsed); // Calcula el multiplicador ajustado
            
            // Actualiza los textos que muestran el multiplicador y la ganancia
            multiplierText.text = "Multiplier X" + adjustedMultiplier.ToString("F2");

            //Debug.Log("Multiplicador: " + currentMultiplier + ", Ganancia: " + winnings);


            yield return null;
        }
    }


    public float GetCurrentBetAmount()
    {
        return currentBetAmount;
    }

    public void WithdrawBet()
    {
        if (!hasWithdrawnBet)
        {
            hasWithdrawnBet = true;
            float currentMultiplier = bombProjectilePrefab.GetCurrentMultiplier();
            float betAmount = GetCurrentBetAmount();
            withdrawnWinnings = bombExploded ? 0 : CalculateWinnings(currentMultiplier, betAmount);
            //Debug.LogFormat("Apuesta retirada antes de la explosión. Ganancia: {0:F1}", withdrawnWinnings);

        }
        else
        {
            Debug.LogWarning("Ya has retirado tu apuesta.");
        }
    }

    public float CalculateWinnings(float currentMultiplier, float betAmount)
    {   
     
        float timeElapsed = Time.time - launchTime; // Utiliza el mismo tiempo de lanzamiento
        float adjustedMultiplier = currentMultiplier + (0.2f * timeElapsed);

        if (hasWithdrawnBet) // Muestra el multiplicador solo si has retirado tu apuesta
        {
            multiplierTextBomb.text = "Multiplier Bomb X" + adjustedMultiplier.ToString("F2");
        }
        else
        {
            multiplierTextBomb.text = ""; // Vacia el texto si no has retirado tu apuesta
        }

        //Debug.Log("Multiplier" + adjustedMultiplier);
        return betAmount * adjustedMultiplier;
    }

    public bool HasWithdrawnBet()
    {
        return hasWithdrawnBet;
    }

    public void ShowResults(float winnings)
    {
        if (hasWithdrawnBet)
        {

            resultText.text = "Tus Ganancias Retiradas " + withdrawnWinnings.ToString("F1");
        }
        else
        {
            resultText.text = bombExploded ? "Has Perdido Tu Apuesta " : "Tus Ganancias: " + winnings.ToString("F1") ;
        }
        resultPanel.SetActive(true); // Activa el panel de resultados
        playAgainButton.interactable = true; // Hace interactuable el botón 
    }


    public void PlayAgain()
    {
        // Reinicia el juego y oculta el panel de resultados
        resultPanel.SetActive(false);
        playAgainButton.interactable = false;
        currentBetAmount = 0;  // Reinicia el valor de la apuesta
        bombExploded = false;  // Reinicia el estado de la bomba
        hasWithdrawnBet = false; // Reinicia el estado de retiro de apuesta
        withdrawnWinnings = 0; // Reinicia las ganancias retiradas antes de la explosión

        // Carga nuevamente la escena actual (Cannonball)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }



}




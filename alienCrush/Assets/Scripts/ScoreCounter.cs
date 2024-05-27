
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { get; private set; }

    public int score;

    public int Score
    {
        get => score;

        set
        {
            if (score == value) return;
            score = value;
            //estefi
            if (scoreText != null)//estefi
            {
                scoreTextTotal.SetText($"{score}");
                scoreText.SetText($"{score}");
                //estefi
            }
            else
            {
                Debug.LogWarning("ScoreText is not assigned.");
            } //estefi

            //if (score >= 100)
            //{
            //    Debug.Log("Cambio de escena");
            //    SceneManager.LoadScene(1);
            //}

            //estefi
            if (score >= 1000)
            {
                Debug.Log("Puntaje alcanzado: 1000");
               
                Board.Instance.EndGame();
                //GameOver.Inst+ance.AnimateGameOver(); // Llama a la animación de Game Over
            }
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextTotal;
    private void Awake() => Instance = this;




}
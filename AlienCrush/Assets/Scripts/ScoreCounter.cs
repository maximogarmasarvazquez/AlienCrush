
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
          
            if (scoreText != null)
            {
                scoreTextTotal.SetText($"{score}");
                scoreText.SetText($"{score}");
               
            }
            else
            {
                Debug.LogWarning("ScoreText is not assigned.");
            } 

   

           
            if (score >= 300)
            {
                Debug.Log("Puntaje alcanzado: 1000");
               
                Board.Instance.EndGame();
                
            }
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextTotal;
    private void Awake() => Instance = this;




}
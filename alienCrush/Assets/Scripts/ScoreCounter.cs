
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

            scoreText.SetText($"{score}");

            //if (score >= 100)
            //{
            //    Debug.Log("Cambio de escena");
            //    SceneManager.LoadScene(1);
            //}
            
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake() => Instance = this;



  
}
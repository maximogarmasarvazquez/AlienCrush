using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public UnityEngine.UI.Image[] stars;

    //public Animator animator;

    
    [SerializeField] private Animator animator;
    private bool gameHasEnded = false; // Variable para rastrear si el juego ha terminado

    
        void Start()
    {
        // Asegúrate de que el panel de GameOver esté desactivado al inicio
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnReplayClicked()
    {
        SceneManager.LoadScene(2);

    }

    public void OnDoneClicked()
    {
        SceneManager.LoadScene(1);
    }

    internal void AnimateGameOver()
    {
        if (gameHasEnded) return; // Si el juego ya ha terminado, no hacer nada
        gameHasEnded = true; // Marcar que el juego ha terminado

        Debug.Log("Setting GameOver panel active");
        
        gameObject.SetActive(true); // Activa el panel de GameOver

        Debug.Log("Triggering GameOver animation");

        // Adjust the animation speed
        animator.speed = 0.5f; // Example: double the speed

        animator.SetTrigger("GameOverTrigger");

        Debug.Log("Animating GameOver");
    }

      /*private IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(true); // Asegúrate de que el panel esté activo
        animator.SetTrigger("GameOverTrigger");
    }*/

}

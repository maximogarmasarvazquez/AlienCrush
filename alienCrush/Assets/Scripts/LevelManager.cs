using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public bool nivel1 ;
    public bool nivel2;
    public bool nivel3;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


    }


    public void BotonStart()
    {
        SceneManager.LoadScene(1);
    }
    public void BotonExit()
    {
        Debug.Log("salir del juego");
        Application.Quit();
    }

    public void Nivel1()
    {

        nivel1 = true;

    }

    public void Nivel2()
    {

        nivel2 = true;

    }

    public void Nivel3()
    {

        nivel3 = true;

    }


}
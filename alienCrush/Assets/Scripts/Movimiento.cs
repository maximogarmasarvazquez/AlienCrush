using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{

    [SerializeField] private float velocidadDeMovimiento;
    [SerializeField] private Transform[] puntosDeMovimiento;
    [SerializeField] private float distanciaMinima;
    private SpriteRenderer spriteRenderer;
    public LevelManager levelManager;
    private int siguientePaso = 0;
  
     void Start()
    {
     
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Girar()
    {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1f;
            transform.localScale = newScale;
     }

        // Update is called once per frame
        void Update()
    {
        if (levelManager.nivel1 || levelManager.nivel2 || levelManager.nivel3)
        {
            // Verifica que siguientePaso esté dentro de los límites del arreglo
            if (siguientePaso < puntosDeMovimiento.Length)
            {
                transform.position = Vector2.MoveTowards(transform.position, puntosDeMovimiento[siguientePaso].position, velocidadDeMovimiento * Time.deltaTime);

                if (Vector2.Distance(transform.position, puntosDeMovimiento[siguientePaso].position) < distanciaMinima)
                {
                    siguientePaso += 1;

                   

                    if (levelManager.nivel1)
                    {
                        if (siguientePaso >= 3)
                        {
                            Debug.Log("nivel1" + levelManager.nivel1);
                            SceneManager.LoadScene(2);
                        }
                    }
                    else if (levelManager.nivel2)
                    {
                        if (siguientePaso == 3 || siguientePaso == 5)
                        {
                            Girar();
                        }

                        if (siguientePaso >= 6)
                        {
                            Debug.Log("nivel2" + levelManager.nivel2);
                            SceneManager.LoadScene(3);
                        }
                    }
                    else if (levelManager.nivel3)
                    {
                        if (siguientePaso == 3 || siguientePaso == 5)
                        {
                            Girar();
                        }

                        if (siguientePaso >= puntosDeMovimiento.Length)
                        {
                            Debug.Log("nivel3" + levelManager.nivel3);
                            SceneManager.LoadScene(4);
                        }
                    }
                }
            }
        }
    }


}


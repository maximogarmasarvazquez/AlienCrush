using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Movimiento : MonoBehaviour
{
    [SerializeField] private float velocidadDeMovimiento;
    [SerializeField] private Transform[] puntosDeMovimiento;
    [SerializeField] private float distanciaMinima;

    private SpriteRenderer SpriteRenderer;
    public LevelManager levelManager;
    private int siguientePaso = 0;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
 
    private void Rotar()
    {
        // Calcula la dirección hacia el siguiente punto
        Vector2 direccion = puntosDeMovimiento[siguientePaso].position - transform.position;
        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
      
        // Ajusta el ángulo si la escala X es negativa
        if (transform.localScale.x < 0)
        {
            angle += 180f;
        }

        // Verifica si el ángulo de rotación supera cierto umbral
        if (transform.rotation.z > 0.7)
        {
            Vector3 newScale = transform.localScale;
            newScale.y =  -0.34f;
            transform.localScale = newScale;
    
           
        }else
        {
            Vector3 newScale = transform.localScale;
            newScale.y = 0.34f;
            transform.localScale = newScale;
        }
    
        // Aplica la rotación al objeto
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // Update is called once per frame
    void Update()
    {
        
        if (levelManager.nivel1 || levelManager.nivel2 || levelManager.nivel3)
        {
            // Verifica que siguientePaso esté dentro de los límites del arreglo
            if (siguientePaso < puntosDeMovimiento.Length)
            {

                Rotar();
                // Mueve el cohete hacia el siguiente punto
                transform.position = Vector2.MoveTowards(transform.position, puntosDeMovimiento[siguientePaso].position, velocidadDeMovimiento * Time.deltaTime);

                // Verifica si ha llegado al punto de movimiento
                if (Vector2.Distance(transform.position, puntosDeMovimiento[siguientePaso].position) < distanciaMinima)
                {
                    siguientePaso += 1;

                    if (levelManager.nivel1)
                    {
                      
                        if (siguientePaso == 3)
                        {
                           
                            Debug.Log("nivel1" + levelManager.nivel1);
                            SceneManager.LoadScene(3);
                        }
                    }
                    else if (levelManager.nivel2)
                    {
                       
                        if (siguientePaso == 5)
                        {
                            Debug.Log("nivel2" + levelManager.nivel2);
                            SceneManager.LoadScene(3);
                        }
                    }
                    else if (levelManager.nivel3)
                    {


                        if (siguientePaso == puntosDeMovimiento.Length)
                        {
                            Debug.Log("nivel3" + levelManager.nivel3);
                            SceneManager.LoadScene(3);
                        }


                    }
                }
            }
        }
    }
}


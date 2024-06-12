using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapSceneController : MonoBehaviour
{
    public Text puntosText;
    public Text estrellasText;

    void Start()
    {
        // Mostrar la información del jugador en los carteles
        puntosText.text =  PlayerData.PuntosGanados.ToString();
        estrellasText.text =  PlayerData.EstrellasGanadas.ToString();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Burst.Intrinsics.X86.Avx;
using Random = UnityEngine.Random;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;//  [SerializeField] permite hacer visible una variable privada desde el inspector de unity

    [SerializeField] private AudioSource audioSource;

    //estefi
    [SerializeField] private GameOver gameOver;


    public Row[] rows; // desde unity se traen los rows en el que se encuentran los tiles
    public Tile[,] Tiles { get; private set; } //[,] la coma significa que la matriz va a ser bidimennsional en lugar de  unidimensional, accedes utilizando dos �ndices, uno para la fila y otro para la columna

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();//readonly: Una vez que se haya inicializado este campo, no se puede cambiar su valor,
                                                              //significa que una vez que se haya creado una instancia de List<Tile> y se haya asignado a este campo,
                                                              //no se puede asignar otra instancia de List<Tile> a este campo. Sin embargo, los elementos dentro de la lista pueden ser modificados.
                                                              //List<Tile>: Es el tipo de datos del campo.Es una
                                                              //lista generica que contiene elementos del tipo Tile.Una lista es una coleccion de elementos que puede crecer o decrecer dinamicamente.
    private const float TweenDuration = 0.25f;

    //estefi
    private bool isGameOver = false; // Variable to track game state

    private void Awake() => Instance = this;//void indica que el metodo no devuelve un valor
                                            //El metodo Awake() en Unity se utiliza para inicializar objetos antes de que comience el juego.
                                            //Es parte del ciclo de vida de los objetos en Unity y se llama una vez cuando el objeto se activa o se carga en la escena,justo antes de que comience el m�todo Start().
    private void Start()                //En resumen, la funci�n Start() inicializa el tablero del juego asignando aleatoriamente elementos a cada tile y configurando sus coordenadas x e y.
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)

            {

                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;


                tile.Item = ItemDataBase.Items[Random.Range(0, ItemDataBase.Items.Length)];

                Tiles[x, y] = tile;


            }
        }

    }

    public async void Select(Tile tile)//En resumen, esta funci�n gestiona el proceso de selecci�n de tiles en el tablero del juego, realizando intercambios,
                                       //verificaciones y limpieza de la lista de selecci�n seg�n sea necesario.
                                       //Adem�s, utiliza async y await para esperar la finalizaci�n de las operaciones as�ncronas, como la animaci�n de intercambio de tiles.
    {

        //estefi
        if (isGameOver) return; 


        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1) _selection.Add(tile);
            }
            else
            {
                _selection.Add(tile);
            }
        }

        if (_selection.Count < 2) return;

        Debug.Log($"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }

        _selection.Clear();
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;

        Debug.Log($"Swapped tiles at ({tile1.x}, {tile1.y}) and ({tile2.x}, {tile2.y})");
    }

    private bool CanPop() // esta funcion es parte de un sistema de deteccion de combinaciones
                          // en un juego de fichas y devuelve true si hay al menos una combinacion que se puede "eliminar", de acuerdo con las reglas del juego.
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2)
                {
                    Debug.Log($"Can pop at ({x}, {y})");
                    return true;
                }
            }
        }
        return false;
    }


    public List<Coordenadas> listaMovimientosPosibles = new List<Coordenadas>();

    private List<Coordenadas> EncontrarMovimientosPosibles()
    {
        listaMovimientosPosibles.Clear();
        int filas = Tiles.GetLength(0);
        int columnas = Tiles.GetLength(1);

        for (int x = 0; x < filas; x++)
        {
            for (int y = 0; y < columnas; y++)
            {
                if (y + 1 < columnas && PuedeCombinar(x, y, x, y + 1))
                {
                    listaMovimientosPosibles.Add(new Coordenadas { x1 = x, y1 = y, x2 = x, y2 = y + 1 });
                }

                if (x + 1 < filas && PuedeCombinar(x, y, x + 1, y))
                {
                    listaMovimientosPosibles.Add(new Coordenadas { x1 = x, y1 = y, x2 = x + 1, y2 = y });
                }
            }
        }

        Debug.Log($"Found {listaMovimientosPosibles.Count} possible moves");
        return listaMovimientosPosibles;
    }

    private bool PuedeCombinar(int x1, int y1, int x2, int y2)//La función PuedeCombinar verifica si al intercambiar dos elementos del tablero se
                                                              //forma una combinación válida hace un intercambio temporal, verificacicando combinaciones
                                                              //en ambas posiciones y luego restaurando los elementos.
    {
        var temp = Tiles[x1, y1].Item;
        Tiles[x1, y1].Item = Tiles[x2, y2].Item;
        Tiles[x2, y2].Item = temp;

        bool hayCombinacion = HayCombinacionEnPosicion(x1, y1) || HayCombinacionEnPosicion(x2, y2);

        Tiles[x2, y2].Item = Tiles[x1, y1].Item;
        Tiles[x1, y1].Item = temp;

        Debug.Log($"Move ({x1}, {y1}) <-> ({x2}, {y2}) results in combination: {hayCombinacion}");
        return hayCombinacion;
    }

    private bool HayCombinacionEnPosicion(int x, int y)//comprueba si al menos tres elementos iguales están alineados horizontal o verticalmente en la posición
                                                       //(x, y) del tablero. Si se encuentra una combinación válida se retorna true. Si no, se retorna false.
    {
        int id = Tiles[x, y].Item.id;

        int contador = 1;
        for (int i = x - 1; i >= 0 && Tiles[i, y].Item.id == id; i--) contador++;
        for (int i = x + 1; i < Tiles.GetLength(0) && Tiles[i, y].Item.id == id; i++) contador++;
        if (contador >= 3)
        {
            Debug.Log($"Combination found at ({x}, {y}) horizontally");
            return true;
        }

        contador = 1;
        for (int i = y - 1; i >= 0 && Tiles[x, i].Item.id == id; i--) contador++;
        for (int i = y + 1; i < Tiles.GetLength(1) && Tiles[x, i].Item.id == id; i++) contador++;
        if (contador >= 3)
        {
            Debug.Log($"Combination found at ({x}, {y}) vertically");
            return true;
        }

        return false;
    }

    private bool CanCombinacion()
    {
        foreach (var coordenada in EncontrarMovimientosPosibles())
        {
            if (PuedeCombinar(coordenada.x1, coordenada.y1, coordenada.x2, coordenada.y2))
            {
                Debug.Log("Hay combinaciones posibles");
                return true;
            }
        }

        Debug.Log("No hay combinaciones posibles");
        return false;
    }

    private async void Pop() //esta funcion maneja la eliminacion de grupos de fichas conectadas en un juego de puzzle, animando la eliminacion y la aparicion de nuevas fichas.
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

               

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                }

                audioSource.PlayOneShot(collectSound);

                ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count;

                await deflateSequence.Play().AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                do
                {

                    foreach (var connectedTile in connectedTiles)
                    {
                        connectedTile.Item = ItemDataBase.Items[Random.Range(0, ItemDataBase.Items.Length)];
                        //bomba
                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                    }


                } while (!CanCombinacion());

                await inflateSequence.Play().AsyncWaitForCompletion();

            }
        }

        //estefi
        if (ScoreCounter.Instance.Score >= 300)
        {
            EndGame();
            
        }
    }


    //estefi
    public void EndGame()
    {
        isGameOver = true; // Set the game over flag
        
        gameOver.AnimateGameOver();
      
        Debug.Log("Ending Game");

    }

}
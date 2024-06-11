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


    public Row[] rows; // desde unity se traen los rows (filas) en el que se encuentran los tiles
    public Tile[,] Tiles { get; private set; } //[,] la coma significa que la matriz va a ser bidimennsional en lugar de
                                               //unidimensional, accedes utilizando dos �ndices, uno para la fila y otro para la columna

    public int Width => Tiles.GetLength(0); //numero de filas x vertical
    public int Height => Tiles.GetLength(1);//numero de colum y horizotal

    private readonly List<Tile> _selection = new List<Tile>();//readonly: Una vez que se haya inicializado este campo, no se puede cambiar su valor,
                                                              //significa que una vez que se haya creado una instancia de List<Tile> y se haya asignado a este campo,
                                                              //no se puede asignar otra instancia de List<Tile> a este campo. Sin embargo, los elementos dentro de la lista pueden ser modificados.
                                                              //List<Tile>: Es el tipo de datos del campo.Es una lista generica que contiene elementos del tipo Tile.
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
            // Verifica si el juego ha terminado. Si es así, no hace nada.
            if (isGameOver) return;

            // Si el tile seleccionado no está en la lista de selección
            if (!_selection.Contains(tile))
            {
                // Si ya hay al menos un tile seleccionado
                if (_selection.Count > 0)
                {
                    // Si el tile seleccionado es vecino del primero en la lista, lo añade a la selección
                    if (Array.IndexOf(_selection[0].Neighbours, tile) != -1) _selection.Add(tile);
                }
                else
                {
                    // Si no hay tiles en la selección, añade el tile seleccionado
                    _selection.Add(tile);
                }
            }

            // Si no se han seleccionado al menos dos tiles, sale de la función
            if (_selection.Count < 2) return;

            // Muestra en el log los tiles seleccionados
            Debug.Log($"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

            // Intercambia los dos tiles seleccionados
            await Swap(_selection[0], _selection[1]);

            // Verifica si se puede realizar una acción "pop" (eliminar o hacer desaparecer los tiles)
            if (CanPop())
            {
             
                Pop();
            }
            else
            {
                // Si no se puede hacer "pop", deshace el intercambio
                await Swap(_selection[0], _selection[1]);
            }

            // Limpia la lista de selección
            _selection.Clear();
    }

       public async Task Swap(Tile tile1, Tile tile2)
        {
            var icon1 = tile1.icon;
            var icon2 = tile2.icon;

            var icon1Transform = icon1.transform;
            var icon2Transform = icon2.transform;

            // Crea una secuencia de animación con DOTween
            var sequence = DOTween.Sequence();

            // Añade las animaciones de movimiento a la secuencia
            sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                    .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

            // Espera a que la secuencia de animación termine
            await sequence.Play().AsyncWaitForCompletion();

            // Intercambia los padres de los iconos
            icon1Transform.SetParent(tile2.transform);
            icon2Transform.SetParent(tile1.transform);

            // Intercambia las referencias de los iconos en los tiles
            tile1.icon = icon2;
            tile2.icon = icon1;

            // Intercambia los elementos de los tiles
            var tile1Item = tile1.Item;
            tile1.Item = tile2.Item;
            tile2.Item = tile1Item;

            // Muestra en el log los tiles intercambiados
            Debug.Log($"Swapped tiles at ({tile1.x}, {tile1.y}) and ({tile2.x}, {tile2.y})");
        }


    private bool CanPop() // esta funcion es parte de un sistema de deteccion de combinaciones  devuelve true si hay al menos una combinacion que se puede "eliminar", de acuerdo con las reglas del juego.
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (Tiles[x, y].GetValidConnectedTiles().Count >= 3)
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
        // Limpia la lista de movimientos posibles antes de buscar nuevos movimientos.
        listaMovimientosPosibles.Clear();

        // Obtiene el número de filas y columnas en la matriz de tiles.
        int filas = Tiles.GetLength(0);
        int columnas = Tiles.GetLength(1);

        // Recorre cada tile en la matriz.
        for (int x = 0; x < filas; x++)
        {
            for (int y = 0; y < columnas; y++)
            {
                // Verifica si el tile a la derecha (y + 1) está dentro de los límites y si puede combinarse con el tile actual.
                if (y + 1 < columnas && PuedeCombinar(x, y, x, y + 1))
                {
                    // Agrega las coordenadas del movimiento posible a la lista.
                    listaMovimientosPosibles.Add(new Coordenadas { x1 = x, y1 = y, x2 = x, y2 = y + 1 });
                }

                // Verifica si el tile de abajo (x + 1) está dentro de los límites y si puede combinarse con el tile actual.
                if (x + 1 < filas && PuedeCombinar(x, y, x + 1, y))
                {
                    // Agrega las coordenadas del movimiento posible a la lista.
                    listaMovimientosPosibles.Add(new Coordenadas { x1 = x, y1 = y, x2 = x + 1, y2 = y });
                }
            }
        }

        // Imprime la cantidad de movimientos posibles encontrados para depuración.
        Debug.Log($"Found {listaMovimientosPosibles.Count} possible moves");

        // Devuelve la lista de movimientos posibles.
        return listaMovimientosPosibles;
    }


    private bool PuedeCombinar(int x1, int y1, int x2, int y2)
    {
        // esta funcion intercambia temporalmente dos elementos y verifica si el intercambio resulta en una combinación válida.

        var temp = Tiles[x1, y1].Item;//intercambio tempora.
        Tiles[x1, y1].Item = Tiles[x2, y2].Item;
        Tiles[x2, y2].Item = temp;

        // Verifica si hay una combinación válida en las posiciones intercambiadas
        bool hayCombinacion = HayCombinacionEnPosicion(x1, y1) || HayCombinacionEnPosicion(x2, y2);

        // Restaura los elementos a su posición original
        Tiles[x2, y2].Item = Tiles[x1, y1].Item;
        Tiles[x1, y1].Item = temp;

        Debug.Log($"Move ({x1}, {y1}) <-> ({x2}, {y2}) results in combination: {hayCombinacion}");
        return hayCombinacion;
    }


    private bool HayCombinacionEnPosicion(int x, int y)
    {
        // Comprueba si al menos tres elementos iguales están alineados horizontal o verticalmente
        // en la posición (x, y) del tablero. 

        int id = Tiles[x, y].Item.id;

        // Verifica combinaciones horizontales
        int contador = 1;
        for (int i = x - 1; i >= 0 && Tiles[i, y].Item.id == id; i--) contador++;   //inicia el elemento moviendose un espacio x a la izquierda
                                                                                    //y revisa si alguno de los elementos es de igual id en horizontal, si lo es ,incrementa el contador
        for (int i = x + 1; i < Tiles.GetLength(0) && Tiles[i, y].Item.id == id; i++) contador++;
        if (contador >= 3)
        {
            Debug.Log($"Combination found at ({x}, {y}) horizontally");
            return true;
        }

        // Verifica combinaciones verticales
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
        //esta funcion busca todos los movimientos posibles y verifica si alguno de ellos resulta en una combinación válida
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

    private async void Pop()
    {
        // Itera sobre cada posición en la matriz de tiles
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var connectedTiles = tile.GetValidConnectedTiles();

                // Si el número de fichas conectadas es menor que 3, continúa con la siguiente iteración
                if (connectedTiles.Count < 3) continue;

                // Crea una secuencia de animación para desinflar las fichas conectadas
                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    // Añade la animación de escala a la secuencia para cada ficha conectada
                    deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                }

                // Reproduce un sonido al eliminar las fichas
                audioSource.PlayOneShot(collectSound);

                // Incrementa el puntaje en función del valor de las fichas eliminadas
                ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count;

                // Espera a que la secuencia de desinflado se complete
                await deflateSequence.Play().AsyncWaitForCompletion();

                // Crea una nueva secuencia de animación para inflar nuevas fichas
                var inflateSequence = DOTween.Sequence();

                do
                {//repite el ciclo random hasta que la funcion CanCombinacion retorne true
                    foreach (var connectedTile in connectedTiles)
                    {
                        // Asigna un nuevo ítem aleatorio a cada ficha conectada
                        connectedTile.Item = ItemDataBase.Items[Random.Range(0, ItemDataBase.Items.Length)];

                        // Añade la animación de escala a la secuencia para cada nueva ficha
                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                    }
                } while (!CanCombinacion());

                // Espera a que la secuencia de inflado se complete
                await inflateSequence.Play().AsyncWaitForCompletion();
            }
        }

        // Si el puntaje alcanza o supera 300, finaliza el juego
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
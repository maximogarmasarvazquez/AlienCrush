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

    public Row[] rows; // desde unity se traen los rows en el que se encuentran los tiles

 
    public Tile[,] Tiles { get; private set; } //[,] la coma significa que la matriz va a ser bidimennsional en lugar de  unidimensional, accedes utilizando dos índices, uno para la fila y otro para la columna

    public int Width => Tiles.GetLength( 0);
    public int Height => Tiles.GetLength( 1);

    private readonly List<Tile> _selection = new List<Tile>();//readonly: Una vez que se haya inicializado este campo, no se puede cambiar su valor,
                                                             //significa que una vez que se haya creado una instancia de List<Tile> y se haya asignado a este campo,
                                                            //no se puede asignar otra instancia de List<Tile> a este campo. Sin embargo, los elementos dentro de la lista pueden ser modificados.
                                                           //List<Tile>: Es el tipo de datos del campo.Es una
                                                          //lista genérica que contiene elementos del tipo Tile.Una lista es una colección de elementos que puede crecer o decrecer dinámicamente.
    private const float TweenDuration = 0.25f;
    private void Awake() => Instance = this;//void indica que el metodo no devuelve un valor
                                           //El método Awake() en Unity se utiliza para inicializar objetos antes de que comience el juego.
                                         //Es parte del ciclo de vida de los objetos en Unity y se llama una vez cuando el objeto se activa o se carga en la escena,justo antes de que comience el método Start().
    private void Start()                //En resumen, la función Start() inicializa el tablero del juego asignando aleatoriamente elementos a cada tile y configurando sus coordenadas x e y.
    {
        Tiles = new Tile[rows.Max( row => row.tiles.Length), rows.Length]; 
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



    public async void Select(Tile tile)
    {
                                          //En resumen, esta función gestiona el proceso de selección de tiles en el tablero del juego, realizando intercambios,
                                            //verificaciones y limpieza de la lista de selección según sea necesario.
                                              //Además, utiliza async y await para esperar la finalización de las operaciones asíncronas, como la animación de intercambio de tiles.
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

        Debug.Log( $"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

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

    public async Task Swap(Tile tile1, Tile tile2) //esta función anima el intercambio visual y de datos entre dos Tile en una interfaz gráfica, con una animación suave usando DOTween.
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play()
                      .AsyncWaitForCompletion();


        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;

        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;



    }

    private bool CanPop() // esta función es parte de un sistema de detección de combinaciones en un juego de fichas y devuelve true si hay al menos una combinación que se puede "eliminar", de acuerdo con las reglas del juego.
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            { 
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) return true;
            }
           
        }
        return false;        
    } 
    public void Win()   //funcion para cambiar la scena al llegar al objetivo
    {

      if (ScoreCounter.Instance.Score >= 100)
     {
              Debug.Log("Cambio de escena");
             SceneManager.LoadScene(1);  
         
        }
     }


    public List<Coordenadas> lista = new List<Coordenadas>
{
new Coordenadas { x1 = 0, y1 = 0, x2 = 1, y2 = 0, x3 = 3 , y3 = 0 },
    
};

    private bool CanCombinacion() // función para analizar las posibles combinaciones del tablero antes de que se realicen
    {
        foreach (var cordenada in lista)
        {
            if (Tiles[cordenada.x1, cordenada.y1].Item.id == Tiles[cordenada.x2, cordenada.y2].Item.id && Tiles[cordenada.x3, cordenada.y3].Item.id == Tiles[cordenada.x2, cordenada.y2].Item.id)
            {
                Debug.Log("hay combinaciones");
                return true;
            }
        }

        Debug.Log("no hay combinaciones");
        return false;
    }



    private async void Pop() //esta función maneja la eliminación de grupos de fichas conectadas en un juego de puzzle, animando la eliminación y la aparición de nuevas fichas.
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

                await deflateSequence.Play()
                            .AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();
                
                foreach (var connectedTile in connectedTiles)
                {
                   
                    connectedTile.Item = ItemDataBase.Items[Random.Range(0, ItemDataBase.Items.Length)];
                    
                    inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }  
                if (CanCombinacion())
                    {
                       
                    }
                      

                await inflateSequence.Play()
                            .AsyncWaitForCompletion();

                x = 0;
                y = 0;

            }

        }
    }

}



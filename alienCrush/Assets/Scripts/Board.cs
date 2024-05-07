using System.Linq;
using UnityEngine;

public sealed class Board : MonoBehaviour
{

    public static Board Instance { get; private set; }

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);


    private void Awake()=>Instance = this;

    private void Start()
    {
        Tiles = new Tile[ rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {

            for (var x = 0; y < Width; x++)
            {   
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                tile.Item = ItemDataBase.Items[Random.Range(0, ItemDataBase.Items.Length)];

                Tiles[x, y] = tile;
            }
        }
    }
   
 }

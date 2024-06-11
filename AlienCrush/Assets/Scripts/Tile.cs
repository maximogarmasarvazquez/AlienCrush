using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{
    public int x;
    public int y;
    private Board Board;
    private Item _item;

    public Item Item
    {
        get => _item;
        set
        {
            if (_item == value) return;

            _item = value;
            icon.sprite = _item.sprite;
        }
    }

    public Image icon;
    public Button button;

    //devuelve la ficha del lugar que se le indique si no es podible devuekve null
    public Tile Left => x > 0 ? Board.Instance.Tiles[x - 1, y] : null;
    public Tile Top => y > 0 ? Board.Instance.Tiles[x, y - 1] : null;
    public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.Tiles[x + 1, y] : null;
    public Tile Bottom => y < Board.Instance.Height - 1 ? Board.Instance.Tiles[x, y + 1] : null;

    public Tile[] Neighbours => new[]
    {
        Left,
        Top,
        Right,
        Bottom,
    };

    private void Start() => button.onClick.AddListener(() => Board.Instance.Select(this));

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null) //inicia null hasta que se le cree la lista
    {
        // Lista que almacenará todas las fichas conectadas.
        var result = new List<Tile> { this };

        // Si la lista de exclusión es nula, crea una nueva lista con la ficha actual.
        if (exclude == null)
        {
            exclude = new List<Tile> { this };
        }
        // De lo contrario, agrega la ficha actual a la lista de exclusión.
        else
        {
            exclude.Add(this);
        }

        // Itera sobre todas las fichas vecinas.
        foreach (var neighbour in Neighbours)
        {
            // Si la ficha vecina es nula, está en la lista de exclusión o tiene un elemento diferente, continúa con la siguiente iteración.
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;

            // Agrega todas las fichas conectadas a la ficha vecina y sus sub-fichas conectadas a la lista de resultados.
            result.AddRange(neighbour.GetConnectedTiles(exclude));
        }

        // Devuelve la lista de fichas conectadas.
        return result;
    }

    public List<Tile> GetValidConnectedTiles()
    {
        // Obtiene todas las fichas conectadas.
        var connectedTiles = GetConnectedTiles();

        // Conjunto que almacenará las fichas conectadas válidas.
        var validTiles = new HashSet<Tile>();

        // Comprueba grupos horizontales y verticales.
        foreach (var tile in connectedTiles)
        {
            // Lista que almacenará las fichas del grupo horizontal.
            var horizontalGroup = new List<Tile> { tile };
            var currentTile = tile;

            // Comprueba a la izquierda.
            while (currentTile.Left != null && currentTile.Left.Item == tile.Item)
            {
                horizontalGroup.Add(currentTile.Left);
                currentTile = currentTile.Left;
            }

            currentTile = tile;

            // Comprueba a la derecha.
            while (currentTile.Right != null && currentTile.Right.Item == tile.Item)
            {
                horizontalGroup.Add(currentTile.Right);
                currentTile = currentTile.Right;
            }

            // Si el grupo horizontal tiene al menos tres fichas, las agrega al conjunto de fichas conectadas válidas.
            if (horizontalGroup.Count >= 3)
            {
                foreach (var t in horizontalGroup)
                {
                    validTiles.Add(t);
                }
            }
        }

        // Comprueba grupos verticales.
        foreach (var tile in connectedTiles)
        {
            // Lista que almacenará las fichas del grupo vertical.
            var verticalGroup = new List<Tile> { tile };
            var currentTile = tile;

            // Comprueba arriba.
            while (currentTile.Top != null && currentTile.Top.Item == tile.Item)
            {
                verticalGroup.Add(currentTile.Top);
                currentTile = currentTile.Top;
            }

            currentTile = tile;

            // Comprueba abajo.
            while (currentTile.Bottom != null && currentTile.Bottom.Item == tile.Item)
            {
                verticalGroup.Add(currentTile.Bottom);
                currentTile = currentTile.Bottom;
            }

            // Si el grupo vertical tiene al menos tres fichas, las agrega al conjunto de fichas conectadas válidas.
            if (verticalGroup.Count >= 3)
            {
                foreach (var t in verticalGroup)
                {
                    validTiles.Add(t);
                }
            }
        }

        // Devuelve la lista de fichas conectadas válidas convertida de un HashSet a una lista.
        return new List<Tile>(validTiles);
    }
}
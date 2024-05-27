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

    public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
    {
        var result = new List<Tile> { this };

        if (exclude == null)
        {
            exclude = new List<Tile> { this };
        }
        else
        {
            exclude.Add(this);
        }

        foreach (var neighbour in Neighbours)
        {
            if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;

            result.AddRange(neighbour.GetConnectedTiles(exclude));
        }

        return result;
    }

    public List<Tile> GetValidConnectedTiles()
    {
        var connectedTiles = GetConnectedTiles();
        var validTiles = new HashSet<Tile>();

        // Check horizontal groups
        foreach (var tile in connectedTiles)
        {
            var horizontalGroup = new List<Tile> { tile };
            var currentTile = tile;

            // Check left
            while (currentTile.Left != null && currentTile.Left.Item == tile.Item)
            {
                horizontalGroup.Add(currentTile.Left);
                currentTile = currentTile.Left;
            }

            currentTile = tile;

            // Check right
            while (currentTile.Right != null && currentTile.Right.Item == tile.Item)
            {
                horizontalGroup.Add(currentTile.Right);
                currentTile = currentTile.Right;
            }

            if (horizontalGroup.Count >= 3)
            {
                foreach (var t in horizontalGroup)
                {
                    validTiles.Add(t);
                }
            }
        }

        // Check vertical groups
        foreach (var tile in connectedTiles)
        {
            var verticalGroup = new List<Tile> { tile };
            var currentTile = tile;

            // Check top
            while (currentTile.Top != null && currentTile.Top.Item == tile.Item)
            {
                verticalGroup.Add(currentTile.Top);
                currentTile = currentTile.Top;
            }

            currentTile = tile;

            // Check bottom
            while (currentTile.Bottom != null && currentTile.Bottom.Item == tile.Item)
            {
                verticalGroup.Add(currentTile.Bottom);
                currentTile = currentTile.Bottom;
            }

            if (verticalGroup.Count >= 3)
            {
                foreach (var t in verticalGroup)
                {
                    validTiles.Add(t);
                }
            }
        }

        return new List<Tile>(validTiles);
    }
}
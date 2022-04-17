using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap<T>
{
    T[] cells;
    public T this[int x, int y]
    {
        get
        {
            return this[new Vector2Int(x, y)];
        }
        set
        {
            this[new Vector2Int(x, y)] = value;
        }
    }
    public T this[Vector2Int pos]
    {
        get
        {
            pos += Offset;
            return cells[GetIndex(pos)];
        }
        set
        {
            pos += Offset;
            cells[GetIndex(pos)] = value;
        }
    }

    private Vector2Int size;
    public Vector2Int Size
    {
        get { return size; }
        private set { size = value; }
    }

    private Vector2Int offset;
    public Vector2Int Offset
    {
        get { return offset; }
        set { offset = value; }
    }

    public GridMap(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;

        cells = new T[size.x * size.y];
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Offset);
    }
}

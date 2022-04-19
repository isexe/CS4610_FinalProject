using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D<T>
{
    T[] data;

    private Vector2Int origin;
    public Vector2Int Origin
    {
        get
        {
            return origin;
        }
        private set
        {
            origin = value;
        }
    }
    private Vector2Int size;
    public Vector2Int Size
    {
        get
        {
            return size;
        }
        private set
        {
            size = value;
        }
    }

    public Grid2D(Vector2Int size, Vector2Int origin)
    {
        Origin = origin;
        Size = size;

        data = new T[size.x * size.y];
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (pos.y * Size.x);
    }

    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Origin);
    }

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
            pos += Origin;
            return data[GetIndex(pos)];
        }
        set
        {
            pos += Origin;
            data[GetIndex(pos)] = value;
        }
    }
}

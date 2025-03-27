using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace StickBlast.Grid
{
    public enum GridType
    {
        Rectangle,
        Hexagon
    }

    public enum AxisType
    {
        XZ,
        XY
    }

    public enum Direction
    {
        Up,
        Down,
        Right,
        Left,
        // UpRight,
        // UpLeft,
        // DownRight,
        // DownLeft
    }


    public class GridCreator
    {
        public void Create(GridType gridType, AxisType axisType, TileSetting setting, Vector2 size, bool createAsPrefab)
        {
            var prefab = GetPrefab(gridType, setting);

            //Create GridService Object
            var gridManager = new GameObject
            {
                transform =
                {
                    name = $"Grid {gridType} [{size.x},{size.y}]"
                }
            }.AddComponent<GridService>();

            //Create Tiles
            var tiles = new List<TileController>();
            var tilePosition = Vector3.zero;


            for (int y = 0; y < size.y; y++)
            {
                tilePosition = GetTileVerticalPosition(tilePosition, y, axisType, gridType, setting);

                for (int x = 0; x < size.x; x++)
                {
                    var coordinate = new Vector2Int(x, y);
                    if (!CanSpawn(gridType, coordinate))
                        continue;

                    tilePosition.x = x * GetDistance(gridType, setting).x;

                    TileController tile = null;
                    if (createAsPrefab)
                    {
#if UNITY_EDITOR
                        var prefabGameObject = prefab.gameObject;
                        var tileGameObject = PrefabUtility.InstantiatePrefab(prefabGameObject) as GameObject;
                        if (tileGameObject != null) tile = tileGameObject.GetComponent<TileController>();
                        if (tile != null) tile.transform.position = tilePosition;
#endif
                    }
                    else
                    {
                        tile = Object.Instantiate(prefab, tilePosition, Quaternion.identity);
                    }

                    if (tile == null) continue;
                    tile.transform.name = $"Tile [{x},{y}]";
                    tile.coordinate = coordinate;
                    tiles.Add(tile);
                }
            }

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                tiles[i].transform.SetParent(gridManager.transform);
            }

            /*
             * 
             
            for (int vertical = 0; vertical < size.y; vertical++)
            {
                tilePosition = GetTileVerticalPosition(tilePosition, vertical, axisType, gridType, setting);

                for (int horizontal = 0; horizontal < size.x; horizontal++)
                {
                    var coordinate = new Vector2Int(horizontal, vertical);
                    if (!CanSpawn(gridType, coordinate))
                        continue;

                    tilePosition.x = horizontal * GetDistance(gridType, setting).x;

                    TileController tile = null;
                    if (createAsPrefab)
                    {
#if UNITY_EDITOR
                        var prefabGameObject = prefab.gameObject;
                        var tileGameObject = PrefabUtility.InstantiatePrefab(prefabGameObject) as GameObject;
                        if (tileGameObject != null) tile = tileGameObject.GetComponent<TileController>();
                        if (tile != null) tile.transform.position = tilePosition;
#endif
                    }
                    else
                    {
                        tile = Object.Instantiate(prefab, tilePosition, Quaternion.identity);
                    }
                    
                    if (tile == null) continue;
                    tile.transform.name = $"Tile [{horizontal},{vertical}]";
                    tile.coordinate = coordinate;
                    tile.transform.SetParent(gridManager.transform);
                    tiles.Add(tile);
                }
            }
            */

            gridManager.SetTiles(tiles);
            SetNeighbor(gridManager, gridType);
            SetCenter(gridManager.gameObject);
        }

        private TileController GetPrefab(GridType gridType, TileSetting tileSetting)
        {
            return gridType switch
            {
                GridType.Rectangle => tileSetting.rectangle.prefab,
                GridType.Hexagon => tileSetting.hexagon.prefab,
                _ => null
            };
        }

        private Vector2 GetDistance(GridType gridType, TileSetting tileSetting)
        {
            return gridType switch
            {
                GridType.Rectangle => tileSetting.rectangle.distance,
                GridType.Hexagon => tileSetting.hexagon.distance,
                _ => Vector2Int.zero
            };
        }

        private Vector3 GetTileVerticalPosition(Vector3 position, int count, AxisType type, GridType gridType,
            TileSetting setting)
        {
            var axis = count * GetDistance(gridType, setting).y;
            switch (type)
            {
                case AxisType.XY:
                    position.y = axis;
                    break;
                case AxisType.XZ:
                    position.z = axis;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return position;
        }

        private bool CanSpawn(GridType type, Vector2Int coordinate)
        {
            var isPairHorizontal = coordinate.x % 2 == 0;
            var isPairVertical = coordinate.y % 2 == 0;
            return type switch
            {
                GridType.Rectangle => true,
                GridType.Hexagon => isPairHorizontal == isPairVertical,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static void SetCenter(GameObject parent)
        {
            var pivotHelper = parent.AddComponent<SetCenterPosition>();
            pivotHelper.SetCenter();
            parent.transform.position = Vector3.zero;
            Object.DestroyImmediate(pivotHelper);
        }

        private void SetNeighbor(GridService manager, GridType gridType)
        {
            var length = Enum.GetNames(typeof(Direction)).Length;
            foreach (var tile in manager.Tiles)
            {
                var data = new List<TileData>();
                for (int i = 0; i < length; i++)
                {
                    var direction = (Direction)i;
                    var coordinate = tile.coordinate.GetCoordinate(direction);

                    //One step more if hexagon and vertical
                    if (gridType == GridType.Hexagon)
                    {
                        if (direction is Direction.Down or Direction.Up)
                            coordinate = coordinate.GetCoordinate(direction);
                    }

                    var neighbour = manager.GetTile(coordinate);
                    if (!neighbour) continue;
                    data.Add(new TileData()
                    {
                        tile = manager.GetTile(coordinate),
                        direction = direction
                    });
                }

                tile.PrepareNeighbour(data);
            }
        }
    }
}
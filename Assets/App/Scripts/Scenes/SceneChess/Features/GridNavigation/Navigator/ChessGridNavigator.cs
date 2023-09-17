using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Piece;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        public ChessUnit chessUnit;
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            var dsa = unit.ToString();
            var piece = grid.Get(from);
            var fdsfgdf = FindTypePiece(unit, piece);
            
            var locations = fdsfgdf.ShortestTurn(from, to, grid);

            return locations;
        }


        private Piece FindTypePiece(ChessUnitType unitType, ChessUnit chessUnit)
        {
            Piece piece = unitType.ToString() switch
            {
                "Pon" => new Pon(chessUnit),
                "King" => new King(chessUnit),
                "Queen" => new Queen(chessUnit),
                "Rook" => new Rook(chessUnit),
                "Knight" => new Knight(chessUnit),
                "Bishop" => new Bishop(chessUnit),
                _ => null

            };
            return piece;
        }

    }



    public abstract class Piece
    {
        protected ChessUnitType chessUnitType;
        protected ChessUnit chessUnit;
        protected List<Vector2Int> zeroPath = new List<Vector2Int>();
        protected Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)};
        protected Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

        protected Piece(ChessUnit chessUnit)
        {
            this.chessUnit = chessUnit;
        }

        public abstract List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid);
    }

    public class Pon : Piece
    {
        public List<List<Vector2Int>> listPaths = new List<List<Vector2Int>>();

        public Pon(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            List<Vector2Int> locations = new List<Vector2Int>();
            var wrongDirection = (to.y - from.y);
            int forwardDirection = chessUnit.PieceModel.Color == 0 ? -1 : 1;
            if (from.x != to.x || forwardDirection > wrongDirection)
            {
                return zeroPath;
            }
            int firstGridRow = 0;
            Vector2Int currentPosition = new Vector2Int(from.x, from.y);
            for (int i = 0; i < 7; i++)
            {
                Vector2Int forward = new Vector2Int(currentPosition.x, currentPosition.y + forwardDirection);
                if (forward.y < grid.Size.y && forward.y > firstGridRow && grid.Get(forward.x,forward.y) == null)
                {
                    currentPosition = forward;
                    locations.Add(forward);
                }
            }
            if(currentPosition != to)
            {
                return zeroPath;
            }

            return locations;
        }
    }


    public class King : Piece
    {
        public King(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            List<Vector2Int> locations = new List<Vector2Int>();
            List<Vector2Int> directions = new List<Vector2Int>(BishopDirections);
            directions.AddRange(RookDirections);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextGridPoint = new Vector2Int(from.x + dir.x, from.y + dir.y);
                locations.Add(nextGridPoint);
            }

            return locations;
        }
    }

    public class Queen : Piece
    {
        public Queen(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            List<Vector2Int> locations = new List<Vector2Int>();
            List<Vector2Int> directions = new List<Vector2Int>(BishopDirections);
            directions.AddRange(RookDirections);

            foreach (Vector2Int dir in directions)
            {
                for (int i = 1; i < 8; i++)
                {
                    Vector2Int nextGridPoint = new Vector2Int(from.x + i * dir.x, from.y + i * dir.y);
                    if (grid.Get(nextGridPoint.x, nextGridPoint.y) != null)
                    {
                        break;
                    }
                    locations.Add(nextGridPoint);
                }
            }

            return locations;
        }
    }

    public class Rook : Piece
    {
        public Rook(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {

            List<Vector2Int> locations = new List<Vector2Int>();

            foreach (Vector2Int dir in RookDirections)
            {
                for (int i = 1; i < 8; i++)
                {
                    Vector2Int nextGridPoint = new Vector2Int(from.x + i * dir.x, from.y + i * dir.y);
                    if (grid.Get(nextGridPoint.x, nextGridPoint.y) != null)
                    {
                        break;
                    }
                    locations.Add(nextGridPoint);
                }
            }

            return locations;
        }
    }

    class Pair
    {
        public Pair(int first, int second)
        {
            First = first;
            Second = second;
        }

        public int First { get; set; }
        public int Second { get; set; }
    }

    public class Knight : Piece
    {
        public Knight(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        private Vector2Int topLeftLoc = new Vector2Int(-1, 2);
        private Vector2Int topRightLoc = new Vector2Int(1, 2);

        private Vector2Int rightTopLoc = new Vector2Int(2, 1);
        private Vector2Int leftTopLoc = new Vector2Int(-2, 1);

        private Vector2Int rightBotLoc = new Vector2Int(2, -1);
        private Vector2Int leftBotLoc = new Vector2Int(-2, -1);

        private Vector2Int botRightLoc = new Vector2Int(1, -2);
        private Vector2Int botLeftLoc = new Vector2Int(-1, -2);

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {



            List<Vector2Int> locations = LoadLocations(from, grid);


            bool[][] used = new bool [1000][];
            int[][] dst = new int[1000][];

            bool not_visited(int x, int y)
            {
                return x >= 0 && x < 8
                    && y >= 0 && y < 8
                    && !used[x][y];
            }

            int x1 = from.x, y1 = from.y, x2 = to.x, y2 = to.y;
            Queue<(int, int)> q = new Queue<(int,int)>();
            q.Enqueue((from.x, from.y));       
            used[x1][y1] = true;
            dst[x1][y1] = 0;

            while (q.Count!=0)
            {
                (int, int) cur = q.Peek();
                q.Dequeue();
                int cx = cur.Item1, cy = cur.Item2;

                List<Pair> moves = new List<Pair>(){ new Pair(1, 2), new Pair(1, -2), new Pair(-1, 2), new Pair(-1, -2), new Pair(2, 1), new Pair(2, -1), new Pair(-2, 1), new Pair(-2, -1) };

                foreach (var move in moves)
                {
                    int dx = move.First, dy = move.Second;
                    if (not_visited(cx + dx, cy + dy))
                    {
                        q.Enqueue( (cx + dx, cy + dy));
                        used[cx + dx][cy + dy] = true;
                        dst[cx + dx][cy + dy] = dst[cx][cy] + 1;
                    }
                }
            }

                if (dst[x2][y2] != -1) {
                    Debug.Log(dst[x2][y2]);
                } else
                {
                Debug.Log("Impossible");
                }


        return locations;
        }

        private List<Vector2Int> LoadLocations(Vector2Int from, ChessGrid grid)
        {
            List<Vector2Int> loc = new List<Vector2Int>();
            topLeftLoc += from;
            topRightLoc += from;
            rightTopLoc += from;
            leftTopLoc += from;
            rightBotLoc += from;
            leftBotLoc += from;
            botRightLoc += from;
            botLeftLoc += from;
            if (((topLeftLoc.x >= 0 && topLeftLoc.x <= 7) && ( topLeftLoc.y >= 0 && topLeftLoc.y <= 7)))
            {
                if(grid.Get(topLeftLoc.x, topLeftLoc.y) == null)
                loc.Add(topLeftLoc);
            }
            if (((topRightLoc.x >= 0 && topRightLoc.x <= 7 ) && (topRightLoc.y >= 0 && topRightLoc.y <= 7)) )
            {
                if (grid.Get(topRightLoc.x, topRightLoc.y) == null)
                    loc.Add(topRightLoc);
            }
            if (((rightTopLoc.x >= 0 && rightTopLoc.x <= 7 ) && (rightTopLoc.y >= 0 && rightTopLoc.y <= 7)) )
            {
                if (grid.Get(rightTopLoc.x, rightTopLoc.y) == null)
                    loc.Add(rightTopLoc);
            }
            if (((leftTopLoc.x >= 0 && leftTopLoc.x <= 7) && ( leftTopLoc.y >= 0 && leftTopLoc.y <= 7))  )
            {
                if (grid.Get(leftTopLoc.x, leftTopLoc.y) == null)
                    loc.Add(leftTopLoc);
            }
            if (((rightBotLoc.x >= 0 && rightBotLoc.x <= 7 ) && (rightBotLoc.y >= 0 && rightBotLoc.y <= 7)) )
            {
                if (grid.Get(rightBotLoc.x, rightBotLoc.y) == null)
                    loc.Add(rightBotLoc);
            }
            if (((leftBotLoc.x >= 0 && leftBotLoc.x <= 7) && ( leftBotLoc.y >= 0 && leftBotLoc.y <= 7)) )
            {
                if (grid.Get(leftBotLoc.x, leftBotLoc.y) == null)
                    loc.Add(leftBotLoc);
            }
            if (((botRightLoc.x >= 0 && botRightLoc.x <= 7) && ( botRightLoc.y >= 0 && botRightLoc.y <= 7)) )
            {
                if (grid.Get(botRightLoc.x, botRightLoc.y) == null)
                    loc.Add(botRightLoc);
            }
            if (((botLeftLoc.x >= 0 && botLeftLoc.x <= 7) && (  botLeftLoc.y >= 0 && botLeftLoc.y <= 7)) )
            {
                if (grid.Get(botLeftLoc.x, botLeftLoc.y) == null)
                    loc.Add(botLeftLoc);
            }

            return loc;
        }
    }

    public class Bishop : Piece
    {
        public Bishop(ChessUnit chessUnit) : base(chessUnit)
        {
        }

        public override List<Vector2Int> ShortestTurn(Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            List<Vector2Int> locations = new List<Vector2Int>();
            SearchTurn();
            foreach (Vector2Int dir in BishopDirections)
            {
                for (int i = 1; i < 8; i++)
                {
                    Vector2Int nextGridPoint = new Vector2Int(from.x + i * dir.x, from.y + i * dir.y);
                    if (((nextGridPoint.x < 0 || nextGridPoint.y < 0) || (nextGridPoint.x > 7 || nextGridPoint.y > 7)))
                    {
                        break;
                    }
                    else if(grid.Get(nextGridPoint) != null)
                    {
                        break;
                    }
                    locations.Add(nextGridPoint);
                }
            }

            return locations;
        }

        public Dictionary<List<Vector2Int>, float> SearchTurn()
        { 
            return null; 
        }

    }
}
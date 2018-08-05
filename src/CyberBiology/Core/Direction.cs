using System;
using System.Collections.Generic;

namespace CyberBiology.Core
{
    public class Direction
    {
        private static readonly Random Rnd = new Random();

        public static readonly Direction NorthWest = new Direction(0, -1, -1);
        public static readonly Direction North = new Direction(1, 0, -1);
        public static readonly Direction NorthEast = new Direction(2, 1, -1);
        public static readonly Direction East = new Direction(3, 1, 0);
        public static readonly Direction SouthEast = new Direction(4, 1, 1);
        public static readonly Direction South = new Direction(5, 0, 1);
        public static readonly Direction SouthWest = new Direction(6, -1, 1);
        public static readonly Direction West = new Direction(7, -1, 0);

        private readonly int _index;
        public readonly int Dx;
        public readonly int Dy;

        public static readonly IReadOnlyList<Direction> All = new List<Direction>
        {
            NorthWest, North, NorthEast, East, SouthEast, South, SouthWest, West
        };

        
        private Direction(int index, int dx, int dy)
        {
            _index = index;
            Dx = dx;
            Dy = dy;
        }

        private static readonly Direction[] array =
        {
            NorthWest, North, NorthEast, East, SouthEast, South, SouthWest, West,
            NorthWest, North, NorthEast, East, SouthEast, South, SouthWest, West,
        };

        public static IEnumerable<Direction> From(Direction direction)
        {
            yield return direction;
            yield return array[direction._index + 1];
            yield return array[direction._index + 2];
            yield return array[direction._index + 3];
            yield return array[direction._index + 4];
            yield return array[direction._index + 5];
            yield return array[direction._index + 6];
            yield return array[direction._index + 7];
        }

        public static Direction Random()
        {
            return array[Rnd.Next(100) % array.Length];
        }

        public static Direction Offset(Direction direction, int offsetValue)
        {
            return array[(direction._index + offsetValue) % array.Length];
        }
    }
}
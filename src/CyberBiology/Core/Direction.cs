﻿using System;
using System.Collections.Generic;

namespace CyberBiology.Core
{
    public class Direction
    {
        private static readonly Random Rnd = new Random();

        public static readonly Direction NORTHWEST = new Direction(0, -1, -1);
        public static readonly Direction NORTH = new Direction(1, 0, -1);
        public static readonly Direction NORTHEAST = new Direction(2, 1, -1);
        public static readonly Direction EAST = new Direction(3, 1, 0);
        public static readonly Direction SOUTHEAST = new Direction(4, 1, 1);
        public static readonly Direction SOUTH = new Direction(5, 0, 1);
        public static readonly Direction SOUTHWEST = new Direction(6, -1, 1);
        public static readonly Direction WEST = new Direction(7, -1, 0);
       
        public readonly int Index;
        public readonly int Dx;
        public readonly int Dy;

        public static readonly IReadOnlyList<Direction> All = new List<Direction>
        {
            NORTHWEST, NORTH, NORTHEAST, EAST, SOUTHEAST, SOUTH, SOUTHWEST, WEST
        };

        
        private Direction(int index, int dx, int dy)
        {
            Index = index;
            Dx = dx;
            Dy = dy;
        }

        private static readonly Direction[] array =
        {
            NORTHWEST, NORTH, NORTHEAST, EAST, SOUTHEAST, SOUTH, SOUTHWEST, WEST,
            NORTHWEST, NORTH, NORTHEAST, EAST, SOUTHEAST, SOUTH, SOUTHWEST, WEST,
        };

        public static IEnumerable<Direction> From(Direction direction)
        {
            yield return direction;
            yield return array[direction.Index + 1];
            yield return array[direction.Index + 2];
            yield return array[direction.Index + 3];
            yield return array[direction.Index + 4];
            yield return array[direction.Index + 5];
            yield return array[direction.Index + 6];
            yield return array[direction.Index + 7];
        }

        public static Direction Random()
        {
            return array[Rnd.Next(100) % array.Length];
        }
    }
}
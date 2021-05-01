using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MobulaPuzzleGame
{
    public static class VectorHelper
    {
        public static Vector Zero()
        {
            return new Vector(0, 0);
        }

        public static Vector MoveToward(Vector current, Vector target, float speed)
        {
            Vector direction = target - current;
            direction.Normalize();
            return current + direction * speed;
        }

        public static double Distance(Vector current, Vector target)
        {
            return Math.Sqrt((current.X - target.X)* (current.X - target.X)+ (current.Y - target.Y)* (current.Y - target.Y));
        }
    }
}

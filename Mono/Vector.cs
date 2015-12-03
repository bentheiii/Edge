extern alias MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame::Microsoft.Xna.Framework;

namespace CobraMono.Vectors
{
    public static class VectorExtentions
    {
        public static Vector2 add(this Vector2 p, float x, float y)
        {
            return new Vector2((p.X + x), (p.Y + y));
        }
        public static Vector2 add(this Vector2 p, Vector2 t)
        {
            return new Vector2((p.X + t.X), (p.Y + t.Y));
        }
    }
}

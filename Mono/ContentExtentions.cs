extern alias MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame::Microsoft.Xna.Framework.Content;
using MonoGame::Microsoft.Xna.Framework.Graphics;

namespace CobraMono.ContentExtentions
{
    public static class ContentExtentions
    {
        public static Texture2D LoadTexture2D(this ContentManager @this, string name)
        {
            return @this.Load<Texture2D>(name);
        }
        public static SpriteFont LoadSpriteFont(this ContentManager @this, string name)
        {
            return @this.Load<SpriteFont>(name);
        }
    }
}

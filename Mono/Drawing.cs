extern alias MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CobraMono.Animation;
using MonoGame::Microsoft.Xna.Framework;
using MonoGame::Microsoft.Xna.Framework.Graphics;

namespace CobraMono.Drawing
{
    public abstract class Drawable
    {
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint, float rotation, Vector2 origin,Vector2 scale,
                                  SpriteEffects effect, float layerdepth);
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint)
        {
            this.Draw(spriteBatch,location,tint,0f,Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            this.Draw(spriteBatch,Vector2.Zero,Color.White);
        }
    }
    public class Texture2DDrawable : Drawable
    {
        private readonly Texture2D _internal;
        public Texture2DDrawable(Texture2D @internal)
        {
            this._internal = @internal;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerdepth)
        {
            spriteBatch.Draw(_internal,location,null,tint,rotation,origin, scale,effect,layerdepth);
        }
    }
    public static class AnimationExtentions
    {
        public static void Draw(this SpriteBatch @this, Drawable sprite)
        {
            sprite.Draw(@this);
        }
        public static void Draw(this SpriteBatch @this, Drawable sprite, Vector2 location, Color tint)
        {
            sprite.Draw(@this, location, tint);
        }
        public static void Draw(this SpriteBatch @this, Drawable sprite, Vector2 location, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerdepth)
        {
            sprite.Draw(@this, location, tint, rotation, origin, scale, effect, layerdepth);
        }
    }
}

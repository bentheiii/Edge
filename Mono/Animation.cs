extern alias MonoGame;
using CobraCore.NumbersMagic.SpecialVals;
using CobraMono.Drawing;
using MonoGame::Microsoft.Xna.Framework;
using MonoGame::Microsoft.Xna.Framework.Graphics;

namespace CobraMono.Animation
{
    extern alias MonoGame;

    public class AtlasSprite : Drawable
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private Roller<int> _currentFrame;
        public int totalFrames => this.Rows * this.Columns;

        public AtlasSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            this._currentFrame = new Roller<int>(0,0,totalFrames);
        }

        public void Update(int val=0)
        {
            this._currentFrame+=val;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)(this._currentFrame / (float)Columns);
            int column = this._currentFrame % Columns;
            var sourceRectangle = new Rectangle(width * column, height * row, width, height);
            var destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, tint);
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerdepth)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)(this._currentFrame / (float)Columns);
            int column = this._currentFrame % Columns;
            var sourceRectangle = new Rectangle(width * column, height * row, width, height);
            var destinationRectangle = new Rectangle((int)location.X, (int)location.Y, (int) (width*scale.X), (int) (height*scale.Y));
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, tint, rotation, origin, effect, layerdepth);
        }
    }
    
}

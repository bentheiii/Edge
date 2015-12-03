extern alias MonoGame;
using System;
using System.Collections;
using System.Collections.Generic;
using CobraCore.Shapes.Lines;
using CobraCore.Shapes.Polygons;
using CobraCore.Units.Angle;
using CobraMono.Drawing;
using MonoGame::Microsoft.Xna.Framework;
using MonoGame::Microsoft.Xna.Framework.Graphics;
using MonoGame::Microsoft.Xna.Framework.Input;
using Rectangle = CobraCore.Shapes.Polygons.Rectangle;

/*TODO: collisions
        clicks
*/
namespace CobraMono.Theater2D
{
    public class Theater : Game
    {
        protected IList<Layer> Layers = new List<Layer>();
        private SpriteBatch _spriteBatch;

        protected override void Update(GameTime gametime)
        {
            foreach (Layer layer in Layers)
            {
                foreach (Entity e in layer)
                {
                    e.onUpdate(this,layer,e,gametime);
                }
            }
        }
        protected override void Draw(GameTime gametime)
        {
            _spriteBatch.Begin();
            foreach (Layer layer in Layers)
            {
                foreach (Entity e in layer)
                {
                    _spriteBatch.Draw(e);
                }
            }
            _spriteBatch.End();
        }
        protected override void LoadContent()
        {
            this._spriteBatch = new SpriteBatch(GraphicsDevice);
        }
    }
    public class Layer : ICollection<Entity>
    {
        public Layer(string name)
        {
            this.name = name;
        }
        private readonly ICollection<Entity> _members = new List<Entity>(); 
        public string name { get; }
        public IEnumerator<Entity> GetEnumerator()
        {
            return this._members.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this._members).GetEnumerator();
        }
        public void Add(Entity item)
        {
            this._members.Add(item);
        }
        public void Clear()
        {
            this._members.Clear();
        }
        public bool Contains(Entity item)
        {
            return this._members.Contains(item);
        }
        public void CopyTo(Entity[] array, int arrayIndex)
        {
            this._members.CopyTo(array, arrayIndex);
        }
        public bool Remove(Entity item)
        {
            return this._members.Remove(item);
        }
        public int Count
        {
            get
            {
                return this._members.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return this._members.IsReadOnly;
            }
        }
        public bool Replace(Entity oldEntity, Entity newEntity)
        {
            if (Remove(oldEntity))
            {
                Add(newEntity);
                return true;
            }
            return false;
        }
        public bool Replace(Entity oldEntity, Func<Entity,Entity> transform)
        {
            return Replace(oldEntity, transform(oldEntity));
        }
    }
    public abstract class Entity : Drawable
    {
        public virtual IPolygon CollisionShape() => new EmptyShape();
        public virtual IPolygon InputShape() => new EmptyShape();
        public virtual Int64 collisionflags() => 1;
        public event Action<Theater, Layer, Entity, GameTime> Update;
        public event Action<Theater, Layer, Entity, GameTime, MouseState> Click;
        public event Action<Theater, Layer, Entity, Entity, GameTime, LineSegment> Collide;
        public virtual void onUpdate(Theater arg1, Layer arg2, Entity arg3, GameTime arg4)
        {
            this.Update?.Invoke(arg1, arg2, arg3, arg4);
        }
        public virtual void onClick(Theater arg1, Layer arg2, Entity arg3, GameTime arg4, MouseState arg5)
        {
            this.Click?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
        protected virtual void onCollide(Theater arg1, Layer arg2, Entity arg3, Entity arg4, GameTime arg5, LineSegment arg6)
        {
            this.Collide?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    public class TransformationEntity : Entity
    {
        private readonly Entity _internal;
        private readonly Vector2 _scale;
        private readonly Vector2 _location;
        private readonly float _rotation;
        public TransformationEntity(Entity @internal, Vector2 location, float rotation, Vector2 scale)
        {
            this._internal = @internal;
            this._location = location;
            this._rotation = rotation;
            this._scale = scale;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerdepth)
        {
            this._internal.Draw(spriteBatch, location, tint, rotation, origin, scale, effect, layerdepth);
        }
        public TransformationEntity Transform(Vector2 location, float rotation, Vector2 scale)
        {
            return new TransformationEntity(_internal,_location+location,_rotation+rotation,_scale*scale);
        }
        public override IPolygon CollisionShape()
        {
            return _internal.CollisionShape().Transform(new AbsoluteAngle(_rotation, Angle.Radian), _scale.X, _scale.Y, _location.X,_location.Y);
        }
        public override IPolygon InputShape()
        {
            return _internal.InputShape().Transform(new AbsoluteAngle(_rotation, Angle.Radian), _scale.X, _scale.Y, _location.X,_location.Y);
        }
        public override long collisionflags()
        {
            return _internal.collisionflags();
        }
    }
    public class TextureEntity : Entity
    {
        protected readonly Texture2D _internal;
        public TextureEntity(Texture2D @internal)
        {
            this._internal = @internal;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 location, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float layerdepth)
        {
            spriteBatch.Draw(_internal, location, null, tint, rotation, origin, scale, effect, layerdepth);
        }
    }
    public class InputTexture : TextureEntity
    {
        public InputTexture(Texture2D @internal) : base(@internal) { }
        public override IPolygon InputShape()
        {
            return new Rectangle(_internal.Width,_internal.Height);
        }
    }
    public class CollisionTexture : TextureEntity
    {
        private readonly Int64 _colflags;
        public CollisionTexture(Texture2D @internal, long colflags) : base(@internal)
        {
            this._colflags = colflags;
        }
        public override IPolygon CollisionShape()
        {
            return new Rectangle(_internal.Width, _internal.Height);
        }
        public override long collisionflags()
        {
            return _colflags;
        }
    }
}

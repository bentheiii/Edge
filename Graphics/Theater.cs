using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Edge.Arrays;
using Edge.Comparison;
using Edge.Imaging;
using Edge.Looping;
using Edge.NumbersMagic;
using Edge.Shapes;
using Edge.SystemExtensions;
using Edge.Timer;
using Edge.Units.Angles;
using Edge.Units.Frequencies;
using Edge.Units.Rotationals;

namespace Edge.Plane
{
    public class SpriteTheater : IDisposable
    {
        private readonly LinkedList<Drawable> _sprites = new LinkedList<Drawable>();
        private readonly LinkedList<IAnimation> _animations = new LinkedList<IAnimation>();
        private readonly Bitmap _bitmap;
        private readonly IdleTimer _timer = new IdleTimer();
        public SpriteTheater(int width, int height)
        {
            _bitmap = new Bitmap(width, height);
        }
        private void Render(Point p, LockBitmap lockedBits)
        {
            if (!_bitmap.isWithin(p))
                return;
            Color c = Color.Transparent;
            foreach (Drawable drawable in _sprites)
            {
                if (drawable.CoversPixel(p))
                {
                    c = drawable.getPixel(p);
                    break;
                }
            }
            lockedBits.SetPixel(p,c);
        }
        public void Render()
        {
            Render(Loops.Range(_bitmap.Width).Join(Loops.Range(_bitmap.Height)).Select(a => new Point(a.Item1, a.Item2)));
        }
        public void Render(IEnumerable<Point> p)
        {
            var lockedBits = new LockBitmap(_bitmap);
            lockedBits.LockBits();
            foreach (Point point in p.Distinct())
            {
                Render(point, lockedBits);
            }
            lockedBits.UnlockBits();
        }
        public Image getImage()
        {
            return this._bitmap.Copy();
        }
        public void AdvanceFrame()
        {
            foreach (IAnimation animation in _animations)
            {
                animation.NextFrame(this._timer.timeSinceStart,this);
            }
            _timer.Reset();
        }
        public void AddDrawable(Drawable d)
        {
            _sprites.AddFirst(d);
            d.theater = this;
        }
        public void RemoveDrawable(Drawable d)
        {
            _sprites.Remove(d);
            d.theater = null;
        }
        public void AddAnimation(IAnimation d)
        {
            _animations.AddFirst(d);
        }
        public void RemoveAnimation(IAnimation d)
        {
            _animations.Remove(d);
        }
        protected virtual void Dispose(bool disposing)
        {
            _bitmap.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public interface IAnimation
    {
        void NextFrame(TimeSpan timeSinceLastFrame, SpriteTheater theater);
    }
    public class RotationAnimation : IAnimation
    {
        private RotationSprite _sprite = null;
        private readonly Drawable _drawable;
        private readonly RotationalSpeed _speed;
        private readonly FocusPoint _focus;
        public RotationAnimation(Drawable drawable, RotationalSpeed speed, FocusPoint focus = FocusPoint.Center)
        {
            this._drawable = drawable;
            this._speed = speed;
            this._focus = focus;
        }
        public void NextFrame(TimeSpan timeSinceLastFrame, SpriteTheater theater)
        {
            _drawable.Mutate(a =>
            {
                if (_sprite == null)
                {
                    _sprite = new RotationSprite(a,_focus,new Angle(0, Angle.Degree));
                    a = _sprite;
                }
                _sprite.angle += _speed * timeSinceLastFrame;
                return a;
            });
        }
    }
    public class LocationAnimation : IAnimation
    {
        private LocationSprite _sprite = null;
        private readonly Drawable _drawable;
        private readonly Frequency _x, _y;
        public LocationAnimation(Drawable drawable, Frequency x, Frequency y)
        {
            this._drawable = drawable;
            this._x = x;
            this._y = y;
        }
        public void NextFrame(TimeSpan timeSinceLastFrame, SpriteTheater theater)
        {
            _drawable.Mutate(a =>
            {
                if (_sprite == null)
                {
                    _sprite = new LocationSprite(a,0,0);
                    a = _sprite;
                }
                _sprite.move= _sprite.move.add((int) (this._x*timeSinceLastFrame),(int) (this._y*timeSinceLastFrame));
                return a;
            });
        }
    }
    public class ScaleAnimation : IAnimation
    {
        private ScaleSprite _sprite = null;
        private readonly Drawable _drawable;
        private readonly Frequency _x, _y;
        public ScaleAnimation(Drawable drawable, Frequency x, Frequency y)
        {
            this._drawable = drawable;
            this._x = x;
            this._y = y;
        }
        public void NextFrame(TimeSpan timeSinceLastFrame, SpriteTheater theater)
        {
            _drawable.Mutate(a =>
            {
                if (_sprite == null)
                {
                    _sprite = new ScaleSprite(a, 0, 0);
                    a = _sprite;
                }
                _sprite.ScaleX += this._x * timeSinceLastFrame;
                _sprite.ScaleY += this._y * timeSinceLastFrame;
                return a;
            });
        }
    }
    public class Drawable : ISprite
    {
        public SpriteTheater theater { get; set; } = null;
        public ISprite sprite { get; set; }
        public Drawable(ISprite sprite)
        {
            this.sprite = sprite;
        }
        public override bool CoversPixel(Point p)
        {
            return this.sprite.CoversPixel(p);
        }
        public override Color getPixel(Point p)
        {
            return this.sprite.getPixel(p);
        }
        public override IEnumerable<Point> getReleventpixels()
        {
            return this.sprite.getReleventpixels();
        }
        public void Mutate(Func<ISprite, ISprite> mutation)
        {
            var prevpix = sprite.getReleventpixels();
            sprite = mutation(sprite);
            theater?.Render(prevpix.Concat(sprite.getReleventpixels()));
        }
    }
    public enum FocusPoint
    {
        Zero = 0,
        Center,
        TopLeft,
        BottomRight
    };
    public abstract class ISprite {
        public abstract bool CoversPixel(Point p);
        public abstract Color getPixel(Point p);
        public abstract IEnumerable<Point> getReleventpixels();
        public virtual Point getFocusPoint(FocusPoint f)
        {
            switch (f)
            {
                case FocusPoint.Center:
                    return this.getReleventpixels().Center();
                case FocusPoint.BottomRight:
                    return this.getReleventpixels().getMax(new FunctionComparer<Point>(a => a.X + a.Y));
                case FocusPoint.TopLeft:
                    return this.getReleventpixels().getMin(new FunctionComparer<Point>(a => a.X + a.Y));
                case FocusPoint.Zero:
                    return Point.Empty;
                default:
                    throw new ArgumentException("not recognized focus point");
            }
        }
    }
    public class Imageprite : ISprite, IDisposable
    {
        private readonly Bitmap _i;
        public Imageprite(Image i)
        {
            this._i = new Bitmap(i);
        }
        public override bool CoversPixel(Point p)
        {
            return p.X.iswithinPartialExclusive(0, _i.Width) && p.Y.iswithinPartialExclusive(0, _i.Height);
        }
        public override Color getPixel(Point p)
        {
            return _i.GetPixel(p.X,p.Y);
        }
        public override IEnumerable<Point> getReleventpixels()
        {
            return
                Loops.Range(_i.Width).Join(Loops.Range(_i.Height)).Select(a => new Point(a.Item1, a.Item2));
        }
        public override Point getFocusPoint(FocusPoint f)
        {
            switch (f)
            {
                case FocusPoint.Center:
                    return new Point(_i.Width/2,_i.Height/2);
                case FocusPoint.BottomRight:
                    return new Point(_i.Width, _i.Height);
                case FocusPoint.TopLeft:
                    return Point.Empty;
                default:
                    return base.getFocusPoint(f);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            _i.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class RotationSprite : ISprite
    {
        private readonly ISprite _sprite;
        private readonly Point _rotateAround;
        public Angle angle { get; set; }
        public RotationSprite(ISprite sprite, FocusPoint f, Angle angle) : this(sprite, sprite.getFocusPoint(f), angle) { }
        public RotationSprite(ISprite sprite, Point rotateAround, Angle angle)
        {
            this._sprite = sprite;
            this._rotateAround = rotateAround;
            this.angle = angle;
        }
        public override bool CoversPixel(Point p)
        {
            return _sprite.CoversPixel(p.RotateAround(_rotateAround, -this.angle));
        }
        public override Color getPixel(Point p)
        {
            return _sprite.getPixel(p.RotateAround(_rotateAround, -this.angle));
        }
        public override IEnumerable<Point> getReleventpixels()
        {
            Angle a0 = this.angle;
            Point p0 = _rotateAround;
            return _sprite.getReleventpixels().Select(a => a.RotateAround(p0, a0));
        }
    }
    public class LocationSprite : ISprite
    {
        private readonly ISprite _sprite;
        public Point move { get; set; }
        public LocationSprite(ISprite sprite, int x, int y) : this(sprite, new Point(x, y)) { }
        public LocationSprite(ISprite sprite, Point move)
        {
            this._sprite = sprite;
            this.move = move;
        }
        public override bool CoversPixel(Point p)
        {
            return _sprite.CoversPixel(p.subtract(move));
        }
        public override Color getPixel(Point p)
        {
            return _sprite.getPixel(p.subtract(move));
        }
        public override IEnumerable<Point> getReleventpixels()
        {
            Point m0 = move;
            return _sprite.getReleventpixels().Select(a => a.add(m0));
        }
        public override Point getFocusPoint(FocusPoint f)
        {
            return _sprite.getFocusPoint(f).add(move);
        }
    }
    public class ScaleSprite : ISprite
    {
        private readonly ISprite _sprite;
        private readonly Point _scaleAround;
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public ScaleSprite(ISprite sprite, FocusPoint f, double scale) : this(sprite, f, scale, scale) { }
        public ScaleSprite(ISprite sprite, FocusPoint f, double scaleX, double scaleY) : this(sprite, sprite.getFocusPoint(f), scaleX, scaleY) { }
        public ScaleSprite(ISprite sprite, Point scaleAround, double scaleX, double scaleY)
        {
            this._sprite = sprite;
            this._scaleAround = scaleAround;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
        }
        public override bool CoversPixel(Point p)
        {
            return _sprite.CoversPixel(p.ScaleAround(this._scaleAround, 1.0 / ScaleX, 1.0 / ScaleY));
        }
        public override Color getPixel(Point p)
        {
            return _sprite.getPixel(p.ScaleAround(this._scaleAround, 1.0 / ScaleX, 1.0 / ScaleY));
        }
        public override IEnumerable<Point> getReleventpixels()
        {
            double x0 = ScaleX, y0 = ScaleY;
            var a0 = _scaleAround;
            return
                _sprite.getReleventpixels().Select(a => a.ScaleAround(a0, x0, y0)).Select(
                    a => Loops.Range(x0.ceil()).Join(Loops.Range(y0.ceil())).Select(x => a.add(x.Item1, x.Item2))).Concat();
        }
        public override Point getFocusPoint(FocusPoint f)
        {
            switch (f)
            {
                case FocusPoint.Center:
                    return _sprite.getFocusPoint(FocusPoint.Center).ScaleAround(this._scaleAround, ScaleX, ScaleY);
                case FocusPoint.BottomRight:
                case FocusPoint.TopLeft:
                    if (ScaleX * ScaleY < 0)
                        break;
                    return _sprite.getFocusPoint(f).ScaleAround(this._scaleAround, ScaleX, ScaleY);
            }
            return base.getFocusPoint(f);
        }
    }
}

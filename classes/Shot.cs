using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


public class Shot(Texture2D _sprite, Vector2 _position, float _rotation, float _speed=.25f) {
    public Texture2D sprite    { get; }      = _sprite;
    public Rectangle rectangle { get; }      = _sprite.Bounds;
    public Vector2 origin      { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);

    public Vector2 position    { get; set; } = _position;
    public Vector2 velocity    { get; set; }   = new(
                                                    _speed * MathF.Cos(_rotation - MathF.PI / 2),
                                                    _speed * MathF.Sin(_rotation - MathF.PI / 2)
                                                );
    public float rotation      { get; }      = _rotation;
}

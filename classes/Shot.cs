using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


public class Shot(Texture2D _sprite, Vector2 _position, Vector2 initial_velocity, float _rotation, float _speed=.25f) {
    public Texture2D sprite    { get; }      = _sprite;
    public Rectangle rectangle { get; set; } = new((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
    public Vector2 origin      { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);

    public Vector2 position    { get; set; } = _position;
    public Vector2 velocity    { get; set; }   = new(
                                                    _speed * MathF.Cos(_rotation - MathF.PI / 2) + initial_velocity.X / 1000,
                                                    _speed * MathF.Sin(_rotation - MathF.PI / 2) + initial_velocity.Y / 1000
                                                );
    public float rotation      { get; }      = _rotation;

    public void draw(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            sprite,
            position,
            null,
            Color.White,
            rotation,
            origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
    }

    public void move(GameTime gameTime) {
        position += velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        rectangle = new((int)position.X, (int)position.Y, (int)(sprite.Width * 1.5f), (int)(sprite.Height * 1.5f));
    }
}

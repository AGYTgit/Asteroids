using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Asteroid(Texture2D _sprite, Vector2 _position, Vector2 _direction, float _rotation, float _speed) {
    public Texture2D sprite    { get; }      = _sprite;
    public Vector2 origin      { get; }      = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
    public Rectangle rectangel { get; }      = _sprite.Bounds;
    public Vector2 position    { get; set; } = _position;
    public float rotation      { get; set; } = _rotation;

    public Vector2 direction   { get; }      = _direction;
    public float speed         { get; }      = _speed;
}


public class Asteroid_Spawner(Texture2D sprite, Vector2 target) {
    public List<Asteroid> asteroid_list { get; set; } = [];

    private void spawn() {
        Random random = new Random();
        Texture2D sprite_temp = sprite;
        float ran_w_temp = (float)random.NextDouble();
        float ran_h_temp = (float)random.NextDouble();
        if (random.Next(2) == 0) {
            ran_w_temp = (int)Math.Round(ran_w_temp);
        } else {
            ran_h_temp = (int)Math.Round(ran_h_temp);
        }
        Vector2 position_temp = new Vector2(
            // GraphicsDevice.Viewport.Width * ran_w_temp + (sprite_temp.Width * ((float)Math.Round(ran_w_temp) * 2 - 1)) + 25 * (random.Next(2) * 2 - 1),
            // GraphicsDevice.Viewport.Height * ran_h_temp + (sprite_temp.Height * ((float)Math.Round(ran_h_temp) * 2 - 1)) + 25 * (random.Next(2) * 2 - 1)
            0,0
        );

        Vector2 direction = target - position_temp;
        float mag = (float)Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Y, 2));
        Vector2 norm_direction = new Vector2(direction.X / mag, direction.Y / mag);

        Asteroid asteroid = new(
            sprite_temp,
            position_temp,
            norm_direction,

            (float)random.NextDouble() * MathF.PI * 2,
            (float)random.NextDouble() * 50f
        );
        asteroid_list.Add(asteroid);
    }
}


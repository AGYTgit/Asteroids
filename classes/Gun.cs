using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;


public class Gun(Texture2D _sprite, Vector2 _position, Vector2 _position_offset, Cross _cross, int _ammo=50, float _firerate=250) {
    public Texture2D sprite        { get; }      = _sprite;
    public Vector2 origin          { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);

    public Vector2 position        { get; set; } = _position;
    public Vector2 position_offset { get; }      = _position_offset;
    public float rotation          { get; set; } = 0f;

    public int ammo                { get; set; } = _ammo;
    public float firerate          { get; }      = _firerate;
    public long last_time_fired    { get; set; } = 0;

    public Cross cross             { get; }      = _cross;

    public List<Shot> shots        { get; set; } = [];


    public void move(Vector2 velocity) {
        position += velocity;
        cross.position += velocity;
    }

    public void move_cross(Vector2 velocity) {
        position += velocity;
        cross.position += velocity;
    }

    public void shoot(Texture2D sprite, GameTime gameTime, MouseState mstate) {
        if (gameTime.TotalGameTime.TotalMilliseconds > (last_time_fired + (60000 / firerate))) {
            Shot shot = new(sprite, position, rotation);
            shots.Add(shot);
            last_time_fired = (long)gameTime.TotalGameTime.TotalMilliseconds;
        }
    }

    public void move_shots(GameTime gameTime, Vector2 velocity) {
        foreach (Shot shot in shots) {
            shot.position += shot.velocity * gameTime.ElapsedGameTime.Milliseconds + velocity / 500;
        }
    }
};
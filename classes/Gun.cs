using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;


public class Gun(Texture2D _sprite, Vector2 _position, Vector2 _position_offset, Cross _cross, int _ammo=5, float _firerate=250) {
    public Texture2D sprite         { get; }      = _sprite;
    public Vector2 origin           { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);

    public Vector2 position         { get; set; } = _position;
    public Vector2 position_offset  { get; }      = _position_offset;
    public float rotation           { get; set; } = 0f;

    public Vector2 initial_velocity { get; set; } = Vector2.Zero;

    public int ammo                 { get; set; } = _ammo;
    public float firerate           { get; }      = _firerate;
    public long last_time_fired     { get; set; } = 0;

    public Cross cross              { get; }      = _cross;

    public List<Shot> shots         { get; set; } = [];


    public void move(Vector2 velocity) {
        position += velocity;
        cross.position += velocity;
    }

    public void move_cross(Vector2 velocity) {
        position += velocity;
        cross.position += velocity;
    }

    public void shoot(Texture2D sprite, GameTime gameTime, Vector2 initial_velocity) {
        if (gameTime.TotalGameTime.TotalMilliseconds > (last_time_fired + (60000 / firerate))) {
            Shot shot = new(sprite, position, rotation);
            shots.Add(shot);
            last_time_fired = (long)gameTime.TotalGameTime.TotalMilliseconds;

            ammo--;
        }
    }

    public void move_shots(GameTime gameTime, Vector2 velocity) {
        foreach (Shot shot in shots) {
            shot.position += shot.velocity * gameTime.ElapsedGameTime.Milliseconds + initial_velocity / 200;
        }
    }

    public void draw(SpriteBatch sprite_batch) {
        draw_shots(sprite_batch);

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

    public void draw_shots(SpriteBatch sprite_batch) {
        foreach (Shot shot in shots) {
            shot.draw(sprite_batch);
        }
    }
};
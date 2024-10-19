using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;
using System.Linq;


public class Gun(Texture2D _sprite, Texture2D _shot_sprite, Vector2 _position, Vector2 _position_offset, Cross _cross, int _ammo=15, float _firerate=250, long _reload_delay=2000) {
    public Texture2D sprite         { get; }      = _sprite;
    public Texture2D shot_sprite    { get; }      = _shot_sprite;
    public Vector2 origin           { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);

    public Vector2 position         { get; set; } = _position;
    public Vector2 position_offset  { get; }      = _position_offset;
    public float rotation           { get; set; } = 0f;

    public Vector2 initial_velocity { get; set; } = Vector2.Zero;

    public int max_ammo             { get; }      = _ammo;
    public int ammo                 { get; set; } = _ammo;
    public float firerate           { get; }      = _firerate;
    public long last_time_fired     { get; set; } = 0;
    public long reload_delay         { get; }      = _reload_delay;

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

    public void shoot(GameTime gameTime, Vector2 initial_velocity) {
        if (ammo > 0) {
            if (gameTime.TotalGameTime.TotalMilliseconds > (last_time_fired + (60000 / firerate))) {
                Shot shot = new(shot_sprite, position, initial_velocity, rotation);
                shots.Add(shot);
                last_time_fired = (long)gameTime.TotalGameTime.TotalMilliseconds;

                ammo--;
            }
        }
    }

    public void reload(GameTime gameTime) {
        if (ammo == max_ammo) {
            return;
        }
        if (gameTime.TotalGameTime.TotalMilliseconds > last_time_fired + reload_delay) {
            ammo = max_ammo;
        }
    }

    public void move_shots(GameTime gameTime) {
        foreach (Shot shot in shots) {
            shot.move(gameTime);
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

    public bool check_for_collision(Rectangle rect) {
        for (int i = shots.Count - 1; i >= 0; i--) {
            if (rect.Intersects(shots[i].rectangle)) {
                shots.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void check_for_shot_to_shot_collision() {
        HashSet<int> hash_set = [];
        for (int i = shots.Count - 1; i >= 0; i--) {
            for (int j = i - 1; j >= 0; j--) {
                if (shots[i].rectangle.Intersects(shots[j].rectangle)) {
                    hash_set.Add(i);
                    hash_set.Add(j);
                    break;
                }
            }
        }

        List<int> list = hash_set.ToList();
        list.Sort((a, b) => b.CompareTo(a));

        foreach(int i in list) {
            shots.RemoveAt(i);
        }
    }
};
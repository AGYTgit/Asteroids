using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

public class Asteroid(Texture2D _sprite, Vector2 _position, Vector2 _velocity, float _rotation, float _rotation_speed) {
    public Texture2D sprite     { get; }      = _sprite;
    public Vector2 origin       { get; }      = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
    public Rectangle rectangle  { get; set; } = new((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);

    public Vector2 position     { get; set; } = _position;
    public float rotation       { get; set; } = _rotation;

    public Vector2 velocity     { get; }      = _velocity;
    public float rotation_speed { get; }      = _rotation_speed;

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
}


public class Asteroid_Spawner(Viewport _viewport, Texture2D _asteroid_sprite, float _bottom_speed=25f, float _top_speed=100f, float _top_rotation_speed=MathF.PI, float _angle_randomness=MathF.PI/6, int _spawn_delay=100) {
    public Viewport viewport            { get; }      = _viewport;
    public Texture2D asteroid_sprite    { get; }      = _asteroid_sprite;

    public float bottom_speed           { get; }      = _bottom_speed;
    public float top_speed              { get; }      = _top_speed;
    public float top_rotation_speed     { get; }      = _top_rotation_speed;
    public float angle_randomness       { get; }      = _angle_randomness;

    public float spawn_delay            { get; set; } = _spawn_delay;
    public long last_spawn              { get; set; } = 0;

    public List<Asteroid> asteroid_list { get; set; } = [];


    public void spawn(GameTime gameTime, Vector2 target) {
        if (gameTime.TotalGameTime.TotalMilliseconds < last_spawn + spawn_delay / (gameTime.TotalGameTime.Seconds + 60) * 60) {
            return;
        }

        last_spawn = (long)gameTime.TotalGameTime.TotalMilliseconds;

        Random random = new Random();

        float ran_w = (float)random.NextDouble();
        float ran_h = (float)random.NextDouble();

        if (random.Next(2) == 0) {
            ran_w = (int)Math.Round(ran_w);
        } else {
            ran_h = (int)Math.Round(ran_h);
        }

        Vector2 asteroid_position = new Vector2(
            viewport.Width * ran_w + (asteroid_sprite.Width * ((float)Math.Round(ran_w) * 2 - 1)) + 25 * (random.Next(2) * 2 - 1),
            viewport.Height * ran_h + (asteroid_sprite.Height * ((float)Math.Round(ran_h) * 2 - 1)) + 25 * (random.Next(2) * 2 - 1)
        );

        Vector2 direction = target - asteroid_position;
        float angle_randomness_temp = angle_randomness;
        int ran = random.Next(3);
        if (ran == 0) {
            direction = new Vector2(
                asteroid_position.X + 100 * (ran_w == 0 ? 1 : -1),
                asteroid_position.Y + 100 * (ran_h == 0 ? 1 : -1)
            ) - asteroid_position;
        } else if (ran == 1) {
            angle_randomness_temp = 0;
        }

        float angle_diff = MathF.Atan2(direction.Y, direction.X);
        float randomized_angle_diff = angle_diff + ((float)random.NextDouble() * angle_randomness_temp * (random.Next(2) * 2 - 1));

        float speed = (float)random.NextDouble() * random.Next((int)(bottom_speed*1000), (int)(top_speed*1000)) / 1000;
        Vector2 velocity = new(
            speed * MathF.Cos(randomized_angle_diff),
            speed * MathF.Sin(randomized_angle_diff)
        );

        Asteroid asteroid = new(
            asteroid_sprite,

            asteroid_position,
            velocity,

            (float)random.NextDouble() * top_rotation_speed,
            (float)random.NextDouble() * top_rotation_speed * (random.Next(2) * 2 - 1)
        );
        asteroid_list.Add(asteroid);
    }

    public void update_asteroids(GameTime gameTime) {
        foreach (Asteroid asteroid in asteroid_list) {
            asteroid.position += asteroid.velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            asteroid.rotation += asteroid.rotation_speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            asteroid.rectangle = new((int)asteroid.position.X, (int)asteroid.position.Y, (int)(asteroid.sprite.Width * .5f), (int)(asteroid.sprite.Height * .5f));
        }
    }

    public void draw(SpriteBatch sprite_batch) {
        foreach (Asteroid asteroid in asteroid_list) {
            asteroid.draw(sprite_batch);
        }
    }

    public bool check_for_collision(Rectangle rect) {
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            if (rect.Intersects(asteroid_list[i].rectangle)) {
                asteroid_list.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void check_for_asteroid_to_asteroid_collision() {
        HashSet<int> hash_set = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            for (int j = i - 1; j >= 0; j--) {
                if (asteroid_list[i].rectangle.Intersects(asteroid_list[j].rectangle)) {
                    hash_set.Add(i);
                    hash_set.Add(j);
                    break;
                }
            }
        }

        List<int> list = hash_set.ToList();
        list.Sort((a, b) => b.CompareTo(a));

        foreach(int i in list) {
            asteroid_list.RemoveAt(i);
        }
    }
}

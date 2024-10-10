using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;

namespace Asteroids;

public class Asteroids : Game {   
    public Asteroids() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public class Entity {
        public Texture2D sprite;
        public Vector2 origin;
        public Vector2 position;
        public Vector2 direction;

        public float speed;
        public float speed_multiplier;
        public Vector2 velocity;

        public float secondary_speed;
        public float secondary_speed_multiplier;
        public Vector2 secondary_velocity;

        public float rotation;
        public float rotate_speed;
        public float rotate_speed_multiplier;

        public int health;

        public List<int> collision_list;
    }
    readonly Entity spaceship = new();

    readonly List<Entity> shots_list = [];

    readonly List<Entity> asteroid_list = [];

    protected override void Initialize() {
        init_spaceship();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    float shoot_cooldown_ms = 0f;
    float time_of_last_shoot = 0f;

    float spawn_cooldown_ms = 0f;
    float time_of_last_spawn = 0f;

    float despawn_cooldown_ms = 0f;
    float time_of_last_despawn = 0f;

long game_time_ms = 0;
    protected override void Update(GameTime gameTime) {
        game_time_ms += gameTime.ElapsedGameTime.Milliseconds;

        var kstate = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kstate.IsKeyDown(Keys.Escape)) {
            Exit();
        }

        spaceship.velocity = new Vector2(
            spaceship.speed * spaceship.speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(spaceship.rotation),
            -1 * spaceship.speed * spaceship.speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(spaceship.rotation)
        );
        spaceship.secondary_velocity = new Vector2(
            spaceship.secondary_speed * spaceship.secondary_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(spaceship.rotation),
            spaceship.secondary_speed * spaceship.secondary_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(spaceship.rotation)
        );
        
        if (kstate.IsKeyDown(Keys.E)) {
            spaceship.position += spaceship.velocity;
        }
        
        if (kstate.IsKeyDown(Keys.S)) {
            spaceship.position -= spaceship.secondary_velocity;
        }
        
        if (kstate.IsKeyDown(Keys.F)) {
            spaceship.position += spaceship.secondary_velocity;
        }

        if (kstate.IsKeyDown(Keys.R)) {
            spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
        }

        if (kstate.IsKeyDown(Keys.Space)) {
            spaceship.speed_multiplier = 8f;
            spaceship.secondary_speed_multiplier = 4f;
            spaceship.rotate_speed_multiplier = 4f;
        } else {
            spaceship.speed_multiplier = 1f;
            spaceship.secondary_speed_multiplier = 1f;
            spaceship.rotate_speed_multiplier = 1f;
        }

        if (kstate.IsKeyDown(Keys.G) && time_of_last_shoot < game_time_ms - shoot_cooldown_ms) {
            time_of_last_shoot = game_time_ms;
            shoot();
        }

        
        for (int i = 0; i < shots_list.Count; i++) {
            shots_list[i].velocity = new Vector2(
                shots_list[i].speed * shots_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(shots_list[i].rotation),
                -1 * shots_list[i].speed * shots_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(shots_list[i].rotation)
            );

            shots_list[i].position += shots_list[i].velocity;
        }

        List<int> shots_to_remove = [];
        List<int> asteroids_to_remove = [];
        for (int i = shots_list.Count - 1; i >= 0; i--) {
            if (
                shots_list[i].position.X < 0 || shots_list[i].position.X > _graphics.PreferredBackBufferWidth ||
                shots_list[i].position.Y < 0 || shots_list[i].position.Y > _graphics.PreferredBackBufferHeight
                ) {
                shots_to_remove.Add(i);
                continue;
            }
            for (int j = asteroid_list.Count - 1; j >= 0; j--) {
                if (is_collision(shots_list[i], asteroid_list[j])) {
                    shots_to_remove.Add(i);
                    asteroids_to_remove.Add(j);
                    break;
                }
            }
        }

        shots_to_remove.Sort((a, b) => b.CompareTo(a));
        foreach (int index in shots_to_remove) {
            if (index < shots_list.Count) {
                shots_list.RemoveAt(index);
            }
        }

        asteroids_to_remove.Sort((a, b) => b.CompareTo(a));
        foreach (int index in asteroids_to_remove) {
            if (index < asteroid_list.Count) {
                asteroid_list.RemoveAt(index);
            }
        }


        if (kstate.IsKeyDown(Keys.W) && time_of_last_spawn < game_time_ms - spawn_cooldown_ms) {
            time_of_last_spawn = game_time_ms;
            create_asteroid();
        }

        if (kstate.IsKeyDown(Keys.Q) && time_of_last_despawn < game_time_ms - despawn_cooldown_ms) {
            time_of_last_despawn = game_time_ms;
            destroy_asteroid();
        }


        asteroids_to_remove = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            asteroid_list[i].velocity = new Vector2(
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.X,
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.Y
            );
            asteroid_list[i].position += asteroid_list[i].velocity;
            asteroid_list[i].rotation += (float)(asteroid_list[i].rotate_speed * asteroid_list[i].rotate_speed_multiplier / 360 * Math.PI * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (is_collision(spaceship, asteroid_list[i])) {
                // spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
                asteroids_to_remove.Add(i);
                continue;
            } else if (
                asteroid_list[i].position.X < 0 || asteroid_list[i].position.X > _graphics.PreferredBackBufferWidth ||
                asteroid_list[i].position.Y < 0 || asteroid_list[i].position.Y > _graphics.PreferredBackBufferHeight
                ) {
                asteroids_to_remove.Add(i);
                continue;
            }
            // for (int j = i - 1; j >= 0; j--) {
            //     if (is_collision(asteroid_list[i], asteroid_list[j])) {
            //         if (asteroid_list[i].speed > asteroid_list[j].speed) {
            //             asteroids_to_remove.Add(j);
            //             break;
            //         } else if (asteroid_list[i].speed < asteroid_list[j].speed) {
            //             asteroids_to_remove.Add(i);
            //             break;
            //         }
                    
            //         asteroids_to_remove.Add(j);
            //         asteroids_to_remove.Add(i);
            //         break;
            //     }
            // }
        }

        asteroids_to_remove.Sort((a, b) => b.CompareTo(a));
        foreach (int index in asteroids_to_remove) {
            if (index < asteroid_list.Count) {
                asteroid_list.RemoveAt(index);
            }
        }



        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

        Vector2 direction = mouse_position - spaceship.position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        spaceship.rotation = (spaceship.rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle + (float)(Math.PI / 2) - spaceship.rotation;


        if (angle_diff > MathF.PI) {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI) {
            angle_diff += 2 * MathF.PI;
        }


        float rotate_amount = spaceship.rotate_speed * spaceship.rotate_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (angle_diff > 0) {
            if (angle_diff < rotate_amount) {
                spaceship.rotation += angle_diff;
            } else {
                spaceship.rotation += rotate_amount;
            }
        } else if (angle_diff < 0) {
            if (0 - angle_diff < rotate_amount) {
                spaceship.rotation += angle_diff;
            } else {
                spaceship.rotation -= rotate_amount;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _spriteBatch.Draw(
            spaceship.sprite,
            spaceship.position,
            null,
            Color.White,
            spaceship.rotation,
            new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2),
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        foreach (Entity asteroid in asteroid_list) {
            _spriteBatch.Draw(
                asteroid.sprite,
                asteroid.position,
                null,
                Color.White,
                asteroid.rotation,
                new Vector2(asteroid.sprite.Width / 2, asteroid.sprite.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        foreach (Entity shot in shots_list) {
            _spriteBatch.Draw(
                shot.sprite,
                shot.position,
                null,
                Color.White,
                shot.rotation,
                new Vector2(shot.sprite.Width / 2, shot.sprite.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void init_spaceship() {
        spaceship.sprite = Content.Load<Texture2D>("spaceship");
        spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        spaceship.origin = new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);

        spaceship.speed = 100f;
        spaceship.speed_multiplier = 1f;
        spaceship.velocity = new Vector2(0,0);

        spaceship.secondary_speed = 50f;
        spaceship.secondary_speed_multiplier = 1f;
        spaceship.secondary_velocity = new Vector2(0,0);
        
        spaceship.rotation = 0f;
        spaceship.rotate_speed = 5f;
        spaceship.rotate_speed_multiplier = 1f;

        spaceship.collision_list = [1];
    }

    private void create_asteroid() {
        Random random = new Random();
        Texture2D sprite_temp = Content.Load<Texture2D>("asteroid_1_1");
        float direction_X_temp = random.Next(2000) / 1000 * 2 - 1;
        float direction_Y_temp = (1 - direction_X_temp) * random.Next(2) * 2 - 1;
        Entity asteroid = new() {
            sprite = sprite_temp,
            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
            position = new Vector2(
                random.Next(0, GraphicsDevice.Viewport.Width),
                random.Next(0, GraphicsDevice.Viewport.Height)
            ),
            direction = new Vector2(
                direction_X_temp,
                direction_Y_temp
            ),

            speed = random.Next(1,21),
            speed_multiplier = 1f,
            velocity = new Vector2(0,0),

            rotation = random.Next(0, (int)(Math.PI * 2 * 1000)) / 1000,
            rotate_speed = random.Next(60) * 2 - 60,
            rotate_speed_multiplier = 1f,

            collision_list = [1,2],
        };
        asteroid_list.Add(asteroid);
    }

    private void destroy_asteroid() {
        Random random = new();
        if (asteroid_list.Count > 0) {
            asteroid_list.RemoveAt(random.Next(0, asteroid_list.Count - 1));
        }
    }

    private static bool is_collision(Entity e1, Entity e2, float tolerance=10f) {
        // foreach (int c in e1.collision_list) {
        //     if (e2.collision_list.Contains(c)) {
                float distance = Vector2.Distance(e1.position, e2.position);
                float e1_radius = e1.sprite.Width / 2 * (1 - tolerance / 100);
                float e2_radius = e2.sprite.Width / 2 * (1 - tolerance / 100);;
                return distance < (e1_radius + e2_radius);
        //     }
        // }
        // return false;
    }

    private void shoot() {
        Vector2 shot_position_offset = new(
            -13f * (float)Math.Cos(spaceship.rotation),
            -10f * (float)Math.Sin(spaceship.rotation)
        );;

        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
        Vector2 direction = mouse_position - spaceship.position + shot_position_offset;
        float target_angle = MathF.Atan2(direction.Y, direction.X);
        float angle_diff = target_angle + (float)(Math.PI / 2) - spaceship.rotation;

        Texture2D sprite_temp = Content.Load<Texture2D>("shot_1_1");
        Entity shot = new() {
            sprite = sprite_temp,
            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
            position = spaceship.position + shot_position_offset,

            speed = 250,
            speed_multiplier = 1f,
            velocity = new Vector2(0,0),

            rotation = spaceship.rotation - angle_diff,
            rotate_speed = 0f,
            rotate_speed_multiplier = 0f,

            collision_list = [2],
        };
        shots_list.Add(shot);
        Entity shot2 = new() {
            sprite = sprite_temp,
            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
            position = spaceship.position - shot_position_offset,

            speed = 250,
            speed_multiplier = 1f,
            velocity = new Vector2(0,0),

            rotation = spaceship.rotation + angle_diff,
            rotate_speed = 0f,
            rotate_speed_multiplier = 0f,

            collision_list = [2],
        };
        shots_list.Add(shot2);
    }
}

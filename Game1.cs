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

        public int collision_layer;
    }
    readonly Entity spaceship = new();

    readonly List<Entity> asteroid_list = [];

    protected override void Initialize() {
        init_spaceship();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    bool w_down = false;
    bool q_down = false;

    protected override void Update(GameTime gameTime) {
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



        if (kstate.IsKeyDown(Keys.W) && w_down == false) {
            // w_down = true;
            create_asteroid();
        } else if (!kstate.IsKeyDown(Keys.W) && w_down == true) {
            w_down = false;
        }

        if (kstate.IsKeyDown(Keys.Q) && q_down == false) {
            // q_down = true;
            destroy_asteroid();
        } else if (!kstate.IsKeyDown(Keys.Q) && q_down == true) {
            q_down = false;
        }



        List<int> to_remove = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            asteroid_list[i].velocity = new Vector2(
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.X,
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.Y
            );
            asteroid_list[i].position += asteroid_list[i].velocity;
            asteroid_list[i].rotation += (float)(asteroid_list[i].rotate_speed * asteroid_list[i].rotate_speed_multiplier / 360 * Math.PI * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (is_collision(spaceship, asteroid_list[i])) {
                // spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
                to_remove.Add(i);
                continue;
            } else if (
                asteroid_list[i].position.X < 0 || asteroid_list[i].position.X > _graphics.PreferredBackBufferWidth ||
                asteroid_list[i].position.Y < 0 || asteroid_list[i].position.Y > _graphics.PreferredBackBufferHeight
                ) {
                to_remove.Add(i);
                continue;
            }
            for (int j = i - 1; j >= 0; j--) {
                if (is_collision(asteroid_list[i], asteroid_list[j])) {
                    if (asteroid_list[i].speed > asteroid_list[j].speed) {
                        to_remove.Add(j);
                        break;
                    } else if (asteroid_list[i].speed < asteroid_list[j].speed) {
                        to_remove.Add(i);
                        break;
                    }
                    
                    to_remove.Add(j);
                    to_remove.Add(i);
                    break;
                }
            }
        }

        foreach (int index in to_remove) {
            asteroid_list.RemoveAt(index);
        }



        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

        Vector2 direction = mouse_position - spaceship.position;
        float target_angle_r = MathF.Atan2(direction.Y, direction.X);

        spaceship.rotation = (spaceship.rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle_r + (float)(Math.PI / 2) - spaceship.rotation;


        if (angle_diff > MathF.PI) {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI) {
            angle_diff += 2 * MathF.PI;
        }


        if (angle_diff > 0) {
            spaceship.rotation += spaceship.rotate_speed * spaceship.rotate_speed_multiplier;
        } else if (angle_diff < 0) {
            spaceship.rotation -= spaceship.rotate_speed * spaceship.rotate_speed_multiplier;
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
        spaceship.rotate_speed = .02f;
        spaceship.rotate_speed_multiplier = 1f;
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
            rotate_speed_multiplier = 1f
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
        float distance = Vector2.Distance(e1.position, e2.position);
        float e1_radius = e1.sprite.Width / 2 * (1 - tolerance / 100);
        float e2_radius = e2.sprite.Width / 2 * (1 - tolerance / 100);;
        return distance < (e1_radius + e2_radius);
    }

    private static void shoot() {

    }
}

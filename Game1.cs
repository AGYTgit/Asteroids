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

    public class Spaceship {
        public Texture2D sprite;
        public Vector2 position;
        public Vector2 origin;

        public float speed_forward;
        public float speed_forward_multiplier;
        public float velocity_forward;

        public float speed_other;
        public float speed_other_multiplier;
        public float velocity_other;

        public float rotation;
        public float rotate_speed;
        public float rotate_speed_multiplier;
    }
    readonly Spaceship spaceship = new();

    public class Asteroid {
        public Texture2D sprite;
        public Vector2 position;
        public Vector2 origin;

        public Vector2 direction;
        public float speed;
        public float speed_multiplier;
        public float velocity;

        public float rotation;
        public float rotate_speed;
        public float rotate_speed_multiplier;
    }
    readonly List<Asteroid> asteroid_list = [];

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        spaceship.velocity_forward = spaceship.speed_forward * spaceship.speed_forward_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
        spaceship.velocity_other = spaceship.speed_other * spaceship.speed_other_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.E)) {
            spaceship.position.Y -= spaceship.velocity_forward * (float)Math.Cos(spaceship.rotation);
            spaceship.position.X += spaceship.velocity_forward * (float)Math.Sin(spaceship.rotation);
        }
        
        if (kstate.IsKeyDown(Keys.S)) {
            spaceship.position.Y -= spaceship.velocity_other * (float)Math.Sin(spaceship.rotation);
            spaceship.position.X -= spaceship.velocity_other * (float)Math.Cos(spaceship.rotation);
        }
        
        if (kstate.IsKeyDown(Keys.F)) {
            spaceship.position.Y += spaceship.velocity_other * (float)Math.Sin(spaceship.rotation);
            spaceship.position.X += spaceship.velocity_other * (float)Math.Cos(spaceship.rotation);
        }

        if (kstate.IsKeyDown(Keys.R)) {
            spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
        }

        if (kstate.IsKeyDown(Keys.W) && w_down == false) {
            w_down = true;
            create_asteroid();
        } else if (!kstate.IsKeyDown(Keys.W) && w_down == true) {
            w_down = false;
        }

        if (kstate.IsKeyDown(Keys.Q) && q_down == false) {
            q_down = true;
            destroy_asteroid();
        } else if (!kstate.IsKeyDown(Keys.Q) && q_down == true) {
            q_down = false;
        }

        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            asteroid_list[i].velocity = asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            asteroid_list[i].position += new Vector2(asteroid_list[i].velocity * asteroid_list[i].direction.X, asteroid_list[i].velocity * asteroid_list[i].direction.Y);

            if (is_collision(spaceship, asteroid_list[i])) {
                spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
                asteroid_list.RemoveAt(i);
            }

            if (
                asteroid_list[i].position.X < 0 ||
                asteroid_list[i].position.X > _graphics.PreferredBackBufferWidth ||
                asteroid_list[i].position.Y < 0 ||
                asteroid_list[i].position.Y > _graphics.PreferredBackBufferHeight
                ) {
                asteroid_list.RemoveAt(i);
            }
        }

        if (kstate.IsKeyDown(Keys.Space)) {
            spaceship.speed_forward_multiplier = 8f;
            spaceship.speed_other_multiplier = 4f;
            spaceship.rotate_speed_multiplier = 4f;
        } else {
            spaceship.speed_forward_multiplier = 1f;
            spaceship.speed_other_multiplier = 1f;
            spaceship.rotate_speed_multiplier = 1f;
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
        foreach (Asteroid asteroid in asteroid_list) {
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

        spaceship.speed_forward = 100f;
        spaceship.speed_forward_multiplier = 1f;
        spaceship.velocity_forward = 0f;

        spaceship.speed_other = 50f;
        spaceship.speed_other_multiplier = 1f;
        spaceship.velocity_other = 0f;
        
        spaceship.rotation = 0f;
        spaceship.rotate_speed = .02f;
        spaceship.rotate_speed_multiplier = 1f;
    }

    private void create_asteroid() {
        Random random = new Random();
        Texture2D sprite_temp = Content.Load<Texture2D>("asteroid_1_1");
        Asteroid asteroid = new() {
            sprite = sprite_temp,

            position = new Vector2(
                random.Next(0, GraphicsDevice.Viewport.Width),
                random.Next(0, GraphicsDevice.Viewport.Height)
            ),

            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),

            direction = new Vector2(random.Next(-1,2), random.Next(-1,2)),
            speed = random.Next(10,51),
            speed_multiplier = 1f,
            velocity = 0f,

            rotation = 0,
            rotate_speed = .015f,
            rotate_speed_multiplier = 1f
        };
        asteroid_list.Add(asteroid);
    }

    private void destroy_asteroid() {
        if (asteroid_list.Count > 0) {
            asteroid_list.RemoveAt(asteroid_list.Count - 1);
        }
    }

    private static bool is_collision(Spaceship spaceship, Asteroid asteroid) {
        float distance = Vector2.Distance(spaceship.position, asteroid.position);
        float spaceship_radius = spaceship.sprite.Width / 2;
        float asteroid_radius = asteroid.sprite.Width / 2;
        return distance < (spaceship_radius + asteroid_radius);
    }
}

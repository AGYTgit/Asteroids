using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;

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

        public float speed;
        public float speed_multiplier;
        public float velocity;

        public float rotation;
        public float rotate_speed;
        public float rotate_speed_multiplier;
        public bool exists;
    }
    readonly Asteroid asteroid = new();

    protected override void Initialize() {
        spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        spaceship.rotation = 0f;
        spaceship.speed_forward = 100f;
        spaceship.speed_forward_multiplier = 1f;
        spaceship.speed_other = 50f;
        spaceship.speed_other_multiplier = 1f;
        spaceship.rotate_speed = .02f;
        spaceship.rotate_speed_multiplier = 1f;

        asteroid.position = new Vector2(0,0);
        asteroid.rotation = 0;
        asteroid.speed = 50f;
        asteroid.speed_multiplier = 1f;
        asteroid.rotate_speed = .015f;
        asteroid.rotate_speed_multiplier = 1f;

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        spaceship.sprite = Content.Load<Texture2D>("spaceship");
        asteroid.sprite = Content.Load<Texture2D>("asteroid_1_1");
    }

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

        if (kstate.IsKeyDown(Keys.W)) {
            asteroid.exists = true;
            asteroid.position = new Vector2(0,0);
        }
        if (kstate.IsKeyDown(Keys.Q)) {
            asteroid.exists = false;
        }
        if (asteroid.exists) {
            asteroid.velocity = asteroid.speed * asteroid.speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            asteroid.position += new Vector2(asteroid.velocity, asteroid.velocity);
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
        if (asteroid.exists) {
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
}

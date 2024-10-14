using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public float rotation;
    }

    public class Spaceship : Entity {
        public float speed;
        public int rotation_speed;

        public int health;

        public Gun gun_left;
        public Gun gun_right;
    };
    public class Gun : Entity {
        public Vector2 position_offset;

        public int ammo;
        public float firerate;
        public int rotation_speed;
    };
    public class Asteroid : Entity;

    Spaceship ss;

    protected override void Initialize() {
        Texture2D ss_sprite_temp = Content.Load<Texture2D>("spaceship");
        Vector2 ss_origin_temp = new(ss_sprite_temp.Width / 2, ss_sprite_temp.Height / 2);
        Vector2 ss_position_temp = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        float ss_rotation_temp = 0f;
        

        Texture2D gl_sprite_temp = Content.Load<Texture2D>("railgun_left");
        Gun gl = new() {
            sprite = gl_sprite_temp,
            origin = new Vector2(gl_sprite_temp.Width / 2, gl_sprite_temp.Height / 2),

            position = ss_position_temp,
            position_offset = new Vector2( // y axis not working
                -11f * (float)Math.Cos(ss_rotation_temp),
                -2f * (float)Math.Sin(ss_rotation_temp)
            ),

            rotation_speed = 180,

            ammo = 50,
            firerate = 5f,
        };

        Texture2D gr_sprite_temp = Content.Load<Texture2D>("railgun_right");
        Gun gr = new() {
            sprite = gr_sprite_temp,
            origin = new Vector2(gr_sprite_temp.Width / 2, gr_sprite_temp.Height / 2),

            position = ss_position_temp,
            position_offset = new Vector2( // y axis not working
                10f * (float)Math.Cos(ss_rotation_temp),
                -2f * (float)Math.Sin(ss_rotation_temp)
            ),

            rotation_speed = 180,

            ammo = 50,
            firerate = 5f,
        };

        ss = new() {
            sprite = ss_sprite_temp,
            origin = ss_origin_temp,

            position = ss_position_temp,

            speed = 75f,
            rotation_speed = 90,

            health = 10,

            gun_left = gl,
            gun_right = gr,
        };

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        KeyboardState kstate = Keyboard.GetState();
        MouseState mstate = Mouse.GetState();

        if (kstate.IsKeyDown(Keys.Escape)) {
            Exit();
        }

        update_ss(gameTime, kstate, mstate);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        // if (true) {
            GraphicsDevice.Clear(Color.Black);
        // } else {
        //     GraphicsDevice.Clear(Color.Green);
        // }

        _spriteBatch.Begin();
        _spriteBatch.Draw(
            ss.sprite,
            ss.position,
            null,
            Color.White,
            ss.rotation,
            ss.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.Draw(
            ss.gun_left.sprite,
            ss.gun_left.position + ss.gun_left.position_offset,
            null,
            Color.White,
            ss.gun_left.rotation,
            ss.gun_left.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.Draw(
            ss.gun_right.sprite,
            ss.gun_right.position + ss.gun_right.position_offset,
            null,
            Color.White,
            ss.gun_right.rotation,
            ss.gun_right.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.End();

        base.Draw(gameTime);
    }


    public void update_ss(GameTime gameTime, KeyboardState kstate, MouseState mstate) {
        Vector2 velocity = Vector2.Zero;
        
        float s = ss.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(ss.rotation);
        float c = ss.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(ss.rotation);

        if (kstate.IsKeyDown(Keys.E)) { // move FORWARD
            velocity += new Vector2(s, c * -1);
        }
        if (kstate.IsKeyDown(Keys.S) && !kstate.IsKeyDown(Keys.F)) { // move LEFT
            velocity += new Vector2(c / 2 * -1, s);
        } else if (kstate.IsKeyDown(Keys.F) && !kstate.IsKeyDown(Keys.S)) { // move RIGHT
            velocity += new Vector2(c / 2, s);
        }

        ss.position += velocity;
        ss.gun_left.position += velocity;
        ss.gun_right.position += velocity;

        if (kstate.IsKeyDown(Keys.R)) { // reset spaceship's position
            ss.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            ss.gun_left.position = ss.position;
            ss.gun_right.position = ss.position;
        }



        Vector2 mouse_position = new Vector2(mstate.Position.X, mstate.Position.Y);
        Vector2 direction = mouse_position - ss.position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        ss.rotation = (ss.rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle + (float)(Math.PI / 2) - ss.rotation;

        if (angle_diff == 0f) {
            return;
        }

        if (angle_diff > MathF.PI) {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI) {
            angle_diff += 2 * MathF.PI;
        }

        float rotate_amount = ss.rotation_speed * (float)gameTime.ElapsedGameTime.TotalSeconds * MathF.PI / 180;

        if (angle_diff > 0) {
            if (angle_diff < rotate_amount) {
                ss.rotation += angle_diff;
            } else {
                ss.rotation += rotate_amount;
            }
        } else if (angle_diff < 0) {
            if (0 - angle_diff < rotate_amount) {
                ss.rotation += angle_diff;
            } else {
                ss.rotation -= rotate_amount;
            }
        }

        ss.gun_left.rotation = ss.rotation;
        ss.gun_right.rotation = ss.rotation;

        ss.gun_left.position_offset = new Vector2( // y axis not working
            -11f * (float)Math.Cos(ss.rotation),
            -2f * (float)Math.Sin(ss.rotation)
        );
        ss.gun_right.position_offset = new Vector2(
            10f * (float)Math.Cos(ss.rotation),
            -2f * (float)Math.Sin(ss.rotation)
        );
    }
}

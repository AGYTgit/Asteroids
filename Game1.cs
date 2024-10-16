using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Asteroids;

public class Asteroids : Game {   
    public Asteroids() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        IsFixedTimeStep = false;
    }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    Spaceship ss;
    Entity mouse_cross;

    protected override void Initialize() {
        Texture2D ss_sprite_temp = Content.Load<Texture2D>("spaceship");
        Vector2 ss_origin_temp = new(ss_sprite_temp.Width / 2, ss_sprite_temp.Height / 2);
        Vector2 ss_position_temp = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

        ss = new() {
            sprite = ss_sprite_temp,
            origin = ss_origin_temp,

            position = ss_position_temp,

            speed = 75f,
            rotation = 0f,
            rotation_speed = 90,

            health = 10,
        };

        Texture2D gl_sprite_temp = Content.Load<Texture2D>("minigun");
        Gun gl = new() {
            sprite = gl_sprite_temp,
            origin = new Vector2(gl_sprite_temp.Width / 2, gl_sprite_temp.Height / 2),

            position = ss.position,
            position_offset = new(-10,-2),

            rotation = 0f,
            rotation_speed = 180,

            ammo = 50,
            firerate = 250f,

            last_time_fired = 0,

            shots = [],
        };
        ss.gun_left = gl;

        Texture2D gr_sprite_temp = Content.Load<Texture2D>("minigun");
        Gun gr = new() {
            sprite = gr_sprite_temp,
            origin = new Vector2(gr_sprite_temp.Width / 2, gr_sprite_temp.Height / 2),

            position = ss.position,
            position_offset = new(10,-2),

            rotation = 0f,
            rotation_speed = 180,

            ammo = 50,
            firerate = 250f,
            
            last_time_fired = 0,

            shots = [],
        };
        ss.gun_right = gr;

        Texture2D ss_cross_sprite_temp = Content.Load<Texture2D>("ss_cross");
        Entity ss_cross = new() {
            sprite = ss_cross_sprite_temp,
            origin = new Vector2(ss_cross_sprite_temp.Width / 2, ss_cross_sprite_temp.Height / 2),

            position = ss.position,
            rotation = 0f,
        };
        ss.cross = ss_cross;

        Texture2D ss_cross_2_sprite_temp = Content.Load<Texture2D>("ss_cross_2");
        Entity ss_cross_2 = new() {
            sprite = ss_cross_2_sprite_temp,
            origin = new Vector2(ss_cross_2_sprite_temp.Width / 2, ss_cross_2_sprite_temp.Height / 2),

            position = ss.position,
            rotation = 0f,
        };
        ss.cross_2 = ss_cross_2;

        Texture2D mouse_cross_sprite_temp = Content.Load<Texture2D>("mouse_cross");
        mouse_cross = new() {
            sprite = mouse_cross_sprite_temp,
            origin = new Vector2(mouse_cross_sprite_temp.Width / 2, mouse_cross_sprite_temp.Height / 2),

            position = ss.position,
            rotation = 0f,
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

        if (true) { // move spaceship
            Vector2 velocity = Vector2.Zero;
            float s = ss.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(ss.rotation);
            float c = ss.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(ss.rotation);

            if (kstate.IsKeyDown(Keys.E)) { // move FORWARD
                velocity += new Vector2(s, c * -1);
            }
            if (kstate.IsKeyDown(Keys.S) && !kstate.IsKeyDown(Keys.F)) { // move LEFT
                velocity += new Vector2(c / 2 * -1, s / 2 * -1);
            } else if (kstate.IsKeyDown(Keys.F) && !kstate.IsKeyDown(Keys.S)) { // move RIGHT
                velocity += new Vector2(c / 2, s / 2);
            }

            ss.position += velocity;
        }

        if (kstate.IsKeyDown(Keys.R)) { // reset spaceship's position
            ss.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }


        if (true) { // update ss rotation
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
        }

        if (true) { // update ss guns position and rotation
            ss.gun_left.position = ss.position + new Vector2(
                (ss.gun_left.position_offset.X * MathF.Cos(ss.rotation)) + ss.gun_left.position_offset.Y * MathF.Sin(ss.rotation) * -1,
                (ss.gun_left.position_offset.X * MathF.Sin(ss.rotation)) + (ss.gun_left.position_offset.Y * MathF.Cos(ss.rotation))
            );
            ss.gun_right.position = ss.position + new Vector2(
                (ss.gun_right.position_offset.X * MathF.Cos(ss.rotation)) + ss.gun_right.position_offset.Y * MathF.Sin(ss.rotation) * -1,
                (ss.gun_right.position_offset.X * MathF.Sin(ss.rotation)) + (ss.gun_right.position_offset.Y * MathF.Cos(ss.rotation))
            );

            Vector2 direction_left = ss.cross_2.position - ss.gun_right.position;
            float target_angle_left = MathF.Atan2(direction_left.Y, direction_left.X);

            Vector2 direction_right = ss.cross_2.position - ss.gun_left.position;
            float target_angle_right = MathF.Atan2(direction_right.Y, direction_right.X);

            ss.gun_left.rotation = target_angle_right + MathF.PI / 2;
            ss.gun_right.rotation = target_angle_left + MathF.PI / 2;
        }

        if (true) { // update ss cross and ss cross 2
            Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
            Vector2 direction = mouse_position - ss.position;
            float target_angle = MathF.Atan2(direction.Y, direction.X);

            if (direction.X < 0 && direction.Y < 0) {
                target_angle = (target_angle + MathF.PI * 4) % (MathF.PI * 2);
            }
            float angle_diff = ss.rotation - target_angle - (float)(Math.PI / 2);
            float angle_diff_offset = target_angle + (float)(Math.PI / 2) - ss.rotation;

            if (angle_diff_offset > MathF.PI / 9) {
                angle_diff_offset = MathF.PI / 9;
            } else if (angle_diff_offset < -MathF.PI / 9) {
                angle_diff_offset = -MathF.PI / 9;
            }

            // if (angle_diff_offset > MathF.PI) {
            //     angle_diff_offset = MathF.PI * 2;
            // } else if (angle_diff_offset < -MathF.PI) {
            //     angle_diff_offset = MathF.PI * 2;
            // }

            ss.cross.position = ss.position + new Vector2(
                (direction.X * MathF.Cos(angle_diff)) + direction.Y * MathF.Sin(angle_diff) * -1,
                (direction.X * MathF.Sin(angle_diff)) + (direction.Y * MathF.Cos(angle_diff))
            );

            ss.cross_2.position = ss.position + new Vector2(
                (direction.X * MathF.Cos(angle_diff + angle_diff_offset)) + direction.Y * MathF.Sin(angle_diff + angle_diff_offset) * -1,
                (direction.X * MathF.Sin(angle_diff + angle_diff_offset)) + (direction.Y * MathF.Cos(angle_diff + angle_diff_offset))
            );
        }


        if (true) { // update mouse cross
            mouse_cross.position = new Vector2(
                mstate.Position.X,
                mstate.Position.Y
            );
        }


        if (true) { // shoot
            if (mstate.LeftButton == ButtonState.Pressed || mstate.RightButton == ButtonState.Pressed) {
                if (gameTime.TotalGameTime.TotalMilliseconds > (ss.gun_left.last_time_fired + (60000 / ss.gun_left.firerate))) {
                    Texture2D sprite_temp = Content.Load<Texture2D>("shot");
                    if (mstate.LeftButton == ButtonState.Pressed) {
                        Shot shot = new() {
                            sprite = sprite_temp,
                            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
                            position = ss.gun_left.position,

                            rotation = ss.gun_left.rotation,

                            speed = 250
                        };
                        ss.gun_left.shots.Add(shot);
                        ss.gun_left.last_time_fired = (long)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
                if (gameTime.TotalGameTime.TotalMilliseconds > (ss.gun_right.last_time_fired + (60000 / ss.gun_right.firerate))) {
                    Texture2D sprite_temp = Content.Load<Texture2D>("shot");
                    if (mstate.RightButton == ButtonState.Pressed) {
                        Shot shot = new() {
                            sprite = sprite_temp,
                            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
                            position = ss.gun_right.position,

                            rotation = ss.gun_right.rotation,

                            speed = 250
                        };
                        ss.gun_right.shots.Add(shot);
                        ss.gun_right.last_time_fired = (long)gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
            }
        }

        if (true) { // update shots
            foreach (Shot shot in ss.gun_left.shots) {
                shot.position.X += shot.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(shot.rotation - MathF.PI / 2);
                shot.position.Y += shot.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(shot.rotation - MathF.PI / 2);
            }
            foreach (Shot shot in ss.gun_right.shots) {
                shot.position.X += shot.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(shot.rotation - MathF.PI / 2);
                shot.position.Y += shot.speed * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(shot.rotation - MathF.PI / 2);
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        // if (Mouse.GetState().Y > 0) {
            GraphicsDevice.Clear(Color.Black);
        // } else {
        //     GraphicsDevice.Clear(Color.Green);
        // }

        _spriteBatch.Begin();
        foreach (Shot shot in ss.gun_left.shots) {
            _spriteBatch.Draw(
                shot.sprite,
                shot.position,
                null,
                Color.White,
                shot.rotation,
                shot.origin,
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        foreach (Shot shot in ss.gun_right.shots) {
            _spriteBatch.Draw(
                shot.sprite,
                shot.position,
                null,
                Color.White,
                shot.rotation,
                shot.origin,
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        _spriteBatch.Draw(
            ss.gun_left.sprite,
            ss.gun_left.position,
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
            ss.gun_right.position,
            null,
            Color.White,
            ss.gun_right.rotation,
            ss.gun_right.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
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
            ss.cross.sprite,
            ss.cross.position,
            null,
            Color.White,
            ss.cross.rotation,
            ss.cross.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.Draw(
            ss.cross_2.sprite,
            ss.cross_2.position,
            null,
            Color.White,
            ss.cross_2.rotation,
            ss.cross_2.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.Draw(
            mouse_cross.sprite,
            mouse_cross.position,
            null,
            Color.White,
            mouse_cross.rotation,
            mouse_cross.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.End();

        base.Draw(gameTime);
    }

}

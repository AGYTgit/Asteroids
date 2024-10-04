using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroids;

public class Asteroids : Game
{   
    Texture2D ss_sprite;
    Vector2 ss_position;
    float ss_rotation;
    float ss_speed_forward;
    float ss_speed_forward_multiplier;
    float ss_speed_other;
    float ss_speed_other_multiplier;
    float ss_rotate_speed;
    float ss_rotate_speed_multiplier;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Asteroids()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        ss_position = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                    _graphics.PreferredBackBufferHeight / 2);

        ss_speed_forward = 100f;
        ss_speed_forward_multiplier = 1f;
        ss_speed_other = 50f;
        ss_speed_other_multiplier = 1f;
        ss_rotate_speed = .02f;
        ss_rotate_speed_multiplier = 1f;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        ss_sprite = Content.Load<Texture2D>("spaceship");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        float updated_ss_speed_forward = ss_speed_forward * ss_speed_forward_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float updated_ss_speed_other = ss_speed_other * ss_speed_other_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.E))
        {
            ss_position.Y -= updated_ss_speed_forward * (float)Math.Cos(ss_rotation);
            ss_position.X += updated_ss_speed_forward * (float)Math.Sin(ss_rotation);
        }
        
        if (kstate.IsKeyDown(Keys.S))
        {
            ss_position.Y -= updated_ss_speed_other * (float)Math.Sin(ss_rotation);
            ss_position.X -= updated_ss_speed_other * (float)Math.Cos(ss_rotation);
        }
        
        if (kstate.IsKeyDown(Keys.F))
        {
            ss_position.Y += updated_ss_speed_other * (float)Math.Sin(ss_rotation);
            ss_position.X += updated_ss_speed_other * (float)Math.Cos(ss_rotation);
        }

        if (kstate.IsKeyDown(Keys.R)) {
            ss_position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(ss_sprite.Width / 2, ss_sprite.Height / 2);
        }

        if (kstate.IsKeyDown(Keys.Space)){
            ss_speed_forward_multiplier = 8f;
            ss_speed_other_multiplier = 4f;
            ss_rotate_speed_multiplier = 4f;
        } else {
            ss_speed_forward_multiplier = 1f;
            ss_speed_other_multiplier = 1f;
            ss_rotate_speed_multiplier = 1f;
        }



        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

        Vector2 direction = mouse_position - ss_position;
        float target_angle_r = MathF.Atan2(direction.Y, direction.X);

        ss_rotation = (ss_rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle_r + (float)(Math.PI / 2) - ss_rotation;


        if (angle_diff > MathF.PI)
        {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI)
        {
            angle_diff += 2 * MathF.PI;
        }


        if (angle_diff > 0) {
            ss_rotation += ss_rotate_speed * ss_rotate_speed_multiplier;
        } else if (angle_diff < 0) {
            ss_rotation -= ss_rotate_speed * ss_rotate_speed_multiplier;
        }

        

        // ss_rotation = angle_r + (float)(Math.PI / 2);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        _spriteBatch.Draw(
            ss_sprite,
            ss_position,
            null,
            Color.White,
            ss_rotation,
            new Vector2(ss_sprite.Width / 2, ss_sprite.Height / 2),
            Vector2.One,
            SpriteEffects.None,
            0f
            );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

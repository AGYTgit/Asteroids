using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids;

public class Asteroids : Game
{   
    Texture2D ss_sprite;
    Vector2 ss_position;
    float ss_rotation;
    float ss_speed_forward;
    float ss_speed_other;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Asteroids()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        ss_position = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                    _graphics.PreferredBackBufferHeight / 2);

        ss_speed_forward = 100f;
        ss_speed_other = 50f;

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

        // TODO: Add your update logic here

        float updated_ss_speed_forward = ss_speed_forward * (float)gameTime.ElapsedGameTime.TotalSeconds;
        float updated_ss_speed_other = ss_speed_other * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.Up))
        {
            ss_position.Y -= updated_ss_speed_forward;
        }
        
        if (kstate.IsKeyDown(Keys.Left))
        {
            ss_position.X -= updated_ss_speed_other;
        }
        
        if (kstate.IsKeyDown(Keys.Right))
        {
            ss_position.X += updated_ss_speed_other;
        }

        if (kstate.IsKeyDown(Keys.S)) {
            ss_rotation -= .05f;
        }

        if (kstate.IsKeyDown(Keys.F)) {
            ss_rotation += .05f;
        }

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

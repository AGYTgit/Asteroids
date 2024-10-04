using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids;

public class Asteroids : Game
{   
    Texture2D ss_sprite;
    Vector2 ss_position;
    float ss_speed;

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

        ss_speed = 100f;

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

        float updatedBallSpeed = ss_speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.Up))
        {
            ss_position.Y -= updatedBallSpeed;
        }
        
        if (kstate.IsKeyDown(Keys.Down))
        {
            ss_position.Y += updatedBallSpeed;
        }
        
        if (kstate.IsKeyDown(Keys.Left))
        {
            ss_position.X -= updatedBallSpeed;
        }
        
        if (kstate.IsKeyDown(Keys.Right))
        {
            ss_position.X += updatedBallSpeed;
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
            0f,
            new Vector2(ss_sprite.Width / 2, ss_sprite.Height / 2),
            Vector2.One,
            SpriteEffects.None,
            0f
            );
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

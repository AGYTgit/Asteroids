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
    Cross mouse_cross;

    protected override void Initialize() {
        Vector2 start_position = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

        Texture2D cross_sprite = Content.Load<Texture2D>("mouse_cross");
        mouse_cross = new(cross_sprite, new Vector2(0,0));
        

        Texture2D lg_cross_sprite = Content.Load<Texture2D>("ss_cross_2");
        Cross lg_cross = new(lg_cross_sprite, start_position + new Vector2(-10,-32));

        Texture2D lg_sprite = Content.Load<Texture2D>("minigun");
        Gun lg = new(lg_sprite, start_position, new Vector2(-10,-2), lg_cross);


        Texture2D rg_cross_sprite = Content.Load<Texture2D>("ss_cross_2");
        Cross rg_cross = new(rg_cross_sprite, start_position + new Vector2(10,-32));

        Texture2D rg_sprite = Content.Load<Texture2D>("minigun");
        Gun rg = new(rg_sprite, start_position, new Vector2(10,-2), rg_cross);


        Texture2D ss_cross_sprite = Content.Load<Texture2D>("ss_cross");
        Cross ss_cross = new(ss_cross_sprite, start_position + new Vector2(0,-32));

        Texture2D ss_sprite = Content.Load<Texture2D>("spaceship");
        ss = new(ss_sprite, start_position, lg, rg, ss_cross);

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

        mouse_cross.position = new Vector2(mstate.Position.X, mstate.Position.Y);

        bool moving = false;
        if (kstate.IsKeyDown(Keys.E)) {
            ss.update_velocity(1);
            moving = true;
        }
        if (kstate.IsKeyDown(Keys.S)) {
            ss.update_velocity(2);
            moving = true;
        }
        if (kstate.IsKeyDown(Keys.F)) {
            ss.update_velocity(3);
            moving = true;
        }
        if (kstate.IsKeyDown(Keys.D)) {
            ss.update_velocity(5);
            moving = true;
        } 
        if (!moving) {
            ss.update_velocity(0);
        }

        if (kstate.IsKeyDown(Keys.R)) {
            ss.position = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        if (mstate.LeftButton == ButtonState.Pressed) { // right not working
            Texture2D shot_sprite = Content.Load<Texture2D>("shot");
            ss.gun_left.shoot(shot_sprite, gameTime, mstate);
        } 
        if (mstate.RightButton == ButtonState.Pressed) {
            Texture2D shot_sprite = Content.Load<Texture2D>("shot");
            ss.gun_right.shoot(shot_sprite, gameTime, mstate);
        }

        ss.update_all(gameTime, mstate);
        
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
                shot.rectangle,
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
                shot.rectangle,
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
            ss.rectangle,
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
            ss.gun_left.cross.sprite,
            ss.gun_left.cross.position,
            null,
            Color.White,
            ss.gun_left.cross.rotation,
            ss.gun_left.cross.origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.Draw(
            ss.gun_right.cross.sprite,
            ss.gun_right.cross.position,
            null,
            Color.White,
            ss.gun_right.cross.rotation,
            ss.gun_right.cross.origin,
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

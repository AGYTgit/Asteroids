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
    Asteroid_Spawner spawner;

    UI_Resources ui_resources;

    protected override void Initialize() {
        Vector2 start_position = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

        Texture2D cross_sprite = Content.Load<Texture2D>("mouse_cross");
        mouse_cross = new(cross_sprite, new Vector2(0,0));
        

        Texture2D lg_cross_sprite = Content.Load<Texture2D>("gun_cross");
        Cross lg_cross = new(lg_cross_sprite, start_position + new Vector2(-10,-32));

        Texture2D lg_sprite = Content.Load<Texture2D>("minigun");
        Gun lg = new(lg_sprite, start_position, new Vector2(-10,-2), lg_cross);


        Texture2D rg_cross_sprite = Content.Load<Texture2D>("gun_cross");
        Cross rg_cross = new(rg_cross_sprite, start_position + new Vector2(10,-32));

        Texture2D rg_sprite = Content.Load<Texture2D>("minigun");
        Gun rg = new(rg_sprite, start_position, new Vector2(10,-2), rg_cross);


        Texture2D ss_cross_sprite = Content.Load<Texture2D>("ss_cross");
        Cross ss_cross = new(ss_cross_sprite, start_position + new Vector2(0,-32));

        Texture2D ss_sprite = Content.Load<Texture2D>("spaceship");
        ss = new(ss_sprite, start_position, lg, rg, ss_cross);

        Texture2D spawner_sprite = Content.Load<Texture2D>("asteroid_1");
        spawner = new(GraphicsDevice.Viewport, spawner_sprite);

        ui_resources = new(
            _graphics,
            ss.health,
            // ss.shield,
            10,
            ss.gun_left.ammo,
            ss.gun_right.ammo,
            // ss.boost
            2
        );

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        ui_resources.load_sprites(Content);
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

        if (mstate.LeftButton == ButtonState.Pressed) {
            Texture2D shot_sprite = Content.Load<Texture2D>("shot");
            ss.gun_left.shoot(shot_sprite, gameTime, ss.velocity); // rework adding ss's velocity
        } 
        if (mstate.RightButton == ButtonState.Pressed) {
            Texture2D shot_sprite = Content.Load<Texture2D>("shot");
            ss.gun_right.shoot(shot_sprite, gameTime, ss.velocity); // rework adding ss's velocity
        }

        ss.update_all(gameTime, mstate);

        spawner.spawn(gameTime, ss.position);

        spawner.update_asteroids(gameTime);

        ui_resources.update_resources(
            ss.health,
            // ss.shield,
            10,
            ss.gun_left.ammo,
            ss.gun_right.ammo,
            // ss.boost
            2
        );
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        // if (Mouse.GetState().Y > 0) {
            GraphicsDevice.Clear(Color.Black);
        // } else {
        //     GraphicsDevice.Clear(Color.Green);
        // }

        _spriteBatch.Begin();
        ss.draw_all(_spriteBatch);
        spawner.draw(_spriteBatch);
        mouse_cross.draw(_spriteBatch);
        ss.draw_crosses(_spriteBatch);
        ui_resources.draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace Asteroids;

public class Asteroids : Game {   
    public Asteroids() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        IsFixedTimeStep = false;
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ToggleFullScreen();
    }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private int game_state = 0;
    private bool boost_button_pressed = false;
    private bool emergency_break_button_pressed = false;
    
    Cross mouse_cross;
    Start_Menu start_menu;

    Spaceship ss;
    Asteroid_Spawner spawner;
    UI_Resources ui_resources;


    protected override void Initialize() {
        Texture2D cross_sprite = Content.Load<Texture2D>("mouse_cross");
        mouse_cross = new(cross_sprite, new Vector2(0,0));

        switch (game_state) {
            case 0: // start menu
                start_menu = new(_graphics);
            break;
            case 1: // game
                Vector2 start_position = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

                Texture2D lg_cross_sprite = Content.Load<Texture2D>("gun_cross");
                Cross lg_cross = new(lg_cross_sprite, start_position + new Vector2(-10,-32));

                Texture2D lg_sprite = Content.Load<Texture2D>("minigun");
                Texture2D shot_sprite = Content.Load<Texture2D>("shot");
                Gun lg = new(lg_sprite, shot_sprite, start_position, new Vector2(-10,-2), lg_cross);


                Texture2D rg_cross_sprite = Content.Load<Texture2D>("gun_cross");
                Cross rg_cross = new(rg_cross_sprite, start_position + new Vector2(10,-32));

                Texture2D rg_sprite = Content.Load<Texture2D>("minigun");
                Gun rg = new(rg_sprite, shot_sprite, start_position, new Vector2(10,-2), rg_cross);


                Texture2D ss_cross_sprite = Content.Load<Texture2D>("ss_cross");
                Cross ss_cross = new(ss_cross_sprite, start_position + new Vector2(0,-32));

                Texture2D ss_sprite = Content.Load<Texture2D>("spaceship");
                ss = new(ss_sprite, start_position, lg, rg, ss_cross);

                Texture2D spawner_sprite = Content.Load<Texture2D>("asteroid_1");
                spawner = new(GraphicsDevice.Viewport, spawner_sprite);

                ui_resources = new(
                    _graphics,
                    ss.health,
                    ss.shield,
                    ss.gun_left.ammo,
                    ss.gun_right.ammo,
                    ss.boost_ch
                );
            break;
        }

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        switch (game_state) {
            case 0: // start menu\
                start_menu.load_sprites(Content);
            break;
            case 1: // game
                ui_resources.load_sprites(Content);
            break;
        } 
    }

    protected override void Update(GameTime gameTime) {
        KeyboardState kstate = Keyboard.GetState();
        MouseState mstate = Mouse.GetState();

        mouse_cross.position = new Vector2(mstate.Position.X, mstate.Position.Y);

        switch (game_state) {
            case 0: // start menu
                switch (start_menu.is_pressed(mstate)) {
                    case 1:
                        game_state = 1;
                        Initialize();
                        LoadContent();
                    break;
                    case 2:
                        Exit();
                    break;
                }
            break;
            case 1: // game
                if (kstate.IsKeyDown(Keys.Escape)) {
                    game_state = 0;
                    Initialize();
                    LoadContent();
                }
                if (ss.health <= 0) {
                    game_state = 0;
                    Initialize();
                    LoadContent();
                }

                if (ss.gun_left.ammo == 0) {
                    ss.gun_left.reload(gameTime);
                }
                if (ss.gun_right.ammo == 0) {
                    ss.gun_right.reload(gameTime);
                }
                ss.reload_boost(gameTime);
                ss.recharge_shield(gameTime);

                bool moving = false;
                if (kstate.IsKeyDown(Keys.E)) {
                    ss.update_velocity(gameTime, 1);
                    moving = true;
                }
                if (kstate.IsKeyDown(Keys.S)) {
                    ss.update_velocity(gameTime, 2);
                    moving = true;
                }
                if (kstate.IsKeyDown(Keys.F)) {
                    ss.update_velocity(gameTime, 3);
                    moving = true;
                }
                if (kstate.IsKeyDown(Keys.D) && !emergency_break_button_pressed) {
                    emergency_break_button_pressed = true;
                    ss.update_velocity(gameTime, 5);
                    ss.take_damage(gameTime, 2);
                    moving = true;
                } else if (!kstate.IsKeyDown(Keys.D) && emergency_break_button_pressed) {
                    emergency_break_button_pressed = false;
                }
                if (!moving) { // rework auto brake
                    ss.update_velocity(gameTime, 0);
                }

                if (kstate.IsKeyDown(Keys.Space) && boost_button_pressed == false) {
                    boost_button_pressed = true;
                    ss.boost(gameTime);
                } else if (!kstate.IsKeyDown(Keys.Space) && boost_button_pressed == true) {
                    boost_button_pressed = false;
                }

                if (kstate.IsKeyDown(Keys.R)) {
                    ss.position = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
                }

                if (mstate.LeftButton == ButtonState.Pressed) {
                    ss.gun_left.shoot(gameTime, ss.velocity);
                } 
                if (mstate.RightButton == ButtonState.Pressed) {
                    ss.gun_right.shoot(gameTime, ss.velocity);
                }

                ss.update_all(gameTime, mstate);

                spawner.spawn(gameTime, ss.position);

                spawner.update_asteroids(gameTime);

                ui_resources.update_resources(
                    ss.health,
                    ss.shield,
                    ss.gun_left.ammo,
                    ss.gun_right.ammo,
                    ss.boost_ch
                );

                if (spawner.check_for_collision(ss.rectangle)) {
                    ss.take_damage(gameTime, 1);
                }

                for (int i = spawner.asteroid_list.Count - 1; i >= 0 ;i--) {
                    if (ss.gun_left.check_for_collision(spawner.asteroid_list[i].rectangle)) {
                        spawner.asteroid_list.RemoveAt(i);
                    }
                    if (ss.gun_right.check_for_collision(spawner.asteroid_list[i].rectangle)) {
                        spawner.asteroid_list.RemoveAt(i);
                    }
                }

                // if (ss.gun_left.check_for_collision(ss.rectangle)) { // sometimes instantly collides with ship
                //     ss.take_damage(gameTime, 3);
                // }

                spawner.check_for_asteroid_to_asteroid_collision();
                ss.gun_left.check_for_shot_to_shot_collision();
                ss.gun_right.check_for_shot_to_shot_collision();
            break;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        switch (game_state) {
            case 0: // start menu
                _spriteBatch.Begin();
                start_menu.draw(_spriteBatch);
                mouse_cross.draw(_spriteBatch);
                _spriteBatch.End();
            break;
            case 1: // game
                _spriteBatch.Begin();
                ss.draw_all(_spriteBatch);
                spawner.draw(_spriteBatch);
                ui_resources.draw(_spriteBatch);
                ss.draw_crosses(_spriteBatch);
                mouse_cross.draw(_spriteBatch);
                _spriteBatch.End();
            break;
        }
        

        base.Draw(gameTime);
    }

}

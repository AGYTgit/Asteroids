using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


public class UI_Resources(GraphicsDeviceManager _graphics, int _health, int _shield, int _ammo_left, int _ammo_right, int _boost) {
    private Vector2 position   { get; set; } = new(_graphics.PreferredBackBufferWidth / 2 - 140, _graphics.PreferredBackBufferHeight - 48);

    private int max_health         { get; } = _health;
    private int max_shield         { get; } = _shield;
    private int max_ammo_left      { get; } = _ammo_left;
    private int max_ammo_right     { get; } = _ammo_right;
    private int max_boost          { get; } = _boost;

    private int current_health       { get; set; } = _health;
    private int current_shield       { get; set; } = _shield;
    private int current_ammo_left    { get; set; } = _ammo_left;
    private int current_ammo_right   { get; set; } = _ammo_right;
    private int current_boost        { get; set; } = _boost;

    private Texture2D ui_background_sprite        { get; set; }

    private List<Texture2D> ui_health_sprites     { get; set; } = [];
    private List<Texture2D> ui_shield_sprites     { get; set; } = [];
    private List<Texture2D> ui_ammo_left_sprites  { get; set; } = [];
    private List<Texture2D> ui_ammo_right_sprites { get; set; } = [];
    private List<Texture2D> ui_boost_sprites      { get; set; } = [];


    public void load_sprites(ContentManager _content) {
        ui_background_sprite = _content.Load<Texture2D>("ui_empty");

        ui_health_sprites.Add(_content.Load<Texture2D>("ui_health_1"));
        ui_health_sprites.Add(_content.Load<Texture2D>("ui_health_2"));
        ui_health_sprites.Add(_content.Load<Texture2D>("ui_health_3"));
        ui_health_sprites.Add(_content.Load<Texture2D>("ui_health_4"));
        ui_health_sprites.Add(_content.Load<Texture2D>("ui_health_5"));

        ui_shield_sprites.Add(_content.Load<Texture2D>("ui_shield_1"));
        ui_shield_sprites.Add(_content.Load<Texture2D>("ui_shield_2"));
        ui_shield_sprites.Add(_content.Load<Texture2D>("ui_shield_3"));
        ui_shield_sprites.Add(_content.Load<Texture2D>("ui_shield_4"));
        ui_shield_sprites.Add(_content.Load<Texture2D>("ui_shield_5"));

        ui_ammo_left_sprites.Add(_content.Load<Texture2D>("ui_ammo_left_1"));
        ui_ammo_left_sprites.Add(_content.Load<Texture2D>("ui_ammo_left_2"));
        ui_ammo_left_sprites.Add(_content.Load<Texture2D>("ui_ammo_left_3"));
        ui_ammo_left_sprites.Add(_content.Load<Texture2D>("ui_ammo_left_4"));
        ui_ammo_left_sprites.Add(_content.Load<Texture2D>("ui_ammo_left_5"));

        ui_ammo_right_sprites.Add(_content.Load<Texture2D>("ui_ammo_right_1"));
        ui_ammo_right_sprites.Add(_content.Load<Texture2D>("ui_ammo_right_2"));
        ui_ammo_right_sprites.Add(_content.Load<Texture2D>("ui_ammo_right_3"));
        ui_ammo_right_sprites.Add(_content.Load<Texture2D>("ui_ammo_right_4"));
        ui_ammo_right_sprites.Add(_content.Load<Texture2D>("ui_ammo_right_5"));
        
        ui_boost_sprites.Add(_content.Load<Texture2D>("ui_booster_1"));
        ui_boost_sprites.Add(_content.Load<Texture2D>("ui_booster_2"));
    }

    public void update_resources(int _health, int _shield, int _ammo_left, int _ammo_right, int _boost) {
        current_health =     _health     > max_health     ? max_health     : _health;
        current_shield =     _shield     > max_shield     ? max_shield     : _shield;
        current_ammo_left =  _ammo_left  > max_ammo_left  ? max_ammo_left  : _ammo_left;
        current_ammo_right = _ammo_right > max_ammo_right ? max_ammo_right : _ammo_right;
        current_boost =      _boost      > max_boost      ? max_boost      : _boost;
    }

    public void draw(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            ui_background_sprite,
            position,
            Color.White
        );
        if (current_health > 0) {
            sprite_batch.Draw(
                ui_health_sprites[Mapping.Map(current_health, 1, max_health, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_shield > 0) {
            sprite_batch.Draw(
                ui_shield_sprites[Mapping.Map(current_shield, 1, max_shield, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_ammo_left > 0) {
            sprite_batch.Draw(
                ui_ammo_left_sprites[Mapping.Map(current_ammo_left, 1, max_ammo_left, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_ammo_right > 0) {
            sprite_batch.Draw(
                ui_ammo_right_sprites[Mapping.Map(current_ammo_right, 1, max_ammo_right, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_boost > 0) {
            sprite_batch.Draw(
                ui_boost_sprites[Mapping.Map(current_boost, 1, max_boost, 0, 1)],
                position,
                Color.White
            );
        }
    }
    
};

public class Start_Menu(GraphicsDeviceManager _graphics) {
    private Texture2D button_start_sprite { get; set; }
    private Texture2D button_exit_sprite  { get; set; }

    private Vector2 button_start_position { get; set; }
    private Vector2 button_exit_position  { get; set; }

    private Vector2 button_start_origin   { get; set; }
    private Vector2 button_exit_origin    { get; set; }

    private Vector2 screen_center              { get; }      = new(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

    public void load_sprites(ContentManager content) {
        button_start_sprite = content.Load<Texture2D>("ui_start_button");
        button_exit_sprite = content.Load<Texture2D>("ui_exit_button");

        button_start_position = screen_center - new Vector2(0, button_start_sprite.Height / 4 * 3);
        button_exit_position = screen_center + new Vector2(0, button_exit_sprite.Height / 4 * 3);

        button_start_origin = new(button_start_sprite.Width / 2, button_start_sprite.Height / 2);
        button_exit_origin = new(button_exit_sprite.Width / 2, button_exit_sprite.Height / 2);
    }

    public void draw(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            button_start_sprite,
            button_start_position,
            null,
            Color.White,
            0f,
            button_start_origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        sprite_batch.Draw(
            button_exit_sprite,
            button_exit_position,
            null,
            Color.White,
            0f,
            button_exit_origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
    }

    public int is_pressed(MouseState mstate) {
        if (mstate.LeftButton == ButtonState.Pressed) {
            Vector2 m_pos = new(mstate.Position.X, mstate.Position.Y);
            if (m_pos.X >= button_start_position.X - button_start_sprite.Width / 2 && m_pos.X <= button_start_position.X + button_start_sprite.Width / 2 &&
                m_pos.Y >= button_start_position.Y - button_start_sprite.Height / 2 && m_pos.Y <= button_start_position.Y + button_start_sprite.Height / 2) {
                return 1;
            }
            if (m_pos.X >= button_exit_position.X - button_start_sprite.Width / 2 && m_pos.X <= button_exit_position.X + button_start_sprite.Width / 2 &&
                m_pos.Y >= button_exit_position.Y - button_start_sprite.Height / 2 && m_pos.Y <= button_exit_position.Y + button_start_sprite.Height / 2) {
                return 2;
            }
        }
        return 0;
    }
}

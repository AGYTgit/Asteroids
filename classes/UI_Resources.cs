using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


public class UI_Resources(GraphicsDeviceManager _graphics, int _health, int _shield, int _ammo_left, int _ammo_right, int _boost) {
    public GraphicsDeviceManager graphics = _graphics;
    

    public Vector2 position   { get; set; } = new(_graphics.PreferredBackBufferWidth / 2 - 140, _graphics.PreferredBackBufferHeight - 48);

    public int max_health         { get; } = _health;
    public int max_shield         { get; } = _shield;
    public int max_ammo_left      { get; } = _ammo_left;
    public int max_ammo_right     { get; } = _ammo_right;
    public int max_boost          { get; } = _boost;

    public int current_health       { get; set; } = _health;
    public int current_shield       { get; set; } = _shield;
    public int current_ammo_left    { get; set; } = _ammo_left;
    public int current_ammo_right   { get; set; } = _ammo_right;
    public int current_boost        { get; set; } = _boost;

    public Texture2D ui_background_sprite        { get; set; }

    public List<Texture2D> ui_health_sprites     { get; set; } = [];
    public List<Texture2D> ui_shield_sprites     { get; set; } = [];
    public List<Texture2D> ui_ammo_left_sprites  { get; set; } = [];
    public List<Texture2D> ui_ammo_right_sprites { get; set; } = [];
    public List<Texture2D> ui_boost_sprites      { get; set; } = [];


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
        current_shield =     _shield     > max_shield     ? max_shield     : _shield;
    }

    public void draw(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            ui_background_sprite,
            position,
            Color.White
        );
        if (current_health > 0) {
            sprite_batch.Draw(
                ui_health_sprites[Mapping.Map(current_health, 0, max_health, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_shield > 0) {
            sprite_batch.Draw(
                ui_shield_sprites[Mapping.Map(current_shield, 0, max_shield, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_ammo_left > 0) {
            sprite_batch.Draw(
                ui_ammo_left_sprites[Mapping.Map(current_ammo_left, 0, max_ammo_left, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_ammo_right > 0) {
            sprite_batch.Draw(
                ui_ammo_right_sprites[Mapping.Map(current_ammo_right, 0, max_ammo_right, 0, 4)],
                position,
                Color.White
            );
        }
        if (current_boost > 0) {
            sprite_batch.Draw(
                ui_boost_sprites[Mapping.Map(current_boost, 0, max_boost, 0, 1)],
                position,
                Color.White
            );
        }
    }

    
};

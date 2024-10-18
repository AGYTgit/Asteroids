using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Cross(Texture2D _sprite, Vector2 _position) {
    public Texture2D sprite { get; }      = _sprite;
    public Vector2 origin   { get; }      = new Vector2(_sprite.Width / 2, _sprite.Height / 2);
    public Vector2 position { get; set; } = _position;
    public float rotation   { get; set; } = 0f;

    public void draw(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            sprite,
            position,
            null,
            Color.White,
            rotation,
            origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
    }
}

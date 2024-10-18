using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


public class Spaceship(Texture2D _sprite, Vector2 _position, Gun _gun_left, Gun _gun_right, Cross _cross, float _acceleration=.25f, float _rotation_speed=90f, int _health=5, int _shield=5, float _velocity_liniter=75) {
    public Texture2D sprite             { get; }      = _sprite;
    public Rectangle rectangle          { get; }      = _sprite.Bounds;
    public Vector2 origin               { get; }      = new(_sprite.Width / 2, _sprite.Height / 2);
    
    public Vector2 position             { get; set; } = _position;
    public Vector2 velocity             { get; set; } = Vector2.Zero;
    public Vector2 max_velocity         { get; set; } = new(_velocity_liniter,_velocity_liniter);
    public float rotation               { get; set; } = 0;
    public float acceleration           { get; }      = _acceleration;
    public float rotation_speed         { get; }      = _rotation_speed;

    public int health                   { get; set; } = _health;
    public int shield                   { get; set; } = _shield;

    public Gun gun_left                 { get; }      = _gun_left;
    public Gun gun_right                { get; }      = _gun_right;
    public Cross cross                  { get; }      = _cross;


    public void update_velocity(int mode) {
        Vector2 downward_velocity = new(
            acceleration * (float)Math.Sin(rotation),
            acceleration * (float)Math.Cos(rotation)
        );

        switch (mode) {
            case 0: // slow down
                velocity -= velocity / 200;
                break;
            case 1: // forward
                velocity += new Vector2(downward_velocity.X, downward_velocity.Y * -1);
                break;
            case 2: // left
                velocity += new Vector2(downward_velocity.Y / 2 * -1, downward_velocity.X / 2 * -1);
                break;
            case 3: // right
                velocity += new Vector2(downward_velocity.Y / 2, downward_velocity.X / 2);
                break;
            case 4: // super speed (space_warp)
                velocity += max_velocity * 10;
                break;
            case 5: // stop (emergency_break)
                velocity = new Vector2(0, 0);
                break;
        }


        // max_velocity = new Vector2(
        //     MathF.Abs((max_velocity.X * MathF.Cos(rotation)) + (max_velocity.Y * MathF.Sin(rotation) * -1)),
        //     MathF.Abs((max_velocity.X * MathF.Sin(rotation)) + (max_velocity.Y * MathF.Cos(rotation)))
        // );

        if (velocity.X > max_velocity.X) {
            velocity = new(max_velocity.X, velocity.Y);
        } else if (velocity.X < -max_velocity.X) {
            velocity = new(-max_velocity.X, velocity.Y);
        }

        if (velocity.Y > max_velocity.Y) {
            velocity = new(velocity.X, max_velocity.Y);
        } else if (velocity.Y < -max_velocity.Y) {
            velocity = new(velocity.X, -max_velocity.Y);
        }
    }


    public void update_all(GameTime gameTime, MouseState mstate) {
        update(gameTime, mstate);
        update_left_gun(gameTime, mstate);
        update_right_gun(gameTime, mstate);
        update_cross();
    }


    public void update(GameTime gameTime, MouseState mstate) {
        move(gameTime);
        rotate(gameTime, mstate);
    }

    public void move(GameTime gameTime) {
        Vector2 velocit_frame = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        position += velocit_frame;
    }
    public void rotate(GameTime gameTime, MouseState mstate) {
        Vector2 mouse_position = new Vector2(mstate.Position.X, mstate.Position.Y);
        Vector2 direction = mouse_position - position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        rotation = (rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle + (float)(Math.PI / 2) - rotation;

        if (angle_diff == 0f) {
            return;
        }

        if (angle_diff > MathF.PI) {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI) {
            angle_diff += 2 * MathF.PI;
        }

        float rotate_amount = rotation_speed * (float)gameTime.ElapsedGameTime.TotalSeconds * MathF.PI / 180;

        if (angle_diff > 0) {
            if (angle_diff < rotate_amount) {
                rotation += angle_diff;
            } else {
                rotation += rotate_amount;
            }
        } else if (angle_diff < 0) {
            if (0 - angle_diff < rotate_amount) {
                rotation += angle_diff;
            } else {
                rotation -= rotate_amount;
            }
        }
    }


    public void update_left_gun(GameTime gameTime, MouseState mstate) {
        move_left_gun();
        move_left_gun_cross(mstate);
        rotate_left_gun();
        move_left_gun_shots(gameTime);
    }

    public void move_left_gun() {
        gun_left.position = position + new Vector2(
            (gun_left.position_offset.X * MathF.Cos(rotation)) - gun_left.position_offset.Y * MathF.Sin(rotation),
            (gun_left.position_offset.X * MathF.Sin(rotation)) + (gun_left.position_offset.Y * MathF.Cos(rotation))
        );
    }
    public void move_left_gun_cross(MouseState mstate) {
        Vector2 mouse_position = new Vector2(mstate.Position.X, mstate.Position.Y);
        Vector2 direction = mouse_position - position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        if (direction.X < 0 && direction.Y < 0) {
            target_angle = (target_angle + MathF.PI * 2) % (MathF.PI * 2);
        }

        float angle_diff = rotation - target_angle - (float)(Math.PI / 2);
        float angle_diff_offset = target_angle + (float)(Math.PI / 2) - rotation;

        if (angle_diff_offset > MathF.PI / 9) {
            angle_diff_offset = MathF.PI / 9;
            if (angle_diff < -MathF.PI ) {
                angle_diff_offset = MathF.PI * 2 - angle_diff_offset;
            }
        } else if (angle_diff_offset < -MathF.PI / 9) {
            angle_diff_offset = -MathF.PI / 9;
            if (angle_diff > MathF.PI) {
                angle_diff_offset = MathF.PI * 2 - angle_diff_offset;
            }
        }

        gun_left.cross.position = position + new Vector2(
            (direction.X * MathF.Cos(angle_diff + angle_diff_offset)) + direction.Y * MathF.Sin(angle_diff + angle_diff_offset) * -1,
            (direction.X * MathF.Sin(angle_diff + angle_diff_offset)) + (direction.Y * MathF.Cos(angle_diff + angle_diff_offset))
        );
    }
    public void rotate_left_gun() {
        Vector2 direction_left = gun_left.cross.position - gun_left.position;
        float target_angle_left = MathF.Atan2(direction_left.Y, direction_left.X);

        gun_left.rotation = target_angle_left + MathF.PI / 2;
    }
    public void move_left_gun_shots(GameTime gameTime) {
        gun_left.move_shots(gameTime, velocity);
    }


    public void update_right_gun(GameTime gameTime, MouseState mstate) {
        move_right_gun();
        move_right_gun_cross(mstate);
        rotate_right_run();
        move_right_gun_shots(gameTime);
    }

    public void move_right_gun() {
        gun_right.position = position + new Vector2(
            (gun_right.position_offset.X * MathF.Cos(rotation)) - gun_right.position_offset.Y * MathF.Sin(rotation),
            (gun_right.position_offset.X * MathF.Sin(rotation)) + (gun_right.position_offset.Y * MathF.Cos(rotation))
        );
    }
    public void move_right_gun_cross(MouseState mstate) {
        Vector2 mouse_position = new Vector2(mstate.Position.X, mstate.Position.Y);
        Vector2 direction = mouse_position - position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        if (direction.X < 0 && direction.Y < 0) {
            target_angle = (target_angle + MathF.PI * 2) % (MathF.PI * 2);
        }

        float angle_diff = rotation - target_angle - (float)(Math.PI / 2);
        float angle_diff_offset = target_angle + (float)(Math.PI / 2) - rotation;

        if (angle_diff_offset > MathF.PI / 9) {
            angle_diff_offset = MathF.PI / 9;
            if (angle_diff < -MathF.PI ) {
                angle_diff_offset = MathF.PI * 2 - angle_diff_offset;
            }
        } else if (angle_diff_offset < -MathF.PI / 9) {
            angle_diff_offset = -MathF.PI / 9;
            if (angle_diff > MathF.PI) {
                angle_diff_offset = MathF.PI * 2 - angle_diff_offset;
            }
        }

        gun_right.cross.position = position + new Vector2(
            (direction.X * MathF.Cos(angle_diff + angle_diff_offset)) + direction.Y * MathF.Sin(angle_diff + angle_diff_offset) * -1,
            (direction.X * MathF.Sin(angle_diff + angle_diff_offset)) + (direction.Y * MathF.Cos(angle_diff + angle_diff_offset))
        );
    }
    public void rotate_right_run() {
        Vector2 direction_right = gun_right.cross.position - gun_right.position;
        float target_angle_right = MathF.Atan2(direction_right.Y, direction_right.X);

        gun_right.rotation = target_angle_right + MathF.PI / 2;
    }
    public void move_right_gun_shots(GameTime gameTime) {
        gun_right.move_shots(gameTime, velocity);
    }


    public void update_cross() {
        move_cross();
    }

    public void move_cross() {
        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
        Vector2 direction = mouse_position - position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        if (direction.X < 0 && direction.Y < 0) {
            target_angle = (target_angle + MathF.PI * 2) % (MathF.PI * 2);
        }

        float angle_diff = rotation - target_angle - (float)(Math.PI / 2);

        cross.position = position + new Vector2(
            (direction.X * MathF.Cos(angle_diff)) + direction.Y * MathF.Sin(angle_diff) * -1,
            (direction.X * MathF.Sin(angle_diff)) + (direction.Y * MathF.Cos(angle_diff))
        );
    }


    public void draw_all(SpriteBatch sprite_batch) {
        draw_guns(sprite_batch);
        draw_spaceship(sprite_batch);
    }

    public void draw_spaceship(SpriteBatch sprite_batch) {
        sprite_batch.Draw(
            sprite,
            position,
            rectangle,
            Color.White,
            rotation,
            origin,
            Vector2.One,
            SpriteEffects.None,
            0f
        );
    }
    public void draw_guns(SpriteBatch sprite_batch) {
        gun_left.draw(sprite_batch);
        gun_right.draw(sprite_batch);
    }
    public void draw_crosses(SpriteBatch sprite_batch) {
        gun_left.cross.draw(sprite_batch);
        // gun_right.cross.draw(sprite_batch);
        cross.draw(sprite_batch);
    }
};

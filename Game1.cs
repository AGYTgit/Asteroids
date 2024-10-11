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
        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public class Entity {
        public Texture2D sprite;
        public Vector2 origin;
        public Vector2 position;
        public Vector2 direction;

        public float speed;
        public float speed_multiplier;
        public Vector2 velocity;

        public float secondary_speed;
        public float secondary_speed_multiplier;
        public Vector2 secondary_velocity;

        public float rotation;
        public float rotate_speed;
        public float rotate_speed_multiplier;

        public int health;

        public List<int> collision_list;
    }
    
    // readonly static int gun_count = 2;
    
    readonly Entity spaceship = new();
    readonly List<List<Entity>> shots_list = [[], []];
    readonly List<Entity> asteroid_list = [];

    protected override void Initialize() {
        init_spaceship();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    float shoot_cooldown_ms = 100f;
    float time_of_last_shoot = 0f;

    float spawn_cooldown_ms = 0f;
    float time_of_last_spawn = 0f;

    float despawn_cooldown_ms = 0f;
    float time_of_last_despawn = 0f;

    long game_time_ms = 0;

    protected override void Update(GameTime gameTime) {
        game_time_ms += gameTime.ElapsedGameTime.Milliseconds;

        update_from_input(game_time_ms);

        update_spaceship(gameTime);
        update_shots(gameTime);
        update_asteroids(gameTime);

        // ocassional crash when both shot and asteroid exists

        if (true) {
            List<List<int>> to_remove = validate_shots_position();
            if (to_remove.Count > 0) {
                for (int i = 0; i < to_remove.Count; i++) {
                    if (to_remove[i].Count > 0) {
                        remove_at_indices(shots_list[i], to_remove[1]);
                    }
                }
            }
        }

        if (true) {
            List<int> to_remove = validate_asteroids_position();
            remove_at_indices(asteroid_list, to_remove);
        }

        if (true) {
            List<List<int>> to_remove = check_for_shots_shots_collision();
            if (to_remove.Count > 0) {
                for (int i = 0; i < to_remove.Count; i++) {
                    if (to_remove[i].Count > 0) {
                        remove_at_indices(shots_list[i], to_remove[i]);
                    }
                }
            }
        }

        if (true) {
            List<List<int>> to_remove = check_for_shots_asteroids_collision(); // needs rework
            if (to_remove.Count > 2) {
                for (int i = 0; i < to_remove.Count - 1; i++) {
                    if (to_remove[i + 1].Count > 0) {
                        remove_at_indices(shots_list[i], to_remove[i + 1]);
                    }
                }
            }
            if (to_remove.Count > 0) {
                if (to_remove[0].Count > 0) {
                    remove_at_indices(asteroid_list, to_remove[0]);
                }
            }
        }

        if (true) {
            List<int> to_remove = check_for_spaceship_asteroids_collision();
            // if (to_remove.Count > 0) {
            //     spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
            // }
            remove_at_indices(asteroid_list, to_remove);
        }

        if (true) {
            List<int> to_remove = check_for_asteroids_asteroids_collision();
            remove_at_indices(asteroid_list, to_remove);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _spriteBatch.Draw(
            spaceship.sprite,
            spaceship.position,
            null,
            Color.White,
            spaceship.rotation,
            new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2),
            Vector2.One,
            SpriteEffects.None,
            0f
        );
        foreach (Entity asteroid in asteroid_list) {
            _spriteBatch.Draw(
                asteroid.sprite,
                asteroid.position,
                null,
                Color.White,
                asteroid.rotation,
                new Vector2(asteroid.sprite.Width / 2, asteroid.sprite.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
        foreach (List<Entity> shots in shots_list) {
            foreach (Entity shot in shots) {
                _spriteBatch.Draw(
                    shot.sprite,
                    shot.position,
                    null,
                    Color.White,
                    shot.rotation,
                    new Vector2(shot.sprite.Width / 2, shot.sprite.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }


    private void init_spaceship() {
        spaceship.sprite = Content.Load<Texture2D>("spaceship");
        spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        spaceship.origin = new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);

        spaceship.speed = 100f;
        spaceship.speed_multiplier = 1f;
        spaceship.velocity = new Vector2(0,0);

        spaceship.secondary_speed = 50f;
        spaceship.secondary_speed_multiplier = 1f;
        spaceship.secondary_velocity = new Vector2(0,0);
        
        spaceship.rotation = 0f;
        spaceship.rotate_speed = 5f;
        spaceship.rotate_speed_multiplier = 1f;

        spaceship.collision_list = [1];
    }


    private void create_asteroid() {
        Random random = new Random();
        Texture2D sprite_temp = Content.Load<Texture2D>("asteroid_1_1");
        float direction_X_temp = random.Next(2000) / 1000 * 2 - 1;
        float direction_Y_temp = (1 - direction_X_temp) * random.Next(2) * 2 - 1;
        Entity asteroid = new() {
            sprite = sprite_temp,
            origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
            position = new Vector2(
                random.Next(0, GraphicsDevice.Viewport.Width),
                random.Next(0, GraphicsDevice.Viewport.Height)
            ),
            direction = new Vector2(
                direction_X_temp,
                direction_Y_temp
            ),

            speed = random.Next(1,21),
            speed_multiplier = 1f,
            velocity = new Vector2(0,0),

            rotation = random.Next(0, (int)(Math.PI * 2 * 1000)) / 1000,
            rotate_speed = random.Next(60) * 2 - 60,
            rotate_speed_multiplier = 1f,

            collision_list = [1,2],
        };
        asteroid_list.Add(asteroid);
    }

    private void destroy_asteroid() {
        Random random = new();
        if (asteroid_list.Count > 0) {
            asteroid_list.RemoveAt(random.Next(0, asteroid_list.Count - 1));
        }
    }

    private void shoot(short mode=0) {
        Vector2 shot_position_offset = new(
            -13f * (float)Math.Cos(spaceship.rotation),
            -10f * (float)Math.Sin(spaceship.rotation)
        );;

        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
        Vector2 direction = mouse_position - spaceship.position + shot_position_offset;
        float target_angle = MathF.Atan2(direction.Y, direction.X);
        float angle_diff = target_angle + (float)(Math.PI / 2) - spaceship.rotation;

        Texture2D sprite_temp = Content.Load<Texture2D>("shot_1_1");

        if (mode == 0 || mode == 2) {
            Entity shot = new() {
                sprite = sprite_temp,
                origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
                position = spaceship.position + shot_position_offset,

                speed = 250,
                speed_multiplier = 1f,
                velocity = new Vector2(0,0),

                rotation = spaceship.rotation - angle_diff,
                rotate_speed = 0f,
                rotate_speed_multiplier = 0f,

                collision_list = [2],
            };
            shots_list[0].Add(shot);
        }
        if (mode == 1 || mode == 2) {
            Entity shot = new() {
                sprite = sprite_temp,
                origin = new Vector2(sprite_temp.Width / 2, sprite_temp.Height / 2),
                position = spaceship.position - shot_position_offset,

                speed = 250,
                speed_multiplier = 1f,
                velocity = new Vector2(0,0),

                rotation = spaceship.rotation + angle_diff,
                rotate_speed = 0f,
                rotate_speed_multiplier = 0f,

                collision_list = [2],
            };
            shots_list[1].Add(shot);
        }
    }


    public bool is_collision(Entity e1, Entity e2, float tolerance=10f) {
        float distance = Vector2.Distance(e1.position, e2.position);
        float e1_radius = e1.sprite.Width / 2 * (1 - tolerance / 100);
        float e2_radius = e2.sprite.Width / 2 * (1 - tolerance / 100);;
        return distance < (e1_radius + e2_radius);
    }

    public void remove_at_indices(List<Entity> entities, List<int> indices) {
        if (entities.Count == 0) {
            return;
        }

        indices.Sort((a, b) => b.CompareTo(a));
        foreach (int index in indices) {
            entities.RemoveAt(index);
        }
    }


    public void update_from_input(long game_time_ms) {
        var kstate = Keyboard.GetState();
        var mstate = Mouse.GetState();

        if (kstate.IsKeyDown(Keys.Escape)) {
            Exit();
        }

        if ((mstate.LeftButton == ButtonState.Pressed || mstate.RightButton == ButtonState.Pressed) && time_of_last_shoot < game_time_ms - shoot_cooldown_ms) { // shoot
            if (mstate.LeftButton == ButtonState.Pressed && mstate.RightButton == ButtonState.Pressed) {
                shoot(2);
            } else if (mstate.LeftButton == ButtonState.Pressed) {
                shoot(0);
            } else if (mstate.RightButton == ButtonState.Pressed) {
                shoot(1);
            }
            time_of_last_shoot = game_time_ms;
        }

        if (kstate.IsKeyDown(Keys.E)) { // move FORWARD
            spaceship.position += spaceship.velocity;
        }
        if (kstate.IsKeyDown(Keys.S) && !kstate.IsKeyDown(Keys.F)) { // move LEFT
            spaceship.position -= spaceship.secondary_velocity;
        } else if (kstate.IsKeyDown(Keys.F) && !kstate.IsKeyDown(Keys.S)) { // move RIGHT
            spaceship.position += spaceship.secondary_velocity;
        }

        if (kstate.IsKeyDown(Keys.R)) { // reset spaceship's position
            spaceship.position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2) - new Vector2(spaceship.sprite.Width / 2, spaceship.sprite.Height / 2);
        }

        if (kstate.IsKeyDown(Keys.Space)) { // speed boost
            spaceship.speed_multiplier = 8f;
            spaceship.secondary_speed_multiplier = 4f;
            spaceship.rotate_speed_multiplier = 4f;
        } else {
            spaceship.speed_multiplier = 1f;
            spaceship.secondary_speed_multiplier = 1f;
            spaceship.rotate_speed_multiplier = 1f;
        }

        if (kstate.IsKeyDown(Keys.W) && time_of_last_spawn < game_time_ms - spawn_cooldown_ms) { // spawn asteroids
            time_of_last_spawn = game_time_ms;
            create_asteroid();
        }

        if (kstate.IsKeyDown(Keys.Q) && time_of_last_despawn < game_time_ms - despawn_cooldown_ms) { // despawn asteroids
            time_of_last_despawn = game_time_ms;
            destroy_asteroid();
        }
    }


    public void update_spaceship(GameTime gameTime) {
        spaceship.velocity = new Vector2(
            spaceship.speed * spaceship.speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(spaceship.rotation),
            -1 * spaceship.speed * spaceship.speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(spaceship.rotation)
        );
        spaceship.secondary_velocity = new Vector2(
            spaceship.secondary_speed * spaceship.secondary_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(spaceship.rotation),
            spaceship.secondary_speed * spaceship.secondary_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(spaceship.rotation)
        );


        Vector2 mouse_position = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
        Vector2 direction = mouse_position - spaceship.position;
        float target_angle = MathF.Atan2(direction.Y, direction.X);

        spaceship.rotation = (spaceship.rotation + (float)(Math.PI * 2)) % (float)(Math.PI * 2);

        float angle_diff = target_angle + (float)(Math.PI / 2) - spaceship.rotation;

        if (angle_diff == 0f) {
            return;
        }

        if (angle_diff > MathF.PI) {
            angle_diff -= 2 * MathF.PI;
        }
        else if (angle_diff < -MathF.PI) {
            angle_diff += 2 * MathF.PI;
        }

        float rotate_amount = spaceship.rotate_speed * spaceship.rotate_speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (angle_diff > 0) {
            if (angle_diff < rotate_amount) {
                spaceship.rotation += angle_diff;
            } else {
                spaceship.rotation += rotate_amount;
            }
        } else if (angle_diff < 0) {
            if (0 - angle_diff < rotate_amount) {
                spaceship.rotation += angle_diff;
            } else {
                spaceship.rotation -= rotate_amount;
            }
        }
    }

    public void update_shots(GameTime gameTime) {
        for (int i = 0; i < shots_list.Count; i++) {
            for (int j = 0; j < shots_list[i].Count; j++) {
                shots_list[i][j].velocity = new Vector2(
                    shots_list[i][j].speed * shots_list[i][j].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Sin(shots_list[i][j].rotation),
                    -1 * shots_list[i][j].speed * shots_list[i][j].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)Math.Cos(shots_list[i][j].rotation)
                );

                shots_list[i][j].position += shots_list[i][j].velocity;
            }
        }
    }

    public void update_asteroids(GameTime gameTime) {
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            asteroid_list[i].velocity = new Vector2(
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.X,
                asteroid_list[i].speed * asteroid_list[i].speed_multiplier * (float)gameTime.ElapsedGameTime.TotalSeconds * asteroid_list[i].direction.Y
            );
            asteroid_list[i].position += asteroid_list[i].velocity;
            asteroid_list[i].rotation += (float)(asteroid_list[i].rotate_speed * asteroid_list[i].rotate_speed_multiplier / 360 * Math.PI * 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }


    public List<List<int>> validate_shots_position() {
        List<List<int>> to_remove = [[], []];
        for (int i = 0; i < shots_list.Count; i++) {
            for (int j = shots_list[i].Count - 1; j >= 0; j--) {
                if (
                    shots_list[i][j].position.X < 0 - (shots_list[i][j].sprite.Width / 2) || shots_list[i][j].position.X > _graphics.PreferredBackBufferWidth + (shots_list[i][j].sprite.Width / 2) ||
                    shots_list[i][j].position.Y < 0 - (shots_list[i][j].sprite.Height / 2) || shots_list[i][j].position.Y > _graphics.PreferredBackBufferHeight + (shots_list[i][j].sprite.Height / 2)
                    ) {
                    to_remove[i].Add(j);
                }
            }
        }

        return to_remove;
    }

    public List<int> validate_asteroids_position() {
        List<int> to_remove = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            if (
                asteroid_list[i].position.X < 0 - (asteroid_list[i].sprite.Height / 2) || asteroid_list[i].position.X > _graphics.PreferredBackBufferWidth + (asteroid_list[i].sprite.Height / 2) ||
                asteroid_list[i].position.Y < 0 - (asteroid_list[i].sprite.Height / 2) || asteroid_list[i].position.Y > _graphics.PreferredBackBufferHeight + (asteroid_list[i].sprite.Height / 2)
                ) {
                to_remove.Add(i);
            }
        }

        return to_remove;
    }


    public List<List<int>> check_for_shots_shots_collision() { // needs rework
        List<List<int>> to_remove = [[], []];
        for (int i = 0; i < shots_list.Count; i++) {
            for (int l = i - 1; l >= 0; l--) {
                for (int j = shots_list[i].Count - 1; j >= 0; j--) {
                    for (int k = j - 1; k >= 0; k--) {
                        // if (to_remove[i].Contains(j) || to_remove[i].Contains(l)) {
                        //     continue;
                        // }
                        if (is_collision(shots_list[l][j], shots_list[l][k])) {
                            to_remove[l].Add(j);
                            to_remove[l].Add(k);
                            break;
                        }
                    }
                }
            }
        }
        
        return to_remove;
    }

    public List<List<int>> check_for_shots_asteroids_collision() {
        List<List<int>> to_remove = [[], [], []];
        for (int i = 0; i < shots_list.Count; i++) {
            for (int j = shots_list[i].Count - 1; j >= 0; j--) {
                for (int l = asteroid_list.Count - 1; l >= 0; l--) {
                    if (to_remove[0].Contains(l)) {
                        continue;
                    }
                    if (is_collision(shots_list[i][j], asteroid_list[l])) {
                        to_remove[i + 1].Add(j);
                        to_remove[0].Add(l);
                        break;
                    }
                }
            }
        }

        return to_remove;
    }

    public List<int> check_for_spaceship_asteroids_collision() {
        List<int> to_remove = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            if (is_collision(spaceship, asteroid_list[i])) {
                to_remove.Add(i);
                continue;
            }
        }

        return to_remove;
    }

    public List<int> check_for_asteroids_asteroids_collision() {
        List<int> to_remove = [];
        for (int i = asteroid_list.Count - 1; i >= 0; i--) {
            for (int j = i - 1; j >= 0; j--) {
                if (is_collision(asteroid_list[i], asteroid_list[j])) {
                    if (asteroid_list[i].speed > asteroid_list[j].speed) {
                        to_remove.Add(j);
                        break;
                    } else if (asteroid_list[i].speed < asteroid_list[j].speed) {
                        to_remove.Add(i);
                        break;
                    }
                    
                    to_remove.Add(j);
                    to_remove.Add(i);
                    break;
                }
            }
        }

        return to_remove;
    }

}

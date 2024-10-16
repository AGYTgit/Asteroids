using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class Entity {
    public Texture2D sprite;
    public Vector2 origin;
    public Vector2 position;
    public float rotation;
}

public class Spaceship : Entity {
    public float speed;
    public int rotation_speed;

    public int health;

    public Gun gun_left;
    public Gun gun_right;

    public Entity cross;
    
    public Entity cross_2;
};

public class Gun : Entity {
    public Vector2 position_offset;

    public int ammo;
    public float firerate;
    public int rotation_speed;

    public long last_time_fired;

    public List<Shot> shots;
};

public class Shot : Entity {
    public float speed;
}

public class Asteroid : Entity;

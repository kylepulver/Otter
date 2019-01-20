using System;
using System.Collections.Generic;
using System.Linq;

namespace Otter {
    /// <summary>
    /// The base class used for collision detection.  Entities contain Colliders that can check for collisions
    /// which each other.  Note that some functions for detecting collisions can take generic Enums as their tags.
    /// Be aware that these functions are slightly slower than casting your Enums to int for the int tags functions.
    /// </summary>
    public abstract class Collider {

        #region Private Fields

        #endregion

        #region Public Fields

        /// <summary>
        /// The X position of the Collider relative to the Entity.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y position of the Collider relative to the Entity.
        /// </summary>
        public float Y;

        /// <summary>
        /// The OriginX of the Collider.
        /// </summary>
        public float OriginX;

        /// <summary>
        /// The OriginY of the Collider.
        /// </summary>
        public float OriginY;

        /// <summary>
        /// The entity that this collider is parented to.
        /// </summary>
        public Entity Entity;

        /// <summary>
        /// Determines if this collider is collidable.  If false, it will not register collisions with
        /// other colliders, but can still check for collisions with other colliders.
        /// </summary>
        public bool Collidable = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// The tags that this Collider is registered with.
        /// </summary>
        public List<int> Tags { get; protected set; }

        /// <summary>
        /// The width of the Collider.
        /// </summary>
        public virtual float Width { get; set; }

        /// <summary>
        /// The height of the Collider.
        /// </summary>
        public virtual float Height { get; set; }

        /// <summary>
        /// The X position of the center of the Collider.
        /// </summary>
        public virtual float CenterX {
            get { return X + Entity.X - OriginX + HalfWidth; }
        }

        /// <summary>
        /// The Y position of the center of the Collider.
        /// </summary>
        public virtual float CenterY {
            get { return Y + Entity.Y - OriginY + HalfHeight; }
        }

        /// <summary>
        /// The X position of the left side of the Collider.
        /// </summary>
        public virtual float Left {
            get { return X + Entity.X - OriginX; }
        }

        /// <summary>
        /// The X position of the right side of the Collider.
        /// </summary>
        public virtual float Right {
            get { return X + Entity.X + Width - OriginX; }
        }

        /// <summary>
        /// The Y position of the top of the Collider.
        /// </summary>
        public virtual float Top {
            get { return Y + Entity.Y - OriginY; }
        }

        /// <summary>
        /// The Y position of the bottom of the Collider.
        /// </summary>
        public virtual float Bottom {
            get { return Y + Entity.Y + Height - OriginY; }
        }

        /// <summary>
        /// Half of the Collider's height.
        /// </summary>
        public float HalfHeight {
            get { return Height / 2f; }
        }

        /// <summary>
        /// Half of the Collider's width.
        /// </summary>
        public float HalfWidth {
            get { return Width / 2f; }
        }

        #endregion

        #region Constructors

        internal Collider() {
            Tags = new List<int>();
            Width = 0;
            Height = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks for a collision with a specific point.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="px">The x of the point to check.</param>
        /// <param name="py">The y of the point to check.</param>
        /// <returns>True if the Collider at position x, y overlaps px, py.</returns>
        public bool Overlap(float x, float y, int px, int py) {
            var point = new PointCollider(px, py);
            var e = new Entity(0, 0, null, point);
            return OverlapTest(this, point);
        }

        /// <summary>
        /// Checks for a collision against the specified tags and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, params int[] tags) {
            return (Collide(x, y, tags) != null);
        }

        /// <summary>
        /// Checks for a collision against a specific Collider and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="c">The Collider to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, Collider c) {
            return (Collide(x, y, c) != null);
        }

        /// <summary>
        /// Checks for a collision against a specific Entity and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="e">The Collider to check.</param>
        /// <returns>True of there was a collision.</returns>
        public bool Overlap(float x, float y, Entity e) {
            return (Collide(x, y, e) != null);
        }

        /// <summary>
        /// Checks for a collision against a list of Collliders and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The list of colliders to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, List<int> tags) {
            return (Collide(x, y, tags) != null);
        }

        /// <summary>
        /// Checks for a collision against the specified tags and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, params Enum[] tags) {
            return (Collide(x, y, tags) != null);
        }

        /// <summary>
        /// Checks for a collision against the specified tags and returns true or false.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, List<Enum> tags) {
            return (Collide(x, y, tags) != null);
        }

        /// <summary>
        /// Checks for a collision against a list of Entities and all of their colliders.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The Entities to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap(float x, float y, List<Entity> entities) {
            return (Collide(x, y, entities) != null);
        }

        /// <summary>
        /// Checks for a collision against a list of Entities and all of their colliders.
        /// </summary>
        /// <typeparam name="T">The type of entity to check.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The Entities to check.</param>
        /// <returns>True if there was a collision.</returns>
        public bool Overlap<T>(float x, float y, List<T> entities) where T : Entity {
            return (Collide(x, y, entities.ToList<Entity>()) != null);
        }

        /// <summary>
        /// Checks for a collision against a list of Entities and all of their colliders.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The Entities to check.</param>
        /// <returns>True if there was a collision.</returns>
        public Collider Collide(float x, float y, List<Entity> entities) {
            foreach (var e in entities) {
                var c = Collide(x, y, e);
                if (c != null) return c;
            }
            return null;
        }

        public Collider Collide<T>(float x, float y, List<T> entities) where T : Entity {
            return Collide(x, y, entities.ToList<Entity>());
        }

        /// <summary>
        /// Checks for a collision against specified tags.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The collider that was hit first.</returns>
        public Collider Collide(float x, float y, params int[] tags) {
            if (Entity == null) return null;
            if (Entity.Scene == null) return null;

            float tempX = Entity.X, tempY = Entity.Y;
            Entity.X = x;
            Entity.Y = y;

            if (tags.Length == 0) {
                tags = new int[Entity.Scene.Colliders.Count];
                var id = 0;
                foreach (int i in Entity.Scene.Colliders.Keys) {
                    tags[id] = i;
                    id++;
                }
            }

            foreach (int t in tags) {
                if (Entity.Scene.Colliders.ContainsKey(t)) {
                    foreach (Collider c in Entity.Scene.Colliders[t]) {
                        if (c.Entity != null) {
                            if (!c.Entity.Collidable) continue;
                            if (!c.Collidable) continue;
                            if (c.Entity == Entity) continue;
                        }
                        else { // Emergency back up fix. Colliders with no entity should NOT be checked, or in the list.
                            continue;
                        }
                        if (OverlapTest(this, c)) {
                            Entity.X = tempX;
                            Entity.Y = tempY;
                            return c;
                        }
                    }
                }
            }

            Entity.X = tempX;
            Entity.Y = tempY;
            return null;
        }

        /// <summary>
        /// Checks for a collision against specified tags.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The collider that was hit first.</returns>
        public Collider Collide(float x, float y, params Enum[] tags) {
            return Collide(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Checks for a collision with a specific collider.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="c">The collider to check for.</param>
        /// <returns>The collider that was hit first.</returns>
        public Collider Collide(float x, float y, Collider c) {
            if (Entity == null) return null;
            if (Entity.Scene == null) return null;
            if (!c.Entity.Collidable) return null;
            if (!c.Collidable) return null;

            float tempX = Entity.X, tempY = Entity.Y;
            Entity.X = x;
            Entity.Y = y;

            if (OverlapTest(this, c)) {
                Entity.X = tempX;
                Entity.Y = tempY;
                return c;
            }

            Entity.X = tempX;
            Entity.Y = tempY;
            return null;
        }

        /// <summary>
        /// Checks for a collision with a specific entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="e">The entity to check for.</param>
        /// <returns>The collider that was hit.</returns>
        public Collider Collide(float x, float y, Entity e) {
            if (Entity == null) return null;
            if (Entity.Scene == null) return null;
            if (Entity == e) return null; // Can't collide with self
            if (!Entity.Collidable) return null;

            float tempX = Entity.X, tempY = Entity.Y;
            Entity.X = x;
            Entity.Y = y;

            foreach (Collider c in e.Colliders) {
                if (!c.Collidable) continue;
                if (OverlapTest(this, c)) {
                    Entity.X = tempX;
                    Entity.Y = tempY;
                    return c;
                }
            }

            Entity.X = tempX;
            Entity.Y = tempY;
            return null;
        }

        /// <summary>
        /// Checks for a collision with a list of tags.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Collider that was hit.</returns>
        public Collider Collide(float x, float y, List<int> tags) {
            Collider result;
            foreach (int c in tags) {
                result = Collide(x, y, c);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks for a collision with a list of tags.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Collider that was hit.</returns>
        public Collider Collide(float x, float y, List<Enum> tags) {
            return Collide(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, params int[] tags) {
            var c = Collide(x, y, tags);
            if (c == null) return null;
            return c.Entity;
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, params int[] tags) where T : Entity {
            return (T)CollideEntity(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, params Enum[] tags) {
            return CollideEntity(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, params Enum[] tags) where T : Entity {
            return (T)CollideEntity(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with a specific Entity and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="e">The Entity to check for.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, Entity e) {
            return Collide(x, y, e).Entity;
        }

        /// <summary>
        /// Checks for a collision with a specific Entity and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="e">The Entity to check for.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, Entity e) where T : Entity {
            return (T)CollideEntity(x, y, e);
        }

        /// <summary>
        /// Checks for a collision with a specific Collider and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="c">The Collider to check for.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, Collider c) {
            return Collide(x, y, c).Entity;
        }

        /// <summary>
        /// Checks for a collision with a specific Collider and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="c">The Collider to check for.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, Collider c) where T : Entity {
            return (T)CollideEntity(x, y, c);
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The list of tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, List<int> tags) {
            foreach (int t in tags) {
                Collider c;
                c = Collide(x, y, t);
                if (c != null) {
                    return c.Entity;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks for a collision with a list of tags and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The list of tags to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, List<int> tags) where T : Entity {
            return (T)CollideEntity(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with a list of Entities and returns an Entity.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The list of Entities to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public Entity CollideEntity(float x, float y, List<Entity> entities) {
            if (Collide(x, y, entities) == null) return null;
            return Collide(x, y, entities).Entity;
        }

        /// <summary>
        /// Checks for a collision with a list of Entities and returns an Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The list of Entities to check.</param>
        /// <returns>The Entity that was hit.</returns>
        public T CollideEntity<T>(float x, float y, List<T> entities) where T : Entity {
            return (T)CollideEntity(x, y, entities.ToList<Entity>()); // This blows up for some reason sometimes?
        }

        /// <summary>
        /// Creates a list of Colliders that this Collider is colliding with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of colliders.</returns>
        public List<Collider> CollideList(float x, float y, params int[] tags) {
            List<Collider> collided = new List<Collider>();
            if (Entity == null) return null;
            if (Entity.Scene == null) return null;

            float tempX = Entity.X, tempY = Entity.Y;
            Entity.X = x;
            Entity.Y = y;

            if (tags.Length == 0) {
                tags = new int[Entity.Scene.Colliders.Count];
                var id = 0;
                foreach (int i in Entity.Scene.Colliders.Keys) {
                    tags[id] = i;
                    id++;
                }
            }

            foreach (int t in tags) {
                if (Entity.Scene.Colliders.ContainsKey(t)) {
                    foreach (var c in Entity.Scene.Colliders[t]) {
                        if (c.Entity != null) {
                            if (!c.Entity.Collidable) continue;
                            if (!c.Collidable) continue;
                        }
                        if (OverlapTest(this, c)) {
                            collided.Add(c);
                        }
                    }
                }
            }

            Entity.X = tempX;
            Entity.Y = tempY;

            return collided;
        }

        /// <summary>
        /// Creates a list of Colliders that this Collider is colliding with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of colliders.</returns>
        public List<Collider> CollideList(float x, float y, params Enum[] tags) {
            return CollideList(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Creates a list of Colliders that this Collider is colliding with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of colliders.</returns>
        public List<Collider> CollideList(float x, float y, List<Enum> tags) {
            return CollideList(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Creates a list of Colliders that this Collider is colliding with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of colliders.</returns>
        public List<Collider> CollideList(float x, float y, List<int> tags) {
            List<Collider> collided = new List<Collider>();
            List<Collider> c;

            foreach (int tag in tags) {
                c = CollideList(x, y, tag);
                foreach (Collider col in c) {
                    collided.Add(col);
                }
            }

            return collided;
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<Entity> CollideEntities(float x, float y, params int[] tags) {
            List<Entity> collided = new List<Entity>();

            List<Collider> clist = CollideList(x, y, tags);
            foreach (Collider c in clist) {
                if (!collided.Contains(c.Entity)) {
                    if (c.Entity != Entity) {
                        collided.Add(c.Entity);
                    }
                }
            }

            return collided;
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<Entity> CollideEntities(float x, float y, params Enum[] tags) {
            return CollideEntities(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<Entity> CollideEntities(float x, float y, List<Enum> tags) {
            return CollideEntities(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <typeparam name="T">The type of list to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<T> CollideEntities<T>(float x, float y, params int[] tags) where T : Entity {
            return CollideEntities(x, y, tags).Cast<T>().ToList();
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <typeparam name="T">The type of list to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<T> CollideEntities<T>(float x, float y, params Enum[] tags) where T : Entity {
            return CollideEntities(x, y, tags).Cast<T>().ToList();
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <typeparam name="T">The type of list to return.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<T> CollideEntities<T>(float x, float y, List<Enum> tags) where T : Entity {
            return CollideEntities(x, y, tags).Cast<T>().ToList();
        }

        /// <summary>
        /// Creates a list of Entities that the Collider has collided with.
        /// </summary>
        /// <typeparam name="T">The type of Entity.</typeparam>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="entities">The Entities to check.</param>
        /// <returns>A list of entities.</returns>
        public List<T> CollideEntities<T>(float x, float y, List<T> entities) where T : Entity {
            var list = new List<T>();

            foreach (var e in entities) {
                var c = Collide(x, y, e);
                if (c != null) {
                    if (c.Entity != null) {
                        list.Add(e);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Creates a list of entities that the Collider has collided with.
        /// </summary>
        /// <param name="x">The x position to check.</param>
        /// <param name="y">The y position to check.</param>
        /// <param name="tags">The tags to check.</param>
        /// <returns>A list of entities.</returns>
        public List<Entity> CollideEntities(float x, float y, List<int> tags) {
            List<Entity> collided = new List<Entity>();
            List<Collider> c;

            foreach (int tag in tags) {
                c = CollideList(x, y, tag);
                foreach (Collider col in c) {
                    if (collided.Contains(col.Entity)) continue;
                    collided.Add(col.Entity);
                }
            }

            return collided;
        }

        /// <summary>
        /// Callback for when the Collider has been added to an Entity.
        /// </summary>
        public virtual void Added() {

        }

        /// <summary>
        /// Callback for when the Collider has been removed from an Entity.
        /// </summary>
        public virtual void Removed() {

        }

        /// <summary>
        /// Callback for when the Collider renders (usually for debugging purposes.)
        /// </summary>
        public virtual void Render(Color color = null) {

        }

        /// <summary>
        /// Add a new tag to the Collider.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns>The Collider.</returns>
        public Collider AddTag(int tag) {
            if (Tags.Contains(tag)) return this;

            if (Entity != null) {
                if (Entity.Scene != null) {
                    Entity.Scene.RemoveColliderInternal(this);
                }
            }

            Tags.Add(tag);

            if (Entity != null) {
                if (Entity.Scene != null) {
                    Entity.Scene.AddColliderInternal(this);
                }
            }

            return this;
        }

        /// <summary>
        /// Add new tags to the Collider.
        /// </summary>
        /// <param name="tags">The tags to add.</param>
        /// <returns>The Collider.</returns>
        public Collider AddTag(params Enum[] tags) {
            AddTag(Util.EnumToIntArray(tags));

            return this;
        }

        public bool HasTag(params Enum[] tags) {
            return HasTag(Util.EnumToIntArray(tags));
        }

        public bool HasTag(params int[] tags) {
            foreach (var t in tags) {
                if (Tags.Contains(t)) return true;
            }
            return false;
        }

        /// <summary>
        /// Add new tags to the Collider.
        /// </summary>
        /// <param name="tags">The tags to add.</param>
        /// <returns>The Collider.</returns>
        public Collider AddTag(params int[] tags) {
            foreach (int t in tags) {
                AddTag(t);
            }

            return this;
        }

        /// <summary>
        /// Remove a tag from the Collider.
        /// </summary>
        /// <param name="tag">The tags to remove.</param>
        /// <returns>The Collider.</returns>
        public Collider RemoveTag(int tag) {
            if (Entity != null) {
                if (Entity.Scene != null) {
                    Entity.Scene.RemoveColliderInternal(this);
                }
            }

            Tags.Remove(tag);

            if (Entity != null) {
                if (Entity.Scene != null) {
                    Entity.Scene.AddColliderInternal(this);
                }
            }

            return this;
        }

        /// <summary>
        /// Remove tags from the Collider.
        /// </summary>
        /// <param name="tags">The tags to remove.</param>
        /// <returns>The Collider.</returns>
        public Collider RemoveTag(params int[] tags) {
            foreach (int t in tags) {
                RemoveTag(t);
            }

            return this;
        }

        /// <summary>
        /// Remove tags from the Collider.
        /// </summary>
        /// <param name="tags">The tags to remove.</param>
        /// <returns>The Collider.</returns>
        public Collider RemoveTag(params Enum[] tags) {
            RemoveTag(Util.EnumToIntArray(tags));

            return this;
        }

        /// <summary>
        /// Center the origin of the Collider.  Based off of Width and Height.
        /// </summary>
        public virtual void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = HalfHeight;
        }

        /// <summary>
        /// Set the positon of the Collider.
        /// </summary>
        /// <param name="x">The X position of the Collider.</param>
        /// <param name="y">The Y position of the Collider.</param>
        public void SetPosition(float x, float y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Set the position of the Collider.
        /// </summary>
        /// <param name="xy">The Vector2 position of the Collider.</param>
        public void SetPosition(Vector2 xy) {
            SetPosition(xy.X, xy.Y);
        }

        /// <summary>
        /// Set the origin of the Collider 
        /// </summary>
        /// <param name="x">The X origin of the Collider.</param>
        /// <param name="y">The Y origin of the Collider.</param>
        public void SetOrigin(float x, float y) {
            OriginX = x;
            OriginY = y;
        }

        /// <summary>
        /// Set the origin of the Collider.
        /// </summary>
        /// <param name="xy">The Vector2 origin of the Collider.</param>
        public void SetOrigin(Vector2 xy) {
            SetOrigin(xy.X, xy.Y);
        }

        #endregion

        #region Internal

        /// <summary>
        /// Giant function that checks for overlaps between many different types
        /// </summary>
        /// <param name="first">The first collider to test.</param>
        /// <param name="second">The second collider to test.</param>
        /// <returns>True if overlapping something</returns>
        internal static bool OverlapTest(Collider first, Collider second) {
            #region Box vs Box
            if (first is BoxCollider && second is BoxCollider) {
                if (first.Right <= second.Left) return false;
                if (first.Bottom <= second.Top) return false;
                if (first.Left >= second.Right) return false;
                if (first.Top >= second.Bottom) return false;
                return true;
            }
            #endregion

            #region Box vs Circle
            else if ((first is BoxCollider && second is CircleCollider) || (first is CircleCollider && second is BoxCollider)) {
                CircleCollider circle;
                BoxCollider box;
                if (first is CircleCollider) {
                    circle = first as CircleCollider;
                    box = second as BoxCollider;
                }
                else {
                    circle = second as CircleCollider;
                    box = first as BoxCollider;
                }

                //check is c center point is in the rect
                if (Util.InRect(circle.CenterX, circle.CenterY, box.Left, box.Top, box.Width, box.Height)) {
                    return true;
                }

                //check to see if any corners are in the circle
                if (Util.DistanceRectPoint(circle.CenterX, circle.CenterY, box.Left, box.Top, box.Width, box.Height) < circle.Radius) {
                    return true;
                }

                //check to see if any lines on the box intersect the circle
                Line2 boxLine;

                boxLine = new Line2(box.Left, box.Top, box.Right, box.Top);
                if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                boxLine = new Line2(box.Right, box.Top, box.Right, box.Bottom);
                if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                boxLine = new Line2(box.Right, box.Bottom, box.Left, box.Bottom);
                if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                boxLine = new Line2(box.Left, box.Bottom, box.Left, box.Top);
                if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                return false;
            }
            #endregion

            #region Box vs Grid
            else if ((first is BoxCollider && second is GridCollider) || (first is GridCollider && second is BoxCollider)) {
                BoxCollider box;
                GridCollider grid;
                if (first is BoxCollider) {
                    box = first as BoxCollider;
                    grid = second as GridCollider;
                }
                else {
                    box = second as BoxCollider;
                    grid = first as GridCollider;
                }

                if (box.Right <= grid.Left) return false;
                if (box.Bottom <= grid.Top) return false;
                if (box.Left >= grid.Right) return false;
                if (box.Top >= grid.Bottom) return false;

                //This is a quick fix and might have to be addressed later
                if (grid.GetRect(box.Left, box.Top, box.Right - 1, box.Bottom - 1, false)) return true;
                return false;
            }
            #endregion

            #region Box vs Line
            else if ((first is BoxCollider && second is LineCollider) || (first is LineCollider && second is BoxCollider)) {
                BoxCollider box;
                LineCollider line;
                if (first is BoxCollider) {
                    box = first as BoxCollider;
                    line = second as LineCollider;
                }
                else {
                    box = second as BoxCollider;
                    line = first as LineCollider;
                }

                if (line.Line2.IntersectsRect(box.Left, box.Top, box.Width, box.Height)) return true;

                return false;
            }
            #endregion
                
            #region Circle vs Circle
            else if (first is CircleCollider && second is CircleCollider) {
                CircleCollider
                    circle1 = first as CircleCollider,
                    circle2 = second as CircleCollider;
                if (Util.Distance(circle1.CenterX, circle1.CenterY, circle2.CenterX, circle2.CenterY) < circle1.Radius + circle2.Radius) {
                    return true;
                }
                return false;
            }
            #endregion

            #region Circle vs Grid
                        else if ((first is CircleCollider && second is GridCollider) || (first is GridCollider && second is CircleCollider)) {
                            //make a rectangle out of the circle, check for any tiles in that rectangle
                            //if there are tiles, check each tile as a rectangle against the circle
                            CircleCollider circle;
                            GridCollider grid;
                            if (first is CircleCollider) {
                                circle = first as CircleCollider;
                                grid = second as GridCollider;
                            }
                            else {
                                circle = second as CircleCollider;
                                grid = first as GridCollider;
                            }

                            int gridx, gridy, gridx2, gridy2;
                            gridx = (int)(Util.SnapToGrid(circle.Left - grid.Left, grid.TileWidth) / grid.TileWidth);
                            gridy = (int)(Util.SnapToGrid(circle.Top - grid.Top, grid.TileHeight) / grid.TileHeight);
                            gridx2 = (int)(Util.SnapToGrid(circle.Right - grid.Left, grid.TileWidth) / grid.TileWidth);
                            gridy2 = (int)(Util.SnapToGrid(circle.Bottom - grid.Top, grid.TileHeight) / grid.TileHeight);

                            //if (grid.GetRect(gridx, gridy, gridx2, gridy2, false)) {
                            if (grid.GetRect(circle.Left, circle.Top, circle.Right, circle.Bottom, false)) {
                                float rectX, rectY;
                                for (int i = gridx; i <= gridx2; i++) {
                                    for (int j = gridy; j <= gridy2; j++) {
                                        if (grid.GetTile(i, j)) {
                                            rectX = (i * grid.TileWidth) + grid.Left;
                                            rectY = (j * grid.TileHeight) + grid.Top;

                                            //check is c center point is in the rect
                                            if (Util.InRect(circle.CenterX, circle.CenterY, rectX, rectY, grid.TileWidth, grid.TileHeight)) {
                                                return true;
                                            }

                                            //check to see if any corners are in the circle
                                            if (Util.DistanceRectPoint(circle.CenterX, circle.CenterY, rectX, rectY, grid.TileWidth, grid.TileHeight) < circle.Radius) {
                                                //return true;
                                            }

                                            //check to see if any lines on the box intersect the circle
                                            Line2 boxLine;

                                            boxLine = new Line2(rectX, rectY, rectX + grid.TileWidth, rectY);
                                            if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                                            boxLine = new Line2(rectX + grid.TileWidth, rectY, rectX + grid.TileWidth, rectY + grid.TileHeight);
                                            if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                                            boxLine = new Line2(rectX + grid.TileWidth, rectY + grid.TileHeight, rectX, rectY + grid.TileHeight);
                                            if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                                            boxLine = new Line2(rectX, rectY + grid.TileHeight, rectX, rectY);
                                            if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) return true;

                                        }
                                    }
                                }
                            }
                            return false;

                        }
                        #endregion

            #region Line vs Line
            else if (first is LineCollider && second is LineCollider) {
                return (first as LineCollider).Line2.Intersects((second as LineCollider).Line2);
            }
            #endregion

            #region Line vs Grid
            else if ((first is LineCollider && second is GridCollider) || (first is GridCollider && second is LineCollider)) {
                //check any tiles along the line segment, somehow?
                LineCollider line;
                GridCollider grid;
                if (first is LineCollider) {
                    line = first as LineCollider;
                    grid = second as GridCollider;
                }
                else {
                    line = second as LineCollider;
                    grid = first as GridCollider;
                }

                //make a rectangle out of the line segment, check for any tiles in that rectangle

                //if there are tiles in there, loop through and check each one as a rectangle against the line
                if (grid.GetRect(line.Left, line.Top, line.Right, line.Bottom, false)) {
                    float rectX, rectY;
                    int
                        gridx = grid.GridX(line.Left),
                        gridy = grid.GridY(line.Top),
                        gridx2 = grid.GridX(line.Right),
                        gridy2 = grid.GridY(line.Bottom);

                    for (int i = gridx; i <= gridx2; i++) {
                        for (int j = gridy; j <= gridy2; j++) {
                            if (grid.GetTile(i, j)) {
                                rectX = i * grid.TileWidth + grid.Left;
                                rectY = j * grid.TileHeight + grid.Top;
                                if (Util.InRect((float)line.Line2.PointA.X, (float)line.Line2.PointA.Y, rectX, rectY, grid.TileWidth, grid.TileHeight)) {
                                    return true;
                                }
                                if (Util.InRect((float)line.Line2.PointB.X, (float)line.Line2.PointB.Y, rectX, rectY, grid.TileWidth, grid.TileHeight)) {
                                    return true;
                                }
                                if (line.Line2.IntersectsRect(rectX, rectY, grid.TileWidth, grid.TileHeight)) {
                                    return true;
                                }

                            }
                        }
                    }
                }
                return false;

            }
            #endregion

            #region Line vs Circle
            else if ((first is LineCollider && second is CircleCollider) || (first is CircleCollider && second is LineCollider)) {
                CircleCollider circle;
                LineCollider line;
                if (first is LineCollider) {
                    line = first as LineCollider;
                    circle = second as CircleCollider;
                }
                else {
                    line = second as LineCollider;
                    circle = first as CircleCollider;
                }

                if (line.Line2.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) {
                    return true;
                }
                return false;
            }
            #endregion
                
            #region Grid vs Grid
            else if (first is GridCollider && second is GridCollider) {
                //loop through one grid, check tile as rectangle in second grid?
                //first check if grids even touch with basic box test
                //then check each tile on first as a rect against second's tilemap
                //maybe optimize by looping through the smaller (area wise) tilemap

                if (!Util.IntersectRectangles(first.Left, first.Top, first.Width, first.Height, second.Left, second.Top, second.Width, second.Height)) {
                    return false;
                }

                GridCollider small, large;

                if ((first as GridCollider).TileArea < (second as GridCollider).TileArea) {
                    small = first as GridCollider;
                    large = second as GridCollider;
                }
                else {
                    small = second as GridCollider;
                    large = first as GridCollider;
                }

                for (int i = 0; i < small.TileColumns; i++) {
                    for (int j = 0; j < small.TileRows; j++) {
                        if (small.GetTile(i, j)) {
                            //check rects
                            float rectx, recty;
                            rectx = i * small.TileWidth + small.Left;
                            recty = j * small.TileHeight + small.Top;
                            if (large.GetRect(rectx, recty, rectx + small.TileWidth, recty + small.TileHeight, false)) {
                                return true;
                            }
                        }
                    }
                }

                return false;

            }
            #endregion

            #region Point vs Point
            else if (first is PointCollider && second is PointCollider) {
                if (first.Left != second.Left) return false;
                if (first.Top != second.Top) return false;
                return true;
            }
            #endregion

            #region Point vs Circle
            else if ((first is PointCollider && second is CircleCollider) || (first is CircleCollider && second is PointCollider)) {
                PointCollider point;
                CircleCollider circle;

                if (first is PointCollider) {
                    point = first as PointCollider;
                    circle = second as CircleCollider;
                }
                else {
                    point = second as PointCollider;
                    circle = first as CircleCollider;
                }

                if (Util.Distance(point.Left, point.Top, circle.CenterX, circle.CenterY) < circle.Radius) return true;
                return false;
            }
            #endregion

            #region Point vs Box
            else if ((first is PointCollider && second is BoxCollider) || (first is BoxCollider && second is PointCollider)) {
                PointCollider point;
                BoxCollider box;

                if (first is PointCollider) {
                    point = first as PointCollider;
                    box = second as BoxCollider;
                }
                else {
                    point = second as PointCollider;
                    box = first as BoxCollider;
                }

                if (Util.InRect(point.Left, point.Top, box.Left, box.Top, box.Width, box.Height)) return true;
                return false;
            }
            #endregion

            #region Point vs Grid
            else if ((first is PointCollider && second is GridCollider) || (first is GridCollider && second is PointCollider)) {
                PointCollider point;
                GridCollider grid;

                if (first is PointCollider) {
                    point = first as PointCollider;
                    grid = second as GridCollider;
                }
                else {
                    point = second as PointCollider;
                    grid = first as GridCollider;
                }

                int gridx, gridy;
                gridx = (int)Util.SnapToGrid(point.Left, grid.TileWidth) / grid.TileWidth;
                gridy = (int)Util.SnapToGrid(point.Top, grid.TileHeight) / grid.TileHeight;

                if (grid.GetTile(gridx, gridy)) return true;
                return false;
            }
            #endregion

            #region Point vs Line
            else if ((first is PointCollider && second is LineCollider) || (first is LineCollider && second is PointCollider)) {
                PointCollider point;
                LineCollider line;

                if (first is PointCollider) {
                    point = first as PointCollider;
                    line = second as LineCollider;
                }
                else {
                    point = second as PointCollider;
                    line = first as LineCollider;
                }

                //first take care of weird cases that might result in division by 0
                Line2 line2 = line.Line2;
                if (line2.X1 == line2.X2) {
                    if (line2.Y1 == line2.Y2) {
                        if (point.Left == line2.X1 && point.Top == line2.Y1) {
                            return true;
                        }
                    }
                    if (point.Left == line2.X1 && point.Top >= Math.Min(line2.Y1, line2.Y2) && point.Top <= Math.Max(line2.Y1, line2.Y2)) {
                        return true;
                    }
                }
                if (line2.Y1 == line2.Y2) {
                    if (point.Top == line2.Y1 && point.Left >= Math.Min(line2.X1, line2.X2) && point.Left <= Math.Max(line2.X1, line2.X2)) {
                        return true;
                    }
                }

                //if no special cases, this should work!
                if ((point.Left - line2.X1) / (line2.X2 - line2.X1) == (point.Top - line2.Y1) / (line2.Y2 - line2.Y1)) return true;
                return false;
            }
            #endregion

            #region Pixel vs Pixel
            else if ((first is PixelCollider && second is PixelCollider)) {
                //AABB test first

                var pixel1 = first as PixelCollider;
                var pixel2 = second as PixelCollider;

                if (first.Right <= second.Left) return false;
                if (first.Left >= second.Right) return false;
                if (first.Top >= second.Bottom) return false;
                if (first.Bottom <= second.Top) return false;

                //get intersecting area

                float x1 = Math.Max(first.Left, second.Left);
                float x2 = Math.Min(first.Right, second.Right);

                float y1 = Math.Max(first.Top, second.Top);
                float y2 = Math.Min(first.Bottom, second.Bottom);

                //scan both pixels and see if they collide

                var p1 = first as PixelCollider;
                var p2 = second as PixelCollider;
                for (var i = x1; i < x2; i++) {
                    for (var j = y1; j < y2; j++) {
                        var xx = (int)Math.Floor(i - first.Left);
                        var yy = (int)Math.Floor(j - first.Top);

                        if (p1.PixelAt(xx, yy)) {
                            xx = (int)Math.Floor(i - second.Left);
                            yy = (int)Math.Floor(j - second.Top);
                            if (p2.PixelAt(xx, yy)) {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            #endregion

            #region Pixel vs Box
            else if ((first is PixelCollider && second is BoxCollider) || (first is BoxCollider && second is PixelCollider)) {
                PixelCollider pixel;
                BoxCollider box;

                if (first is PixelCollider) {
                    pixel = first as PixelCollider;
                    box = second as BoxCollider;
                }
                else {
                    pixel = second as PixelCollider;
                    box = first as BoxCollider;
                }

                //AABB test

                if (box.Right <= pixel.Left) return false;
                if (box.Left >= pixel.Right) return false;
                if (box.Top >= pixel.Bottom) return false;
                if (box.Bottom <= pixel.Top) return false;

                //get intersecting area

                float x1 = Math.Max(box.Left, pixel.Left);
                float x2 = Math.Min(box.Right, pixel.Right);

                float y1 = Math.Max(box.Top, pixel.Top);
                float y2 = Math.Min(box.Bottom, pixel.Bottom);

                //scan for pixels in area

                for (var i = x1; i < x2; i++) {
                    for (var j = y1; j < y2; j++) {
                        var xx = (int)Math.Floor(i - pixel.Left);
                        var yy = (int)Math.Floor(j - pixel.Top);

                        if (pixel.PixelAt(xx, yy)) {
                            return true;
                        }
                    }
                }

                return false;

            }
            #endregion

            #region Pixel vs Circle
            else if ((first is PixelCollider && second is CircleCollider) || (first is CircleCollider && second is PixelCollider)) {
                PixelCollider pixel;
                CircleCollider circle;

                if (first is PixelCollider) {
                    pixel = first as PixelCollider;
                    circle = second as CircleCollider;
                }
                else {
                    pixel = second as PixelCollider;
                    circle = first as CircleCollider;
                }

                //circle to rectangle collision first

                bool firstCollisionCheck = false;

                //check is c center point is in the rect
                if (Util.InRect(circle.CenterX, circle.CenterY, pixel.Left, pixel.Top, pixel.Width, pixel.Height)) {
                    firstCollisionCheck = true;
                }

                if (!firstCollisionCheck) {
                    //check to see if any corners are in the circle
                    if (Util.DistanceRectPoint(circle.CenterX, circle.CenterY, pixel.Left, pixel.Top, pixel.Width, pixel.Height) < circle.Radius) {
                        firstCollisionCheck = true;
                    }
                }

                if (!firstCollisionCheck) {
                    //check to see if any lines on the box intersect the circle
                    Line2 boxLine;

                    boxLine = new Line2(pixel.Left, pixel.Top, pixel.Right, pixel.Top);
                    if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) firstCollisionCheck = true;

                    if (!firstCollisionCheck) {
                        boxLine = new Line2(pixel.Right, pixel.Top, pixel.Right, pixel.Bottom);
                        if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) firstCollisionCheck = true;
                    }

                    if (!firstCollisionCheck) {
                        boxLine = new Line2(pixel.Right, pixel.Bottom, pixel.Left, pixel.Bottom);
                        if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) firstCollisionCheck = true;
                    }

                    if (!firstCollisionCheck) {
                        boxLine = new Line2(pixel.Left, pixel.Bottom, pixel.Left, pixel.Top);
                        if (boxLine.IntersectCircle(new Vector2(circle.CenterX, circle.CenterY), circle.Radius)) firstCollisionCheck = true;
                    }

                }

                if (!firstCollisionCheck) return false;

                //get intersecting area as if circle was rect

                //scan for pixels in area

                //only check pixels inside circle radius

                //okay the math for that is super hard so we're doing it the easy + dumb wy

                //dumb way, scan through pixels to see if one of the pixels is in the circle?

                for (var xx = 0; xx < pixel.Width; xx++) {
                    for (var yy = 0; yy < pixel.Height; yy++) {
                        var pixelx = xx + pixel.Left;
                        var pixely = yy + pixel.Top;
                        if (Util.Distance(pixelx, pixely, circle.CenterX, circle.CenterY) <= circle.Radius + 1) {
                            if (pixel.PixelAt(xx, yy)) {
                                return true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Pixel vs Point
            else if ((first is PixelCollider && second is PointCollider) || (first is PointCollider && second is PixelCollider)) {
                PixelCollider pixel;
                PointCollider point;

                if (first is PixelCollider) {
                    pixel = first as PixelCollider;
                    point = second as PointCollider;
                }
                else {
                    pixel = second as PixelCollider;
                    point = first as PointCollider;
                }

                //rectangle to point first
                if (!Util.InRect(point.Left, point.Top, pixel.Left, pixel.Top, pixel.Width, pixel.Height)) return false;

                //check for pixel at point
                if (pixel.PixelAtRelative((int)point.Left, (int)point.Top)) {
                    return true;
                }
                return false;
            }
            #endregion

            #region Pixel vs Line
            else if ((first is PixelCollider && second is LineCollider) || (first is LineCollider && second is PixelCollider)) {
                PixelCollider pixel;
                LineCollider line;

                if (first is PixelCollider) {
                    pixel = first as PixelCollider;
                    line = second as LineCollider;
                }
                else {
                    pixel = second as PixelCollider;
                    line = first as LineCollider;
                }

                //line to rectangle first

                if (!line.Line2.IntersectsRect(pixel.Left, pixel.Top, pixel.Width, pixel.Height)) return false;

                //dumb way, check all pixels for distance to line == 0
                for (var xx = 0; xx < pixel.Width; xx++) {
                    for (var yy = 0; yy < pixel.Height; yy++) {
                        if (pixel.PixelAt(xx, yy)) {
                            if (Util.DistanceLinePoint(xx + pixel.Left, yy + pixel.Top, line.Line2) < 0.1f) {
                                return true;
                            }
                        }
                    }
                }

            }
            #endregion

            #region Pixel vs Grid
            else if ((first is PixelCollider && second is GridCollider) || (first is GridCollider && second is PixelCollider)) {
                PixelCollider pixel;
                GridCollider grid;

                if (first is PixelCollider) {
                    pixel = first as PixelCollider;
                    grid = second as GridCollider;
                }
                else {
                    pixel = second as PixelCollider;
                    grid = first as GridCollider;
                }

                //collide rectangle into tiles first
                if (pixel.Right <= grid.Left) return false;
                if (pixel.Bottom <= grid.Top) return false;
                if (pixel.Left >= grid.Right) return false;
                if (pixel.Top >= grid.Bottom) return false;

                //This is a quick fix and might have to be addressed later
                if (!grid.GetRect(pixel.Left, pixel.Top, pixel.Right - 1, pixel.Bottom - 1, false)) return false;

                //go through tiles that pixel is overlapping
                for (var xx = 0; xx < pixel.Width; xx++) {
                    for (var yy = 0; yy < pixel.Height; yy++) {
                        float checkx = xx + pixel.Left;
                        float checky = yy + pixel.Top;
                        if (grid.GetTileAtPosition(checkx, checky)) {
                            if (pixel.PixelAt(xx, yy)) {
                                return true;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Polygon vs Polygon
            else if (first is PolygonCollider && second is PolygonCollider) {
                var p1 = first as PolygonCollider;
                var p2 = second as PolygonCollider;

                // make copies of the polygons with applied offsets for testing
                var poly1 = new Polygon(p1.Polygon);
                var poly2 = new Polygon(p2.Polygon);

                poly1.OffsetPoints(p1.Left, p1.Top);
                poly2.OffsetPoints(p2.Left, p2.Top);

                return poly1.Overlap(poly2);
            }
            #endregion

            #region Polygon vs Box

            else if (first is PolygonCollider && second is BoxCollider || first is BoxCollider && second is PolygonCollider) {
                PolygonCollider poly;
                BoxCollider box;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    box = second as BoxCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    box = first as BoxCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                //create poly for box

                var poly2 = new Polygon(box.Left, box.Top, box.Right, box.Top, box.Right, box.Bottom, box.Left, box.Bottom);

                //test polys
                return poly1.Overlap(poly2);
            }

            #endregion

            #region Polygon vs Circle
            else if (first is PolygonCollider && second is CircleCollider || first is CircleCollider && second is PolygonCollider) {
                PolygonCollider poly;
                CircleCollider circ;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    circ = second as CircleCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    circ = first as CircleCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                // check each point of poly for distance to circle
                foreach (var p in poly1.Points) {
                    if (Util.Distance(p.X, p.Y, circ.CenterX, circ.CenterY) < circ.Radius) {
                        return true;
                    }
                }

                //check if center point is in poly
                if (poly1.ContainsPoint(circ.CenterX, circ.CenterY)) {
                    return true;
                }

                //check each edge for intersection with the circle
                var lines = poly1.GetEdgesAsLines();

                foreach (var l in lines) {
                    if (l.IntersectCircle(new Vector2(circ.CenterX, circ.CenterY), circ.Radius)) {
                        return true;
                    }
                }

                return false;
                
            }
            #endregion

            #region Polygon vs Grid
            else if (first is PolygonCollider && second is GridCollider || first is GridCollider && second is PolygonCollider) {
                PolygonCollider poly;
                GridCollider grid;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    grid = second as GridCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    grid = first as GridCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                //test against grid bounding box first
                var bbox = new Polygon(grid.Left, grid.Top, grid.Right, grid.Top, grid.Right, grid.Bottom, grid.Left, grid.Bottom);
                if (!poly1.Overlap(bbox)) {
                    return false;
                }

                //check rect in bounding box where polygon is
                if (!grid.GetRect(poly.Left, poly.Top, poly.Right, poly.Bottom, false)) {
                    return false;
                }

                //check each occupied cell as a box in the bounding box
                int startX = grid.GridX(poly.Left - grid.Left);
                int startY = grid.GridY(poly.Top - grid.Top);

                int endX = grid.GridX(poly.Left - grid.Left + poly.Width) + 1;
                int endY = grid.GridY(poly.Top - grid.Top + poly.Height) + 1;

                //Console.WriteLine("StartX {0} StartY {1} EndX {2} EndY {3}", startX, startY, endX, endY);

                for (var xx = startX; xx < endX; xx++) {
                    for (var yy = startY; yy < endY; yy++) {
                        if (grid.GetTile(xx, yy)) {
                            var realX = xx * grid.TileWidth + grid.Left;
                            var realY = yy * grid.TileHeight + grid.Top;
                            var gridPoly = new Polygon(
                                realX, realY,
                                realX + grid.TileWidth, realY,
                                realX + grid.TileWidth, realY + grid.TileHeight,
                                realX, realY + grid.TileHeight
                                );
                            if (gridPoly.Overlap(poly1)) {
                                //Console.WriteLine("Collision with tile X:{0} Y:{1}", xx, yy);
                                //Console.WriteLine("Poly overlapped with {0}", gridPoly);
                                return true;
                            }
                        }
                    }
                }

                return false;

            }
            #endregion

            #region Polygon vs Line
            else if (first is PolygonCollider && second is LineCollider || first is LineCollider && second is PolygonCollider) {
                PolygonCollider poly;
                LineCollider line;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    line = second as LineCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    line = first as LineCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                var poly2 = new Polygon(line.Left, line.Top, line.Right, line.Bottom);
                return poly1.Overlap(poly2);
            }
            #endregion

            #region Polygon vs Pixel
            else if (first is PolygonCollider && second is PixelCollider || first is PixelCollider && second is PolygonCollider) {
                PolygonCollider poly;
                PixelCollider pixel;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    pixel = second as PixelCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    pixel = first as PixelCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                // check bounding box of pixel first
                var bbox = new Polygon(pixel.Left, pixel.Top, pixel.Right, pixel.Top, pixel.Right, pixel.Bottom, pixel.Left, pixel.Bottom);
                if (!poly1.Overlap(bbox)) {
                    return false;
                }

                // get intersecting area of the bounding boxes
                float x1 = Math.Max(poly.Left, pixel.Left);
                float x2 = Math.Min(poly.Right, pixel.Right);

                float y1 = Math.Max(poly.Top, pixel.Top);
                float y2 = Math.Min(poly.Bottom, pixel.Bottom);

                // check each pixel in area as point in poly
                for (var i = x1; i < x2; i++) {
                    for (var j = y1; j < y2; j++) {
                        var xx = (int)Math.Floor(i - pixel.Left);
                        var yy = (int)Math.Floor(j - pixel.Top);

                        if (pixel.PixelAt(xx, yy)) {
                            if (poly1.ContainsPoint(xx + pixel.Left, yy + pixel.Top)) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            #endregion

            #region Polygon vs Point
            else if (first is PolygonCollider && second is PointCollider || first is PointCollider && second is PolygonCollider) {
                PolygonCollider poly;
                PointCollider point;

                if (first is PolygonCollider) {
                    poly = first as PolygonCollider;
                    point = second as PointCollider;
                }
                else {
                    poly = second as PolygonCollider;
                    point = first as PointCollider;
                }

                var poly1 = new Polygon(poly.Polygon);
                poly1.OffsetPoints(poly.Left, poly.Top);

                return poly1.ContainsPoint(point.Left, point.Top);
            }
            #endregion

            #region Unknown
            return false;
            #endregion

        }

        #endregion
        
    }
}

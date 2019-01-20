using System;
using System.Collections.Generic;

namespace Otter {

    /// <summary>
    /// Class used for a game object. The bread and butter of your game. Entities are added to Scenes which are controlled by the Game.
    /// </summary>
    public class Entity {

        #region Public Fields

        /// <summary>
        /// The X position of the Entity.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y position of the Entity.
        /// </summary>
        public float Y;

        /// <summary>
        /// How long the Entity has been active.
        /// </summary>
        public float Timer;

        /// <summary>
        /// Determines if the Entity will render.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Determines if the Entity will collide with other entities. The entity can still check for
        /// collisions, but will not register as a collision with other entities.
        /// </summary>
        public bool Collidable = true;

        /// <summary>
        /// Deteremines if the Entity's update functions will run automatically from the Scene.
        /// </summary>
        public bool AutoUpdate = true;

        /// <summary>
        /// Determines if the Entity's render functions will run automatically from the Scene.
        /// </summary>
        public bool AutoRender = true;

        /// <summary>
        /// The tween manager that controls all tweens on this entity.
        /// </summary>
        public Tweener Tweener = new Tweener();

        /// <summary>
        /// An action that fires when the entity is added to a Scene.
        /// </summary>
        public Action OnAdded = delegate { };

        /// <summary>
        /// An action that fires when the entity is updated.
        /// </summary>
        public Action OnUpdate = delegate { };

        /// <summary>
        /// An action that fires in the entity's UpdateFirst().
        /// </summary>
        public Action OnUpdateFirst = delegate { };

        /// <summary>
        /// An action that is fired in the entity's UpdateLast().
        /// </summary>
        public Action OnUpdateLast = delegate { };

        /// <summary>
        /// An action that fires when the entity is removed from a Scene.
        /// </summary>
        public Action OnRemoved = delegate { };

        /// <summary>
        /// An action that fires when the entity is rendered.
        /// </summary>
        public Action OnRender = delegate { };

        /// <summary>
        /// The name of this entity. Default's to the Type name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The order in which to render this entity.  Higher numbers draw later.
        /// </summary>
        public int Layer;

        /// <summary>
        /// The order in which to update this entity.  Higher numbers update later.
        /// </summary>
        public int Order;

        /// <summary>
        /// The pause group this entity is a part of.
        /// </summary>
        public int Group;

        /// <summary>
        /// How long the entity should live in the scene before removing itself. If this is set the
        /// entity will be automatically removed when the Timer exceeds this value.
        /// </summary>
        public float LifeSpan;

        /// <summary>
        /// Is true if the Entity has been updated by the Scene at least once.
        /// </summary>
        public bool UpdatedOnce { get; private set; }

        #endregion Public Fields

        #region Public Indexers

        public Component this[int id] {
            get {
                if (componentsById.ContainsKey(id)) return componentsById[id];
                return null;
            }
        }

        #endregion

        #region Internal Fields

        internal bool MarkedForRemoval = false;
        internal bool MarkedForAdd = false;

        internal int
            oldLayer,
            oldOrder;

        #endregion Internal Fields

        #region Private Fields

        private List<Component> componentsToRemove = new List<Component>();
        private List<Component> componentsToAdd = new List<Component>();
        Dictionary<int, Component> componentsById = new Dictionary<int, Component>();
        int nextComponentId;
        

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Create an entity.
        /// </summary>
        /// <param name="x">The x position to place the entity.</param>
        /// <param name="y">The y position to place the entity.</param>
        /// <param name="graphic">The graphic to assign to the entity.  Defaults to null.</param>
        /// <param name="collider">The collider to assign to the entity.  Defaults to null.</param>
        /// <param name="name">The name of the entity. Defaults to the type name.</param>
        public Entity(float x = 0, float y = 0, Graphic graphic = null, Collider collider = null, string name = "") {
            X = x;
            Y = y;

            InstanceId = -1;

            Graphics = new List<Graphic>();
            Components = new List<Component>();
            Colliders = new List<Collider>();
            Surfaces = new List<Surface>();

            if (graphic != null) {
                Graphic = graphic;
            }

            if (collider != null) {
                Collider = collider;
            }

            if (name == "") {
                Name = this.GetType().Name;
            }
            else {
                Name = name;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The list of graphics to render.
        /// </summary>
        public List<Graphic> Graphics { get; private set; }

        /// <summary>
        /// The list of components to update and render.
        /// </summary>
        public List<Component> Components { get; private set; }

        /// <summary>
        /// The list of colliders to use for collision checks.
        /// </summary>
        public List<Collider> Colliders { get; private set; }

        /// <summary>
        /// The list of surfaces the entity should draw to.
        /// </summary>
        public List<Surface> Surfaces { get; private set; }

        /// <summary>
        /// The Scene that controls and updates this entity.
        /// </summary>
        public Scene Scene { get; internal set; }

        /// <summary>
        /// The int id of the Entity for the Scene its currently in.
        /// </summary>
        public int InstanceId { get; internal set; }

        /// <summary>
        /// Returns true if the entity is currently in a Scene, or is queued to be added to a Scene next update.
        /// </summary>
        public bool IsInScene { get { return Scene != null; } }

        /// <summary>
        /// The default Surface that the entity should render to.
        /// </summary>
        public Surface Surface {
            get {
                if (Surfaces == null) return null;
                if (Surfaces.Count == 0) return null;
                return Surfaces[Surfaces.Count - 1];
            }
            set {
                Surfaces.Clear();
                Surfaces.Add(value);
            }
        }

        /// <summary>
        /// The currently overlapped entity.  This only works when using an Overlap collision check, and there is a result.
        /// </summary>
        public Entity Overlapped { get; private set; }

        /// <summary>
        /// Set to a collider by using the SetHitbox method.  Shortcut reference.
        /// </summary>
        public BoxCollider Hitbox { get; private set; }

        /// <summary>
        /// Returns the first available collider, or set the Collider.
        /// </summary>
        public Collider Collider {
            get {
                if (Colliders.Count == 0) return null;
                return Colliders[0];
            }
            set { SetCollider(value); }
        }

        /// <summary>
        /// A reference to the Input object in the Game that controls the Scene.
        /// </summary>
        public Input Input {
            get { return Scene.Game.Input; }
        }

        /// <summary>
        /// A reference to the Game that controls the Scene.
        /// </summary>
        public Game Game {
            get { return Scene.Game; }
        }

        /// <summary>
        /// If the entity is currently paused by the scene.
        /// </summary>
        public bool IsPaused {
            get {
                if (Scene != null) {
                    return Scene.IsGroupPaused(Group);
                }
                return false;
            }
        }

        /// <summary>
        /// The x position in screen space of the entity.
        /// </summary>
        public float ScreenX {
            get {
                if (Scene != null) {
                    return X - Scene.CameraX;
                }
                return X;
            }
        }

        /// <summary>
        /// The y position in screen space of the entity.
        /// </summary>
        public float ScreenY {
            get {
                if (Scene != null) {
                    return Y - Scene.CameraY;
                }
                return Y;
            }
        }

        /// <summary>
        /// Returns the first available graphic, or set the graphic.
        /// </summary>
        public Graphic Graphic {
            get {
                if (Graphics.Count == 0) return null;
                return Graphics[0];
            }
            set { SetGraphic(value); }
        }

        /// <summary>
        /// The position of the Entity represented as a Vector2
        /// </summary>
        public Vector2 Position {
            get {
                return new Vector2(X, Y);
            }
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the graphic to a new graphic, removing all previous graphics.
        /// </summary>
        /// <param name="g"></param>
        public void SetGraphic(Graphic g) {
            Graphics.Clear();
            Graphics.Add(g);
        }

        /// <summary>
        /// Set the X and Y position to a value.
        /// </summary>
        /// <param name="xy">The value of the X and Y position.</param>
        public void SetPosition(float xy) {
            X = Y = xy;
        }

        /// <summary>
        /// Add to the X and Y positions of the Entity.
        /// </summary>
        /// <param name="x">The amount to add to the x position.</param>
        /// <param name="y">The amount to add to the y position.</param>
        public void AddPosition(float x, float y) {
            X += x;
            Y += y;
        }

        /// <summary>
        /// Add to the X and Y position of the Entity.
        /// </summary>
        /// <param name="axis">The axis to add from.</param>
        /// <param name="multiplier">The amount to muliply the axis values by before adding.</param>
        public void AddPosition(Axis axis, float multiplier = 1) {
            AddPosition(axis.X * multiplier, axis.Y * multiplier);
        }

        /// <summary>
        /// Add to the X and Y position of the Entity.
        /// </summary>
        /// <param name="vector">The Vector2 to add to the position.</param>
        public void AddPosition(Vector2 vector) {
            AddPosition(vector.X, vector.Y);
        }

        /// <summary>
        /// Set the position of the Entity.
        /// </summary>
        /// <param name="x">The new x position.</param>
        /// <param name="y">The new y position.</param>
        public void SetPosition(float x, float y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Set the position of the Entity to another Entity's position.
        /// </summary>
        /// <param name="e">The Entity to match positions with.</param>
        public void SetPosition(Entity e, float offsetX = 0, float offsetY = 0) {
            SetPosition(e.X + offsetX, e.Y + offsetY);
        }

        /// <summary>
        /// Set the position of the Entity.
        /// </summary>
        /// <param name="v">The vector of the new position.</param>
        public void SetPosition(Vector2 v) {
            SetPosition(v.X, v.Y);
        }

        /// <summary>
        /// Adds a Graphic to the Entity.
        /// </summary>
        /// <param name="g">The Graphic to add.</param>
        /// <returns>The added Graphic.</returns>
        public T AddGraphic<T>(T g) where T : Graphic {
            Graphics.Add(g);
            return g;
        }

        /// <summary>
        /// Adds a Graphic to the Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="g">The Graphic to add.</param>
        /// <param name="x">The X position to place the Graphic relative to the Entity.</param>
        /// <param name="y">The Y position to place the Graphic relative to the Entity.</param>
        /// <returns>The added Graphic.</returns>
        public T AddGraphic<T>(T g, float x, float y) where T : Graphic {
            Graphics.Add(g);
            g.X = x;
            g.Y = y;
            return g;
        }

        /// <summary>
        /// Adds the graphics to the Entity.
        /// </summary>
        /// <param name="graphics">The Graphics to add.</param>
        public List<Graphic> AddGraphics(params Graphic[] graphics) {
            var r = new List<Graphic>();
            foreach (var g in graphics) {
                r.Add(AddGraphic(g));
            }
            return r;
        }

        /// <summary>
        /// Adds a graphic to the Entity and sets its Scroll value to 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="g">The Graphic to add.</param>
        /// <returns>The added Graphic.</returns>
        public T AddGraphicGUI<T>(T g) where T : Graphic {
            g.Scroll = 0;
            return AddGraphic(g);
        }

        /// <summary>
        /// Adds a graphic to the Entity and sets its Scroll value to 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="g">The Graphic to add.</param>
        /// <param name="x">The X position to place the Graphic relative to the Entity.</param>
        /// <param name="y">The Y position to place the Graphic relative to the Entity.</param>
        /// <returns>The added Graphic.</returns>
        public T AddGraphicGUI<T>(T g, float x, float y) where T : Graphic {
            g.Scroll = 0;
            return AddGraphic(g, x, y);
        }

        /// <summary>
        /// Adds Graphics to the Entity and sets their Scroll values to 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graphics">The Graphics to add.</param>
        /// <returns>The added Graphics.</returns>
        public List<Graphic> AddGraphicsGUI(params Graphic[] graphics) {
            var r = new List<Graphic>();
            foreach (var g in graphics) {
                r.Add(AddGraphicGUI(g));
            }
            return r;
        }

        /// <summary>
        /// Removes a Graphic from the Entity.
        /// </summary>
        /// <param name="g">The Graphic to remove.</param>
        public T RemoveGraphic<T>(T g) where T : Graphic {
            Graphics.Remove(g);
            return g;
        }

        /// <summary>
        /// Removes Graphics from the Entity.
        /// </summary>
        /// <param name="g">The Graphics to remove.</param>
        public void RemoveGraphics(params Graphic[] g) {
            g.Each(gr => RemoveGraphic(gr));
        }

        /// <summary>
        /// Remove all the graphics from the entity.
        /// </summary>
        public void ClearGraphics() {
            Graphics.Clear();
        }

        /// <summary>
        /// Adds a component to the entity.
        /// </summary>
        /// <param name="c"></param>
        public T AddComponent<T>(T c) where T : Component {
            if (c.Entity != null) return c;
            c.Entity = this;
            componentsToAdd.Add(c);
            return c;
        }

        /// <summary>
        /// Creates and adds a Component to the entity.
        /// </summary>
        /// <typeparam name="T">The type of Component to create.</typeparam>
        /// <param name="constructorArgs">The arguments for the Component's constructor.</param>
        /// <returns>The newly created Component.</returns>
        public T AddComponent<T>(params object[] constructorArgs) where T : Component {
            var c = (T)Activator.CreateInstance(typeof(T), constructorArgs);
            return AddComponent(c);
        }

        /// <summary>
        /// Add multiple components to the entity.
        /// </summary>
        /// <param name="c">The components to add.</param>
        /// <returns>A list of the added components.</returns>
        public List<Component> AddComponents(params Component[] c) {
            var r = new List<Component>();
            foreach (var com in c) {
                r.Add(AddComponent(com));
            }
            return r;
        }

        /// <summary>
        /// Removes a component from the Entity.
        /// </summary>
        /// <param name="c"></param>
        public T RemoveComponent<T>(T c) where T : Component {
            if (componentsToAdd.Contains(c)) {
                componentsToAdd.Remove(c);
                return c;
            }
            componentsToRemove.Add(c);
            return c;
        }

        /// <summary>
        /// Removes the first component of type T from the Entity.
        /// </summary>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        /// <returns>The removed component.</returns>
        public T RemoveComponent<T>() where T : Component {
            return RemoveComponent(GetComponent<T>());
        }

        /// <summary>
        /// Remove all components from the Entity.
        /// </summary>
        public void ClearComponents() {
            foreach (var c in Components) {
                RemoveComponent(c);
            }
        }

        /// <summary>
        /// Add a surface that the entity should render to.
        /// </summary>
        /// <param name="target"></param>
        public void AddSurface(Surface target) {
            Surfaces.Add(target);
        }

        /// <summary>
        /// Remove a surface from the list of surfaces that the entity should render to.
        /// </summary>
        /// <param name="target"></param>
        public void RemoveSurface(Surface target) {
            Surfaces.Remove(target);
        }

        /// <summary>
        /// Remove all Surfaces from the list of Surfaces that the Entity should render to.
        /// </summary>
        public void ClearSurfaces() {
            Surfaces.Clear();
        }

        /// <summary>
        /// Shortcut to set the Collider of the entity to a BoxCollider.  Using this will set the "Hitbox" field to this Collider.
        /// </summary>
        /// <param name="width">The width of the box collider.</param>
        /// <param name="height">The height of the box collider.</param>
        /// <param name="tags">The tags assigned to the box collider.</param>
        /// <returns>The created box collider.</returns>
        public BoxCollider SetHitbox(int width, int height, params int[] tags) {
            var hitbox = new BoxCollider(width, height, tags);
            SetCollider(hitbox);
            Hitbox = hitbox;
            return hitbox;
        }

        /// <summary>
        /// Shortcut to set the Collider of the entity to a BoxCollider.  Using this will set the "Hitbox" field to this Collider.
        /// </summary>
        /// <param name="width">The width of the box collider.</param>
        /// <param name="height">The height of the box collider.</param>
        /// <param name="tag">The first tag to add.</param>
        /// <param name="tags">The rest of the tags to add.</param>
        /// <returns></returns>
        public BoxCollider SetHitbox(int width, int height, Enum tag, params Enum[] tags) {
            var hitbox = new BoxCollider(width, height, tag, tags);
            SetCollider(hitbox);
            Hitbox = hitbox;
            return hitbox;
        }

        /// <summary>
        /// Get the first instance of an Entity of type T in this Entity's Scene.
        /// </summary>
        /// <typeparam name="T">The entity type to search for.</typeparam>
        /// <returns>The first entity of that type in the scene.</returns>
        public T GetEntity<T>() where T : Entity {
            return Scene.GetEntity<T>();
        }

        /// <summary>
        /// Get a list of entities of type T from this Entity's Scene.
        /// </summary>
        /// <typeparam name="T">The type of entity to collect.</typeparam>
        /// <returns>A list of entities of type T.</returns>
        public List<T> GetEntities<T>() where T : Entity {
            return Scene.GetEntities<T>();
        }

        /// <summary>
        /// Get the first Component of type T.
        /// </summary>
        /// <typeparam name="T">The type of Component to look for.</typeparam>
        /// <returns>The Component.</returns>
        public T GetComponent<T>() where T : Component {
            foreach (var c in Components) {
                if (c is T) return (T)c;
            }
            foreach (var c in componentsToAdd) {
                if (c is T) return (T)c;
            }
            return null;
        }

        /// <summary>
        /// Get the first Component of Type type.
        /// </summary>
        /// <param name="type">The Type of Component to look for.</param>
        /// <returns>The first Component of that Type.</returns>
        public Component GetComponent(Type type) {
            foreach (var c in Components) {
                if (c.GetType() == type) return c;
            }
            return null;
        }

        /// <summary>
        /// Get all Components of type T.
        /// </summary>
        /// <typeparam name="T">The type of Component to look for.</typeparam>
        /// <returns>A list of Components of type T.</returns>
        public List<T> GetComponents<T>() where T : Component {
            var list = new List<T>();
            foreach (var c in Components) {
                if (c is T) list.Add((T)c);
            }
            foreach (var c in componentsToAdd) {
                if (c is T) list.Add((T)c);
            }
            return list;
        }

        /// <summary>
        /// Get the first graphic of type T.
        /// </summary>
        /// <typeparam name="T">The type of graphic to look for.</typeparam>
        /// <returns>The graphic.</returns>
        public T GetGraphic<T>() where T : Graphic {
            foreach (var g in Graphics) {
                if (g is T) return (T)g;
            }
            return null;
        }

        /// <summary>
        ///  Get the first collider of type T.
        /// </summary>
        /// <typeparam name="T">The type of collider to look for.</typeparam>
        /// <returns>The collider.</returns>
        public T GetCollider<T>() where T : Collider {
            foreach (var c in Colliders) {
                if (c is T) return (T)c;
            }
            return null;
        }

        /// <summary>
        /// Add a collider to the entity.
        /// </summary>
        /// <param name="c"></param>
        public T AddCollider<T>(T c) where T : Collider {
            Colliders.Add(c);
            c.Entity = this;
            c.Added();
            if (Scene != null) {
                Scene.AddColliderInternal(c);
            }
            return c;
        }

        /// <summary>
        /// Remove the collider from the entity.
        /// </summary>
        /// <param name="c"></param>
        public T RemoveCollider<T>(T c) where T : Collider {
            if (Colliders.Contains(c)) {
                Colliders.Remove(c);
                c.Removed();
                c.Entity = null;

                if (Scene != null) {
                    Scene.RemoveColliderInternal(c);
                }
            }
            return c;
        }

        /// <summary>
        /// Remove all colliders from the entity.
        /// </summary>
        public void ClearColliders() {
            var colliders = new List<Collider>(Colliders);
            foreach (var c in colliders) {
                RemoveCollider(c);
            }
            Hitbox = null;
        }

        /// <summary>
        /// Remove all current colliders and set collider to a new one.
        /// </summary>
        /// <param name="c"></param>
        public T SetCollider<T>(T c) where T : Collider {
            ClearColliders();
            return AddCollider(c);
        }

        /// <summary>
        /// Adds colliders to the entity.
        /// </summary>
        /// <param name="colliders"></param>
        public List<Collider> AddColliders(params Collider[] colliders) {
            var r = new List<Collider>();
            foreach (var c in colliders) {
                r.Add(AddCollider(c));
            }
            return r;
        }

        /// <summary>
        /// Removes colliders from the entity.
        /// </summary>
        /// <param name="colliders"></param>
        public void RemoveColliders(params Collider[] colliders) {
            foreach (var c in colliders) {
                RemoveCollider(c);
            }
        }

        /// <summary>
        /// Checks for a collision using the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The int tags to check for.</param>
        /// <returns></returns>
        public Collider Collide(float x, float y, params int[] tags) {
            return Collider.Collide(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision using the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The enum tags to check for.</param>
        /// <returns></returns>
        public Collider Collide(float x, float y, params Enum[] tags) {
            return Collider.Collide(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The int tags to check for.</param>
        /// <returns>A list of all colliders touched.</returns>
        public List<Collider> CollideList(float x, float y, params int[] tags) {
            return Collider.CollideList(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The enum tags to check for.</param>
        /// <returns>A list of all colliders touched.</returns>
        public List<Collider> CollideList(float x, float y, params Enum[] tags) {
            return Collider.CollideList(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The int tags to check for.</param>
        /// <returns>A list of all entities touched.</returns>
        public List<Entity> CollideEntities(float x, float y, params int[] tags) {
            return Collider.CollideEntities(x, y, tags);
        }

        /// <summary>
        /// Checks for a collision with the first available Collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The enum tags to check for.</param>
        /// <returns>A list of all Entities touched.</returns>
        public List<Entity> CollideEntities(float x, float y, params Enum[] tags) {
            return Collider.CollideEntities(x, y, tags);
        }

        public List<T> CollideEntities<T>(float x, float y, List<T> entities) where T : Entity {
            return Collider.CollideEntities(x, y, entities);
        }

        public List<T> CollideEntities<T>(float x, float y, params int[] tags) where T : Entity {
            return Collider.CollideEntities<T>(x, y, tags);
        }

        public List<T> CollideEntities<T>(float x, float y, params Enum[] tags) where T : Entity {
            return Collider.CollideEntities<T>(x, y, tags);
        }

        /// <summary>
        /// Checks for an overlap using the first available collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The int tags to check for.</param>
        /// <returns>True if there is a collision.</returns>
        public bool Overlap(float x, float y, params int[] tags) {
            Overlapped = null;
            if (Collider == null) return false; // If no collider, cant possibly overlap.

            var result = Collider.Overlap(x, y, tags);
            if (result) {
                Overlapped = Collider.Collide(x, y, tags).Entity;
            }

            return result;
        }

        /// <summary>
        /// Checks for an overlap using the first available collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="tags">The enum tags to check for.</param>
        /// <returns>True if there is a collision.</returns>
        public bool Overlap(float x, float y, params Enum[] tags) {
            return Overlap(x, y, Util.EnumToIntArray(tags));
        }

        /// <summary>
        /// Checks for an overlap using the first available collider.
        /// </summary>
        /// <param name="x">The X position to check for a collision at.</param>
        /// <param name="y">The Y position to check for a collision at.</param>
        /// <param name="e">The Entity to check for a collision with.</param>
        /// <returns>True if there is a collision.</returns>
        public bool Overlap(float x, float y, Entity e) {
            Overlapped = null;

            var result = Collider.Overlap(x, y, e);
            if (result) {
                Overlapped = Collider.Collide(x, y, e).Entity;
            }

            return result;
        }

        /// <summary>
        /// Called when the entity is added to a scene.  The reference to the Scene is available here.
        /// </summary>
        public virtual void Added() {
        }

        /// <summary>
        /// Called when the entity is removed from a scene.  The reference to Scene is now null.
        /// </summary>
        public virtual void Removed() {
        }

        /// <summary>
        /// Called when the Scene begins.
        /// </summary>
        public virtual void SceneBegin() {
        }

        /// <summary>
        /// Called when the Scene ends.
        /// </summary>
        public virtual void SceneEnd() {
        }

        /// <summary>
        /// Called when the Scene is paused.
        /// </summary>
        public virtual void ScenePause() {
        }

        /// <summary>
        /// Called when the Scene is resumed.
        /// </summary>
        public virtual void SceneResume() {
        }

        /// <summary>
        /// Called when the entity is paused by the Scene.
        /// </summary>
        public virtual void Paused() {
        }

        /// <summary>
        /// Called when the entity is resumed by the Scene.
        /// </summary>
        public virtual void Resumed() {
        }

        /// <summary>
        /// Tweens a set of numeric properties on an object.
        /// </summary>
        /// <param name="target">The object to tween.</param>
        /// <param name="values">The values to tween to, in an anonymous type ( new { prop1 = 100, prop2 = 0} ).</param>
        /// <param name="duration">Duration of the tween in seconds.</param>
        /// <param name="delay">Delay before the tween starts, in seconds.</param>
        /// <returns>The tween created, for setting properties on.</returns>
        public Tween Tween(object target, object values, float duration, float delay = 0) {
            return Tweener.Tween(target, values, duration, delay);
        }

        /// <summary>
        /// Called first during the update.  Happens before Update.
        /// </summary>
        public virtual void UpdateFirst() {
        }

        /// <summary>
        /// Called last during the update.  Happens after Update.
        /// </summary>
        public virtual void UpdateLast() {
        }

        /// <summary>
        /// Called during the update of the game.
        /// </summary>
        public virtual void Update() {
        }

        /// <summary>
        /// Called when the entity is rendering to the screen.
        /// </summary>
        public virtual void Render() {
        }

        /// <summary>
        /// Called before an entity is rendered. Things rendered here will appear below the Entity's Graphics.
        /// </summary>
        public virtual void Prerender() {
        }

        /// <summary>
        /// Remove this entity from the Scene it's in.
        /// </summary>
        public void RemoveSelf() {
            if (Scene != null && !MarkedForRemoval) {
                Scene.Remove(this);
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void UpdateComponentLists() {
            while (componentsToRemove.Count > 0) {
                var removing = new List<Component>(componentsToRemove);
                componentsToRemove.Clear();
                
                foreach (var c in removing) {
                    Components.Remove(c);
                    componentsById.Remove(c.InstanceId);
                    c.InstanceId = -1;
                }
                foreach (var c in removing) {
                    c.Removed();
                    c.Entity = null;
                }
            }

            while (componentsToAdd.Count > 0) {
                var adding = new List<Component>(componentsToAdd);
                componentsToAdd.Clear();
                
                foreach (var c in adding) {
                    Components.Add(c);
                    var id = GetNextComponentId();
                    componentsById.Add(id, c);
                    c.InstanceId = id;
                }
                foreach (var c in adding) {
                    c.Added();
                }
            }
        }

        internal void UpdateFirstInternal() {
            Game.UpdateCount++;

            UpdatedOnce = true;

            UpdateComponentLists();

            foreach (var c in Components) {
                c.UpdateFirst();
            }
            if (OnUpdateFirst != null) {
                OnUpdateFirst();
            }
            UpdateFirst();
        }

        internal void UpdateLastInternal() {
            UpdateComponentLists();

            foreach (var c in Components) {
                c.UpdateLast();
                c.Timer += Game.DeltaTime;
            }

            UpdateLast();
            if (OnUpdateLast != null) {
                OnUpdateLast();
            }

            foreach (var g in Graphics) {
                g.Update();
            }

            Timer += Game.DeltaTime;
            if (LifeSpan > 0) {
                if (Timer >= LifeSpan) {
                    RemoveSelf();
                }
            }
        }

        internal void UpdateInternal() {
            UpdateComponentLists();

            foreach (var c in Components) {
                c.Update();
            }
            if (OnUpdate != null) {
                OnUpdate();
            }
            Tweener.Update(Game.DeltaTime);

            Update();
        }

        internal void RenderInternal() {
            if (!UpdatedOnce) return; //prevent rendering before update
            if (!Visible) return;
            if (Scene == null) return;

            if (Surface == null) {
                RenderEntity();
            }
            else {
                foreach (var surface in Surfaces) {
                    Surface temp = Draw.Target;

                    Draw.SetTarget(surface);

                    RenderEntity();

                    Draw.SetTarget(temp);
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        int GetNextComponentId() {
            var id = nextComponentId;
            nextComponentId++;
            return id;
        }

        private void RenderEntity() {
            Prerender();

            foreach (var c in Components) {
                if (!c.RenderAfterEntity) {
                    if (c.Visible) c.Render();
                }
            }

            foreach (var g in Graphics) {
                if (g.Relative) {
                    g.Render(X, Y);
                }
                else {
                    g.Render(0, 0);
                }
            }

            Render();

            if (OnRender != null) {
                OnRender();
            }

            foreach (var c in Components) {
                if (c.RenderAfterEntity) {
                    if (c.Visible) c.Render();
                }
            }
        }

        #endregion Private Methods
    }
}
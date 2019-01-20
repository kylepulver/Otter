using System.Collections.Generic;
namespace Otter {
    /// <summary>
    /// Base Component class.  Components can be added to Entities.
    /// </summary>
    public abstract class Component {

        #region Public Fields

        /// <summary>
        /// The parent Entity of the Component.
        /// </summary>
        public Entity Entity;

        /// <summary>
        /// Determines if the Component should render after the Entity has rendered.
        /// </summary>
        public bool RenderAfterEntity = true;

        /// <summary>
        /// Determines if the Component will render.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// How long the Component has been alive (added to an Entity and updated.)
        /// </summary>
        public float Timer = 0;

        /// <summary>
        /// The Component's id for the Entity its attached to.
        /// </summary>
        public int InstanceId { get; internal set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Scene that the parent Entity is in.
        /// </summary>
        public Scene Scene {
            get {
                return Entity.Scene;
            }
        }

        /// <summary>
        /// The first Collider of the parent Entity.
        /// </summary>
        public Collider Collider {
            get {
                return Entity.Collider;
            }
        }

        /// <summary>
        /// The first Graphic of the parent Entity.
        /// </summary>
        public Graphic Graphic {
            get {
                return Entity.Graphic;
            }
        }

        /// <summary>
        /// The list of Graphics from the parent Entity.
        /// </summary>
        public List<Graphic> Graphics {
            get {
                return Entity.Graphics;
            }
        }

        /// <summary>
        /// The list of Colliders from the parent Entity.
        /// </summary>
        public List<Collider> Colliders {
            get {
                return Entity.Colliders;
            }
        }

        #endregion

        #region Constructors

        public Component() {
            InstanceId = -1;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the Entity as a specific Type.
        /// </summary>
        /// <typeparam name="T">The Type to get.</typeparam>
        /// <returns>The Entity as Type T</returns>
        public T GetEntity<T>() where T : Entity {
            return (T)Entity;
        }

        /// <summary>
        /// Called when the Component is added to the Entity.
        /// </summary>
        public virtual void Added() {

        }

        /// <summary>
        /// Called when the Component is removed from the Entity.
        /// </summary>
        public virtual void Removed() {

        }

        /// <summary>
        /// Removes the Component from its parent Entity.
        /// </summary>
        public void RemoveSelf() {
            if (Entity != null) {
                Entity.RemoveComponent(this);
            }
        }

        /// <summary>
        /// Called during the UpdateFirst on the parent Entity.
        /// </summary>
        public virtual void UpdateFirst() {

        }

        /// <summary>
        /// Called during the Update on the parent Entity.
        /// </summary>
        public virtual void Update() {

        }

        /// <summary>
        /// Called during the Render on the parent Entity.
        /// </summary>
        public virtual void Render() {

        }

        /// <summary>
        /// Called during the UpdateLast on the parent Entity.
        /// </summary>
        public virtual void UpdateLast() {

        }

        /// <summary>
        /// Gets the first Component of type T from this Component's Entity.
        /// </summary>
        /// <typeparam name="T">The type of the Component.</typeparam>
        /// <returns>The first Component of type T from the Entity's Components.</returns>
        public T GetComponent<T>() where T : Component {
            return Entity.GetComponent<T>();
        }

        /// <summary>
        /// Gets a list of Components of type T from this Component's Entity.
        /// </summary>
        /// <typeparam name="T">The type of the Components.</typeparam>
        /// <returns>A list of Components of type T from the Entity's Components.</returns>
        public List<T> GetComponents<T>() where T : Component {
            return Entity.GetComponents<T>();
        }
        #endregion

    }
}

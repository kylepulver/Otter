using System;

namespace Otter.Components
{
    /// <summary>
    /// Used in StateMachine. Contains functions for enter, update, and exit.
    /// </summary>
    public class State
    {

        #region Public Fields

        /// <summary>
        /// The method to call when entering this state.
        /// </summary>
        public Action OnEnter = delegate { };

        /// <summary>
        /// The method to call when updating this state.
        /// </summary>
        public Action OnUpdate = delegate { };

        /// <summary>
        /// The method to call when exiting this state.
        /// </summary>
        public Action OnExit = delegate { };

        #endregion

        #region Public Properties

        /// <summary>
        /// The Id that this state has been assigned.
        /// </summary>
        public int Id { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new State with three Actions.
        /// </summary>
        /// <param name="onEnter">The method to call when entering this state.</param>
        /// <param name="onUpdate">The method to call when updating this state.</param>
        /// <param name="onExit">The method to call when exiting this state.</param>
        public State(Action onEnter = null, Action onUpdate = null, Action onExit = null)
        {
            Functions(onEnter, onUpdate, onExit);
        }

        /// <summary>
        /// Create a new State with just an update Action.
        /// </summary>
        /// <param name="onUpdate">The method to call when updating this state.</param>
        public State(Action onUpdate) : this(null, onUpdate) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set all three of the methods for enter, update, and exit.
        /// </summary>
        /// <param name="onEnter">The method to call when entering this state.</param>
        /// <param name="onUpdate">The method to call when updating this state.</param>
        /// <param name="onExit">The method to call when exiting this state.</param>
        public void Functions(Action onEnter, Action onUpdate, Action onExit)
        {
            if (onEnter != null)
            {
                OnEnter = onEnter;
            }
            if (onUpdate != null)
            {
                OnUpdate = onUpdate;
            }
            if (onExit != null)
            {
                OnExit = onExit;
            }
        }

        /// <summary>
        /// Call OnUpdate.
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Call OnEnter.
        /// </summary>
        public void Enter()
        {
            OnEnter();
        }

        /// <summary>
        /// Call OnExit.
        /// </summary>
        public void Exit()
        {
            OnExit();
        }

        #endregion

    }
}

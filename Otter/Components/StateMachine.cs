using System;
using System.Collections.Generic;
using System.Reflection;

namespace Otter {
    /// <summary>
    /// State machine that uses a specific type.  This is really meant for using an enum as your list of states.
    /// If an enum is used, the state machine will automatically populate the states using methods in the parent
    /// Entity that match the name of the enum values.
    /// </summary>
    /// <example>
    /// Say you have an enum named State, and it has the value "Walking"
    /// When the state machine is added to the Entity, it will match any methods named:
    /// EnterWalking
    /// UpdateWalking
    /// ExitWalking
    /// And use those to build the states.  This saves a lot of boilerplate set up code.
    /// </example>
    /// <typeparam name="TState">An enum of states.</typeparam>
    public class StateMachine<TState> : Component {

        #region Private Fields

        Dictionary<TState, State> states = new Dictionary<TState, State>();
        List<TState> stateStack = new List<TState>();
        List<TState> pushQueue = new List<TState>();
        List<bool> pushPopBuffer = new List<bool>(); // keep track of the order of commands
        List<float> timers = new List<float>();

        Dictionary<TState, Dictionary<TState, Action>> transitions = new Dictionary<TState, Dictionary<TState, Action>>();

        bool firstChange = true;

        bool populatedMethods;

        TState changeTo;

        bool change;
        bool updating;
        bool noState = true;

        TState topState {
            get {
                if (stateStack.Count > 0) {
                    return stateStack[0];
                }
                return CurrentState;
            }
        }

        #endregion

        #region Private Properties

        State s {
            get {
                if (!states.ContainsKey(CurrentState)) {
                    return null;
                }
                return states[CurrentState];
            }
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Determines if the StateMachine will autopopulate its states based off of the values of the Enum.
        /// </summary>
        public bool AutoPopulate = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// The current state.
        /// </summary>
        public TState CurrentState { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new StateMachine.
        /// </summary>
        public StateMachine() {
        }

        #endregion

        #region Private Methods

        void Transition(TState from, TState to) {
            if (transitions.ContainsKey(from)) {
                if (transitions[from].ContainsKey(to)) {
                    transitions[from][to]();
                }
            }
        }

        void EnsurePopulatedMethods() {
            if (AutoPopulate) { // Populate the methods on the state change.
                if (!populatedMethods) {
                    populatedMethods = true;
                    if (typeof(TState).IsEnum) {
                        foreach (TState value in Enum.GetValues(typeof(TState))) {
                            AddState(value);
                        }
                    }
                }
            }
        }

        void PushStateImmediate(TState state) {
            EnsurePopulatedMethods();

            stateStack.Insert(0, state);
            timers.Insert(0, 0);
            var from = CurrentState;
            if (stateStack.Count > 1) {
                timers[1] = Timer;
            }
            Timer = 0;
            CurrentState = topState;
            s.Enter();
            Transition(from, CurrentState);
            noState = false;
        }

        void PopStateImmediate() {
            EnsurePopulatedMethods();

            if (stateStack.Count == 0) return; // No states to pop!

            stateStack.RemoveAt(0);
            timers.RemoveAt(0);
            s.Exit();
            var from = CurrentState;
            CurrentState = topState;
            Transition(from, CurrentState);

            if (stateStack.Count > 0) {
                Timer = timers[0];
            }
            else {
                noState = true; // No more states... :(
                Timer = 0;
            }
        }

        void ChangeState() {
            if (change) {
                var state = changeTo;
                change = false;

                EnsurePopulatedMethods();

                if (!firstChange) {
                    if (states.ContainsKey(CurrentState)) {
                        if (states[CurrentState] == states[state]) return; // No change actually happening so return
                    }
                }

                Timer = 0;

                var fromState = CurrentState;

                if (states.ContainsKey(state)) {
                    if (s != null && !firstChange) {
                        s.Exit();
                    }
                    CurrentState = state;
                    if (s == null) throw new NullReferenceException("Next state is null.");
                    s.Enter();
                    noState = false;
                    Transition(fromState, CurrentState);
                }

                if (firstChange) {
                    firstChange = false;
                }
            }
            else {
                pushPopBuffer.ForEach(push => {
                    if (push) { // Push
                        var pushState = pushQueue[0];
                        pushQueue.RemoveAt(0);

                        PushStateImmediate(pushState);
                    }
                    else { // Pop
                        PopStateImmediate();
                    }
                });
                pushPopBuffer.Clear();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds methods that match the enum state in the Entity.  This happens in the Added() method of the
        /// component if AutoPopulate is set to true.
        /// </summary>
        /// <param name="e">The Entity to get methods from.</param>
        public void PopulateMethodsFromEntity(Entity e) {
            if (typeof(TState).IsEnum) {
                foreach (TState value in Enum.GetValues(typeof(TState))) {
                    AddState(value, e);
                }
            }
        }

        /// <summary>
        /// Finds methods that match the enum state in the Entity.  This happens in the Added() method of the
        /// component if AutoPopulate is set to true.  If no Entity is specified, get the methods from the
        /// Entity that owns this component.
        /// </summary>
        public void PopulateMethodsFromEntity() {
            PopulateMethodsFromEntity(Entity);
        }

        /// <summary>
        /// Change the state.  Exit will be called on the current state followed by Enter on the new state.
        /// If the state machine is currently updating then the state change will not occur until after the
        /// update has completed.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        public void ChangeState(TState state) {
            pushQueue.Clear();
            stateStack.Clear();

            changeTo = state;
            change = true;

            if (updating) {

            }
            else {
                ChangeState();
            }
        }

        /// <summary>
        /// Push a state onto a stack of states.  The state machine will always run the top of the stack.
        /// </summary>
        /// <param name="state">The state to push.</param>
        public void PushState(TState state) {
            if (updating) {
                pushQueue.Add(state);
                pushPopBuffer.Add(true); //true means push
            }
            else {
                PushStateImmediate(state);
            }
        }

        /// <summary>
        /// Pop the top state on the stack (if there is a stack.)
        /// </summary>
        public void PopState() {
            if (updating) {
                pushPopBuffer.Add(false); //false means pop
            }
            else {
                PopStateImmediate();
            }
        }

        /// <summary>
        /// Update the State Machine.
        /// </summary>
        public override void Update() {
            base.Update();
            if (states.ContainsKey(CurrentState) && !noState) {
                updating = true;
                s.Update();
                updating = false;

                ChangeState();
            }
        }

        /// <summary>
        /// Add a transition callback for when going from one state to another.
        /// </summary>
        /// <param name="fromState">The State that is ending.</param>
        /// <param name="toState">The State that is starting.</param>
        /// <param name="function">The Action to run when the machine goes from the fromState to the toState.</param>
        public void AddTransition(TState fromState, TState toState, Action function) {
            if (!transitions.ContainsKey(fromState)) {
                transitions.Add(fromState, new Dictionary<TState, Action>());
            }
            transitions[fromState].Add(toState, function);
        }

        /// <summary>
        /// Add a state with three Actions.
        /// </summary>
        /// <param name="key">The key to reference the State with.</param>
        /// <param name="onEnter">The method to call when entering this state.</param>
        /// <param name="onUpdate">The method to call when updating this state.</param>
        /// <param name="onExit">The method to call when exiting this state.</param>
        public void AddState(TState key, Action onEnter, Action onUpdate, Action onExit) {
            states.Add(key, new State(onEnter, onUpdate, onExit));
        }

        /// <summary>
        /// Add a state with just an update Action.
        /// </summary>
        /// <param name="key">The key to reference the State with.</param>
        /// <param name="onUpdate">The method to call when updating this state.</param>
        public void AddState(TState key, Action onUpdate) {
            states.Add(key, new State(onUpdate));
        }

        /// <summary>
        /// Add a state.
        /// </summary>
        /// <param name="key">The key to reference the State with.</param>
        /// <param name="value">The State to add.</param>
        public void AddState(TState key, State value) {
            states.Add(key, value);
        }

        /// <summary>
        /// Add a state using reflection to retrieve the approriate methods on the Entity.
        /// For example, a key with a value of "Idle" will retrieve the methods "EnterIdle" "UpdateIdle" and "ExitIdle" automatically.
        /// </summary>
        /// <param name="key">The key to reference the State with.</param>
        public void AddState(TState key, Entity e = null) {
            if (e == null) e = Entity; // Use the Component's Entity if no entity specified.

            if (states.ContainsKey(key)) return; // Dont add duplicate states.

            var state = new State();
            var name = key.ToString();
            //Using reflection to find all the appropriate functions!
            MethodInfo mi;
            mi = e.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) e.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnEnter = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
            }

            mi = e.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) e.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnUpdate = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
            }

            mi = e.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) e.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnExit = (Action)Delegate.CreateDelegate(typeof(Action), e, mi);
            }

            states.Add(key, state);
        }

        #endregion

    }

    /// <summary>
    /// Used in StateMachine. Contains functions for enter, update, and exit.
    /// </summary>
    public class State {

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
        public State(Action onEnter = null, Action onUpdate = null, Action onExit = null) {
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
        public void Functions(Action onEnter, Action onUpdate, Action onExit) {
            if (onEnter != null) {
                OnEnter = onEnter;
            }
            if (onUpdate != null) {
                OnUpdate = onUpdate;
            }
            if (onExit != null) {
                OnExit = onExit;
            }
        }

        /// <summary>
        /// Call OnUpdate.
        /// </summary>
        public void Update() {
            OnUpdate();
        }

        /// <summary>
        /// Call OnEnter.
        /// </summary>
        public void Enter() {
            OnEnter();
        }

        /// <summary>
        /// Call OnExit.
        /// </summary>
        public void Exit() {
            OnExit();
        }

        #endregion
        
    }
}

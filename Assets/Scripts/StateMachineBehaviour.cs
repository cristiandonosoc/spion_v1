using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Mark the options explicitly numbered,
/// so that if new things are added, it doesn't depend on order.
/// </summary>
public class StateMachineEnum : Attribute { }

[Serializable]
public class StateTransitions {
    public int state;
    public List<int> transitions;

    public StateTransitions(int state) {
        this.state = state;
        transitions = new List<int>();
    }

    public bool AddTransition(int transition) {
        if (transitions.Contains(transition)) {
            return false;
        }
        transitions.Add(transition);
        return true;
    }

    public bool HasTransition(int transition) {
        return transitions.Contains(transition);
    }

    public bool RemoveTransition(int transition) {
        return transitions.Remove(transition);
    }

}

public class StateMachineBehaviour : CustomMonoBehaviour {

    #region DATA

    [Serializable]
    public class Data {
        public string name;
        public SerializableType enumType;
        public List<StateTransitions> stateTransitions;
        public int currentStateRepresentation = 0;
        public bool stateSet = false;
    }
    [SerializeField]
    private Data _data;
    private Data InternalData {
        get {
            if (_data == null) {
                _data = new Data();
            }
            return _data;
        }
    }

    public string Name {
        get { return InternalData.name; }
        set { InternalData.name = value; }

    }

    public Type StateEnumType {
        get { return InternalData.enumType.type;  }
    }

    /// <summary>
    /// Normally only used for editor or debugt
    /// </summary>
    public List<StateTransitions> StateTransitions {
        get {
            if (InternalData.stateTransitions == null) {
                InternalData.stateTransitions = new List<StateTransitions>();
            }
            return InternalData.stateTransitions;
        }
    }

    public StateTransitions GetStateTransitionsForState<T>(T state) where T : IConvertible {
        int intState = state.ToInt32(null);
        return GetStateTransitionsForState(intState);
    }

    public StateTransitions GetStateTransitionsForState(int state) {
        foreach (StateTransitions transitions in StateTransitions) {
            if (transitions.state == state) {
                return transitions;
            }
        }
        return null;
    }

    /// <summary>
    /// This shouldn't be used in gameplay, only for editor purposes.
    /// Use ChangeState instead.
    /// </summary>
    public int CurrentInternalStateRepresentation {
        get { return InternalData.currentStateRepresentation; }
        set { InternalData.currentStateRepresentation = value; }
    }

    public StateTransitions CurrentStateTransitions {
        get {
            return GetStateTransitionsForState(InternalData.currentStateRepresentation);
        }
    }

    #endregion DATA

    private void TypeCheck(Type type) {
        if (!InternalData.stateSet) {
            string m = string.Format("State is not set!");
            throw new InvalidProgramException(m);
        }
        if (type != InternalData.enumType) {
            string m = string.Format("Invalid type given: {0}, expected {1}",
                                     type.ToString(), InternalData.enumType.ToString());
            throw new InvalidProgramException(m);
        }
    }

    /// <summary>
    /// Sets the StateMachineEnum to be used by this StateMachineBehaviour.
    /// Will remove all the StateTransitions associated.
    /// </summary>
    /// <param name="newType">Type of the enum.</param>
    public void SetType(Type newType) {
        if (!newType.IsEnum) {
            LogError("New Type {0} is not enum!", newType.ToString());
            return;
        }
        if (!TypeHelpers.TypeHasAttribute(newType, typeof(StateMachineEnum))) {
            LogError("New Type {0} doesn't have needed {1} attribute!",
                     newType, typeof(StateMachineEnum));
        }

        InternalData.enumType = newType;
        StateTransitions.Clear();

        // Enum values
        Array values = Enum.GetValues(newType);
        for (int i = 0; i < values.Length; i++) {
            int state = (int)values.GetValue(i);
            StateTransitions.Add(new StateTransitions(state));
        }
        InternalData.stateSet = true;
    }


    /// <summary>
    /// Gets the current state of the StateMachine
    /// </summary>
    /// <typeparam name="T">The Type of the state machine. This is so that we can correctly interpret the enum.</typeparam>
    /// <param name="ignoreCheck">Whether to perform the correct type check. Should always be true.</param>
    /// <returns>Casted enum for the state machine.</returns>
    public T GetCurrentState<T>(bool ignoreCheck = false) where T : IConvertible {
        if (!ignoreCheck) { TypeCheck(typeof(T)); }
        // TODO(Cristian): Is this really expensive?
        return (T)(object)InternalData.currentStateRepresentation;
    }

    /// <summary>
    /// Ask whether the current state is the one given.
    /// </summary>
    /// <typeparam name="T">Should be deduced from the state machine</typeparam>
    /// <param name="state">The state to query for</param>
    /// <returns>Whether the given state is the current state.</returns>
    public bool IsCurrentState<T>(T state) where T : IConvertible {
        TypeCheck(typeof(T));
        int intState = state.ToInt32(null);
        return InternalData.currentStateRepresentation == intState;
    }

    public bool AddTransition<T>(T fromState, T toState) where T : IConvertible {
        TypeCheck(typeof(T));
        int fromInt = fromState.ToInt32(null);
        int toInt = toState.ToInt32(null);

        // I'm pretty confident the state should be there
        StateTransitions transitions = GetStateTransitionsForState(fromInt);
        return transitions.AddTransition(toInt);
    }

    public bool HasCurrentTransition<T>(T toState, bool ignoreCheck = false) where T : IConvertible {
        if (!ignoreCheck) { TypeCheck(typeof(T)); }
        int toInt = toState.ToInt32(null);
        return CurrentStateTransitions.HasTransition(toInt);
    }

    public bool ChangeState<T>(T toState) where T :IConvertible {
        TypeCheck(typeof(T));
        int toInt = toState.ToInt32(null);
        if (!HasCurrentTransition(toState, ignoreCheck: true)) {
            T currentState = GetCurrentState<T>(ignoreCheck: true);
            LogError("State Machine \"{0}\" [Type: {1}]: No transition from {2} -> {3}",
                     Name, typeof(T), currentState, toState);

            return false;
        }
        InternalData.currentStateRepresentation = toInt;
        return true;
    }
}

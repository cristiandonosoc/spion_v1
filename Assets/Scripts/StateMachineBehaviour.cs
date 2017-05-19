﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public interface IFromIntable {


}

/// <summary>
/// State Machine Enums have only one requisite.
/// There has to be a 0 option
/// Other than that, mark the options explicitly numbered,
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

    [Serializable]
    public class Data {
        public SerializableType enumType;
        public List<StateTransitions> stateTransitions;
        public int currentState = 0;
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

    public SerializableType StateEnumType {
        get { return InternalData.enumType;  }
    }

    public List<StateTransitions> StateTransitions {
        get {
            if (InternalData.stateTransitions == null) {
                InternalData.stateTransitions = new List<StateTransitions>();
            }
            return InternalData.stateTransitions;
        }
    }

    public StateTransitions GetStateTransition(int state) {
        foreach (StateTransitions transitions in StateTransitions) {
            if (transitions.state == state) {
                return transitions;
            }
        }
        return null;
    }

    public StateTransitions CurrentStateTransition {
        get {
            return GetStateTransition(InternalData.currentState);
        }
    }

    public void SetType(Type newType) {
        if (!newType.IsEnum) {
            LogError("New Type {0} is not enum!", newType.ToString());
            return;
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

    // TODO(Cristian): Is this really expensive?
    public T GetCurrentState<T>() where T : IConvertible {
        TypeCheck(typeof(T));
        return (T)Convert.ChangeType(InternalData.currentState, typeof(T));
    }

    public bool AddTransition<T>(T fromState, T toState) where T : IConvertible {
        TypeCheck(typeof(T));
        int fromInt = fromState.ToInt32(null);
        int toInt = toState.ToInt32(null);

        // I'm pretty confident the state should be there
        StateTransitions transitions = GetStateTransition(fromInt);
        return transitions.AddTransition(toInt);
    }

    public bool HasCurrentTransition<T>(T toState) where T : IConvertible {
        TypeCheck(typeof(T));
        int toInt = toState.ToInt32(null);
        return CurrentStateTransition.HasTransition(toInt);
    }


    protected override void EditorAwake() {
    }

    protected override void PlayModeAwake() {
    }

    public override void Refresh() {
    }
}
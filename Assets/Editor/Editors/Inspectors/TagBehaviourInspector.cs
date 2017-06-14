using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagBehaviourEditor : SpecializedInspector {

    private TagBehaviour _target;

    public override void OnInspectorGUI() {
        _target = (TagBehaviour)target;

        IndentedInspector("Tags", TagInspector);
        HorizontalBreak();
        IndentedInspector("Debug", DebugInspector, false);
    }

    private void TagInspector() {


    }

    private void DebugInspector() {
        DrawDefaultInspector();
    }


}

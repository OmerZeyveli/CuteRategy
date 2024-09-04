using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    void Start() {
        // Disable normal mouse icon.
        Cursor.visible = false;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }
}

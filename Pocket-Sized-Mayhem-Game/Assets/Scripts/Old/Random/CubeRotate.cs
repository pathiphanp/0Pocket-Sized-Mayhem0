using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotate : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }
}

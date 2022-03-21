using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
   [SerializeField]
   float rotationspeed;
    [SerializeField]
   GameObject torotate;

    // Update is called once per frame
    void Update()
    {
        torotate.transform.Rotate(0,0,rotationspeed,Space.Self);
    }
}

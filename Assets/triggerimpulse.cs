using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cinemachine{
public class triggerimpulse : MonoBehaviour
{
    
   public CinemachineImpulseSource ImpulseSource;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
           ImpulseSource.GenerateImpulse();
        }

    }
}}

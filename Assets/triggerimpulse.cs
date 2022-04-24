using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class triggerimpulse : MonoBehaviour
{
    
   public CinemachineImpulseSource ImpulseSource;
    void Update()
    {
           
        if (Mouse.current.leftButton.isPressed){
           ImpulseSource.GenerateImpulse();
        Debug.Log("impulse");
        }
        
    }
}

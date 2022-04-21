using UnityEngine;

public class RotateAndBob : MonoBehaviour
{
    [Range(0,2)]
    public float verticalRange = 1;

    [Range(0,10)]
    public float verticalSpeed = 1;

    [Range(0,360)]
    public float angularSpeed = 1;

    private Vector3 startPosition;
    private Quaternion startRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, angularSpeed * Time.deltaTime);
        
        var verticalOffset = verticalRange * Mathf.Sin(Time.time/verticalSpeed);
        transform.position = startPosition + Vector3.up * verticalOffset;
    }
}

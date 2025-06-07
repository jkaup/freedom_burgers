using UnityEngine;

public class Movement : MonoBehaviour
{

    public static float RAYCAST_MAX_DIST = 1000.0f;
    public float MoveSpeed;
    public float CoastHeight;
    public float CoastSpringStrength;
    public float RotationStrength;
    Rigidbody Body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Body = gameObject.GetComponent<Rigidbody>();
    }

    static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Do raycast downwards
        //LayerMask layerMask = LayerMask.GetMask("Wall", "Character");
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 rayDir = Vector3.down;
        bool rayHit = Physics.Raycast(origin, rayDir, out hit, RAYCAST_MAX_DIST);

        if (rayHit)
        {
            // Draw line from origin to object
            Debug.DrawLine(origin, origin + hit.distance * rayDir, Color.red);

            // Force up - keep at height
            float y = hit.distance - CoastHeight;
            float coastSpringForce = y * CoastSpringStrength;

            //Debug.LogFormat("{0}, {1}, {2}, {3}", y, coastSpringForce, CoastSpringStrength, CoastHeight);
            Body.AddForce(rayDir * coastSpringForce);
        }

        // Forwards/backwards input
        Vector3 move = transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical");
        move = Vector3.ClampMagnitude(move, 1f); // Optional: prevents faster diagonal movement
        Body.AddForce(move * MoveSpeed);

        // Rotation input
        Vector3 rotation = transform.TransformDirection(Vector3.up) * Input.GetAxis("Horizontal") * 0.01f;
        Body.AddTorque(rotation * RotationStrength, ForceMode.Impulse);

        // Try to keep body upright
        Quaternion characterCurrent = transform.rotation;
        Quaternion upQ = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward), Vector3.up);
        Quaternion toGoal;
        if (Quaternion.Dot(upQ, characterCurrent) < 0)
        {
            toGoal = upQ * Quaternion.Inverse(Multiply(characterCurrent, -1));
        }
        else
        {
            toGoal = upQ * Quaternion.Inverse(characterCurrent);
        }

        Vector3 rotAxis;
        float rotDegrees;
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        float UprightSpringStrength = 1.0f;
        float UprightSpringDamper = 0.5f;
        Body.AddTorque(rotAxis * (rotRadians * UprightSpringStrength) - Body.angularVelocity * UprightSpringDamper );

        // TODO: slow down forces when letting go of input
        // "Friction"

        // TODO: wobble the body when turning (turn/curve in the other directino)

        // TODO: make coasting less springy
    }
}

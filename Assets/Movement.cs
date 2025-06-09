using UnityEngine;

public class Movement : MonoBehaviour
{

    public static float RAYCAST_MAX_DIST = 1000.0f;
    public float MoveSpeed;
    public float CoastHeight;
    public float CoastSpringStrength;
    public float RotationStrength;
    public float UprightSpringStrength;
    public float UprightSpringDamper;
    public float TurnRotationStrength;
    Rigidbody Body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
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

            Body.AddForce(rayDir * coastSpringForce);
        }

        // TODO: fix so that character cannot fly
        // Forwards/backwards input
        Vector3 move = transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical");
        move = Vector3.ClampMagnitude(move, 1f); // Optional: prevents faster diagonal movement
        Body.AddForce(move * MoveSpeed);

        // Rotation input
        Vector3 rotation = transform.TransformDirection(Vector3.up) * Input.GetAxis("Horizontal") * 0.01f;
        Body.AddTorque(rotation * RotationStrength, ForceMode.Impulse);

        // Try to keep body upright
        // TODO: fix upright correction to work also without rotating player manually
        Quaternion characterCurrent = transform.rotation;
        Vector3 forwards = transform.TransformDirection(Vector3.forward);
        // Y is not considered for forwards vector (shouldn't look down or up), only xz plane matters
        forwards[1] = 0;

        Quaternion upQ = Quaternion.LookRotation(forwards, Vector3.up);
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

        Body.AddTorque(rotAxis * (rotRadians * UprightSpringStrength) - Body.angularVelocity * UprightSpringDamper);

        // Wobble the body when turning (turn/curve in the other direction)
        Vector3 turnAxis = transform.TransformDirection(Vector3.forward);
        Body.AddTorque(turnAxis * Body.angularVelocity[1] * TurnRotationStrength);

        // TODO: adjust player rotation to align with slopes - should tilt backwards a bit when climbing a slope

        // TODO: add obstacles and make play area bigger with walls
    }
}

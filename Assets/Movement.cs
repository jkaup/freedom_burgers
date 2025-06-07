using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Do raycast downwards
        //LayerMask layerMask = LayerMask.GetMask("Wall", "Character");
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 rayDir = Vector3.down;
        bool rayHit = Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity);

        if (rayHit)
        {
            // Draw line from origin to object
            Debug.DrawLine(origin, origin + hit.distance * rayDir, Color.red);
        }

        //Debug.DrawLine(origin, origin + 20 * rayDir, Color.green);
        Vector3 origin2 = new Vector3(0f, 10f, 0f);
        Debug.DrawLine(origin2, origin2 + 20 * rayDir, Color.black);

        // Horizontal input
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1f); // Optional: prevents faster diagonal movement

        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        body.AddForce(move * 1f);
    }
}

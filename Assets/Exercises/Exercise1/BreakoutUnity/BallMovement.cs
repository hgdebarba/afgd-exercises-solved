using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Vector3 direction = Vector3.up;
    public float speed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        this.transform.position += direction * speed * Time.deltaTime;
        Debug.DrawRay(this.transform.position, direction, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 paddle2ball = (this.transform.position - other.transform.position).normalized;
            direction = Vector3.Reflect(direction, paddle2ball);
        }
        else
        {
            Ray ray = new Ray(this.transform.position, direction);
            RaycastHit hitInfo;
            if (other.Raycast(ray, out hitInfo, 100))
                direction = Vector3.Reflect(direction, hitInfo.normal);
        }
    }

}

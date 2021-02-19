using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
            Destroy(this.gameObject);
    }
}

using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class SubmitCube : NetworkBehaviour
{
    bool isHandlingCollision = false;

    IEnumerator HandleCollision(Collider other)
    {
        if (!isHandlingCollision && other.CompareTag("moveable"))
        {
            isHandlingCollision = true;
            
            yield return new WaitForSeconds(10f);

            Rigidbody cubeRigidbody = other.GetComponent<Rigidbody>();

            if (cubeRigidbody != null)
            {
                other.transform.rotation = Quaternion.identity;

                yield return new WaitForSeconds(0.1f);

                cubeRigidbody.isKinematic = true;
            }

            isHandlingCollision = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(HandleCollision(other));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui gère la collision dans les enfants d'un objet possédant un rigidbody et qui remonte l'information au parent.
/// </summary>
public class ChildCollision : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Balloon collided :" + this.gameObject.name + " was hit by " + collision.other.name);
        this.GetComponent<Collider>().attachedRigidbody.SendMessage("OnCollisionEnter", collision);
    }
}

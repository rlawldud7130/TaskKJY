using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Æ¨°Ü³ª°£ ÃÑ¾Ë Áö¿ì±â
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_function : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.back * Time.deltaTime * 25);

    }
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.tag == "Bullet")
    //     {
    //         Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    // private void OnTriggerEnter(Collider collision)
    // {
    //     if (collision.gameObject.tag == "Bullet")
    //     {
    //         Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}

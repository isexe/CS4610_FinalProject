using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword_control : MonoBehaviour
{
    bool swinging;
    // Start is called before the first frame update
    void Start()
    {
        swinging = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("PlayerCamera");
        if (Input.GetMouseButtonDown(0) && (swinging == false))
        {
            swinging = true;
            gameObject.GetComponent<Collider>().enabled = true;
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        Debug.Log("Now Waiting");
        Vector3 Start = transform.parent.transform.eulerAngles;
        Vector3 swordStart = transform.eulerAngles;
        transform.Rotate(0, 0, -100, Space.Self);
        yield return new WaitForSeconds(.1f);
        transform.parent.transform.eulerAngles = Start;
        transform.eulerAngles = swordStart;
        swinging = false;
        gameObject.GetComponent<Collider>().enabled = false;
        Debug.Log("Done");
    }
}

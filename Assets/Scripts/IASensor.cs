using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASensor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            transform.parent.GetComponent<EnemyIA>().PlayerEnterZone();
            transform.parent.GetComponent<EnemyIA>().UpdatePlayerPosition(other.transform.position);
        }
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        //  Keep distance to player update
        if(other.tag == "Player"){
            transform.parent.GetComponent<EnemyIA>().UpdatePlayerPosition(other.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Player"){
            transform.parent.GetComponent<EnemyIA>().PlayerExitZone();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    [SerializeField] private float leftXLimit = -40f;
    [SerializeField] private float rightXLimit = 20f;
    [SerializeField] private float upYLimit = 30f;
    [SerializeField] private float downYLimit = -5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float FitLimits(float val, float inf, float sup){
        if(val < inf)
            return inf;
        if(val >= sup)
            return sup;
        return val;
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = FitLimits(player.position.x, leftXLimit, rightXLimit);
        float yPos = FitLimits(player.position.y, downYLimit, upYLimit);

        transform.position = new Vector3(xPos, yPos, transform.position.z);
        
    }
}

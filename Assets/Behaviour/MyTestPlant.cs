using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyTestPlant : MonoBehaviour
{

    float amountRemaining = 1;
    const float consumeSpeed = 8;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float Consume (float amount) {
        float amountConsumed = Mathf.Max (0, Mathf.Min (amountRemaining, amount));
        amountRemaining -= amount * consumeSpeed;

        transform.localScale = Vector3.one * amountRemaining;

        if (amountRemaining <= 0) {
            //Die (CauseOfDeath.Eaten);
        }

        return amountConsumed;
    }

    public float AmountRemaining {
        get {
            return amountRemaining;
        }
    }
}

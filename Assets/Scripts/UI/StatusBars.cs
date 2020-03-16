using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBars : MonoBehaviour
{
    [Header("unity stuff")]
    private Image hungerBar;
    private Image thirstBar;
    private float hunger = 1f;
    private float thirst = 1f;

    // Start is called before the first frame update
    void Start()
    {
        hungerBar = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        thirstBar = gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hungerBar.fillAmount = hunger;
        thirstBar.fillAmount = thirst;
    }

    public void UpdateStatus(float hunger, float thirst, double energy)
    {
        this.hunger = 1f-hunger;
        this.thirst = 1f-thirst;
    }


}

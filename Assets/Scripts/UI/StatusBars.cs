using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBars : MonoBehaviour
{
    public static Vector3 scale = new Vector3(0.01152718f, 0.02385204f, 0.01152718f);
    [Header("unity stuff")]
    private Image hungerBar;
    private Image thirstBar;
    private Image heatBar;
    private float hunger = 1f;
    private float thirst = 1f;
    private float heat = 0f;

    // Start is called before the first frame update
    void Start()
    {
        hungerBar = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        thirstBar = gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
        heatBar = gameObject.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hungerBar.fillAmount = hunger;
        thirstBar.fillAmount = thirst;
        heatBar.fillAmount = heat;
    }

    public void UpdateStatus(float hunger, float thirst, double energy, float heat)
    {
        this.hunger = 1f - hunger;
        this.thirst = 1f - thirst;
        this.heat = heat;

    }


}

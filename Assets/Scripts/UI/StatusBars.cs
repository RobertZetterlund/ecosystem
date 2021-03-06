﻿using System.Collections;
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
	private Image energyBar;
	private Image line;
	private float hunger = 1f;
	private float thirst = 1f;
	private float heat = 0f;
	private float energy = 0f;
	private float adultSizeFactor;
	private bool drawLine = false;

	// Start is called before the first frame update
	void Start()
	{
		hungerBar = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
		thirstBar = gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
		heatBar = gameObject.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
		energyBar = gameObject.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Image>();

		line = gameObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
	}

	public void Init(float adultSizeFactor)
	{
		this.adultSizeFactor = adultSizeFactor;
		this.drawLine = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (drawLine)
		{
			line.rectTransform.offsetMin = new Vector2(100f * adultSizeFactor - 2, line.rectTransform.offsetMin.y);
			line.rectTransform.offsetMax = new Vector2(-100f * (1 - adultSizeFactor) + 2, line.rectTransform.offsetMax.y);
			drawLine = false;
		}

		hungerBar.fillAmount = hunger;
		thirstBar.fillAmount = thirst;
		heatBar.fillAmount = heat;
		energyBar.fillAmount = energy;
	}

	public void UpdateStatus(float hunger, float thirst, float energy, float heat)
	{
		this.hunger = hunger;
		this.thirst = 1f - thirst;
		this.heat = heat;
		this.energy = energy;


	}

	public void Destroy()
	{
		Destroy(hungerBar.gameObject);
		Destroy(thirstBar.gameObject);
		Destroy(heatBar.gameObject);
		Destroy(energyBar.gameObject);

		Destroy(gameObject.transform.GetChild(0).gameObject);
		Destroy(gameObject.transform.GetChild(1).gameObject);
		Destroy(gameObject.transform.GetChild(2).gameObject);
		Destroy(gameObject.transform.GetChild(3).gameObject);

		Destroy(gameObject);
	}

}

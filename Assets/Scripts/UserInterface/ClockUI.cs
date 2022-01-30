using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockUI: MonoBehaviour
{
	private const float REAL_SECONDS_PER_INGAME_DAY = 5f;

	private Transform clockHandTransform;
	private float day;

	private void Awake()
	{
		clockHandTransform = transform.Find("clockHand");
	}

	// Update is called once per frame
	void Update()
    {
		day += Time.deltaTime / REAL_SECONDS_PER_INGAME_DAY;
		float dayNormalized = day % 1f;
		float rotationDegreesPerDay = 360f;
		clockHandTransform.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesPerDay );
    }
}

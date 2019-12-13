using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviour
{
	[SerializeField] private Transform baseObject;
	[SerializeField] public Transform goal;
	[SerializeField] private Sprite indicatorTexture;
	[SerializeField] private float radiusFromBase = 2.5f;
	
	private Transform indicator;

    // Start is called before the first frame update
    void Start()
    {
		GameObject newGO = new GameObject("Indicator");
		SpriteRenderer sprRenderer = newGO.AddComponent<SpriteRenderer>();
		sprRenderer.sprite = indicatorTexture;

		indicator = newGO.transform;
		indicator.parent = baseObject;
    }

    void Update()
    {
        Vector3 dir = (goal.position - baseObject.position).normalized;

		Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, dir));

		indicator.localRotation = rot;
		indicator.localPosition = dir * radiusFromBase;
    }
}

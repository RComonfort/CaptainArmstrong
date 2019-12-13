using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviour
{
	[SerializeField] private Transform baseObject;
	[SerializeField] private Transform goal;
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

		indicator.rotation = rot;
		indicator.position = indicator.parent.position + dir * radiusFromBase;
    }

	public void SetNewBaseObject(Transform newBase, Bounds bounds)
	{
		baseObject = newBase;
		radiusFromBase = Mathf.Sqrt(Mathf.Pow(bounds.extents.x, 2) + Mathf.Pow(bounds.extents.y, 2));
	}

	public void SetGoal(Transform newGoal)
	{
		goal = newGoal;
	}
}

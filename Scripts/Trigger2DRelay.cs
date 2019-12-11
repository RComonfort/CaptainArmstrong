using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Trigger2DRelay : MonoBehaviour
{
	[Tooltip("Reference to an ITriggerListener")] public MonoBehaviour listenerReference;

	//Whether the trigger should ignore colliders that share an ancestor with our gameobject
	[SerializeField] private bool ignoreAncestor = true;
	
	private ITriggerListener listener;

	[HideInInspector] public Collider2D triggerCollider {get; private set;}

    // Start is called before the first frame update
    void Awake()
    {
		listener = listenerReference as ITriggerListener;

		Assert.IsNotNull(listener, "There must be an assigned listener for the trigger");

		triggerCollider = GetComponent<Collider2D>();

		Assert.IsNotNull<Collider2D>(triggerCollider, "There must be a collider 2D on this object");
        Assert.IsTrue(triggerCollider.isTrigger, "The collider " + triggerCollider.name + "must be set as trigger");
    }

	/*
		Returns true if the collider and the triggerScript that detected it share an ancester
	*/
	private bool HaveSharedAncester(Collider2D collider, Trigger2DRelay triggerScript)
	{
		return 	collider.transform.root == triggerScript.transform.root;
	}

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (ignoreAncestor && HaveSharedAncester(other, this))
			return;

		listener?.OnObjectEnteredTrigger(this, other);	
	}

	private void OnTriggerStay2D(Collider2D other) 
	{
		if (ignoreAncestor && HaveSharedAncester(other, this))
			return;

		listener?.OnObjectStayedTrigger(this, other);	
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (ignoreAncestor && HaveSharedAncester(other, this))
			return;

		listener?.OnObjectExitedTrigger(this, other);	
	}
}

public interface ITriggerListener
{
	void OnObjectEnteredTrigger(Trigger2DRelay triggerObj, Collider2D other);

	void OnObjectExitedTrigger(Trigger2DRelay triggerObj, Collider2D other);

	void OnObjectStayedTrigger(Trigger2DRelay triggerObj, Collider2D other);
}

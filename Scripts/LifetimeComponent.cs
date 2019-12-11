using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeComponent : MonoBehaviour
{
	[SerializeField] private EDestroyPolicy destroyBy;

	//Time before destruction. If destroy policy is time, lifetime is considered from Start(). If it is by leaving screen, it will count after object stops rendering the first time
	[SerializeField] private float lifetime = 100f;    
	

    // Start is called before the first frame update
    void Start()
    {
        if (destroyBy == EDestroyPolicy.ByTime)
			Invoke("DestroyObj", lifetime);
    }

    private void OnBecameInvisible() 
	{
		if (destroyBy == EDestroyPolicy.ByLeavingScreen)
			Invoke("DestroyObj", lifetime);
	}

	private void OnBecameVisible() {
		if (destroyBy == EDestroyPolicy.ByLeavingScreen)
			CancelInvoke();
	}

	private void DestroyObj()
	{
		Destroy(this);
	}
}

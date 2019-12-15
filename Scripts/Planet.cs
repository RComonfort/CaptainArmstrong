using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Planet : MonoBehaviour
{
	[SerializeField] private Sprite[] planetSprites;
	

    void Start()
    {
		GetComponent<SpriteRenderer>().sprite = planetSprites[Random.Range(0, planetSprites.Length)];

		transform.localScale *= Random.Range(0.5f, 1.3f);
    }
}

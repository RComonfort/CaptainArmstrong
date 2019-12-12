using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private Vector3 iconScaleOverride = Vector3.one;
	
	private Player player;
	private Image bar;

    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<Player>();
		bar = GetComponent<Image>();

		GameObject healthIcon = transform.GetChild(0).gameObject;

		for (int i = 0; i < player.MaxHealth - 1; i++)
		{
			GameObject newHealthIcon = Instantiate(healthIcon);
			newHealthIcon.transform.SetParent(transform);

			newHealthIcon.GetComponent<RectTransform>().localScale = iconScaleOverride;
		}
    }

    // Update is called once per frame
    void Update()
    {
        bar.fillAmount = (float)player.hp / player.MaxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadTriggerColorer : MonoBehaviour
{
	[SerializeField] SpriteRenderer renderer;
	[SerializeField] Collider2D collider;
	[SerializeField] Color normalColor = new Color(255, 255, 255);
	[SerializeField] Color offColor = new Color(155, 155, 155);
	[SerializeField] LayerMask playerLayers;


	private void Start()
	{
		if (renderer == null)
		{
			Debug.LogWarning("ReloadTriggerColorer on " + gameObject.name + " got sprite Renderer automatically");
			renderer = GetComponent<SpriteRenderer>();
		}

		if (collider == null)
		{
			Debug.LogWarning("ReloadTriggerColorer on " + gameObject.name + " got collider automatically");
			collider = GetComponent<Collider2D>();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		UpdateColor();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		UpdateColor();
	}

	private void UpdateColor()
	{
		if (collider.IsTouchingLayers(playerLayers))
		{
			renderer.color = offColor;
		}
		else
		{
			renderer.color = normalColor;
		}
	}
}

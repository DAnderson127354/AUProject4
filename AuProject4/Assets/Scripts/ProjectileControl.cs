using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
	public float lifespan = 10f;

	void Start()
	{
		Destroy(gameObject, lifespan);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.name == "Player")
			Destroy(gameObject);
	}

}

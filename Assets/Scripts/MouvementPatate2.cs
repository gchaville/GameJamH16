﻿using UnityEngine;
using System.Collections;

public class MouvementPatate2 : MonoBehaviour
{

    private Rigidbody2D rb1;

    void Start()
    {
        int rnd1 = Random.Range(-1500, -1000);
        int rnd2 = Random.Range(200, 500);
        rb1 = GetComponent<Rigidbody2D>();
        rb1.AddForce(new Vector2(rnd1, rnd2));
    }
}

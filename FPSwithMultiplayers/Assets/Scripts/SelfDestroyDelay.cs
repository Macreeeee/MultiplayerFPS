﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyDelay : MonoBehaviour
{
    private void Start() {
        Destroy(gameObject, 2.0f);
    }
}

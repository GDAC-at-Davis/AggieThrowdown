using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterHitbox : MonoBehaviour
{
    private int playerIndex;

    public void Initialize(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }
}
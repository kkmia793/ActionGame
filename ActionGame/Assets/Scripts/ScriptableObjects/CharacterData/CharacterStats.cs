using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats", order = 1)]
public class CharacterStats : ScriptableObject
{
    public float moveSpeed = 5f;
    public float health = 100f;
    public float jumpForce = 10f;
    public float attackPower = 15f;
}

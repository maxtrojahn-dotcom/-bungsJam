using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;



    [DisallowMultipleComponent]
    public class EnemyReferences : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent navMEshagent;
    [HideInInspector]
    public Animator animator;

    [Header("Stats")]

    public float pathUpdateDelay = 0.2f;

    private void Awake()
    {
        navMEshagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }



}

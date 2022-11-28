using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
/// ����״̬
/// </summary>
public enum EnemyStateType
{
    Idle,Chase,Attack,Die
}
/// <summary>
/// ���˲������Ժ�Ҫ��CharacterInfo���������
/// </summary>
[Serializable]
public class Parameter
{
    public int health = 100;
    public float speed = 10f;
    public Transform target;
    public Animator animator;
    public NavMeshAgent agent;
    public float viewRange = 100f;
    public float attackRange = 5f;

}
public class FSM : MonoBehaviour
{
    public Parameter parameter;
/// <summary>��ǰ״̬/// </summary>
    private IState currentState;
    /// <summary>
    /// ����״̬�ֵ�
    /// </summary>
    private Dictionary<EnemyStateType,IState> states = new Dictionary<EnemyStateType, IState>();


    void Start()
    {
        //���״̬
        states.Add(EnemyStateType.Idle, new IdleStates(this));
        states.Add(EnemyStateType.Chase, new ChaseStates(this));
        states.Add(EnemyStateType.Attack,new AttackStates(this));
        states.Add(EnemyStateType.Die, new DieStates(this));
        //Ѱ�����������
        parameter.animator = GetComponent<Animator>();
        parameter.agent = GetComponent<NavMeshAgent>();
        parameter.target = GameObject.FindWithTag("Player").transform;
        
        TransitionState(EnemyStateType.Idle);//���ó�ʼ״̬

    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();//ִ�ж�Ӧ״̬��Update����
    }
    /// <summary>
    /// ���״̬
    /// </summary>
    /// <param name="type">״̬</param>
    public void TransitionState(EnemyStateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();//ִ�ж�Ӧ״̬Exit����
        }
        currentState = states[type];//�ֵ����״̬
        currentState.OnEnter();//ִ�ж�Ӧ״̬Enter����
    }
/// <summary>
/// ����Ŀ��λ�øı䳯��
/// </summary>
    public void FaceTo(Transform target)
    {
        if(target != null)
        {
            transform.LookAt(target);
        }
    }
}

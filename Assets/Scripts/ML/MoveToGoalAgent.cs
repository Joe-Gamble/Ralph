using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;



public class MoveToGoalAgent : Agent
{
    public bool test = false;

    [SerializeField]
    private Transform targetTransform;

    private float orig_dist;
    private float speed = 100.0f;

    private void Start()
    {
        if (test)
        {
            speed = 1.0f;
        }

        orig_dist = Vector3.Distance(this.transform.position, targetTransform.position);
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        float new_dist = Vector3.Distance(this.transform.position, targetTransform.position);
        if (new_dist > orig_dist)
        {
            Counter.AddTry(false);
            SetReward(-0.5f);
            EndEpisode();
        }
        else
        {
            //???????
            SetReward((orig_dist - new_dist) / orig_dist);
        }
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("start");
        //this.transform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(0, 4));
        //targetTransform.localPosition = new Vector3(Random.Range(-4, 4), 0.5f, Random.Range(-4, 0));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
}
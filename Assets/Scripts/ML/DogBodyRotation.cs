using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DogBodyRotation : Agent
{
    //if head roation is greater than 30 degrees? move the body

    //Dog Vision
    public DogVision vison;

    public float sens = 1.0f;

    private float orignal_angle;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private Vector3 old_rotation;

    private int num = 0;
    private int max_num = 50;

    private bool looking_at_target = false;
    private bool looking_at_center = false;

    //Test Incentive
    Incentive test_incentive;
    public GameObject test_object;

    void Start()
    {
        test_incentive = new Incentive(test_object, new Vector3(0, 0, 1));

        Vector2 toVector = transform.position - test_incentive.incentive.transform.position;
        orignal_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);

        old_rotation = transform.eulerAngles;
    }

    private void FixedUpdate()
    {
        orignal_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);
    }

    void LateUpdate()
    {
        Vector2 toVector = transform.position - test_incentive.incentive.transform.position;
        float new_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);

        #region TargetHit

        Ray RayOrigin;
        RaycastHit hit;

        RayOrigin = new Ray(transform.position, transform.forward);

        test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.yellow;

        if (Physics.Raycast(RayOrigin, out hit, 200))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.gameObject == test_incentive.incentive)
            {
                SetReward(1f);
                test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.green;

                if (Mathf.Abs(new_angle) < 1.5)
                {
                    Debug.Log("Center Hit");
                    SetReward(1f);
                    test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.magenta;

                    if (looking_at_center)
                    {
                        //AddReward(3.0f);
                        Debug.Log("consecutive center");
                    }

                    looking_at_center = true;
                    return;
                }
                else
                {
                    if (looking_at_target)
                    {
                        AddReward(1.0f);
                    }
                    looking_at_target = true;
                }
            }
            EndEpisode();
        }
        else
        {
            if (looking_at_target)
            {
                Debug.Log("Lost Target");
                AddReward(-3f);
                looking_at_target = false;

                if (looking_at_center)
                {
                    Debug.Log("Lost Center");
                    AddReward(-1f);
                    looking_at_center = false;
                }
                test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
        }

        #endregion

        #region TargetCorrections

        #region Rotation

        if (Mathf.Abs(transform.localEulerAngles.x - old_rotation.x) >= 0.5)
        {
            AddReward(-0.25f);
        }
        else
        {
            AddReward(0.25f);
        }

        old_rotation = transform.localEulerAngles;

        #endregion

        if (new_angle > orignal_angle)
        {
            AddReward(-1f);
        }
        else
        {
            if (Mathf.Abs(vison.gameObject.transform.localEulerAngles.y) < 30)
            {
                AddReward(1.0f);
            }
            else
            {
                AddReward(-0.25f);
            }
        }

        #endregion
    }

    void moveObject()
    {
        if (test_incentive.incentive.transform.position.y <= 2)
        {
            if (Random.Range(1, 10) > 5)
            {
                Rigidbody rb = test_incentive.incentive.GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), ForceMode.Impulse);
            }
        }
    }

    public override void OnEpisodeBegin()
    {

    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(test_incentive.incentive.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float damp = sens * 100.0f * Time.deltaTime;
        xRotation += actions.ContinuousActions[0] * damp;

        transform.eulerAngles = new Vector3(-yRotation, xRotation, 0.0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Mouse X");
    }
}

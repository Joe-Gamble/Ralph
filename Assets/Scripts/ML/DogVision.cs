using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DogVision : Agent
{
    public GameObject test_object;
    public float sens = 1.0f;
    public Camera vision;
    public float current_angle;

    private float orignal_angle;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private Vector3 old_rotation;

    private int num = 0;
    private int max_num = 50;

    private bool looking_at_target = false;
    private bool looking_at_center = false;

    private float speed = 0.1f;
    private Vector3 pos2 = new Vector3(0.09f, 0.61f, 0f);
    private Vector3 pos1 = new Vector3(-2.57f, 2.88f, 0f);

    //Move Stuff
    public Transform[] positions;
    public float MoveSpeed = 0.1f;
    Coroutine MoveIE;
    bool forward = true;

    //Test Incentive
    Incentive test_incentive;

    void Start()
    {
        test_incentive = new Incentive(test_object, new Vector3(0, 0, 1));

        Vector2 toVector = transform.position - test_incentive.incentive.transform.position;
        orignal_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);

        old_rotation = transform.eulerAngles;

        //StartCoroutine(moveObject());
    }

    void RandomPos(Vector3 Range)
    {
        test_incentive.incentive.transform.localPosition = new Vector3(Random.Range(-Range.x, Range.x), Random.Range(0.5f, Range.y), Random.Range(0, Range.z));
    }

    private void FixedUpdate()
    {
        orignal_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);
    }

    void LateUpdate()
    {
        //test_object.transform.localPosition = Vector3.Lerp(pos1, pos2, Mathf.PingPong(Time.time * speed, 1.0f));

        Vector2 toVector = transform.position - test_incentive.incentive.transform.position;
        current_angle = Vector3.Angle(transform.forward, test_incentive.incentive.transform.position - transform.position);

        #region TargetHit

        Ray RayOrigin;
        RaycastHit hit;

        RayOrigin = new Ray(vision.transform.position, vision.transform.forward);

        test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.yellow;



        if (Physics.Raycast(RayOrigin, out hit, 200))
        {
            if (hit.transform.gameObject == test_incentive.incentive)
            {
                Debug.Log("Goal");

                SetReward(1f);

                test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.green;

                if (Mathf.Abs(current_angle) < 1.5)
                {
                    Debug.Log("Center Hit");
                    SetReward(1f);
                    test_incentive.incentive.GetComponent<MeshRenderer>().material.color = Color.magenta;

                    if (looking_at_center)
                    {
                        AddReward(3.0f);
                        Debug.Log("consecutive center");
                    }
                    looking_at_center = true;
                    return;
                }
                else
                {
                    if (looking_at_target)
                    {
                        AddReward(2.5f);
                    }
                }
                looking_at_target = true;
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

        Vector3 target_screen_pos = vision.WorldToViewportPoint(test_incentive.incentive.transform.position);

        if (!(((target_screen_pos.x >= 0.0f && target_screen_pos.x <= 1.0f) && (target_screen_pos.y >= 0.0f && target_screen_pos.y <= 1.0f))))
        {
            Debug.Log("Out of screen");
            AddReward(-2.5f);
        }

        #region Rotation

        if (Mathf.Abs(transform.localEulerAngles.x - old_rotation.x) >= 0.5)
        {
            AddReward(-0.25f);
        }
        else
        {
            AddReward(0.25f);
        }

        if (Mathf.Abs(transform.localEulerAngles.y - old_rotation.y) >= 0.5)
        {
            AddReward(-0.25f);
        }
        else
        {
            AddReward(0.25f);
        }

        old_rotation = transform.localEulerAngles;

        #endregion

        if (Mathf.Abs(current_angle) < Mathf.Abs(orignal_angle))
        {
            AddReward(1f);
        }
        //Was x Value too far to the left?
        else
        {
            AddReward(-3f);
        }

        #endregion
    }

    void moveObject()
    {
        if (test_incentive.incentive.transform.position.y <= 2)
        {
            if (Random.Range(1, 10) > 8)
            {
                Rigidbody rb = test_incentive.incentive.GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f)), ForceMode.Impulse);
            }
        }
    }

    IEnumerator Moving(int currentPosition)
    {
        while (test_incentive.incentive.transform.position != positions[currentPosition].position)
        {
            test_incentive.incentive.transform.position = Vector3.MoveTowards(test_incentive.incentive.transform.position, positions[currentPosition].position, MoveSpeed / 30 * Time.deltaTime);
            yield return null;
        }
    }

    public override void OnEpisodeBegin()
    {
        moveObject();

        if (test_incentive.incentive.GetComponent<MeshRenderer>().material.color == Color.green)
        {
            Counter.AddTry(true);
        }
        else
        {
            Counter.AddTry(false);
        }
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
        yRotation += actions.ContinuousActions[1] * damp;

        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        yRotation = Mathf.Clamp(yRotation, -60f, 60f);

        transform.localEulerAngles = new Vector3(-yRotation, xRotation, 0.0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Mouse X");
        continuousActions[1] = Input.GetAxis("Mouse Y");
    }
}

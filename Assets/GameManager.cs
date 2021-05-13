using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text total;
    public Text wins;
    public Text percentage;
    public Text total1000;
    public Text wins1000;
    public Text percentage1000;

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        total.text = Counter.num_of_tries.ToString();
        wins.text = Counter.succesfull.ToString();
        percentage.text = Counter.percentage.ToString();

        total1000.text = Counter.current_1000.ToString();
        wins1000.text = Counter.succesfull_1000.ToString();
        percentage1000.text = Counter.percentage_1000.ToString();
    }
}

public static class Counter
{
    public static void AddTry(bool win)
    {
        num_of_tries++;

        if (num_of_tries % 1000 == 0)
        {
            current_1000 = 0;
            succesfull_1000 = 0;
        }

        current_1000++;
        if (win)
        {
            succesfull++;
            succesfull_1000++;
        }

        percentage = succesfull / num_of_tries * 100;
        percentage_1000 = succesfull_1000 / current_1000 * 100;
    }

    public static float num_of_tries = 0;
    public static float succesfull = 0;

    public static float current_1000 = 0;
    public static float succesfull_1000 = 0;

    public static float percentage;
    public static float percentage_1000;
}

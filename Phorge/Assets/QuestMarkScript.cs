using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestMarkScript : MonoBehaviour
{
    public Text text;
    public GameObject cube;
    public TextMeshProUGUI text2;
    // Start is called before the first frame update
    void Start()
    {
        text2.transform.position = cube.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        text2.transform.position = cube.transform.position;
    }
}

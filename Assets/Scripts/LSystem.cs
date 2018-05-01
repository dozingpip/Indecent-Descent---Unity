using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour {
    
    public string axiom;
    public Hashtable rules = new Hashtable();
    string production;
    public int iterations;
    public float StepSize;
    public float angle;
    public GameObject[] RoadParts;
    private GameObject EmptyParent;

    void Awake()
    {
        rules.Add("A", "B-F+CFC+F-D&F^D-F+&&CFC+F+B//");
        rules.Add("B", "A&F^CFB^F^D^^-F-D^|F^B|FC^F^A//");
        rules.Add("C", "|D^|F^B-F+C^F^A&&FA&F^C+F+B^F^D//");
        rules.Add("D", "|CFB-F+B|FA&F^A&&FB-F+B|FC//");
    }

    // Use this for initialization
    void Start () {

        production = axiom;

        for (int i = 0; i < iterations; i++)
        {
            string temporaryProduction = "";

            for (int j = 0; j < production.Length; j++)
            {
                //Read the leftmost symbol.
                string currentSymbol = char.ToString(production[j]);

                //Check to see if it matches a rule in the hash table.
                if (rules.ContainsKey(currentSymbol))
                {   
                    //Replace the symbol in the temporaryProduction string.
                    temporaryProduction += rules[currentSymbol];
                } else {
                    temporaryProduction += currentSymbol;
                }

            }
            //The new production string is the temporary one.
            production = temporaryProduction;
        }

        ParseProduction(production);
    }

    void ParseProduction(string productionString){

        if (EmptyParent != null)
        {
            DestroyImmediate(EmptyParent);
        }

        EmptyParent = new GameObject("L-System");


        Stack<Vector3> savedPosition = new Stack<Vector3>();
        Stack<Quaternion> savedRotation = new Stack<Quaternion>();

        for (int i = 0; i < productionString.Length; i++)
        {
            switch (productionString[i])
            {   
                case '[':
                    savedPosition.Push(transform.position);
                    savedRotation.Push(transform.rotation);
                    break;

                case ']':
                    transform.SetPositionAndRotation(savedPosition.Pop(), savedRotation.Pop());
                    break;
                case '+':
                    // Turn left by angle
                    transform.Rotate(new Vector3(0, -angle, 0), Space.Self);
                    break;
                case '-':
                    // Turn right by angle
                    transform.Rotate(new Vector3(0, angle, 0), Space.Self);
                    break;
                case '^':
                    // Turn up by angle
                    transform.Rotate(new Vector3(0, 0, angle), Space.Self);
                    break;
                case '&':
                    // Turn down by angle
                    transform.Rotate(new Vector3(0, 0, -angle), Space.Self);     
                    break;
                case '\\':
                    // Roll left by angle
                    transform.Rotate(new Vector3(-angle, 0, 0), Space.Self);
                    break;
                case '/':
                    // Roll right by angle
                    transform.Rotate(new Vector3(angle, 0, 0), Space.Self);
                    break;
                case '|':
                    // Roll right by angle
                    transform.Rotate(new Vector3(0, 180, 0), Space.Self);
                    break;
                case'I':
                    string temp = "";

                    i++;

                    while(productionString[i] != ')'){
                        temp += productionString[i];
                        i++;
                    }

                    break;
                case 'F':
                    Instantiate(RoadParts[0], transform.position, transform.localRotation, EmptyParent.gameObject.transform);

                    Quaternion currentPosition = transform.localRotation;
                    Quaternion stepQuaternion = new Quaternion(1, 0, 0, 0);
                    Quaternion inversePosition = Quaternion.Inverse(currentPosition);

                    Quaternion translationBy = currentPosition * stepQuaternion * inversePosition;

                    Vector3 translationVector = new Vector3(translationBy.x, translationBy.y, translationBy.z);

                    Debug.Log(translationVector.ToString());
                    transform.position = new Vector3(transform.position.x + translationVector.x, transform.position.y + translationVector.y, transform.position.z + translationVector.z);
                    break;
                default:
                    break;
            }
        }
    }
}

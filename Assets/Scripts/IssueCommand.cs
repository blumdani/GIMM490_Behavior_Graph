using Unity.Behavior;
using UnityEngine;
using System.Collections;
using TMPro;

public class IssueCommand : MonoBehaviour
{
    public BehaviorGraphAgent behaviorAgent;
    public TextMeshProUGUI commandText;   // assign in inspector

    void SetCommand(DirectCommands command, string message)
    {
        Debug.Log(message);

        behaviorAgent.SetVariableValue("DirectCommand", command);

        // Update Direct Text UI element
        commandText.text = command.ToString();

        //Interrupt previous version.
        StartCoroutine(RestartNextFrame());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SetCommand(DirectCommands.CollectMedicine, "Collecting Medicine!");
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            SetCommand(DirectCommands.HealMe, "Healing Player!");
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            SetCommand(DirectCommands.HealYourself, "Healing Companion");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SetCommand(DirectCommands.Attack, "Attacking Enemy!");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            SetCommand(DirectCommands.CollectGrass, "Collecting Grass!");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            SetCommand(DirectCommands.CollectRocks, "Collecting Rocks!");
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SetCommand(DirectCommands.CollectWood, "Collecting Wood!");
        }
        else if(Input.GetKeyDown(KeyCode.L))
        {
            SetCommand(DirectCommands.ActonYourOwn, "Act Independently");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            SetCommand(DirectCommands.Wait, "Waiting");
        }
    }

    IEnumerator RestartNextFrame()
    {
        yield return null; // wait one frame
        behaviorAgent.Restart();
    }
}


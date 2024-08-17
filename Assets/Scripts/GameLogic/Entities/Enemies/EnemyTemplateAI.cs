using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplateAI : MonoBehaviour
{
    SimpleStateMachine simpleStateMachine;
    SimpleStateMachine.State idle = new SimpleStateMachine.State(), move = new SimpleStateMachine.State();

    private void Start()
    {
        simpleStateMachine = GetComponent<SimpleStateMachine>();

        idle.onStateEnter = data =>
            {
                print("Entering idle");

                data.ExecuteOverDuration(2, data.destroyCancellationToken, time =>
                {
                    if (time == 1)
                        simpleStateMachine.SetState(move);

                }).Forget();
            };

        move.onStateEnter = (data) => { print("Entering move state"); };
        move.onStateExit = (data) => { print("Exiting move state"); };
        move.onStateStay = OnMoveState;

        simpleStateMachine.SetState(idle);
    }

    void OnMoveState(SimpleStateMachine state)
    {
        transform.Translate(Vector2.right * Time.deltaTime);
        if (Random.Range(0, 1000) >= 900)
            state.SetState(idle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    //A class with a static instance like GameManager to handle the GameEvents that are created and the conditions for each
    public List<GameEvent> GameEvents;

    public static GameEventManager Instance;

    public GameObject LocationEventText;
    public GameObject InteractionEventText;
    public GameObject EventTextGroup;
    private void Awake()
    {
        Instance = this;
    }
    public void GenerateEvents()
    {
        foreach(GameEvent gameEvent in GameEvents)
        {
            GenerateEventUI(gameEvent);
        }
    }
    private void GenerateEventUI(GameEvent gEvent)
    {
        if (gEvent.Conditions != null && gEvent.Conditions.Count != 0) {
            foreach (EventCondition eCondition in gEvent.Conditions)
            { 
                if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Location)
                {
                    TaskListUIElement_Location locationUI = Instantiate(LocationEventText, EventTextGroup.transform).GetComponent<TaskListUIElement_Location>();
                    locationUI.EventUIText.text = gEvent.description;
                    (eCondition as LocationCondition).ConditionUI = locationUI;
                    (eCondition as LocationCondition).UpdateLocationUI((LocationCondition)eCondition);
                }
                if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Interaction)
                {
                    TaskListUIElement_Interaction interactionUI = Instantiate(InteractionEventText, EventTextGroup.transform).GetComponent<TaskListUIElement_Interaction>();
                    interactionUI.EventUIText.text = gEvent.description;
                    (eCondition as InteractionCondition).ConditionUI = interactionUI;
                    (eCondition as InteractionCondition).UpdateInteractionUI((InteractionCondition)eCondition);
                }
            }
        }
    }
    public void UpdateEvents()
    {
        foreach (GameEvent gameEvent in GameEvents)
        {
            if (gameEvent.Conditions != null && gameEvent.Conditions.Count != 0)
            {
                UpdateEventUI(gameEvent);
            }
        }
    }
    private void UpdateEventUI(GameEvent gEvent)
    {
        foreach (EventCondition eCondition in gEvent.Conditions)
        {
            if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Location)
            {
                (eCondition as LocationCondition).UpdateLocationUI((LocationCondition)eCondition);
            }
            if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Interaction)
            {
                (eCondition as InteractionCondition).UpdateInteractionUI((InteractionCondition)eCondition);
            }
        }
    }

    public void ResetEvents()
    {
        //If no events active just back out
        if (GameEvents == null)
            return;
        else
        {
            //otherwise call the EventReset
            for (int i = 0; i < GameEvents.Count; i++)
            {
                GameEvents[i].EventReset();
            }
        }
    }
    public static bool CheckEventCompletion(GameEvent gEvent)
    {
        //Check if an Event's conditions are completed
        gEvent.UpdateConditionsCompletion(gEvent.Conditions);
        return gEvent.ReturnEventCompletion(gEvent.Conditions);
    }
    public bool EventListComplete()
    {
        List<GameEvent> allEvents = Instance.GameEvents;

        if (allEvents != null && allEvents[0] != null)
        {
            for (int i = 0; i < allEvents.Count; i++)
            {
                if (!CheckEventCompletion(allEvents[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

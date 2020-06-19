using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class UITransition : MonoBehaviour
{
    public enum State
    {
        Enable,
        Disable,
        ImmediateEnable,
        ImmediateDisable
    }

    public enum ElementType
    {
        Image,
        Text
    }

    public enum MoveType
    {
        Instant,
        Linear,
        Exponential
    }

    [System.Serializable]
    public class UIElement
    {
        [HideInInspector] public string name;
        public GameObject gameObject;

        [Space(10)]
        public ElementType elementType;

        //Image Variables
        [ConditionalField(nameof(elementType), false, ElementType.Image)] public Image image;

        //Text Variables
        [ConditionalField(nameof(elementType), false, ElementType.Text)] public Text text;

        //Color Change
        [Space(10)]
        public bool allowColorChange;
        [ConditionalField(nameof(allowColorChange))] public Color openColor;
        [ConditionalField(nameof(allowColorChange))] public Color closedColor;
        
        //Position Change
        [Space(10)]
        public bool allowPositionChange;
        [ConditionalField(nameof(allowPositionChange))] public Vector2 closeShift;
        [HideInInspector] public Vector2 openPosition;
        [HideInInspector] public Vector2 closedPosition;
        


        //Rotation Change
        [Space(10)]
        public bool allowRotationChange;

        //Scale Change
        [Space(10)]
        public bool allowScaleChange;
    }
    public bool open;
    public float percentageCurrent;
    public float percentageTarget;
    public State currentState;

    public MoveType transitionType;
    [ConditionalField(nameof(transitionType), false, MoveType.Linear)] public float transitionTime;
    [ConditionalField(nameof(transitionType), false, MoveType.Exponential)] public float transitionScale;
    public List<UIElement> elements;

    private Vector3 tempV3;
    private Vector3 tempV3a;
    private Vector3 tempV3b;

    private Color tempC;
    private Color tempCa;
    private Color tempCb;

    private Button button;

    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private void GetElements()
    {
        GetVariables();
    }
    #endif

    #if UNITY_EDITOR // conditional compilation is not mandatory
    [ButtonMethod]
    private void DisableImmediately()
    {
        SetState(State.ImmediateDisable);
    }
    #endif

    private void GetVariables()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].gameObject != null)
            {
                elements[i].name = elements[i].gameObject.name;
                switch(elements[i].elementType)
                {
                    default: break;
                    case ElementType.Image:
                        elements[i].image = elements[i].gameObject.GetComponent<Image>();
                    break;
                    case ElementType.Text:
                        elements[i].text = elements[i].gameObject.GetComponent<Text>();
                    break;
                }
                if (elements[i].allowPositionChange)
                {
                    elements[i].openPosition = elements[i].gameObject.transform.localPosition;
                    elements[i].closedPosition = elements[i].openPosition + elements[i].closeShift;
                }
            }
            
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        GetVariables();
        SetState(State.ImmediateDisable);
    }

    public void SetState(State uistate)
    {
        switch(uistate)
        {
            case State.Enable:
                percentageTarget = 100f;
            break;

            case State.Disable:
                percentageTarget = 0f;
            break;

            case State.ImmediateEnable:
                percentageTarget = 100f;
                percentageCurrent = 99f;
            break;

            case State.ImmediateDisable:
                percentageTarget = 0f;
                percentageCurrent = 1f;
            break;
        }
        currentState = uistate;
    }

    // Update is called once per frame
    void Update()
    {
        if (percentageCurrent != percentageTarget)
        {
            percentageCurrent = SetNewPercentage(percentageCurrent, percentageTarget, Time.deltaTime);
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].allowColorChange)
                {
                    tempCa = elements[i].closedColor;
                    tempCb = elements[i].openColor;
                    tempC = Color.Lerp(tempCa, tempCb, percentageCurrent/100);
                    switch (elements[i].elementType)
                    {
                        case ElementType.Image: elements[i].image.color = tempC; break;
                        case ElementType.Text: elements[i].text.color = tempC; break;
                    }
                }

                if (elements[i].allowPositionChange)
                {
                    tempV3a = elements[i].closedPosition;
                    tempV3b = elements[i].openPosition;
                    tempV3 = Vector3.Lerp(tempV3a, tempV3b, percentageCurrent/100);
                    elements[i].gameObject.transform.localPosition = tempV3;
                }
            }
        }
        if (button != null)
        {
            if (currentState == State.Disable || currentState == State.ImmediateDisable)
            {
                button.interactable = false;
            }
            if (currentState == State.Enable || currentState == State.ImmediateEnable)
            {
                button.interactable = true;
            }
        }
        
    }

    public float SetNewPercentage(float current, float target, float delta)
    {
        float diff = target - current;
        switch (transitionType)
        {
            case MoveType.Instant:
                return target;
            case MoveType.Linear:
                float change = (current > target) ? -1f : 1f;
                float move = (100 / transitionTime) * delta * change;
                if (Mathf.Abs(move) > Mathf.Abs(diff)) return target;
                else return (current + move);
            case MoveType.Exponential:
                float move2 = diff * delta * transitionScale;
                if (Mathf.Abs(move2) < 0.1) return target;
                else if (Mathf.Abs(move2) > Mathf.Abs(diff)) return target;
                else return (current + move2);
        }
        return 0;
    }
}

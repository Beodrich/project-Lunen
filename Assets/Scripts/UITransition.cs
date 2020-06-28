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
        ImmediateDisable,
        HalfDisable
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
        Exponential,
        SineWave
    }

    [System.Serializable]
    public class UIElement
    {
        [HideInInspector] public string name;

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

        [HideInInspector] public GameObject source;
    }
    public bool open;
    public float percentageCurrent;
    public float percentageTarget;
    public float percentageUse;
    [ReadOnly] public State currentState;
    public MoveType transitionType;
    [ConditionalField(nameof(transitionType), false, MoveType.Linear, MoveType.SineWave)] public float transitionTime;
    [ConditionalField(nameof(transitionType), false, MoveType.Exponential)] public float transitionScale;

    [Space(10)]

    public float openDelay;
    public float closeDelay;
    [ReadOnly] public float percentageDelay;

    [Space(10)]
    
    
    public List<UIElement> elements;

    private Vector3 tempV3;
    private Vector3 tempV3a;
    private Vector3 tempV3b;

    private Color tempC;
    private Color tempCa;
    private Color tempCb;

    private Button button;
    public bool updateButton;

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
            if (elements[i].elementType == ElementType.Image) elements[i].source = elements[i].image.gameObject; 
            else if (elements[i].elementType == ElementType.Text) elements[i].source = elements[i].text.gameObject;
            if (elements[i].source != null)
            {
                elements[i].name = elements[i].source.name;
                if (elements[i].allowPositionChange)
                {
                    elements[i].openPosition = elements[i].source.transform.localPosition;
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
                percentageDelay = openDelay;
                if (button != null && updateButton) button.interactable = true;
                foreach (UIElement element in elements)
                {
                    switch (element.elementType)
                    {
                        case ElementType.Image:
                            element.image.enabled = true;
                        break;
                        case ElementType.Text:
                            element.text.enabled = true;
                        break;
                    }
                }
            break;

            case State.Disable:
                percentageTarget = 0f;
                if (percentageCurrent != 1f) percentageDelay = closeDelay;
            break;

            case State.ImmediateEnable:
                percentageTarget = 100f;
                percentageCurrent = 99f;
                percentageDelay = 0f;
                if (button != null && updateButton) button.interactable = true;
                foreach (UIElement element in elements)
                {
                    switch (element.elementType)
                    {
                        case ElementType.Image:
                            element.image.enabled = true;
                        break;
                        case ElementType.Text:
                            element.text.enabled = true;
                        break;
                    }
                }
            break;

            case State.ImmediateDisable:
                percentageTarget = 0f;
                percentageCurrent = 1f;
                percentageDelay = 0f;
            break;

            case State.HalfDisable:
                percentageTarget = 0f;
                percentageCurrent = 0.7f;
                percentageDelay = 0f;
            break;
        }
        currentState = uistate;
    }

    // Update is called once per frame
    void Update()
    {
        if (percentageCurrent != percentageTarget)
        {
            if (percentageDelay <= 0)
            {
                percentageCurrent = SetNewPercentage(percentageCurrent, percentageTarget, Time.deltaTime);
                percentageUse = percentageCurrent;
                if (transitionType == MoveType.SineWave)
                {
                    percentageUse = Mathf.Sin((percentageCurrent/200) * Mathf.PI) * 100;
                }
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].allowColorChange)
                    {
                        tempCa = elements[i].closedColor;
                        tempCb = elements[i].openColor;
                        tempC = Color.Lerp(tempCa, tempCb, percentageUse/100);
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
                        tempV3 = Vector3.Lerp(tempV3a, tempV3b, percentageUse/100);
                        elements[i].source.transform.localPosition = tempV3;
                    }
                }
                
                //This function is called if the percentage just hit zero.
                if (percentageCurrent == 0)
                {
                    if (button != null && updateButton) button.interactable = false;
                    foreach (UIElement element in elements)
                    {
                        switch (element.elementType)
                        {
                            case ElementType.Image:
                                element.image.enabled = false;
                            break;
                            case ElementType.Text:
                                element.text.enabled = false;
                            break;
                        }
                    }
                }

                if (percentageCurrent == 100)
                {
                    if (button != null)
                    {
                        if (!button.interactable)
                        {
                            button.interactable = true;
                            button.interactable = false;
                        }
                        else
                        {
                            button.interactable = false;
                            button.interactable = true;
                        }
                    } 
                    
                }
            }
            else
            {
                percentageDelay -= Time.deltaTime;
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
            case MoveType.SineWave:
                float change = (current > target) ? -1f : 1f;
                float move = (100 / transitionTime) * delta * change;
                if (Mathf.Abs(move) > Mathf.Abs(diff)) return target;
                else return (current + move);
            case MoveType.Exponential:
                float move2 = diff * delta * transitionScale;
                if (Mathf.Abs(move2) < 0.001) return target;
                else if (Mathf.Abs(move2) > Mathf.Abs(diff)) return target;
                else return (current + move2);
        }
        return 0;
    }
}

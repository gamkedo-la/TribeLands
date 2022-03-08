using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float fillRate = 1f;
    private float fillSpeed;

    public Image image;
    public AnimationCurve fillCurve;
    public float currentPosition = 0;
    private bool active = false;
    
    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        var sign = active ? 1 : -1;
        var delta = (Time.deltaTime / fillRate) * sign;
        currentPosition = Mathf.Clamp01(currentPosition + delta);
        image.fillAmount = fillCurve.Evaluate(currentPosition);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        active = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        active = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        active = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        active = false;
    }
}

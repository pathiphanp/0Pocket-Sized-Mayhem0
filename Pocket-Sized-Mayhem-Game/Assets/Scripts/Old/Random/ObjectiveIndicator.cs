
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private RectTransform _pointerRectTransform;

    [SerializeField] Image _target;

    private Vector3 _targetPosition;
    private void Start()
    {
        _targetPosition = _targetObject.transform.position;
    }
    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.y = _targetPosition.y;
        
        float borderSize = 100f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(_targetPosition);
        _target.transform.position = targetPositionScreenPoint;

        Vector3 dir = (_target.rectTransform.transform.position - _pointerRectTransform.transform.position).normalized;

        Vector2 targetUIPos = _target.rectTransform.transform.position;
        Vector2 pointerUIpos = _pointerRectTransform.transform.position;
        Vector2 targ = new Vector2(_targetPosition.x, _targetPosition.y);

        targ.x = targetUIPos.x - pointerUIpos.x;
        targ.y = targetUIPos.y - pointerUIpos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;

        _pointerRectTransform.localEulerAngles = new Vector3(0, 0, -angle -90);

        if (targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize)
        {
            Vector3 cappedTargetPosition = targetPositionScreenPoint;
            if (cappedTargetPosition.x <= borderSize)
            {
                cappedTargetPosition.x = borderSize;
            }
            if (cappedTargetPosition.x >= Screen.width - borderSize)
            {
                cappedTargetPosition.x = Screen.width - borderSize;
            }
            if (cappedTargetPosition.y <= borderSize)
            {
                cappedTargetPosition.y = borderSize;
            }
            if (cappedTargetPosition.y >= Screen.height - borderSize)
            {
                cappedTargetPosition.y = Screen.height - borderSize;
            }
            _pointerRectTransform.position = cappedTargetPosition;
        }
        else
        {
            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(targetPositionScreenPoint);
            _pointerRectTransform.position = pointerWorldPosition;
            _pointerRectTransform.localPosition = new Vector3(_pointerRectTransform.localPosition.x, _pointerRectTransform.localPosition.y,0);
        }
    }
}

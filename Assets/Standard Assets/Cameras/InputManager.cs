using UnityEngine;
using System.Collections;

public static class InputManager
{

    private static float time;
    private static bool doubleClick;
    private static Vector2 currentPosXY;
    private static Vector2 deltaXY;
    private static float _speed = 0.05f;
    public static float GetMouse_X()
    {
        if (IsMouseButton())
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentPosXY.x = Input.mousePosition.x; ;
            }
            else if (Input.GetMouseButton(0))
            {
                float x = Input.mousePosition.x;
                if (currentPosXY.x != x)
                {
                    deltaXY.x = x - currentPosXY.x;
                    currentPosXY.x = x;
                }
                else
                {
                    deltaXY.x = 0;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                deltaXY.x = 0;
            }
            return deltaXY.x * 1f;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).deltaPosition.x * 0.5f * _speed;
            }
            else
            {
                return 0;
            }
        }
    }

    public static float GetMouse_Y()
    {
        if (IsMouseButton())
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentPosXY.y = Input.mousePosition.y;
            }
            else if (Input.GetMouseButton(0))
            {
                float y = Input.mousePosition.y;
                if (currentPosXY.y != y)
                {
                    deltaXY.y = y - currentPosXY.y;
                    currentPosXY.y = y;
                }
                else
                {
                    deltaXY.y = 0;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                deltaXY.y = 0;
            }
            return deltaXY.y * 0.1f;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).deltaPosition.y * 0.5f * _speed;
            }
            else
            {
                return 0;
            }
        }
    }

    public static float GetMouse_ScrollWheel()
    {
        if (IsMouseButton())
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
        else
        {
            if (Input.touchCount == 2)
            {
                Touch off0 = Input.GetTouch(0);
                Touch off1 = Input.GetTouch(1);

                Vector2 offDis1 = off0.position - off1.position;
                Vector2 offDis2 = (off0.position - off0.deltaPosition) - (off1.position - off1.deltaPosition);

                return (offDis1.magnitude * 0.1f - offDis2.magnitude * 0.1f) * _speed;
            }
            else
            {
                return 0;
            }
        }
    }

    private static bool IsMouseButton()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WebGLPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return true;
        }
        return false;
    }

    public static bool GetMouseButton()
    {
        if (IsMouseButton())
        {
            if (Input.GetMouseButton(0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool GetDoubleClick()
    {
        bool isTrue = false;
        if (!IsMouseButton())
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && !doubleClick)
            {
                time = Time.time;
                doubleClick = true;
            }

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && Time.time - time > 0f && Time.time - time < 0.5f && doubleClick)
            {
                isTrue = true;
                doubleClick = false;
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && doubleClick && Time.time - time >= 0.5f)
            {
                doubleClick = false;
                isTrue = false;
            }
            return isTrue;
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !doubleClick)
            {
                time = Time.time;
                doubleClick = true;
            }
            if (Input.GetMouseButtonDown(0) && Time.time - time > 0f && Time.time - time < 0.5f && doubleClick)
            {
                isTrue = true;
                doubleClick = false;
            }
            else if (Input.GetMouseButtonDown(0) && doubleClick && Time.time - time >= 0.5f)
            {
                doubleClick = false;
                isTrue = false;
            }
            return isTrue;
        }
    }

}

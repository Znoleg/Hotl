using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static class InputExtenstions
    {
        public enum DraggedDirection
        {
            Up,
            Down,
            Right,
            Left
        }
        public static DraggedDirection GetDragDirection(Vector3 dragVector)
        {
            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);
            DraggedDirection draggedDir;
            if (positiveX > positiveY)
            {
                draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
            }
            else
            {
                draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
            }
            //Debug.Log(draggedDir);
            return draggedDir;
        }
    }

    public static bool IsFlying(this Rigidbody2D rb)
    {
        return rb.bodyType == RigidbodyType2D.Dynamic && rb.velocity != Vector2.zero;
    }

    public static bool IsJoyActive(Joystick toCheck)
    {
        if (toCheck.Horizontal != 0 || toCheck.Vertical != 0)
            return true;
        return false;
    }
    public static bool IsZero(this Vector2 vector)
    {
        if (vector == Vector2.zero)
            return true;
        return false;
    }
    public static bool IsZero(this Vector3 vector)
    {
        if (vector == Vector3.zero)
            return true;
        return false;
    }
    public static IEnumerator WaitCoroutine(Action action, float seconds = 0.01f)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    public static Texture2D GetTextureFromSprite(this Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

    public static void CheckColliderTrigger(this MonoBehaviour obj)
    {
        if (!obj.GetComponent<Collider2D>().isTrigger) throw new UnityException($"{obj.name} collider must be trigger!");
    }

    public static void CheckColliderTrigger(this Collider2D collider)
    {
        if (!collider.isTrigger) throw new UnityException($"{collider.transform.name} collider must be trigger!");
    }


    public static bool RaycastObject<T>(Vector2 origin, Vector2 direction, float range, out List<T> ts) where T : MonoBehaviour
    {
        ts = new List<T>();
        var hitObjects = Physics2D.RaycastAll(origin, direction, range);
        foreach (var hit in hitObjects)
        {
            T conditionObj = hit.transform.GetComponent<T>();
            if (conditionObj) ts.Add(conditionObj);
        }
        if (ts.Count > 0) return true;
        else return false;
    }

    public static float GetAngleFromVectorFloat(this Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 Rotate(this Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static IEnumerator FindMoveDirection(this Transform transform, Action<Vector2> getDir)
    {
        var firstPos = transform.position;
        yield return null;
        getDir(transform.position - firstPos);
        yield break;
    }

    public static bool CompareJointLimits(JointAngleLimits2D limit1, JointAngleLimits2D limit2, float difference = 0f)
    {
        return Mathf.Abs(limit1.min) - Mathf.Abs(limit2.min) <= difference && Mathf.Abs(limit1.max) - Mathf.Abs(limit2.max) <= difference;
    }

    public static bool AngleCloseToLimit(JointAngleLimits2D limit, float angle, float difference = 0f)
    {
        return Mathf.Abs(Mathf.Abs(limit.min) - Mathf.Abs(angle)) <= difference || 
            Mathf.Abs(Mathf.Abs(limit.max) - Mathf.Abs(angle)) <= difference;
    }

    public static InStunSprites FindConfomity(this List<InStunSprites> inStunSprites, StunStatus stunStatus)
    {
        return inStunSprites.First(elem => elem.StunStatus == stunStatus);
    }

    public static StunKillSprites FindConfomity(this List<StunKillSprites> inStunSprites, StunStatus stunStatus)
    {
        return inStunSprites.First(elem => elem.StunStatus == stunStatus);
    }

    private static Coroutine setMode;

    public static void PushBody(this Rigidbody2D body, float force, Vector2 direction, ForceMode2D forceMode,
        bool throughKinematic = false, MonoBehaviour toLaunchRoutine = null)
    {
        RigidbodyType2D mode = body.bodyType;
        if (throughKinematic)
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            body.AddForce(force * direction, forceMode);
            setMode = toLaunchRoutine.StartCoroutine(SetBodyModeAfterFlying(body, mode, toLaunchRoutine));
        }
        else 
        {
            body.AddForce(force * direction, forceMode);
        }
    }

    private static IEnumerator SetBodyModeAfterFlying(Rigidbody2D body, RigidbodyType2D mode, MonoBehaviour toStop)
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => !body.IsFlying());
        body.bodyType = mode;
        toStop.StopCoroutine(setMode);
        setMode = null;
        yield break;
    }
}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}

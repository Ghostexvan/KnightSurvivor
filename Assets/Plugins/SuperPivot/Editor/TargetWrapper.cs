//#define TAKE_ACCOUNT_POINT_ENTITIES

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SuperPivot
{
    internal class TargetWrapper
    {
        public enum Component { X=0, Y=1, Z=2 }
        public Transform transform { get; private set; }
        public Bounds cachedBounds { get; private set; }
        public string name { get { return transform.name; } }

        Vector3 m_CachedPosition;
        Quaternion m_CachedRotation;
        Vector3 m_CachedScale;

        public bool areBoundsValid { get
            {
                return cachedBounds.min.x < cachedBounds.max.x
                    || cachedBounds.min.y < cachedBounds.max.y
                    || cachedBounds.min.z < cachedBounds.max.z;
            }
        }
        public bool canModifyBoundsSliders { get { return transform.childCount > 0 && areBoundsValid; } }

        public TargetWrapper(Transform t)
        {
            Debug.Assert(t != null);
            transform = t;
            UpdateTargetCachedData();
        }

        public void UpdateTargetCachedData()
        {
            Debug.Assert(transform);
            m_CachedPosition = transform.position;
            m_CachedRotation = transform.rotation;
            m_CachedScale = transform.localScale;

            cachedBounds = ComputeTotalBounds(transform);
        }

        public bool TargetTransformHasChanged()
        {
            Debug.Assert(transform);
            return transform.position != m_CachedPosition
                || transform.rotation != m_CachedRotation
                || transform.localScale != m_CachedScale;
        }

        public void SetPivotPosition(Vector3 pos, API.Space space)
        {
            API.SetPivotPosition(transform, pos, space);
        }

        public void SetPivotPosition(Component comp, float value, API.Space space)
        {
            Debug.Assert(transform, "Invalid target entity");
            var pivotPos = transform.GetPivotPosition(space);
            pivotPos[(int)comp] = value;
            API.SetPivotPosition(transform, pivotPos, space);
        }

        public void SetPivotRotation(Vector3 euler, API.Space space)
        {
            API.SetPivotRotation(transform, euler, space);
        }

        public void SetPivotRotation(Quaternion quat, API.Space space)
        {
            API.SetPivotRotation(transform, quat, space);
        }

        public Vector3 LerpBoundsPosition(Vector3 factor)
        {
            return new Vector3(
                Mathf.LerpUnclamped(cachedBounds.min.x, cachedBounds.max.x, factor.x),
                Mathf.LerpUnclamped(cachedBounds.min.y, cachedBounds.max.y, factor.y),
                Mathf.LerpUnclamped(cachedBounds.min.z, cachedBounds.max.z, factor.z));
        }

        public float LerpBoundsPosition(Component comp, float factor)
        {
            return Mathf.LerpUnclamped(cachedBounds.min[(int)comp], cachedBounds.max[(int)comp], factor);
        }

        Vector3 InverseLerpBoundsPosition(Vector3 worldPos)
        {
            return new Vector3(
                InverseLerpUnclamped(cachedBounds.min.x, cachedBounds.max.x, worldPos.x),
                InverseLerpUnclamped(cachedBounds.min.y, cachedBounds.max.y, worldPos.y),
                InverseLerpUnclamped(cachedBounds.min.z, cachedBounds.max.z, worldPos.z));
        }

        public Vector3 InverseLerpBoundsPosition() { return InverseLerpBoundsPosition(transform.position); }

        static float InverseLerpUnclamped(float from, float to, float value)
        {
            if (from == to) return 0.5f;
            return (value - from) / (to - from);
        }

        static bool GUIButtonZero()
        {
            var buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedWidth = 40f;
            return GUILayout.Button("Zero", buttonStyle);
        }

        public void GUIPositions()
        {
            if (FoldableHeader.Begin("Position"))
            {
                GUIPosition(API.Space.Global);
                GUIPosition(API.Space.Local);
            }
            FoldableHeader.End();
        }

        void GUIPosition(API.Space space)
        {
            if (space == API.Space.Global || transform.parent != null)
            {
                var str = (space == API.Space.Global) ? "World Position" : string.Format("Local Position (relative to '{0}')", transform.parent.name);
                EditorGUILayout.LabelField(str, EditorStyles.boldLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    var newPos = EditorGUILayout.Vector3Field("", space == API.Space.Global ? transform.position : transform.localPosition);
                    if (EditorGUI.EndChangeCheck())
                        SetPivotPosition(newPos, space);

                    if (GUIButtonZero())
                        SetPivotPosition(Vector3.zero, space);
                }
            }
        }

        public bool CanBeRotated()
        {
            var scale = transform.localScale;
            return Mathf.Approximately(scale.x, scale.y) && Mathf.Approximately(scale.x, scale.z);
        }

        public void GUIRotations()
        {
            if (FoldableHeader.Begin("Rotation"))
            {
                if (CanBeRotated())
                {
                    GUIRotation(API.Space.Global);
                    GUIRotation(API.Space.Local);
                }
                else
                {
                    EditorGUILayout.HelpBox("You cannot change the pivot rotation of this GameObject because its local scale in uneven.\nBut you can wrap it into a unscaled GameObject and then change its pivot rotation.", MessageType.Warning);
                    if (GUILayout.Button("Wrap into unscaled GameObject"))
                    {
                        var wrapper = EditorAPI.WrapTransform(transform);
                        EditorAPI.SetSelectedTransform(wrapper);
                    }
                }
            }
            FoldableHeader.End();
        }

        void GUIRotation(API.Space space)
        {
            if (space == API.Space.Global || transform.parent != null)
            {
                var str = (space == API.Space.Global) ? "World Rotation" : string.Format("Local Rotation (relative to '{0}')", transform.parent.name);
                EditorGUILayout.LabelField(str, EditorStyles.boldLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    var newRot = EditorGUILayout.Vector3Field("", space == API.Space.Global ? transform.rotation.eulerAngles : transform.localRotation.eulerAngles);
                    if (EditorGUI.EndChangeCheck())
                        SetPivotRotation(newRot, space);

                    if (GUIButtonZero())
                        SetPivotRotation(Vector3.zero, space);
                }
            }
        }

        public void GUIChildren()
        {
            var childCount = transform.childCount;
            if (childCount > 0)
            {
                if (FoldableHeader.Begin(string.Format("Children ({0})", childCount), "Children"))
                {
                    var buttonStyle = EditorStyles.miniButton;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Average pivot pos", buttonStyle))
                        {
                            var avgPos = GetChildrenAverageWorldPosition();
                            SetPivotPosition(avgPos, API.Space.Global);
                        }
                    }

                    string TrimName(string name)
                    {
                        const int kMaxStart = 10;
                        const int kMaxEnd = 4;
                        if (name.Length <= kMaxStart + kMaxEnd)
                            return name;
                        else
                            return name.Substring(0, kMaxStart) + "..." + name.Substring(name.Length - kMaxEnd, kMaxEnd);
                    }

                    const int kMaxChildPerLine = 3;
                    int i = 0;
                    while (i < transform.childCount)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            for (int j = 0; j < kMaxChildPerLine && i < transform.childCount; j++, i++)
                            {
                                var child = transform.GetChild(i);
                                if (GUILayout.Button(TrimName(child.name), buttonStyle))
                                    SetPivotPosition(child.transform.position, API.Space.Global);
                            }
                        }
                    }
                }
                FoldableHeader.End();
            }
        }

        Vector3 GetChildrenAverageWorldPosition()
        {
            if (transform.childCount == 0)
            {
                Debug.LogWarningFormat(transform.gameObject, "{0} has no children", transform.name);
                return transform.position;
            }

            var avgPos = Vector3.zero;
            foreach (Transform child in transform)
                avgPos += child.position;
            return avgPos / transform.childCount;
        }

        static Bounds ComputeTotalBounds(Transform parent)
        {
            var bounds = new Bounds();
            bounds.SetMinMax(
                new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                new Vector3(float.MinValue, float.MinValue, float.MinValue));

            var renderersList = new List<Renderer>(parent.GetComponentsInChildren<Renderer>());
            renderersList.AddIfNotNull(parent.GetComponent<Renderer>());

            var count = 0;
            foreach (var child in renderersList)
            {
                bounds.Encapsulate(child.bounds);
                count++;
            }

#if UNITY_5_5_OR_NEWER
            var terrainsList = new List<Terrain>(parent.GetComponentsInChildren<Terrain>());
            terrainsList.AddIfNotNull(parent.GetComponent<Terrain>());
            foreach (var child in terrainsList)
            {
                bounds.Encapsulate(child.transform.TransformPoint(child.terrainData.bounds.min));
                bounds.Encapsulate(child.transform.TransformPoint(child.terrainData.bounds.max));
                count++;
            }
#endif

            if (count == 0)
            {
#if TAKE_ACCOUNT_POINT_ENTITIES
                // if we have no renderer at all as children take account of the point position of each child:
                var children = parent.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    bounds.Encapsulate(child.position);
                }
#else
                bounds.Encapsulate(parent.position); // only take account of current pivot position
#endif
            }

            return bounds;
        }
    }
}

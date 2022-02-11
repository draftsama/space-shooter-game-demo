using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

#if UNITY_EDITOR_64 || UNITY_EDITOR || UNITY_EDITOR_OSX
using UnityEditor;

[CustomEditor(typeof(MovementQueueController))]
public class MovementQueueControllerEditor : Editor
{
    private MovementQueueController _Instance;

    private void OnEnable()
    {
        _Instance = target as MovementQueueController;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();


        if (GUILayout.Button("Add Movement"))
        {
            _Instance.AddMovement(_Instance.transform.position);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_Instance);
        }

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_Instance);
        }
    }
}
#endif

public class MovementQueueController : MonoBehaviour
{
    [SerializeField] private bool m_PlayOnStart;
    [SerializeField] private bool m_Loop;

    [SerializeField] private List<MovementInfo> m_MovementInfos = new List<MovementInfo>();

    private Transform _Transform;

    private Action<MovementResponse> MovementResponseAction;

    public enum Status
    {
        None,
        PreStart,
        Start,
        End
    }

    private void Awake()
    {
        _Transform = transform;

    }

    void Start()
    {

        if (m_PlayOnStart) PlayMovement(m_Loop).Subscribe().AddTo(this);
    }

    public IObservable<MovementResponse> OnMovementUpdateStatus()
    {
        return Observable.FromEvent<MovementResponse>(_e => MovementResponseAction += _e,
            _e => MovementResponseAction -= _e);
    }

    public IObservable<Unit> PlayMovement(bool _loop)
    {
        return Observable.Create<Unit>(_oberser =>
        {
            float timecount = 0;
            int index = 0;

            Vector3 startPos = _Transform.position;
            Vector3 targetPos = _Transform.position;
            var status = Status.None;
            IDisposable disposable = null;
            disposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                if (index >= m_MovementInfos.Count)
                {
                    if (_loop)
                    {
                        index = 0;
                    }
                    else
                    {
                        disposable?.Dispose();
                        return;
                    }
                }

                timecount += Time.deltaTime;
                var movementInfo = m_MovementInfos[index];
                if (status == Status.None)
                {
                    status = Status.PreStart;
                    MovementResponseAction?.Invoke(MovementResponse.GetResponse(index,
                        status));
                }

                if (timecount >= movementInfo.m_DelayTime)
                {
                    if (status == Status.PreStart)
                    {
                        status = Status.Start;
                        MovementResponseAction?.Invoke(MovementResponse.GetResponse(index,
                            status));
                    }

                    var progress = (timecount - movementInfo.m_DelayTime) / movementInfo.m_DurationTime;
                    if (progress < 1)
                    {
                        targetPos = EasingFormula.EaseTypeVector(movementInfo.m_Ease, startPos,
                            movementInfo.m_PositionTarget,
                            progress);

                        _Transform.position = targetPos;
                    }
                    else
                    {
                        if (status == Status.Start)
                        {
                            status = Status.End;
                            MovementResponseAction?.Invoke(MovementResponse.GetResponse(index,
                                status));
                        }
                        status = Status.None;
                        startPos = movementInfo.m_PositionTarget;
                        _Transform.position = movementInfo.m_PositionTarget;
                        timecount = 0;
                        index++;
                    }
                }
            });
            return Disposable.Create(() => { disposable?.Dispose(); });
        });
    }

    public void AddMovement(Vector3 _vector3)
    {
        MovementInfo info;
        info.m_PositionTarget = _vector3;
        info.m_DelayTime = 0.5f;
        info.m_DurationTime = 1f;
        info.m_Ease = Easing.Ease.EaseInOutQuad;
        m_MovementInfos.Add(info);
    }

    [System.Serializable]
    public struct MovementInfo
    {
        [SerializeField] public float m_DelayTime;
        [SerializeField] public float m_DurationTime;
        [SerializeField] public Easing.Ease m_Ease;
        [SerializeField] public Vector3 m_PositionTarget;
    }

    public struct MovementResponse
    {
        [SerializeField] public int m_index;
        [SerializeField] public Status m_Status;

        public static MovementResponse GetResponse(int _index, Status _status)
        {
            MovementResponse respones;
            respones.m_index = _index;
            respones.m_Status = _status;
            return respones;
        }
    }
}
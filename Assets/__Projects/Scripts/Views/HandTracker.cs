using UniRx;
using UnityEngine;

public class HandTracker : MonoBehaviour
{
    public IReadOnlyReactiveProperty<Vector3> HandRotation => handRotation;
    private readonly ReactiveProperty<Vector3> handRotation = new();

    [SerializeField]
    private bool isLeft;

    public bool IsLeft => isLeft;

    public HandTracker()
    {
        Debug.Log($"{nameof(HandTracker)} {(isLeft)} ctor");

        HandRotation.Subscribe(angle =>
            Debug.Log($"{nameof(HandRotation)} {new { isLeft, angle }}"));
    }

    void Start()
    {
        Debug.Log($"{nameof(HandTracker)} {(isLeft)} start");

        this.transform
            .ObserveEveryValueChanged(x => x.rotation.eulerAngles)
            .Subscribe(x => handRotation.Value = x);
    }
}

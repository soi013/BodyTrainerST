using BodyTrainerST.Models;
using UnityEngine;
using Zenject;

namespace BodyTrainerST.Presenter
{
    public class HandTrackerPresenter
    {
        public HandTrackerPresenter([Inject(Id = "LeftHand")] HandTracker leftHandView,
            [Inject(Id = "RightHand")] HandTracker rightHandView,
            AppModel appModel)
        {
            Debug.Log($"{nameof(HandTrackerPresenter)} ctor");
            appModel.SetHandRotations(leftHandView.HandRotation, rightHandView.HandRotation);
        }
    }
}

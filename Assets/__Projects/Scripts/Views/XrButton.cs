using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BodyTrainerST.Views
{
    public class XrButton : MonoBehaviour
    {
        public ReactiveCommand OnNextButtonClick { get; set; }

        [SerializeField]
        private Button nextButton;

        public void Start()
        {
            OnNextButtonClick.BindTo(nextButton);
        }
        public void Click()
        {
            Debug.Log($"{nameof(XrText)} Click");
            OnNextButtonClick?.Execute();
        }
    }
}

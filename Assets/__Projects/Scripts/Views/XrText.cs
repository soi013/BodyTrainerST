using TMPro;
using UnityEngine;

namespace BodyTrainerST.Views
{
    public class XrText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMeshStatus;

        internal void SetText(string t)
        {
            textMeshStatus.text = t;
        }
    }
}

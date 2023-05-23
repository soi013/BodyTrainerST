using BodyTrainerST.Models;
using UniRx;

namespace BodyTrainerST.Presenter
{
    public class XrCameraFadePresenter
    {
        public XrCameraFadePresenter(XrCameraFade fadeView,
            AppModel appModel)
        {
            appModel.IsEnabledFade
                .Subscribe(b =>
                    fadeView.ChangeFade(b));
        }
    }
}

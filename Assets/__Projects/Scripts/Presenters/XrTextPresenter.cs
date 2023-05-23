using BodyTrainerST.Models;
using BodyTrainerST.Views;
using UniRx;

namespace BodyTrainerST.Presenter
{
    public class XrTextPresenter
    {
        public XrTextPresenter(XrText textView,
            AppModel appModel)
        {
            appModel.MainText.Text
                .Subscribe(t =>
                    textView.SetText(t));

            Observable.EveryUpdate()
                .FirstOrDefault()
                .Subscribe(_ => appModel.Initialize());
        }
    }
}

using BodyTrainerST.Models;
using BodyTrainerST.Views;
using UniRx;

namespace BodyTrainerST.Presenter
{
    public class XrButtonPresenter
    {
        public XrButtonPresenter(XrButton buttonView,
            AppModel appModel)
        {
            buttonView.OnNextButtonClick = appModel.State
                .Select(x => x is AppState.Explain or AppState.Result)
                .ToReactiveCommand();

            buttonView.OnNextButtonClick
                .Subscribe(x => appModel.NextMode());
        }
    }
}

using BodyTrainerST.Models;
using UnityEngine;
using Zenject;

namespace BodyTrainerST.Presenter
{
    public class PresenterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log($"{nameof(PresenterInstaller)} ctor");

            //Model�̃o�C���h
            Container.Bind<AppModel>().AsSingle();

            //Presenter�̃o�C���h
            Container.Bind<HandTrackerPresenter>().AsSingle().NonLazy();
            Container.Bind<XrTextPresenter>().AsSingle().NonLazy();
            Container.Bind<XrButtonPresenter>().AsSingle().NonLazy();
            Container.Bind<XrCameraFadePresenter>().AsSingle().NonLazy();
        }
    }
}
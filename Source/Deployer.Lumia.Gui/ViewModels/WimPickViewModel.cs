using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Deployer.UI.ViewModels;
using ReactiveUI;

namespace Deployer.Lumia.Gui.ViewModels
{
    public class WimPickViewModel : ReactiveObject
    {
        private bool applyMrosUI;
        private bool applyWindows12UI;
        private bool allow24H2On905With3GbRam;

        public WimPickViewModel(IOpenFilePicker filePicker)
        {
            WimMetadata = new WimMetadataViewModel();

            var isBusy = WimMetadata.WhenAnyValue(x => x.IsBusy);

            PickWimFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var file = await filePicker.Pick(".wim", "Windows Image Files (.wim)|*.wim");
                if (file != null)
                {
                    await WimMetadata.Load(file);
                }
            }, isBusy.Select(x => !x));

            OpenGetWoaCommand = ReactiveCommand.Create<string>(url => { Process.Start(url); });
        }

        public WimMetadataViewModel WimMetadata { get; }
        public ReactiveCommand<string, Unit> OpenGetWoaCommand { get; }
        public ReactiveCommand<Unit, Unit> PickWimFileCommand { get; }
        public bool HasWim => WimMetadata.Path != null;

        public bool ApplyMrosUI
        {
            get => applyMrosUI;
            set => this.RaiseAndSetIfChanged(ref applyMrosUI, value);
        }

        public bool ApplyWindows12UI
        {
            get => applyWindows12UI;
            set => this.RaiseAndSetIfChanged(ref applyWindows12UI, value);
        }

        public bool Allow24H2On905With3GbRam
        {
            get => allow24H2On905With3GbRam;
            set => this.RaiseAndSetIfChanged(ref allow24H2On905With3GbRam, value);
        }
    }
}

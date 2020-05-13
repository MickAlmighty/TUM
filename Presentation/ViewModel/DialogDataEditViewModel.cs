using Data;
using Logic;
using Presentation.Model;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public abstract class DialogDataEditViewModel<T> : DialogBaseViewModel where T : IUpdatable<T>
    {
        private bool _EditMode;

        protected DialogDataEditViewModel(IDialogHost dialogHost, IDataRepository dataRepository) : base(dialogHost)
        {
            Apply = new RelayCommand(ExecuteApply, CanApply);
            Cancel = new RelayCommand(ExecuteCancel);
            DataRepository = dataRepository;
        }

        private void ExecuteApply()
        {
            if (EditMode)
            {
                ApplyEdit();
            }
            else
            {
                ApplyCreate();
            }
            CloseDialog();
        }

        protected virtual bool CanApply()
        {
            return true;
        }

        protected abstract void ApplyCreate();
        protected abstract void ApplyEdit();

        private void ExecuteCancel()
        {
            CloseDialog();
        }

        public ICommand Apply { get; }
        public ICommand Cancel { get; }
        public IDataRepository DataRepository { get; }
        private bool EditMode
        {
            get
            {
                return _EditMode;
            }
            set
            {
                _EditMode = value;
                RaisePropertyChanged();
            }
        }

        public void OpenDialog()
        {
            EditMode = false;
            ResetProperties();
            Open();
        }

        public void OpenDialog(object dialogIdentifier)
        {
            EditMode = false;
            ResetProperties();
            Open(dialogIdentifier);
        }

        public void OpenDialog(T toUpdate)
        {
            EditMode = true;
            InjectProperties(toUpdate);
            Open();
        }

        public void OpenDialog(T toUpdate, object dialogIdentifier)
        {
            EditMode = true;
            InjectProperties(toUpdate);
            Open(dialogIdentifier);
        }

        protected abstract void InjectProperties(T toUpdate);

        protected abstract void ResetProperties();
    }
}

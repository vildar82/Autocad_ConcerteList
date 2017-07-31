using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using MicroMvvm;
using NetLib;

namespace Autocad_ConcerteList.Errors.UI
{
    public class ErrorModel : ModelBase
    {
	    private bool isExpanded;
	    public readonly ErrorModel parentErr;
        public event EventHandler<bool> SelectionChanged;

        public ErrorModel(IError err, ErrorModel parentErr) : this(err.Yield().ToList())
        {
            this.parentErr = parentErr;
        }

        public ErrorModel(List<IError> sameErrors)
        {
            Count = sameErrors.Count();
            Error = sameErrors.First();
            Message = Error.Message;
            if (Error.Icon != null)
            {
                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    Error.Icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            Show = new RelayCommand(OnShowExecute);
            if (sameErrors.Skip(1).Any())
            {
	            SameErrors = new ObservableCollection<ErrorModel>();
	            foreach (var t in sameErrors)
	            {
		            SameErrors.Add(new ErrorModel(t, this));
	            }
            }
            HasShow = Error.CanShow;                        
        }        

        public IError Error { get; }

	    public ObservableCollection<ErrorModel> SameErrors { get; set; }        
        public string Message { get; set; }
        public BitmapSource Image { get; set; }
        public RelayCommand Show { get; set; }
        public bool IsExpanded {
            get => isExpanded;
	        set {
                if (SameErrors != null)
                {
                    isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool HasShow { get; set; }
        public bool ShowCount => Count != 1;

	    public bool IsSelected { get => isSelected;
		    set { isSelected = value; RaisePropertyChanged(); SelectionChanged?.Invoke(this, value); } }

	    private bool isSelected;

        public int Count { get; set; }

        private void OnShowExecute()
        {            
            Error.Show();
            IsExpanded = !IsExpanded;
            //IsSelected = true;
        }
    }
}

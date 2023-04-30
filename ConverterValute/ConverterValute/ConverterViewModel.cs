using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
namespace ConverterValute
{
    internal class ConverterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ConverterModel _Model = new ConverterModel();
        private DateTime currDate=DateTime.Now;
        public ConverterViewModel()
        {

        }
        public DateTime CurrentDate
        {
            get { return currDate; }
            set { currDate = value; OnPropertyChanged(nameof(CurrentDate)); GetListValutes(); }
        }
        public async void GetListValutes()
        {
            ValuteData data = null;
            try { data = await _Model.GetDateAsync(currDate); }
                
            catch(FlurlHttpException exception)
            {
                if(exception.StatusCode !=404)
                {
                    return;
                }
            }
            if (data == null)
                return;
           foreach(var item in data.Valute.Values)
                valuteList.Add(item);
        }
        private ObservableCollection<Valute> valuteList = new ObservableCollection<Valute>();
        public ObservableCollection<Valute> ValuteList 
        {
            get => valuteList;
            set
            {
                if(valuteList != value)
                {
                    valuteList = value;
                    OnPropertyChanged(nameof(ValuteList));
                }
            }
        }

        private Valute _valueList;
        public Valute ValueList { get=>_valueList; 
            set 
            {
                if(_valueList !=value)
                {
                    _valueList = value;
                    OnPropertyChanged(nameof(ValueList));
                }
            }
        }
       
         
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

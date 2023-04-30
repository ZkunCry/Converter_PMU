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
        private string _textValuteCourse;
        public string TextValuteCourse
        {
            get => _textValuteCourse;
            set 
            {
                if(_textValuteCourse !=value)
                {
                    _textValuteCourse = value;
                    OnPropertyChanged(nameof(TextValuteCourse));
                }
            }
            
        }
        public  ConverterViewModel()
        {
            GetListValutes();
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
                    TextValuteCourse = exception.Message;
                    return;
                }
            }
            if (data == null)
            {
                TextValuteCourse = "Sorry, but no courses found for this date";
                return;
            }
            TextValuteCourse = $"Курс на {data.Date:dd/MM/yyyy}";

            ValuteList.Clear();
            TextFrom = null;
            TextTo = null;

           foreach(var item in data.Valute.Values)
                valuteList.Add(item);
 
        }
        private void Translation()
        {
            if (MainValute == null || SecondValute == null)
                return;
            double.TryParse(_entryMain, out double result);
            ResultTranslation = ((result * MainValute.Value) / SecondValute.Value).ToString();

        }
        private void  CalculateFrom()
        {
            if (MainValute == null || SecondValute == null)
            {
                TextFrom = "";
                return;
            }
            var result = ((MainValute.Nominal * MainValute.Value) / SecondValute.Value).ToString();  
            TextFrom=  $"{MainValute.Nominal} {MainValute.CharCode} = {result} {SecondValute.CharCode} ";

            CalculateTo();
        }
        private void CalculateTo()
        {
            var result = ((SecondValute.Nominal * SecondValute.Value) / MainValute.Value).ToString();
            TextTo = $"{SecondValute.Nominal} {SecondValute.CharCode} = {result} {MainValute.CharCode} ";
        }
        private string _textfrom;
        public string TextFrom { get=>_textfrom; 
            set 
            { 
                if(_textfrom !=value)
                {
                    _textfrom = value;
                    OnPropertyChanged(nameof(TextFrom));
                }
            }
        }
        private string _textTo;
        public string TextTo { get=>_textTo; 
            set 
            { 
                if(_textTo !=value)
                {
                    _textTo = value;
                    OnPropertyChanged(nameof(TextTo));
                }
            } 
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

        private Valute _mainvalute;
        public Valute MainValute { get=> _mainvalute; 
            set 
            {
                if(_mainvalute != value)
                {
                    _mainvalute = value;
                 
                    OnPropertyChanged(nameof(MainValute));
                    Translation();
                    CalculateFrom();
                
                }
            }
        }
        private Valute _secondvalute;
        public Valute SecondValute { get=>_secondvalute; set 
            { 
                if(_secondvalute !=value)
                {
                    _secondvalute = value;
                    OnPropertyChanged(nameof(SecondValute));
                    Translation();
                    CalculateFrom();
                    
                }
            } 
        }
        private string _entryMain;
        public string EntryMain { get=>_entryMain; 
            set 
            { 
                if(_entryMain !=value)
                {
                    _entryMain = value;
                    Translation();
                    OnPropertyChanged(nameof(EntryMain));
                    
                }
            } 
        }
        private string _resultTranslation;
        public string ResultTranslation
        {
            get => _resultTranslation;
            set
            {
                if (_resultTranslation != value)
                {
                    _resultTranslation = value;
                    OnPropertyChanged(nameof(ResultTranslation));
                }
            }
        }


        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

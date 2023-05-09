using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
namespace ConverterValute
{
    enum TypeCalc
    {
        FROM,
        TO,
        MAIN
    }
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

        private ObservableCollection<Valute> valuteList = new ObservableCollection<Valute>();
        public ObservableCollection<Valute> ValuteList
        {
            get => valuteList;
            set
            {
                if (valuteList != value)
                {
                    valuteList = value;
                    OnPropertyChanged(nameof(ValuteList));
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
        private Dictionary<DateTime, ConverterViewModel> _Buffer = 
            new Dictionary<DateTime, ConverterViewModel>();
        public async void GetListValutes()
        {
            if(_Buffer.Count > 0)
            {
                if(_Buffer.ContainsKey(CurrentDate))
                {
                    var temp = _Buffer[currDate];
                    ValuteList = temp.ValuteList;
                    if (!string.IsNullOrWhiteSpace(temp.MainValute.CharCode))
                        MainValute = ValuteList.FirstOrDefault(value => value.CharCode == temp.MainValute.CharCode);
                    if (!string.IsNullOrWhiteSpace(temp.SecondValute.CharCode))
                        SecondValute = ValuteList.FirstOrDefault(value => value.CharCode == temp.SecondValute.CharCode);
                    EntryMain = temp.EntryMain;
                    
                    ResultTranslation = temp.ResultTranslation;
                    TextFrom = temp.TextFrom;
                    TextTo = temp.TextTo;
 
                    return;
                }
               
            }
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

            TextValuteCourse = $"Курс на {data.Date:dd.MM.yyyy}";

            var mainVal =  MainValute?.CharCode;
            var secondval = SecondValute?.CharCode;
            ValuteList.Clear();
            foreach(var item in data.Valute.Values)
                valuteList.Add(item);

            if(!string.IsNullOrWhiteSpace(mainVal))
                MainValute = ValuteList.FirstOrDefault(value => value.CharCode == mainVal);
            if (!string.IsNullOrWhiteSpace(secondval))
                SecondValute = ValuteList.FirstOrDefault(value => value.CharCode == secondval);
            Translation(TypeCalc.MAIN);
            SaveData();

        }
        private void SaveData()
        {
             var temp = (ConverterViewModel)MemberwiseClone();
            if (_Buffer.ContainsKey(currDate))
                _Buffer[currDate] = temp;
            else
                _Buffer.Add(CurrentDate,temp);

        }
        private void Translation( TypeCalc type)
        {
            if (MainValute == null || SecondValute == null)
                return;

            double result;
            string trans;

            switch(type)
            {
                case TypeCalc.FROM:
                    trans = ((MainValute.Nominal * MainValute.Value) / SecondValute.Value).ToString();
                    TextFrom = $"{MainValute.Nominal} {MainValute.CharCode} = {trans} {SecondValute.CharCode} ";
                    Translation(TypeCalc.TO);
                    break;
                case TypeCalc.MAIN:
                    double.TryParse(_entryMain, out result);
                    ResultTranslation = (result * MainValute.Value / SecondValute.Value).ToString();
                    /*double.TryParse(ResultTranslation, out double result2);
                    EntryMain = (result2 * SecondValute.Value / MainValute.Value).ToString();*/
                    break;
                case TypeCalc.TO:
                    trans = ((SecondValute.Nominal * SecondValute.Value) / MainValute.Value).ToString();
                    TextTo = $"{SecondValute.Nominal} {SecondValute.CharCode} = {trans} {MainValute.CharCode} ";
                    
                    break;
            }
            SaveData();
            
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
        

        private Valute _mainvalute;
        public Valute MainValute { get=> _mainvalute; 
            set 
            {
                if(_mainvalute != value)
                {
                    _mainvalute = value;
                 
                    OnPropertyChanged(nameof(MainValute));
                    Translation(TypeCalc.MAIN);
                    Translation(TypeCalc.FROM);
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
                    Translation(TypeCalc.MAIN);
                    Translation(TypeCalc.FROM);
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
                    Translation(TypeCalc.MAIN);
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
                   /* Translation(TypeCalc.MAIN);*/
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

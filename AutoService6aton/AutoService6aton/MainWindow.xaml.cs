using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoService6aton.windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AutoService6aton
{
    public partial class Service
    {
        public Uri ImageUri
        {
            get
            {
                return new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, MainImagePath));
            }
        }
    
    public string CostString
        {
            get
            {
                // тут должно быть понятно - преобразование в строку с нужной точностью
                return Cost.ToString("#.##");
            }
        }

        public string CostWithDiscount
        {
            get
            {
                // Convert.ToDecimal - преобразует double в decimal
                // Discount ?? 0 - разнуливает "Nullable" переменную
                return (Cost * Convert.ToDecimal(1 - Discount ?? 0)).ToString("#.##");
            }
        }

        // ну и сразу пишем геттер на наличие скидки
        public Boolean HasDiscount
        {
            get
            {
                return Discount > 0;
            }
        }

        // и перечёркивание старой цены
        public string CostTextDecoration
        {
            get
            {
                return HasDiscount ? "None" : "Strikethrough";
            }
        }
}


        public partial class MainWindow : Window, INotifyPropertyChanged
    {
            public MainWindow()
            {
                InitializeComponent();
                this.DataContext = this;     
                ServiceList = Core.DB.Service.ToList();

            }
            private void ExitButton_Click(object sender, RoutedEventArgs e)
            {
                Application.Current.Shutdown();
            }
            private List<Service> _ServiceList;
            public List<Service> ServiceList
            {
            get
            {
                if (SortPriceAscending)
                    return _ServiceList
                        .OrderBy(item => Double.Parse(item.CostWithDiscount))
                        .ToList();
                else
                    return _ServiceList
                        .OrderByDescending(item => Double.Parse(item.CostWithDiscount))
                        .ToList();
            }
            set { _ServiceList = value; }
        }

            private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }
        private Boolean _IsAdminMode = false;

        public event PropertyChangedEventHandler PropertyChanged;

        // публичный геттер, который меняет текущий режим (Админ/не Админ)
        public Boolean IsAdminMode
        {
            get { return _IsAdminMode; }
            set
            {
                _IsAdminMode = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AdminModeCaption"));
                    PropertyChanged(this, new PropertyChangedEventArgs("AdminVisibility"));
                }
            }
        }
        // этот геттер возвращает текст для кнопки в зависимости от текущего режима
        public string AdminModeCaption
        {
            get
            {
                if (IsAdminMode) return "Выйти из режима\nАдминистратора";
                return "Войти в режим\nАдминистратора";
            }

        }
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            // если мы уже в режиме Администратора, то выходим из него 
            if (IsAdminMode) IsAdminMode = false;
            else
            {
                // создаем окно для ввода пароля
                var InputBox = new InputBoxWindow("Введите пароль Администратора");
                // и показываем его как диалог (модально)
                if ((bool)InputBox.ShowDialog())
                {
                    // если нажали кнопку "Ok", то включаем режим, если пароль введен верно
                    IsAdminMode = InputBox.InputText == "0000";
                }
            }
        }
        public string AdminVisibility
        {
            get
            {
                if (IsAdminMode) return "Visible";
                return "Collapsed";
            }
        }
        private Boolean _SortPriceAscending = true;
        public Boolean SortPriceAscending
        {
            get { return _SortPriceAscending; }
            set
            {
                _SortPriceAscending = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ServiceList"));
                }
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SortPriceAscending = (sender as RadioButton).Tag.ToString() == "1";
        }
    }
    }


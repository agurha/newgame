using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightCommon;
using NoughtsAndCrossesMultiPlayer.ViewModel;

namespace NoughtsAndCrossesMultiPlayer.Model
{
    public class TicTacSquareViewModel : ViewModelBase
    {
        private Mark? _value;
        public Mark? Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }
        public int GridPosition { get; set; }
        public DelegateCommand PlaceMarkCommand { get; private set; }
        private MainScreenViewModel Controller;

        public TicTacSquareViewModel(MainScreenViewModel controller)
        {
            PlaceMarkCommand = new DelegateCommand(PlaceMark, t => true);
            Controller = controller;
        }

        private void PlaceMark(object sender)
        {
            Controller.PlaceMark(sender);
        }
    }

}

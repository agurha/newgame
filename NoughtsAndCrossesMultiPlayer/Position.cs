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

namespace NoughtsAndCrossesMultiPlayer
{
    public class Position
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        public override string ToString()
        {
            return string.Format("Xpos: {0}, YPos:{1}", XPos, YPos);
        }
    }
}

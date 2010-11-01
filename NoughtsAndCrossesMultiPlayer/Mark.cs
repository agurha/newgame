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
    public enum Mark
    {
        Nought,
        Cross
    }

    public static class Extensions
    {
        public static Mark? GetCurrentMark(this int playerid)
        {
            if (playerid == 0)
                return Mark.Cross;
            return Mark.Nought;
        }
    }
}

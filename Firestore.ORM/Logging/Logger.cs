using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging
{
    [Flags]
    public enum Channels
    {
        Info = 1,
        Warning = 2,
        Critical = 4,
        Log = 8,
    }
    public class Logger
    {
        public static readonly Channels DefaultChannels = Channels.Info | Channels.Warning | Channels.Critical | Channels.Log;

        private static Channels Channels = DefaultChannels;

        private static ConsoleColor Color1 = ConsoleColor.Cyan;
        private static ConsoleColor Color2 = ConsoleColor.DarkCyan;

        private static Dictionary<Channels, ConsoleColor> ChannelsColors = new Dictionary<Channels, ConsoleColor>()
        {
            { Channels.Info,     ConsoleColor.Gray },
            { Channels.Log,     ConsoleColor.DarkGray },
            { Channels.Warning,  ConsoleColor.Yellow },
            { Channels.Critical, ConsoleColor.Red },
        };

        public static void SetChannels(Channels channels)
        {
            Channels = channels;
        }
        public static void EnableChannel(Channels channels)
        {
            Channels |= channels;
        }
        public static void Enable()
        {
            Channels = DefaultChannels;
        }
        public static void Disable()
        {
            Channels = 0x00;
        }
        public static void DisableChannel(Channels channels)
        {
            Channels &= ~channels;
        }

        public static void Write(object value, Channels state = Channels.Log)
        {
            if (Channels.HasFlag(state))
            {
                WriteColored(value, ChannelsColors[state]);
            }
        }
        public static void WriteColor1(object value)
        {
            WriteColored(value, Color1, false);
        }
        public static void WriteColor2(object value)
        {
            WriteColored(value, Color2, false);
        }
        private static void WriteColored(object value, ConsoleColor color, bool timestamp = true)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            if (timestamp)
                Console.WriteLine(DateTime.Now.ToString("dd/MM HH'h'mm") + " - " + value);
            else
                Console.WriteLine(value);

            Console.ForegroundColor = oldColor;
        }
        public static void NewLine()
        {
            if (Channels != 0x00)
            {
                Console.WriteLine(Environment.NewLine);
            }
        }



    }
}

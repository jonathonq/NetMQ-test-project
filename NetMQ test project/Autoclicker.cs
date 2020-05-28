using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Runtime.CompilerServices;

namespace NetMQ_test_project
{
    public class AutoClicker
    {
        //invoking DLLs to have access to GetCursorPos, SetCursorPos, mouse_event 
        //from windows API
        //naming conventions are not standard for C#
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        // Anything below is normal managed code

        private static POINT _buyPos;
        private static POINT _sellPos;

        private static POINT SetPos()
        {
            Console.WriteLine("Move cursor to desired position and press the S key to set. (Don't minimize this window)");
            POINT nullPoint = new POINT();
            if(Console.ReadKey(true).Key == ConsoleKey.S)
            {
                GetCursorPos(out POINT point);
                return point;
            }
            else
            {
                return nullPoint;
            }

        }

        public static void SetBuyPos()
        {
            Console.WriteLine("Setting Buy Position...");
            _buyPos = SetPos();
            Console.WriteLine("Buy Pos: {0},{1}", _buyPos.X, _buyPos.Y);

        }

        public static void SetSellPos()
        {
            Console.WriteLine("Setting Sell Position...");
            _sellPos = SetPos();
            Console.WriteLine("Sell Pos: {0},{1}",_sellPos.X,_sellPos.Y);
        }

        public static void ShowPositions()
        {
            // show graphic on screen where positions are set to? 
            //not sure if possible from console application


        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClickBuy()
        {
            SetCursorPos(_buyPos.X, _buyPos.Y);
            Click();
            //Console.WriteLine("Buy Pos: {0},{1}", _buyPos.X, _buyPos.Y);

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClickSell()
        {
            SetCursorPos(_sellPos.X, _sellPos.Y);
            Click();
            //Console.WriteLine("Sell Pos: {0},{1}", _sellPos.X, _sellPos.Y);


        }
        public static void Test()
        {
            Console.WriteLine("Testing: Enter \"buy\" or \"sell\" ");

            switch (Console.ReadLine().ToLower())
            {
                case "buy":
                    ClickBuy();
                    break;
                case "sell":
                    ClickSell();
                    break;
                default:
                    Console.WriteLine("Invalid Input");
                    break;
            }
        }

    }
}

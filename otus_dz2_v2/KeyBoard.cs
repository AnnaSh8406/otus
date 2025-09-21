using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace otus_dz2_v2
{

    public class Keyboard
    {

        public static ReplyKeyboardMarkup StartButton()
        {
            return new ReplyKeyboardMarkup(
                        new[] { new KeyboardButton[] { "/start" } })
            {
                ResizeKeyboard = true
            };
        }
        public static ReplyKeyboardMarkup RegisteredButtons()
        {
            return new ReplyKeyboardMarkup(new[]
            {

            new[] {new KeyboardButton("/addtask")},
            new[] {new KeyboardButton("/showalltasks")},
            new[] {new KeyboardButton("/showtasks")},
            new[] {new KeyboardButton("/report")}
        })
            {
                ResizeKeyboard = true
            };
        }
    }
}

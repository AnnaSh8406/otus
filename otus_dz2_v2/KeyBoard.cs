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


        public static readonly string dataDir = Path.Combine("C:", "TgBot");
        public static ReplyKeyboardMarkup GetKeyboardButtons(bool userRegistered)
        {
            if (userRegistered)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "/addtask", "/show", "/report" },
            })
                {
                    ResizeKeyboard = true
                };

                return replyKeyboardMarkup;

            }
            else
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "/start" },
            })
                {
                    ResizeKeyboard = true
                };

                return replyKeyboardMarkup;
            }
        }

        public static ReplyKeyboardMarkup GetKeyboardCancel()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "/cancel" },
            })
            {
                ResizeKeyboard = true
            };

            return replyKeyboardMarkup;
        }


        public static void ValidateString(string? str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var item in str)
                {
                    if (!char.IsWhiteSpace(item))
                    {
                        return;
                    }
                }
            }
            throw new ArgumentException("Передаваемый параметр пуст или содержит одни пробелы");
        }
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

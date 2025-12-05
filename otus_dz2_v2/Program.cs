using LinqToDB;
using System.Runtime.CompilerServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using otus_dz2_v2.Infrastructure.DataAccessDb;
using otus_dz2_v2.Infrastructure.DataAccessDb.Repositories;
using otus_dz2_v2.Core;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Scenarios;
using otus_dz2_v2.TelegramBot;
using otus_dz2_v2.Core.Services;



namespace otus_dz2_v2
{
    public class Program
    {
      
            static async Task Main()
            {
                Console.WriteLine("Введите максимально допустимое количество задач от 1 до 100 шт");
                var taskCountLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

                Console.WriteLine("Введите максимально допустимую длину задачи от 1 символа до 100");
                var taskLengthLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);
                string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User) ?? "";
                var botClient = new TelegramBotClient(token);
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                    DropPendingUpdates = true,

                };

                var ping = await botClient.GetMe();

                Console.WriteLine($"Запуск бота - {ping.FirstName} {ping.Username}");

                var toDoRepository = new SqlToDoRepository(new DataContextFactory());
                var cts = new CancellationTokenSource();
                var userService = new UserService();
                var toDoService = new ToDoService(taskCountLimit, taskLengthLimit, toDoRepository);
                var toDoListService = new ToDoListService();

                var scenarioList = new List<IScenario>();
                scenarioList.Add(new AddTaskScenario(userService, toDoService, toDoListService));
                scenarioList.Add(new AddListScenario(userService, toDoListService));
                scenarioList.Add(new DeleteListScenario(userService, toDoListService, toDoService));
                scenarioList.Add(new DeleteTaskScenario(userService, toDoService));

                var scenarioRepository = new InMemoryScenarioContextRepository();


                var handler = new UpdateHandler(userService, botClient, toDoService,
                                                new ToDoReportService(toDoRepository), scenarioList, scenarioRepository, new ToDoListService());

                try
                {
                    handler.SubscribeOnUpdateStarted(UpdateStarted);
                    handler.SubscribeOnUpdateCompleted(UpdateCompleted);

                    await botClient.SetMyCommands(
                    new[]
                    {
                    new BotCommand { Command = "start", Description = "Запуск бота" },
                    new BotCommand { Command = "help", Description = "Помощь" },
                    new BotCommand { Command = "show", Description = "Список задач" },
                    new BotCommand { Command = "report", Description = "Отчет" },
                    }, cancellationToken: cts.Token
                  );

                    botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);

                    var keyPressTask = Task.Run(() => KeyPress(botClient, cts.Token));

                    await Task.WhenAny(keyPressTask);
                    if (keyPressTask.IsCompleted)
                    {
                     
                        cts.Cancel();
                        Console.WriteLine("\nЗавершение работы бота...");
                    }
                }
                catch (OperationCanceledException e)
                {
                    ShowError("Бот остановлен");
                }
                catch (Exception ex)
                {
                    ShowError($"Произошла непредвиденная ошибка:{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
                }
                finally
                {
                    handler.UnSubscribeOnUpdateStarted(UpdateStarted);
                    handler.UnSubscribeOnUpdateCompleted(UpdateCompleted);
                }
            }

            private async static Task KeyPress(TelegramBotClient bot, CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.A)
                    {
                        return;
                    }
                    else
                    {
                         
                        var me = await bot.GetMe();
                        Console.WriteLine($"\nИнформация о боте: @{me.Username}, ID: {me.Id}");
                        Console.WriteLine("Нажмите клавишу A для выхода");
                    }
                    await Task.Delay(100);
                }
            }
            private static void ShowError(string message)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            private static void UpdateStarted(string message)
            {
                Console.WriteLine($"Началась обработка сообщения {message}");
            }

            private static void UpdateCompleted(string message)
            {
                Console.WriteLine($"Закончилась обработка сообщения {message}");
            }
       }
      
 }

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Exceptions;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using static otus_dz2_v2.Program;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using otus_dz2_v2.Scenarios;
using System.Globalization;
using otus_dz2_v2.Dto;

namespace otus_dz2_v2.TelegramBot
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {



        private readonly IUserService _userService;
        private readonly ITelegramBotClient _botClient;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;
        private readonly IEnumerable<IScenario> _scenarios;
        private readonly IScenarioContextRepository _contextRepository;
        private readonly IToDoListService _toDoListService;
        private static readonly int _pageSize = 5;
        private event MessageEventHandler OnHandleUpdateStarted;
        private event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(IUserService userService,
                             ITelegramBotClient botClient,
                             IToDoService toDoService,
                             IToDoReportService toDoReportService,
                             IEnumerable<IScenario> scenarios,
                             IScenarioContextRepository contextRepository,
                             IToDoListService toDoListService)
        {
            _userService = userService;
            _botClient = botClient;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
            _scenarios = scenarios;
            _contextRepository = contextRepository;
            _toDoListService = toDoListService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await (update switch
                {
                    { Message: { } message } => OnMessage(update, cancellationToken),
                    { CallbackQuery: { } callbackQuery } => OnCallbackQuery(update, cancellationToken)
                });

            }

            catch (ArgumentException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: cancellationToken);
            }

            catch (TaskCountLimitException ex)
            {
                if (update.Message != null)
                    await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
            }

            catch (TaskLengthLimitException ex)
            {
                if (update.Message != null)
                    await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
            }

            catch (DuplicateTaskException ex)
            {
                if (update.Message != null)
                    await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                else if (update.CallbackQuery != null)
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, ex.Message, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
            }
        }



        private async Task OnMessage(Update update, CancellationToken cancellationToken)
        {
            PublishOnUpdateStarted(update.Message.Text);
            var context = await _contextRepository.GetContext(update.Message.From.Id, cancellationToken);
            if (context != null)
            {
                if (update.Message.Text == "/cancel")
                {
                    await _contextRepository.ResetContext(update.Message.From.Id, cancellationToken);
                    await _botClient.SendMessage(update.Message.Chat, "Действие отменено", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    PublishOnUpdateCompleted(update.Message.Text);
                    return;
                }
                await ProcessScenario(context, update, cancellationToken);
                PublishOnUpdateCompleted(update.Message.Text);
                return;
            }

            await ExecuteCommand(update, cancellationToken);
            PublishOnUpdateCompleted(update.Message.Text);
        }
        private async Task OnCallbackQuery(Update update, CancellationToken cancellationToken)
        {
            PublishOnUpdateStarted(update.CallbackQuery.Data);
            var context = await _contextRepository.GetContext(update.CallbackQuery.From.Id, cancellationToken);
            if (context != null)
            {
                await ProcessScenario(context, update, cancellationToken);
                PublishOnUpdateCompleted(update.CallbackQuery.Data);
                return;
            }

            var _toDoUser = await _userService.GetUserAsync(update.CallbackQuery.From.Id, cancellationToken);
            if (_toDoUser == null)
            {
                return;
            }
            var callback = CallbackDto.FromString(update.CallbackQuery.Data);
            InlineKeyboardMarkup replyKeyboardMarkup;
            PagedListCallbackDto toDoListCallback;
            IReadOnlyList<ToDoItem> userToDoItemList;
            IReadOnlyList<ToDoItem> userToDoItemListAll;
            List<KeyValuePair<string, string>> listButtons;
            switch (callback.Action)
            {
                case "show":
                    toDoListCallback = PagedListCallbackDto.FromString(update.CallbackQuery.Data);
                    userToDoItemListAll = (await _toDoService.GetByUserIdAndListAsync(_toDoUser.UserId, toDoListCallback.ToDoListId, cancellationToken));
                    userToDoItemList = userToDoItemListAll.Where(x => x.State == ToDoItemState.Active).ToList();

                    listButtons = new List<KeyValuePair<string, string>>();

                    foreach (var toDoItem in userToDoItemList)
                    {
                        listButtons.Add(new KeyValuePair<string, string>(toDoItem.Name, $"showtask|{toDoItem.Id}"));
                    }

                    replyKeyboardMarkup = BuildPagedButtons(listButtons, toDoListCallback, true);

                    await _botClient.EditMessageText(chatId: update.CallbackQuery.Message.Chat.Id,
                                                     messageId: update.CallbackQuery.Message.MessageId,
                                                     text: "Список задач",
                                                     replyMarkup: replyKeyboardMarkup,
                                                     cancellationToken: cancellationToken
                                                     );
                    break;
                case "addlist":
                    await ProcessScenario(new ScenarioContext(update.CallbackQuery.From.Id, ScenarioType.AddList), update, cancellationToken);
                    break;
                case "deletelist":
                    await ProcessScenario(new ScenarioContext(update.CallbackQuery.From.Id, ScenarioType.DeleteList), update, cancellationToken);
                    break;
                case "showtask":
                    var toDoItemCallback = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                    if (toDoItemCallback.ToDoItemId != null)
                    {
                        var toDoItem = await _toDoService.Get((Guid)toDoItemCallback.ToDoItemId, cancellationToken);
                        if (toDoItem == null)
                        {
                            throw new ArgumentNullException($"Задача с ID {toDoItemCallback.ToDoItemId} не найдена");
                        }

                        if ((toDoItem.State == ToDoItemState.Active))
                        {
                            replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData("✅Выполнить", $"completetask|{toDoItem.Id}"),
                                    InlineKeyboardButton.WithCallbackData("❌Удалить", $"deletetask|{toDoItem.Id}")
                                }
                            });

                            await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"{toDoItem.Name}:\n Срок выполнения {toDoItem.Deadline}\n Время создания {toDoItem.CreatedAt}", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);
                        }

                        if ((toDoItem.State == ToDoItemState.Completed))
                        {
                            await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"{toDoItem.Name}:\n Срок выполнения {toDoItem.Deadline}\n Время создания {toDoItem.CreatedAt}\n Время выполнения {toDoItem.StateChangedAt}", cancellationToken: cancellationToken);
                        }
                    }
                    break;
                case "completetask":
                    toDoItemCallback = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                    var toDoItemComplete = await _toDoService.Get(toDoItemCallback.ToDoItemId.GetValueOrDefault(), cancellationToken);
                    await _toDoService.MarkCompletedAsync(toDoItemComplete.Id, cancellationToken);
                    await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"Задача завершена - {toDoItemComplete.Name}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    break;
                case "deletetask":
                    await ProcessScenario(new ScenarioContext(update.CallbackQuery.From.Id, ScenarioType.DeleteTask), update, cancellationToken);
                    break;
                case "show_completed":
                    toDoListCallback = PagedListCallbackDto.FromString(update.CallbackQuery.Data);
                    userToDoItemListAll = await _toDoService.GetByUserIdAndListAsync(_toDoUser.UserId, toDoListCallback.ToDoListId, cancellationToken);
                    userToDoItemList = userToDoItemListAll.Where(x => x.State == ToDoItemState.Completed).ToList();

                    if (userToDoItemList.Count == 0)
                    {
                        await _botClient.SendMessage(update.CallbackQuery.Message.Chat, $"{GetFullOutput("Список задач пуст", _toDoUser)}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                        return;
                    }

                    listButtons = new List<KeyValuePair<string, string>>();

                    foreach (var toDoItem in userToDoItemList)
                    {
                        listButtons.Add(new KeyValuePair<string, string>(toDoItem.Name, $"showtask|{toDoItem.Id}"));
                    }

                    replyKeyboardMarkup = BuildPagedButtons(listButtons, toDoListCallback, false);

                    await _botClient.EditMessageText(chatId: update.CallbackQuery.Message.Chat.Id,
                                                     messageId: update.CallbackQuery.Message.MessageId,
                                                     text: "Выполненные задачи",
                                                     replyMarkup: replyKeyboardMarkup,
                                                     cancellationToken: cancellationToken
                                                     );
                    break;
            }
            PublishOnUpdateCompleted(update.CallbackQuery.Data);
        }
        private async Task ExecuteCommand(Update botUpdate, CancellationToken cancellationToken)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            var userCommand = botUpdate.Message.Text;
            if (userCommand == string.Empty)
                return;

            if (_toDoUser == null)
            {
                if ((userCommand != "/start") && (userCommand != "/help") && (userCommand != "/info"))
                {
                    await _botClient.SendMessage(botUpdate.Message.Chat, "Доступны команды /start, /help, /info", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(false));
                    return;
                }
            }

            int spacePosition = userCommand.IndexOf(' ');
            string command, parameter;

            if (spacePosition == -1)
            {
                command = userCommand;
                parameter = "";
            }
            else
            {
                command = userCommand.Substring(0, spacePosition);
                parameter = userCommand.Substring(spacePosition + 1);
            }

            bool userRegistered = !(_toDoUser == null);
            switch (command)
            {
                case "/start":
                    await CommandStart(botUpdate, cancellationToken);
                    break;

                case "/help":
                    await CommandHelp(botUpdate, cancellationToken, userRegistered);
                    break;

                case "/info":
                    await CommandInfo(botUpdate, cancellationToken, userRegistered);
                    break;

                case "/addtask":
                    await CommandAddTask(parameter, botUpdate, cancellationToken);
                    break;

                case "/show":
                    await CommandShow(parameter, botUpdate, cancellationToken);
                    break;

                case "/report":
                    await CommandReport(botUpdate, cancellationToken);
                    break;

                case "/find":
                    await CommandShow(parameter, botUpdate, cancellationToken);
                    break;

                case "/exit":
                    await CommandExit(botUpdate, cancellationToken);
                    return;

                default:
                    await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput($"Команда {command} не существует", _toDoUser)}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(userRegistered));
                    break;
            }
            return;
        }

        private async Task CommandStart(Update botUpdate, CancellationToken cancellationToken)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            if (_toDoUser == null)
                _toDoUser = await _userService.RegisterUserAsync(botUpdate.Message.From.Id, botUpdate.Message.From.Username, cancellationToken);

            await _botClient.SendMessage(botUpdate.Message.Chat, $"Привет, {_toDoUser.TelegramUserName}. Вы зарегистрированы!", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
        }

        private async Task CommandHelp(Update botUpdate, CancellationToken cancellationToken, bool userRegistered)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            await _botClient.SendMessage(botUpdate.Message.Chat, @$"
                                            {GetFullOutput("Краткое описание:", _toDoUser)}
                                            /start - начало работы
                                            /help - краткое описание доступных комманд
                                            /info - информация о версии
                                            /addtask - добавить задачу в список дел
                                            /show - показать список активных задач
                                            /find - поиск задачи в списке дел
                                            /report - татистика по задачам пользователя
                                            /cancel - отмена действия при вводе задачи
                                            /exit - завершение работы"
                                            , cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(userRegistered)
                                            );
        }

        private async Task CommandInfo(Update botUpdate, CancellationToken cancellationToken, bool userRegistered)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            await _botClient.SendMessage(botUpdate.Message.Chat, GetFullOutput("Версия бота: 1.12", _toDoUser), cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(userRegistered));
        }

        private async Task CommandAddTask(string parameter, Update botUpdate, CancellationToken cancellationToken)
        {
            await ProcessScenario(new ScenarioContext(botUpdate.Message.From.Id, ScenarioType.AddTask), botUpdate, cancellationToken);
        }
        private async Task CommandShow(string parameter, Update botUpdate, CancellationToken cancellationToken)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            var toDoList = await _toDoListService.GetUserLists(toDoUser.UserId, cancellationToken);
            var listButtons = new List<InlineKeyboardButton>();
            foreach (var list in toDoList)
            {
                listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"show|{list.Id}|0"));
            }

            var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📌Без списка", "show|null|0")
                },
                listButtons.ToArray(),
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🆕Добавить", "addlist"),
                    InlineKeyboardButton.WithCallbackData("❌Удалить", "deletelist")
                }
            });

            await _botClient.SendMessage(botUpdate.Message.Chat, $"Выберите список", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);

        }
        private async Task CommandReport(Update botUpdate, CancellationToken cancellationToken)
        {
            var toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            var stats = await Task.Run(() => _toDoReportService.GetUserStatsAsync(toDoUser.UserId, cancellationToken));
            await _botClient.SendMessage(botUpdate.Message.Chat,
                $"Статистика по задачам на {stats.GeneratedAt}. Всего: {stats.Total}; Завершенных: {stats.Completed}; Активных: {stats.Active}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
        }

        private async Task CommandExit(Update botUpdate, CancellationToken cancellationToken)
        {
            var _toDoUser = await _userService.GetUserAsync(botUpdate.Message.From.Id, cancellationToken);
            await _botClient.SendMessage(botUpdate.Message.Chat, $"{GetFullOutput("Завершение работы.", _toDoUser)}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
            Console.Read();
        }

        private string GetFullOutput(string request, ToDoUser? toDoUser)
        {
            if (toDoUser == null)
                return request;
            else
                return $"{toDoUser.TelegramUserName}, {request.ToLower()}";
        }
        public static int ParseAndValidateInt(string? str, int min, int max)
        {
            if ((!int.TryParse(str, out int result)) || (result < min) || (result > max))
            {
                throw new ArgumentException();
            }
            return result;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }

        public void SubscribeOnUpdateStarted(MessageEventHandler handler) => OnHandleUpdateStarted += handler;
        public void SubscribeOnUpdateCompleted(MessageEventHandler handler) => OnHandleUpdateCompleted += handler;
        public void UnSubscribeOnUpdateStarted(MessageEventHandler handler) => OnHandleUpdateStarted -= handler;
        public void UnSubscribeOnUpdateCompleted(MessageEventHandler handler) => OnHandleUpdateCompleted -= handler;
        public void PublishOnUpdateStarted(string message) => OnHandleUpdateStarted.Invoke(message);
        public void PublishOnUpdateCompleted(string message) => OnHandleUpdateCompleted.Invoke(message);
        private IScenario GetScenario(ScenarioType scenario)
        {
            foreach (var item in _scenarios)
            {
                if (item.CanHandle(scenario))
                    return item;
            }
            throw new Exception("Сценарий не найден");
        }

        private async Task ProcessScenario(ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            var scenario = GetScenario(context.CurrentScenario);
            var result = await scenario.HandleMessageAsync(_botClient, context, update, cancellationToken);

            if (result == ScenarioResult.Completed)
            {
                await _contextRepository.ResetContext(context.UserId, cancellationToken);
            }
            else
            {
                await _contextRepository.ResetContext(context.UserId, cancellationToken);
                await _contextRepository.SetContext(context.UserId, context, cancellationToken);
            }
        }



        private InlineKeyboardMarkup BuildPagedButtons(IReadOnlyList<KeyValuePair<string, string>> callbackData, PagedListCallbackDto listDto, bool showCompletedButton)
        {
            int totalPages = (callbackData.Count + _pageSize - 1) / _pageSize;
            var batch = callbackData.GetBatchByNumber(_pageSize, listDto.Page);
            var listButtons = new List<InlineKeyboardButton[]>();
            foreach (var button in batch)
            {
                listButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(button.Key, button.Value) });
            }

            var pageButtons = new List<InlineKeyboardButton>();

            if (listDto.Page > 0)
                pageButtons.Add(InlineKeyboardButton.WithCallbackData("⬅️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page - 1}"));

            if (listDto.Page < totalPages - 1)
                pageButtons.Add(InlineKeyboardButton.WithCallbackData("➡️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page + 1}"));

            listButtons.Add(pageButtons.ToArray());
            if (showCompletedButton)
                listButtons.Add(new[] { InlineKeyboardButton.WithCallbackData("☑️Посмотреть выполненные", $"show_completed|{listDto.ToDoListId}|{0}") });

            return new InlineKeyboardMarkup(listButtons);
        }
    }



}

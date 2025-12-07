using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Infrastructure.DataAccessDb.Models;

namespace otus_dz2_v2.Infrastructure.DataAccessDb
{ 
        internal class ModelMapper
        {
            public static ToDoUser MapFromModel(ToDoUserModel model)
            {
                return new ToDoUser()
                {
                    UserId = model.UserId,
                    TelegramUserName = model.TelegramUserName,
                    RegisteredAt = model.RegisteredAt,
                    TelegramUserId = model.TelegramUserId
                };
            }
            public static ToDoUserModel MapToModel(ToDoUser entity)
            {
                return new ToDoUserModel()
                {
                    UserId = entity.UserId,
                    TelegramUserName = entity.TelegramUserName,
                    RegisteredAt = entity.RegisteredAt,
                    TelegramUserId = entity.TelegramUserId
                };
            }
            public static ToDoItem MapFromModel(ToDoItemModel model)
            {
                var resultToDoItem = new ToDoItem()
                {
                    Id = model.Id,
                    User = new ToDoUser() { UserId = model.UserId },
                    Name = model.Name,
                    CreatedAt = model.CreatedAt,
                    State = model.State,
                    StateChangedAt = model.StateChangedAt,
                    Date = model.Date,
                };

            if (model.ListId != null)
                resultToDoItem.List = new ToDoList() { Id = (Guid)model.ListId };

            return resultToDoItem;

        }
            public static ToDoItemModel MapToModel(ToDoItem entity)
            {
                return new ToDoItemModel()
                {
                    Id = entity.Id,
                    UserId = entity.User.UserId,
                    Name = entity.Name,
                    CreatedAt = entity.CreatedAt,
                    State = entity.State,
                    StateChangedAt = entity.StateChangedAt,
                    Date = entity.Date,
                    ListId = entity.List?.Id
                };
            }
            public static ToDoList MapFromModel(ToDoListModel model)
            {
                return new ToDoList()
                {
                    Id = model.Id,
                    Name = model.Name,
                    User = new ToDoUser() { UserId = model.UserId },
                    CreatedAt = model.CreatedAt
                };
            }
            public static ToDoListModel MapToModel(ToDoList entity)
            {
                return new ToDoListModel()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    UserId = entity.User.UserId,
                    CreatedAt = entity.CreatedAt
                };
            }


        public static Notification MapFromModel(NotificationModel model)
        {
            return new Notification()
            {
                Id = model.Id,
                User = new ToDoUser() { UserId = model.UserId },
                Type = model.Type,
                Text = model.Text,
                ScheduledAt = model.ScheduledAt,
                IsNotified = model.IsNotified,
                NotifiedAt = model.NotifiedAt
            };
        }
        public static NotificationModel MapToModel(Notification entity)
        {
            return new NotificationModel()
            {
                Id = entity.Id,
                UserId = entity.User.UserId,
                Type = entity.Type,
                Text = entity.Text,
                ScheduledAt = entity.ScheduledAt,
                IsNotified = entity.IsNotified,
                NotifiedAt = entity.NotifiedAt
            };
        }
    }
    }

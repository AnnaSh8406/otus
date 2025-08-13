using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using Microsoft.VisualBasic;



namespace otus_dz2_v2
{


 
    internal class Program
    {
        
 

        static void Main(string[] args )
            {
       
            Console.WriteLine("Ведите одну из команд:/start, /help, /info, /exit, /addtask имя задачи, /showtasks, /removetask номер задачи, /completetask номер задачи, /showalltasks");
            var botClient = new ConsoleBotClient();

             
            IUpdateHandler t=new UpdateHandler();
             botClient.StartReceiving(t);
         

        }
    }
}

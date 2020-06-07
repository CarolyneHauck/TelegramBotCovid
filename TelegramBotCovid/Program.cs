using System;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBotCovid
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("1200346381:AAHnNDAsHME7mNl6EhkrGZdf5ZZkoCRkaJg");
        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StartReceiving();
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if(e.Message.Text != null && e.Message.Text != "Total de casos")
                {
                    var url = string.Format("https://covid19-brazil-api.now.sh/api/report/v1/brazil/uf/{0}", e.Message.Text);
                    var dados = HttpClient.Get(url);

                    dynamic jsonTratado = JsonConvert.DeserializeObject(dados);

                    if (jsonTratado["state"] != null)
                    {
                        var state = jsonTratado.state;
                        var cases = jsonTratado.cases;
                        var deaths = jsonTratado.deaths;
                        var suspects = jsonTratado.suspects;
                        var datetime = jsonTratado.datetime;

                        Bot.SendTextMessageAsync(e.Message.Chat.Id, string.Format("Estado:{0}\n\rCasos: {1}\n\rMortes: {2}\n\rSuspeitos: {3}\n\rData: {4}", state, cases, deaths, suspects, datetime));
                    }
                    if (jsonTratado["error"] != null)
                    {
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, string.Format("Informe a sigla do estado que deseja verificar os casos de COVID ou caso queira verificar o total de casos no país digite \"Total de casos\""));
                    }
                }
                else if(e.Message.Text == "Total de casos" || e.Message.Text == "casos")
                {
                    var dados = HttpClient.Get("https://covid19-brazil-api.now.sh/api/report/v1/brazil");

                    dynamic jsonTratado = JsonConvert.DeserializeObject(dados);

                    if (jsonTratado["data"] != null)
                    {
                        var confirmed = jsonTratado.data.confirmed;
                        var deaths = jsonTratado.data.deaths;
                        var updated_at = jsonTratado.data.updated_at;

                        Bot.SendTextMessageAsync(e.Message.Chat.Id, string.Format("Casos confirmados no Brasil: {0}\n\rMortes: {1}\n\rData: {2}", confirmed, deaths, updated_at));
                    }
                }
                else
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, string.Format("Informe a sigla do estado que deseja verificar os casos de COVID ou caso queira verificar o total de casos no país digite \"Total de casos\""));
                }
            }
        }
    }
}

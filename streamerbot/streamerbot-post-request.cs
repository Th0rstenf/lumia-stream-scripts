using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    private static string lumiaToken = Environment.GetEnvironmentVariable("LumiaToken");
    private static string left = "colorLeft";
    private static string right = "colorRight";
    private static string both = "color";
    // This delay time is defined by the duration of the interactive Lumia chat commands.
    //TODO: Check whether it is possible to read that duration via API?
    private static int delayTimeMs = 30000;
    private static JObject buildChatCommand(string command, string color)
    {
        
	    var extraSettings = new JObject();
        extraSettings.Add("message", color);
        var jsonParams = new JObject();
        jsonParams.Add("value", command);
        jsonParams.Add("extraSettings", extraSettings);
        var chatCommand = new JObject();
        chatCommand.Add("type", "chat-command");
        chatCommand.Add("params", jsonParams);
        return chatCommand;
    }

    private static string postRequest(string uri, string command, string color)
    {
        var requestJson = buildChatCommand(command, color);
        using (var client = new HttpClient())
        {
            var endpoint = new Uri(uri);
            var payload = new StringContent(requestJson.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(endpoint, payload).Result.Content.ReadAsStringAsync().Result;
            return result;
        }
    }
	
    private static void sendCommand(string command, string color)
    {
        string url = string.Format("http://localhost:39231/api/send?token={0}", lumiaToken);
        postRequest(url, command, color);
    }
	
    public bool Execute()
    {
        string colors = args["rawInput"].ToString();
        string[] colorsArray = colors.Trim().Split(' '); 
        if (colorsArray.Length == 2)
        {
            sendCommand(left, colorsArray[0]);
	        sendCommand(right, colorsArray[1]);
            CPH.Wait(delayTimeMs);
        }
        else if (colorsArray.Length == 1)
        {
            sendCommand(both, colors);
            CPH.Wait(delayTimeMs);   
        }
        else
        {
            //TODO: Here we could log an error
        }
        return true;
    }
}

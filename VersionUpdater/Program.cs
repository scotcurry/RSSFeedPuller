using System.IO;
using System.Text.RegularExpressions;

var currentDateTime = DateTime.Now;
var month = currentDateTime.Month;
var day = currentDateTime.Day;
var year = currentDateTime.Year;

var midnight = new DateTime(year, month, day, 0, 0, 0);
var minutesOfDayTimeSpan = currentDateTime - midnight;
var minutesOfDay = (minutesOfDayTimeSpan.Hours * 60) + minutesOfDayTimeSpan.Minutes;
var dayString = day.ToString();
if (dayString.Length == 1)
{
    dayString = "0" + dayString;
}


var version = string.Concat(year.ToString().AsSpan(3, 1), ".", month.ToString(), dayString);
version += "." + minutesOfDay.ToString();
Console.WriteLine("Version: " + version);

var pathToBuildScript = "/Users/scot.curry/Projects/RSSFeedPuller/RSSFeedPuller";
if (Directory.Exists(pathToBuildScript))
{
    var buildScript = pathToBuildScript + "/build_container.sh";
    try
    {
        var fileText = File.ReadAllText(buildScript);
        var regExPattern = "rssfeedpuller:v\\d{1}\\.\\d{3}\\.\\d{1,4}";
        var matches = Regex.Match(fileText, regExPattern);
        var newVersion = "rssfeedpuller:v" + version;
        var newFileText = Regex.Replace(fileText, regExPattern, newVersion);
        Console.Write(newFileText);

        var fileInfo = new FileInfo(buildScript);
        File.WriteAllText(buildScript, newFileText);
    } catch (IOException ex)
    {
        Console.WriteLine("Error Reading File: " + ex.Message);
    }
}
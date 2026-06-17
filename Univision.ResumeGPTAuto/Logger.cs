using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.ResumeGPTAuto
{
  public enum LogLevel { Info, Warning, Error }

  public static class Logger
  {
    public static bool EnableConsoleOutput { get; set; } = false;
    public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Info;
    private static readonly object _lock = new object();

    public static void Write(string message, LogLevel level = LogLevel.Info, bool addNewLine = false)
    {
      if (level < CurrentLogLevel)
      {
        return;
      }

      lock (_lock)
      {
        try
        {
          string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
          if (!Directory.Exists(directory))
          {
            Directory.CreateDirectory(directory);
          }

          string filePath = Path.Combine(directory, $"{DateTime.Now:yyyy-MM-dd}.log");
          string timeStamp = $"{DateTime.Now:HH:mm:ss}";
          string logMessage = $"{timeStamp} [{level}] - {message}";

          if (addNewLine)
          {
            logMessage += Environment.NewLine;
          }

          File.AppendAllText(filePath, logMessage + Environment.NewLine);


          if (EnableConsoleOutput)
          {
            if (level == LogLevel.Error)
              Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (level == LogLevel.Error)
              Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (addNewLine)
              Console.WriteLine(logMessage);
            else
              Console.Write(logMessage);
          }
          // 필요하다면 콘솔에도 출력 가능합니다.
          Console.ResetColor();
        }
        catch (Exception ex)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"로그 작성 중 오류 발생: {ex.Message}");
          Console.ResetColor();
        }
      }
    }

    public static void WriteLine(string message, LogLevel level = LogLevel.Info)
    {
      Write(message, level, addNewLine: true);
    }

    public static async Task WriteAsync(string message, LogLevel level = LogLevel.Info)
    {
      await Task.Run(() => Write(message, level));
    }

    public static async Task WriteLineAsync(string message, LogLevel level = LogLevel.Info)
    {
      await Task.Run(() => Write(message, level, true));
    }
  }
}



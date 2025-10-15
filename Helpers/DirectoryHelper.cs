using NUnit.Framework;
using System;
using System.IO;

namespace warehouse.Helpers;

/// <summary>
/// DirectoryHelper
/// </summary>
public static class DirectoryHelper
{
    /// <summary>
    /// GetCurrentDirectory
    /// </summary>
    /// <returns></returns>
    private static string GetCurrentDirectory()
    {
        return TestContext.CurrentContext.WorkDirectory;
    }

    /// <summary>
    /// GetAssetsDirectory
    /// </summary>
    /// <returns></returns>
    public static string GetAssetsDirectory(string fileName)
    {
        var currentDirectory = GetCurrentDirectory();
        return Path.Combine(currentDirectory, "Assets", fileName);
    }

    /// <summary>
    /// GetReportsDirectory
    /// </summary>
    /// <returns></returns>
    public static string GetReportsDirectory()
    {
        string? projectDirectory = GetProjectDirectory();
        return Path.Combine(projectDirectory ?? string.Empty, "Reports", TestContext.CurrentContext.Test.Name, "ExtentReports.html");
    }

    /// <summary>
    /// GetDownloadsDirectory
    /// </summary>
    /// <returns></returns>
    public static string GetDownloadsDirectory(string fileName)
    {
        var projectDirectory = GetCurrentDirectory();
        return Path.Combine(projectDirectory ?? string.Empty, "Downloads", fileName);
    }

    /// <summary>
    /// GetDownloadsDirectory
    /// </summary>
    /// <returns></returns>
    public static string GetFilePath()
    {
        var projectDirectory = GetCurrentDirectory();
        return Path.Combine(projectDirectory ?? string.Empty, "Downloads");
    }


    /// <summary>
    /// GetScreenRecordingDirectory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetScreenRecordingDirectory(string fileName)
    {
        var projectDirectory = GetCurrentDirectory();
        return Path.Combine(projectDirectory ?? string.Empty, "ScreenRecording", fileName);
    }

    /// <summary>
    /// GetScreenshotDirectory
    /// </summary>
    /// <returns></returns>
    public static string GetScreenshotDirectory(string fileName)
    {
        var projectDirectory = GetProjectDirectory();
        return Path.Combine(projectDirectory ?? string.Empty, "Screenshots", fileName);
    }

    /// <summary>
    /// GetProjectDirectory
    /// </summary>
    /// <returns></returns>
    private static string? GetProjectDirectory()
    {
        return Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
    }
}

/// <summary>
/// URLHelper
/// </summary>
public static class URLHelper
{
    /// <summary>
    /// GetTestServer
    /// </summary>
    /// <param name="url"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetTestServer(string url, string username = "", string password = "")
    {
        var index = url.IndexOf("//");
        return url.Insert(index + 2, username == "" ? string.Empty : GetServerAuth(username, password) + '@');
    }

    /// <summary>
    /// GetServerAuth
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetServerAuth(string username, string password) => $"{username}:{password}";
}
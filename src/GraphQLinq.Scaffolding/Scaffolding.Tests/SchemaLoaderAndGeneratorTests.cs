using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scaffolding.Tests;

//TODO: Update tests to support running in an async manner, they use the same output folder per now [one easy way, append 'a random number/curr time/guid" to folder output], in short: run in isolation
//TODO: Better validation than just that the output has some content, and that the tool has not crashed, it's not a proof that the classes can be compiled
[TestClass]
public class SchemaLoaderAndGeneratorTests
{
    # region Global settings default for all unit test methods
    const string Uri = "https://api.spacex.land/graphql";

    const string U = "";            //username
    const string P = "";            //pwd

    //header keys semi colon list
    const string Hk = "";           

    //header values semi colon list
    const string Hv = "";

    string O => "\"" + OutputFolder() + "\"";
    #endregion

    [TestMethod]
    public void export_schema_query_success()
    {
        var f = "";         //file name for single output
        var n = "";         //namespace
        var c = "";         //context name 

        var q = true;       //save query
        var j = false;      //save json schema

        var files = Execute(Uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length == 1, "Error: query output should have created 1 query file");
        Assert.IsTrue(files[0].EndsWith("query"), "Error: extension of file is not .query");
    }

    [TestMethod]
    public void output_from_api_create_single_file_with_jsonschema_success()
    {
        var f = "SingleFile";       //file name for single output
        var n = "SpaceX";           //namespace
        var c = "SpaceX";           //context name 

        var q = false;              //save query
        var j = true;               //save json schema

        var files = Execute(Uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length == 2, "Error: Singlefile reading and outputting json, did not result in two files");

        foreach (var file in files)
            ValidateFileContent(n, file);
    }

    [TestMethod]
    public void output_from_api_Create_multi_files_success()
    {
        var f = "";         //file name for single output
        var n = "SpaceX";   //namespace
        var c = "SpaceX";   //context name 

        var q = false;      //save query
        var j = false;      //save json schema

        var files = Execute(Uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length > 1, "Error: multiline resulted in 0 or 1 files, it should have created multiple files, one per type");

        foreach (var file in files)
            ValidateFileContent(n, file);
    }

    [TestMethod]
    public void output_from_api_create_single_file_success()
    {
        var f = "output_from_api_create_single_file_success";         //file name for single output
        var n = "SpaceX";   //namespace
        var c = "SpaceX";   //context name 

        var q = false;      //save query
        var j = false;      //save json schema

        var files = Execute(Uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length == 1, "Error: multiline resulted in 0 or 1 files, it should have created multiple files, one per type");

        foreach (var file in files)
            ValidateFileContent(n, file);
    }

    [TestMethod]
    public void output_from_local_create_single_file_success()
    {
        var uriPath = GetPath("Data", true) + "\\spacex-schema.json";
        var uri = uriPath;

        var f = "output_from_local_create_single_file_success";         //file name for single output
        var n = "SpaceX";   //namespace
        var c = "SpaceX";   //context name 

        var q = false;      //save query
        var j = false;      //save json schema

        var files = Execute(uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length == 1, "Error: local single file should result in 1 file read from a local json file");

        foreach (var file in files)
            ValidateFileContent(n, file);
    }

    [TestMethod]
    public void output_from_local_create_multi_files_success()
    {
        var uriPath = GetPath("Data", true) + "\\spacex-schema.json";
        var uri = uriPath;

        var f = "";         //file name for single output
        var n = "SpaceX";   //namespace
        var c = "SpaceX";   //context name 

        var q = false;      //save query
        var j = false;      //save json schema

        var files = Execute(uri, O, f, n, c, U, P, q, j, Hk, Hv);

        Assert.IsTrue(files.Length > 1, "Error: local multi files should result in 1 file per type, it did not");

        foreach (var file in files)
            ValidateFileContent(n, file);
    }

    string GetPath(string path, bool root = false)
    {
        if(root == true)
        {
            return Path.Combine(Environment.CurrentDirectory, path);
        }

        var parent = "GraphQLinq.Scaffolding";

        var rootDir = Directory.GetParent(Environment.CurrentDirectory);
        var current = Directory.GetParent(Environment.CurrentDirectory);

        for(int i= 0; i < 10; i++)
        {
            if (current.FullName.EndsWith(parent)) break;

            current = Directory.GetParent(current.FullName);

            if (current == null)
                break;
        }
        
        if (current == null)
        {
            current = rootDir;
        }

        return Path.Combine(current.FullName, path);
    }

    string OutputFolder()
    {
        return GetPath("UnitTestOutput");
    }

    string[] Execute(string uri, string o, string f, string n, string c, string u, string p, bool q, bool j, string hk, string hv)
    {
        var args = uri + " -o=" + o + " -f=" + f + " -n=" + n + " -c=" + c + " -u=" + u + " -p=" + p + " -q=" + q + " -j=" + j + " -hk=\"" + hk + "\" -hv=\"" + hv + "\"";

        var files = Execute(args);

        return files;
    }

    string[] Execute(string args)
    {
        CleanOutput(OutputFolder());

        var exe = GetScaffoldingExe();
        var psi = new ProcessStartInfo();
        psi.FileName = exe;
        psi.RedirectStandardError = true;
        psi.RedirectStandardOutput = true;
        psi.Arguments = args;

        var process = Process.Start(psi);
        var reader = process.StandardError;

        process.WaitForExit();

        var exitCode = process.ExitCode;

        var errors = reader.ReadToEnd();
        
        Assert.IsTrue(errors == null || errors.Length < 1, errors);

        Assert.IsTrue(exitCode == 0, "Exit code is not: " + exitCode + ". Run the test-command.cmd file to debug the Scaffolding.exe...");

        Console.WriteLine("Outputted to " + OutputFolder());

        return Directory.GetFiles(OutputFolder(), "*.*", SearchOption.TopDirectoryOnly);
    }

    static void ValidateFileContent(string @namespace, string file)
    {
        var content = File.ReadAllText(file);

        Assert.IsTrue(content.Is(), "Content inside file " + file + " is blank");

        if (file.EndsWith(".cs"))
        {
            Assert.IsTrue(content.Contains("namespace " + @namespace), "file does not contain namespace: " + file);

            Assert.IsTrue(content.Contains("class") || content.Contains("enum") || content.Contains("interface"), "file is not an enum, interface nor class file: " + file);

            Assert.IsTrue(content.Contains("public"), "file does not contain anything public: " + file);
        }
    }

    string GetScaffoldingExe()
    {
        var directory = Environment.CurrentDirectory;
        var scaffoldingDirectory = "";
        while (scaffoldingDirectory == "")
        {
            var directoryInfo = Directory.GetParent(directory);
            var folders = Directory.GetDirectories(directory);

            foreach (var folder in folders)
                if (folder.EndsWith("Scaffolding"))
                    scaffoldingDirectory = folder;

            directory = directoryInfo.FullName;
        }

        var bin = scaffoldingDirectory + @"\bin\";

        if (!Directory.Exists(bin))
            throw new Exception("Exe file of Scaffolding project is not found, please compile the Scaffolding.csproj first located at: " + scaffoldingDirectory);

        string[] files = Directory.GetFiles(bin, "*.exe", SearchOption.AllDirectories);

        if (files == null) throw new Exception("Exe file of Scaffolding project is not found, please compile the Scaffolding.csproj first located at: " + scaffoldingDirectory);

        string exe = null;

        if (files.Length == 1)
        {
            exe = files[0];
        }
        else
        {
            var file1 = new FileInfo(files[0]);
            var file2 = new FileInfo(files[1]);

            exe = file1.CreationTime > file2.CreationTime ? files[0] : files[1];
        }

        return "\"" + exe + "\"";
    }

    void CleanOutput(string outputFolder)
    {
        outputFolder = outputFolder.Replace("\"", "");

        if (!Directory.Exists(outputFolder)) return;

        var files = Directory.GetFiles(outputFolder, "*.cs", SearchOption.TopDirectoryOnly);
        if (files != null && files.Length > 0)
        {
            foreach (var file in files)
                File.Delete(file);
        }
        Thread.Sleep(25);
        files = Directory.GetFiles(outputFolder, "*.query", SearchOption.TopDirectoryOnly);
        if (files != null && files.Length > 0)
        {
            foreach (var file in files)
                File.Delete(file);
        }
        Thread.Sleep(25);
        files = Directory.GetFiles(outputFolder, "*.json", SearchOption.TopDirectoryOnly);
        if (files != null && files.Length > 0)
        {
            foreach (var file in files)
                File.Delete(file);
        }
        Thread.Sleep(25);
    }
}
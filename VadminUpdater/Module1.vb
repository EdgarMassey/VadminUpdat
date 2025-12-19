Imports System.IO
Imports System.Threading

Module Module1
    Private sokvag As String
    Private Filename As String
    Private logPath As String
    Sub Main()
        Dim version As String = "20251216a"
        ' ONE log path only (Temp is safest)
        logPath = Path.Combine(Path.GetTempPath(), "VadminUpdater.log")
        sokvag = AppDomain.CurrentDomain.BaseDirectory
        Try
            LasVersion()
            Dim targetExe As String = Path.Combine(sokvag, Filename & ".exe")
            Dim tempExe As String = Path.Combine(sokvag, Filename & "temp.exe")
            For Each p In Process.GetProcessesByName("VadminOLFnet")
                Try
                    p.CloseMainWindow()
                    If Not p.WaitForExit(5000) Then
                        p.Kill()
                        p.WaitForExit(5000)
                    End If
                Catch ex As Exception
                    Log("ERROR closing process: " & ex.ToString())
                End Try
            Next
            Thread.Sleep(500)
            If Not File.Exists(tempExe) Then
                Throw New FileNotFoundException("Temp exe not found: " & tempExe)
            End If
            If File.Exists(targetExe) Then
                File.Delete(targetExe)
                Log("Deleted old exe")
            End If

            File.Move(tempExe, targetExe)
            Process.Start(targetExe)
            File.WriteAllText(Path.Combine(sokvag, "UpdateStatus"), "Success")
            Environment.ExitCode = 0
        Catch ex As Exception
            File.WriteAllText(Path.Combine(sokvag, "UpdateStatus"), "Failure")
            Environment.ExitCode = 1
        End Try

    End Sub
    Private Sub LasVersion()
        Dim updateFilePath As String = Path.Combine(sokvag, "UpdateFile.txt")
        Log("UpdateFile path: " & updateFilePath & " exists=" & File.Exists(updateFilePath).ToString())

        If Not File.Exists(updateFilePath) Then
            Throw New FileNotFoundException("UpdateFile.txt not found next to Updater.exe: " & updateFilePath)
        End If

        Dim line As String = File.ReadAllText(updateFilePath).Trim()
        Filename = line.Replace(" ", "")

        If String.IsNullOrWhiteSpace(Filename) Then
            Throw New Exception("UpdateFile.txt is empty (Filename becomes blank).")
        End If
    End Sub
    Private Sub Log(msg As String)
        File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "  " & msg & Environment.NewLine)
    End Sub

End Module

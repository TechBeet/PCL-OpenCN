Public Class PageOtherTest

    Private Shared ReadOnly RandomCaveList As New List(Of String) From {
        "在深山中寻找神秘的洞穴，或许能发现宝藏。",
        "洞穴中的宝藏可能隐藏在黑暗的角落。",
        "使用火把照亮洞穴，避免陷入黑暗。",
        "洞穴深处可能有未知的生物，小心行事。",
        "寻找洞穴入口时，注意地形变化。"
    }

    Private Shared ReadOnly RandomHintList As New List(Of String) From {
        "提示：使用水桶收集水源。",
        "提示：在夜晚使用火把或火炬。",
        "提示：注意观察地形，寻找资源。",
        "提示：与村民交易获取稀有物品。",
        "提示：在洞穴中寻找红石矿石。"
    }

    Private Shared ReadOnly RandomPresetHintList As New List(Of String) From {
        "建议：在白天采集资源，避免生物攻击。",
        "建议：建造一个安全的基地，以备不时之需。",
        "建议：使用工具升级武器和防具。",
        "建议：在洞穴中寻找钻石和金矿。",
        "建议：与其他玩家合作，共同开发资源。"
    }

    Public Shared Sub StartCustomDownload(Url As String, FileName As String, Optional Folder As String = Nothing)
        Try
            If String.IsNullOrEmpty(Url) Then
                Hint("下载地址不能为空！", HintType.Red)
                Return
            End If

            Dim SavePath As String = If(String.IsNullOrEmpty(Folder), Path.GetTempPath(), Folder)
            If Not SavePath.EndsWith("\") Then SavePath &= "\"
            Directory.CreateDirectory(SavePath)

            Dim FullPath As String = Path.Combine(SavePath, FileName)
            If File.Exists(FullPath) Then
                Hint($"文件已存在：{FullPath}", HintType.Red)
                Return
            End If

            Using Client As New Net.WebClient()
                AddHandler Client.DownloadProgressChanged, Sub(sender, e)
                    Dim Percent As Integer = CInt(e.ProgressPercentage)
                    Hint($"正在下载：{Percent}%", HintType.Blue, False)
                End Sub

                Client.DownloadFileAsync(New Uri(Url), FullPath)
                AddHandler Client.DownloadFileCompleted, Sub(sender, e)
                    If e.Error IsNot Nothing Then
                        Hint($"下载失败：{e.Error.Message}", HintType.Red)
                    Else
                        Hint($"下载完成：{FullPath}", HintType.Green)
                    End If
                End Sub
            End Using
        Catch ex As Exception
            Logger.Error(ex, "自定义下载出错")
            Hint($"下载失败：{ex.Message}", HintType.Red)
        End Try
    End Sub

    Public Shared Sub Jrrp()
        Try
            Dim TempPath As String = Path.GetTempPath()
            Dim TempFiles As New List(Of String)
            Dim TempDirs As New List(Of String)
            Dim TotalSize As Long = 0

            For Each FilePath In Directory.GetFiles(TempPath, "*", SearchOption.AllDirectories)
                Try
                    Dim FileInfo As New IO.FileInfo(FilePath)
                    If FileInfo.Exists AndAlso FileInfo.LastWriteTime < DateTime.Now.AddDays(-7) Then
                        TempFiles.Add(FilePath)
                        TotalSize += FileInfo.Length
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, $"检查文件 {FilePath} 时出错")
                End Try
            Next

            For Each DirPath In Directory.GetDirectories(TempPath, "*", SearchOption.AllDirectories)
                Try
                    Dim DirInfo As New IO.DirectoryInfo(DirPath)
                    If DirInfo.GetFiles().Length = 0 AndAlso DirInfo.GetDirectories().Length = 0 Then
                        TempDirs.Add(DirPath)
                    End If
                Catch ex As Exception
                    Logger.Warn(ex, $"检查目录 {DirPath} 时出错")
                End Try
            Next

            For Each FilePath In TempFiles
                Try
                    File.Delete(FilePath)
                Catch ex As Exception
                    Logger.Warn(ex, $"删除文件 {FilePath} 时出错")
                End Try
            Next

            For Each DirPath In TempDirs
                Try
                    Directory.Delete(DirPath)
                Catch ex As Exception
                    Logger.Warn(ex, $"删除目录 {DirPath} 时出错")
                End Try
            Next

            Hint($"已清理 {TempFiles.Count} 个文件，{TempDirs.Count} 个目录，共 {TotalSize / 1024 / 1024} MB。", HintType.Green)
        Catch ex As Exception
            Logger.Error(ex, "清理垃圾文件出错")
            Hint($"清理失败：{ex.Message}", HintType.Red)
        End Try
    End Sub

    Public Shared Sub RubbishClear()
        Jrrp()
    End Sub

    Public Shared Sub MemoryOptimize(ShowHint As Boolean)
        Try
            Dim BeforeMemory As Long = My.Computer.Info.AvailablePhysicalMemory
            GC.Collect()
            GC.WaitForPendingFinalizers()
            Dim AfterMemory As Long = My.Computer.Info.AvailablePhysicalMemory

            If ShowHint Then
                Dim FreedMemory As Long = BeforeMemory - AfterMemory
                Hint($"内存优化完成，释放 {FreedMemory / 1024 / 1024} MB 内存。", HintType.Green)
            End If
        Catch ex As Exception
            Logger.Error(ex, "内存优化出错")
            If ShowHint Then
                Hint($"内存优化失败：{ex.Message}", HintType.Red)
            End If
        End Try
    End Sub

    Public Shared Sub MemoryOptimizeInternal(ShowHint As Boolean)
        MemoryOptimize(ShowHint)
    End Sub

    Public Shared Function GetRandomCave() As String
        If RandomCaveList.Count = 0 Then Return "未知洞穴..."
        Return RandomCaveList(New Random().Next(RandomCaveList.Count))
    End Function

    Public Shared Function GetRandomHint() As String
        If RandomHintList.Count = 0 Then Return "未知提示..."
        Return RandomHintList(New Random().Next(RandomHintList.Count))
    End Function

    Public Shared Function GetRandomPresetHint() As String
        If RandomPresetHintList.Count = 0 Then Return "未知建议..."
        Return RandomPresetHintList(New Random().Next(RandomPresetHintList.Count))
    End Function

End Class

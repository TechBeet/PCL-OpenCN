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

    Private Sub Init() Handles Me.Loaded
        PanBack.ScrollToHome()
        LabCaveResult.Text = GetRandomCave()
    End Sub

#Region "内存优化"

    Private Sub BtnMemoryOptimize_Click() Handles BtnMemoryOptimize.Click
        Dim Freed As Long = MemoryOptimizeCore()
        If Freed > 0 Then
            LabMemoryResult.Text = $"内存优化完成，可用内存增加 {Freed / 1024 / 1024} MB。"
        Else
            LabMemoryResult.Text = "内存优化完成，未检测到明显变化。"
        End If
    End Sub

    Private Shared Function MemoryOptimizeCore() As Long
        Dim BeforeMemory As Long = My.Computer.Info.AvailablePhysicalMemory
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
        Return BeforeMemory - My.Computer.Info.AvailablePhysicalMemory
    End Function

    Public Shared Sub MemoryOptimize(ShowHint As Boolean)
        Try
            Dim FreedMemory As Long = MemoryOptimizeCore()

            If ShowHint Then
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

#End Region

#Region "垃圾清理"

    Private Sub BtnRubbishClear_Click() Handles BtnRubbishClear.Click
        RunInThread(
        Sub()
            Dim Result As String = RubbishClearInternal()
            RunInUi(Sub() LabRubbishResult.Text = Result)
        End Sub)
    End Sub

    ''' <summary>
    ''' 清理 PCL 自身的任务临时文件夹（不会触碰系统的其他临时文件）。
    ''' </summary>
    Private Shared Function RubbishClearInternal() As String
        Dim TotalSize As Long = 0
        Dim DeletedCount As Integer = 0
        'PCL 的任务临时文件只存在于这两个位置
        For Each Folder In {PathTemp & "TaskTemp", OsDrive & "ProgramData\PCL\TaskTemp"}
            Try
                If Directory.Exists(Folder) Then
                    For Each FilePath In Directory.GetFiles(Folder, "*", SearchOption.AllDirectories)
                        Dim FileInfo As New IO.FileInfo(FilePath)
                        If FileInfo.Exists Then
                            TotalSize += FileInfo.Length
                            DeletedCount += 1
                        End If
                    Next
                    Directory.Delete(Folder, True)
                End If
            Catch ex As Exception
                Logger.Warn(ex, $"清理临时文件夹 {Folder} 失败")
            End Try
        Next
        Return $"已删除 {DeletedCount} 个临时文件，释放 {TotalSize / 1024 / 1024} MB 空间。"
    End Function

    Public Shared Sub RubbishClear()
        Try
            Dim Result As String = RubbishClearInternal()
            Hint(Result, HintType.Green)
        Catch ex As Exception
            Logger.Error(ex, "清理垃圾文件出错")
            Hint($"清理失败：{ex.Message}", HintType.Red)
        End Try
    End Sub

#End Region

#Region "今日人品"

    Private Sub BtnJrrp_Click() Handles BtnJrrp.Click
        LabJrrpResult.Text = JrrpText()
    End Sub

    Private Shared Function JrrpText() As String
        Dim Luck As Integer = RandomInteger(0, 100)
        Dim Fortune As String
        Select Case Luck
            Case 0 To 19
                Fortune = "诸事不宜，还是早点躺平吧……"
            Case 20 To 39
                Fortune = "运气平平，适合苟住发育。"
            Case 40 To 59
                Fortune = "不好不坏，按部就班即可。"
            Case 60 To 79
                Fortune = "运气不错，可以大胆尝试！"
            Case 80 To 94
                Fortune = "好运连连，干什么都顺风顺水！"
            Case Else
                Fortune = "鸿运当头，今天你就是天选之人！"
        End Select
        Return $"今日人品：{Luck} 分，{Fortune}"
    End Function

    Public Shared Sub Jrrp()
        Hint(JrrpText(), HintType.Blue)
    End Sub

#End Region

#Region "回声洞"

    Private Sub BtnRandomCave_Click() Handles BtnRandomCave.Click
        LabCaveResult.Text = GetRandomCave()
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

#End Region

#Region "自定义下载"

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

            Dim Client As New Net.WebClient()
            '保持引用，避免 WebClient 在异步下载完成前被垃圾回收
            Dim ClientRef As Net.WebClient = Client
            AddHandler Client.DownloadProgressChanged, Sub(sender, e)
                Dim Percent As Integer = CInt(e.ProgressPercentage)
                Hint($"正在下载：{Percent}%", HintType.Blue, False)
            End Sub
            AddHandler Client.DownloadFileCompleted, Sub(sender, e)
                Try
                    If e.Error IsNot Nothing Then
                        Hint($"下载失败：{e.Error.Message}", HintType.Red)
                    Else
                        Hint($"下载完成：{FullPath}", HintType.Green)
                    End If
                Finally
                    ClientRef.Dispose()
                End Try
            End Sub
            Client.DownloadFileAsync(New Uri(Url), FullPath)
        Catch ex As Exception
            Logger.Error(ex, "自定义下载出错")
            Hint($"下载失败：{ex.Message}", HintType.Red)
        End Try
    End Sub

#End Region

End Class

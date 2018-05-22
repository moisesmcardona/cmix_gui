﻿Public Class Form1
    Private OutputFileName As String = String.Empty
    Private cmix_version As String = String.Empty
    Private Sub CompressRButton_CheckedChanged(sender As Object, e As EventArgs) Handles CompressRButton.CheckedChanged
        InputFileMessage.Text = My.Resources.CompressInputMessage
        OutputFileMessage.Text = My.Resources.CompressOutputMessage
        BrowseFolder.Enabled = True
        My.Settings.Compress = CompressRButton.Checked
        My.Settings.Save()
    End Sub

    Private Sub ExtractRButton_CheckedChanged(sender As Object, e As EventArgs) Handles ExtractRButton.CheckedChanged
        InputFileMessage.Text = My.Resources.ExtractInputMessage
        OutputFileMessage.Text = My.Resources.ExtractOutputMessage
        BrowseFolder.Enabled = False
        My.Settings.Extract = ExtractRButton.Checked
        My.Settings.Save()
    End Sub

    Private Function CheckIfFileOrFolder(PathToCheck As String) As String
        If My.Computer.FileSystem.FileExists(PathToCheck) Then
            OutputFileMessage.Text = My.Resources.CompressOutputMessage
            OutputFileTxt.Enabled = True
            BrowseButton2.Enabled = True
            Return "File"
        ElseIf My.Computer.FileSystem.DirectoryExists(PathToCheck) Then
            OutputFileMessage.Text = My.Resources.CompressFolderSelectedMessage
            OutputFileTxt.Enabled = False
            BrowseButton2.Enabled = False
            Return "Folder"
        End If
        Return "N/A"
    End Function
    Private Sub BrowseButton1_Click(sender As Object, e As EventArgs) Handles BrowseButton1.Click
        OpenFileDialog1.Title = InputFileTxt.Text
        OpenFileDialog1.Filter = "All files (*.*)|*.*"
        If InputFileTxt.Text IsNot String.Empty Then
            If My.Computer.FileSystem.FileExists(InputFileTxt.Text) Then OpenFileDialog1.FileName = My.Computer.FileSystem.GetName(InputFileTxt.Text) Else OpenFileDialog1.FileName = String.Empty
        End If
        Dim response As DialogResult = OpenFileDialog1.ShowDialog
        If response = DialogResult.OK Then
            InputFileTxt.Text = OpenFileDialog1.FileName
            CheckIfFileOrFolder(InputFileTxt.Text)
        End If
    End Sub

    Private Sub BrowseFolder_Click(sender As Object, e As EventArgs) Handles BrowseFolder.Click
        Dim response As DialogResult = FolderBrowserDialog1.ShowDialog()
        If response = DialogResult.OK Then
            InputFileTxt.Text = FolderBrowserDialog1.SelectedPath
            CheckIfFileOrFolder(InputFileTxt.Text)
        End If
    End Sub

    Private Sub BrowseButton2_Click(sender As Object, e As EventArgs) Handles BrowseButton2.Click
        SaveFileDialog1.Title = InputFileTxt.Text
        If CompressRButton.Checked Then SaveFileDialog1.Filter = "cmix file|*.cmix" Else SaveFileDialog1.Filter = "All files (*.*)|*.*"
        If OutputFileTxt.Text IsNot String.Empty Then
            If My.Computer.FileSystem.FileExists(OutputFileTxt.Text) Then SaveFileDialog1.FileName = My.Computer.FileSystem.GetName(OutputFileTxt.Text) Else SaveFileDialog1.FileName = String.Empty
        End If
        Dim response As DialogResult = SaveFileDialog1.ShowDialog
        If response = DialogResult.OK Then
            OutputFileName = SaveFileDialog1.FileName
            If CompressRButton.Checked Then SetOutputFilename()
        End If
    End Sub

    Private Sub SetOutputFilename()
        If cmixVersionDropdown.SelectedItem = "cmix_v15b" Then
            OutputFileTxt.Text = OutputFileName + "15b"
            cmix_version = "15b"
        End If
    End Sub

    Private Sub cmixVersionDropdown_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmixVersionDropdown.SelectedIndexChanged
        My.Settings.Version = cmixVersionDropdown.SelectedItem
        My.Settings.Save()
        If OutputFileName IsNot String.Empty Then
            SetOutputFilename()
        End If
    End Sub

    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles MyBase.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles MyBase.DragDrop
        InputFileTxt.Text = e.Data.GetData(DataFormats.FileDrop)(0)
        CheckIfFileOrFolder(InputFileTxt.Text)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CompressRButton.Checked = My.Settings.Compress
        ExtractRButton.Checked = My.Settings.Extract
        cmixVersionDropdown.SelectedItem = My.Settings.Version
    End Sub

    Private Sub Run_cmix(Input As String, Output As String, action As String)
        Dim cmixProcessInfo As New ProcessStartInfo
        Dim cmixProcess As Process
        cmixProcessInfo.FileName = My.Settings.Version + ".exe"
        cmixProcessInfo.Arguments = action + " """ & Input & """ """ & Output & """"
        cmixProcessInfo.CreateNoWindow = False
        cmixProcess = Process.Start(cmixProcessInfo)
        cmixProcess.WaitForExit()
    End Sub

    Public Sub ProcessFiles(Folder As String, Action As String)
        For Each File In IO.Directory.GetFiles(Folder)
            Run_cmix(File, File + ".cmix" + cmix_version, Action)
        Next
    End Sub

    Public Sub ProcessSubfolders(Folder As String, Action As String)
        For Each Subfolder In IO.Directory.GetDirectories(Folder)
            ProcessFolder(Subfolder, Action)
        Next
    End Sub

    Public Sub ProcessFolder(Folder As String, Action As String)
        ProcessFiles(Folder, Action)
        ProcessSubfolders(Folder, Action)
    End Sub

    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        If InputFileTxt.Text IsNot String.Empty Then
            Dim ProcessAction As String = String.Empty
            If CompressRButton.Checked Then ProcessAction = "-c" Else ProcessAction = "-d"
            Dim CheckInput As String = CheckIfFileOrFolder(InputFileTxt.Text)
            If CheckInput = "File" Then
                If OutputFileTxt.Text IsNot String.Empty Then
                    Run_cmix(InputFileTxt.Text, OutputFileTxt.Text, ProcessAction)
                End If
            ElseIf CheckInput = "Folder" Then
                ProcessFolder(InputFileTxt.Text, ProcessAction)
            End If
            MessageBox.Show("Finished!")
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://github.com/byronknoll/cmix")
    End Sub

End Class

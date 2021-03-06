﻿Module routines_locales
    'Memory location to store strings that have already been output by locale debugger 
    Friend untranslated_strings As New List(Of String)

    'All the words of the current language
    Dim current_language_array As New List(Of String)

    'Stores the language to load JavaRa in
    Friend language As String

    'Translate the user interface and all menues
    Public Function translate_strings() As Boolean

        'Set the language file path to a variable
        Dim lang_path As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) & "\localizations\lang." & language & ".locale"
        'Check if selected language still exists
        Dim localization_support = IO.File.Exists(lang_path)

        Try
            If localization_support = True Then

                'Clear previous language
                current_language_array.Clear()

                'Load the contents of the current lang file into the memory list
                Dim r As IO.StreamReader
                Dim rule As String = Nothing
                r = New IO.StreamReader(lang_path)
                Do While (r.Peek() > -1)

                    'Set current line to a variable
                    rule = (r.ReadLine.ToString)

                    'Assign item to list
                    If rule.StartsWith("//*") = False Then
                        current_language_array.Add(rule)
                    End If

                Loop
            End If

            'Call for UIControl, the first is for all normal forms, followed by the top menu (unmanaged code), then each tab individually
            TranslateControl(UI)

            'Translate the top menu of UI form.
            UI.btnSettings.Text = get_string("Settings")
            UI.btnAbout.Text = get_string("About JavaRa")

            'Translate the version string
            UI.lblVersion.Text = get_string("version") & " " & Application.ProductVersion.Replace(".0.0", "")

            'Translate multi-line text labels
            UI.Label5.Text = get_string("The removal routine will delete files, folder and registry entries that are known" & Environment.NewLine & get_string("to be associated with the older versions of the Java Runtime Environment"))
            UI.Label12.Text = get_string("Would you like to download and install the latest version of JRE? ") & Environment.NewLine & get_string("Use this interface.")
            UI.lblStep1.Text = get_string("We recommend that you try running the Java Runtime Environment's built-in") & Environment.NewLine & get_string("uninstaller before you continue.")

        Catch ex As Exception
            write_error(ex)
        End Try
        Return localization_support
    End Function

    'Recursively translate a specific UI control
    Public Sub TranslateControl(ByVal Ctrl As Control)
        For Each ChildCtrl As Control In Ctrl.Controls
            If ChildCtrl.Text <> "" Then
                ChildCtrl.Text = get_string(ChildCtrl.Text.Replace("  ", ""))
            End If
            TranslateControl(ChildCtrl)
        Next
    End Sub

    'Enumerate all ToolStripMenuItem controls in a given ToolStripMenu
    Public Sub GetMenues(ByVal Current As ToolStripMenuItem, ByRef menues As List(Of ToolStripMenuItem))
        menues.Add(Current)
        For Each menu As ToolStripMenuItem In Current.DropDownItems
            GetMenues(menu, menues)
        Next
    End Sub

    'Return the translated version of a string, depending on language
    Public Function get_string(ByVal initial_string As String) As String

        'Don't execute if language is English
        If language = "English" Then
            Return initial_string
        End If

        'FIX FOR "Remove JRE" string modification
        If initial_string = "Remove Java Runtime" Then
            initial_string = "Remove JRE"
        End If

        Dim new_string As String

        'Iterate through the current list of strings
        For Each line As String In current_language_array

            'Check if the list contains the initial string and the line break, which denotes the end of the string
            If line.StartsWith(initial_string & "|") Then

                'Remove the initial string from the entry in the list
                new_string = line.Replace(initial_string & "|", "")

                'Return the new string
                Return new_string

                'Do not continue looping once correct translation has been found
                Exit Function

            End If
        Next

        'If the code has reached this point, obviously no translation exists. Log this instance
        If IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) & "\localizations\output_strings.true") Then
            untranslated_strings.Add(initial_string)
        End If


        ' Return the English line.
        Return initial_string
    End Function

End Module

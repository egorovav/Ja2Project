   M S B A M L   `   `   `  ����D  @JsdEditor, Version=2.0.1.1, Culture=neutral, PublicKeyToken=null%clr-namespace:JsdEditor	JsdEditor  5   	   L HCommonWpfControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:CommonWpfControls;assembly=CommonWpfControlsCommonWpfControls 5   	       JsdEditor.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   )localclr-namespace:JsdEditor     5   	   Ocontrols:clr-namespace:CommonWpfControls;assembly=CommonWpfControls     5   	   -   5          � Title$  Jsd Editor v 2.0.5��5   	   $	��600��6$   $	��800��61    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5	   
   -   5           Command$ ApplicationCommands.Open��5	      5        5   
   -   5        $  ApplicationCommands.SaveAs��5      5        5   
   -   5        $ ApplicationCommands.New��5      5        5   
   -   5        $ ApplicationCommands.Save��5      5       � 	Resources 5      0  *CommonWpfControls.BoolToCollapsedConverter   BoolToCollapsedConverter��%   &	          5   
   6,   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5   
   o� .@�5       m� $��_File��6   .@�5!      m� $��	_Open JSD��6    m� Command$ ApplicationCommands.Open��6.   m� 5"      $��_New JSD��6   $ ApplicationCommands.New��6-   m� 5#      $��_Save As...��6   $  ApplicationCommands.SaveAs��60   m� 5$      $
��Save��6   $ ApplicationCommands.Save��6)   �� 5%      m� 5&      -   5        $��Open _Folder��5&      61   m� 5'      -   5        $��	Open S_LF��5'      6.   �� 5(      m� 5)      -   5        $��Open STI File��5)      62   m� 5*      -	   5        $��Open STI Folder��5*      64   5+      m� 5,      $��Convert to HD��6   .@�5-      m� -
   5        $
��File��5-      6)   m� 5.      -   5        $��Folder��5.      6+   5/      m� 50      $��_About��6   6)   51      �� 52   
   $��1��6   .@�53      �� $��JSD File��6   .��54      �  � ColumnDefinitions 55      �� 56      $
��Auto �6+   �� 57      $��3 �6+   �� 58      .+�5:      &   System.Windows.Controls.DataGrid  -   5          Name$ 
dgJsdFiles��5:         AutoGenerateColumns .  	 �� AlternationCount$	 2��5=      
   AlternatingRowBackground

 ����   GridLinesVisibility$ Vertical=�5?      $��1q�5@      ��5;      �� *
JsdFiles+ �� SelectedItem 5A      �� *SelectedJsdFile+   Columns 5B      0  *System.Windows.Controls.DataGridTextColumn  5C      ,  &System.Windows.Controls.DataGridColumn   Header$ 	File Name��6\   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6-   ��  �� Path$ FileName� �� Mode$ OneWay=�6\     5D      $ Structure Number��5E   -      Width5  /System.Windows.Controls.DataGridLengthConverter$ 30 6G     ColumnHeaderStyle   HeaderStyle# [ 5F   -    5D   -   �� $ NumberOfStructures�$ OneWay=�5F   -     5G      $ Image Number��5H   -   $ 30 6C   # [ 5I   -    5G   -   �� $ NumberOfImages�5I   -   4  .System.Windows.Controls.DataGridCheckBoxColumn  5J      $ HD��6[      
IsReadOnly .  61   �� $ IsHighDefenition�6g   5L      �� 5M      $��1��6   $��3��6/   $��Stretch=�69   $��Center=�6U   	   JsdEditor.JsdFileView	  5N      -   5         	 Name$	 jfv��5N   $   $��2��61   � 5O      $��2��5P      
���  ��
����  �$��34��5S      $	��512��5T      $��Bottom=�5U      $��0,0,0,60q�5V       � Value 5W      �� *
Progress+y�5X      �� *IsBusy+ �� 	Converter# [  5Z      5[      �� 5\      $��JSD Structure��6   .��5]      
   JsdEditor.StructureView
  -   5         
 Name$ sv��5]   &   5^      5_      �� 5`   
   $��2��6   .@�5a      ��  �� Orientation$ 
Horizontal=�6   .+�5b      �� $��JSD Files Loaded: ��6   $��Medium"�66   �� 5c      ��6   �� *JsdFilesCount+�� 5d      $��; SLF File Name: ��6   $��Medium"�65   y�5e      �� *SlfFileNameIsNotEmpty+# [  �� 5f      ��6   �� *SlfFileName+�� 5g      $��; Folder Name: ��6   $��Medium"�63   y�5h      �� *FolderNameIsNotEmpty+# [  �� 5i      ��6   �� *
FolderName+5j      5k      5l      5m      
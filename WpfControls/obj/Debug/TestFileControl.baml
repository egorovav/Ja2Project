   M S B A M L   `   `   `  ����  CommonWpfControls'    !CommonWpfControls.TestFileControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        J� CommandBindings  5      )  #System.Windows.Input.CommandBinding  5	   
   -   5           Command$ ApplicationCommands.Open��5	      5      .��5      �� .+�5   
   ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ tbxFolderName��5      $	��400��6-   ��69   �� *
FolderName+�� 5      -   5         ��Name$ btnOpenFolder��5      $��Open Folder��6,   $��ApplicationCommands.Open��6B   5      �� 5   
   $ 
Horizontal=�6   .+�5      �� -   5        $ tbxFileName��5      $	��400��6+   ��67   �� *
FileName+ �� Mode$ TwoWay=��� 5      -   5        $ btnOpenFile��5      $��	Open File��6*   6>   5      �� 5   
   -   5         ��Name$ tbTestResult��5      
��� � �$��5q�6[   ��6*   �� *ResultString+6[   �� 5   
   -   5        $ tbTestErrors��5      
���  ��$��5q�6W   ��6(   �� *ErrorString+6W   �� 5   
   -   5        $ tbTestStatus��5      
����  �$��5q�6Z   ��6*   �� *StatusString+6Z   5      5      
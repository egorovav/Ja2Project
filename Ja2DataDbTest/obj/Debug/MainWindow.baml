   M S B A M L   `   `   `  ����H  DJa2DataDbTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:Ja2DataTest.ViewModelJa2DataTest.ViewModel  5   	   L HCommonWpfControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:CommonWpfControls;assembly=CommonWpfControlsCommonWpfControls 5   	       Ja2DataDbTest.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   2vm#clr-namespace:Ja2DataTest.ViewModel     5   	   Ocontrols:clr-namespace:CommonWpfControls;assembly=CommonWpfControls     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)    � 	Resources 5      ,   &Ja2DataTest.ViewModel.SlfTestViewModel   SlfViewModel��%   &	          5   
   6   5	      .��5
      �� $
��Auto=�6   $
��Auto=�66   .��5   
   �� .@�5      �� $��	Slf Table��6    � DataContext# [  6)   .��5      �� .+�5      '  !CommonWpfControls.TestFileControl  -   5          Name$ tfsSlf��5   /      
FileFilter$ SLF Files | *.SLF��6?      FileName 5      �� *
FileName+ �� Mode$ OneWayToSource=�   
FolderName 5      �� *
FolderName+$ OneWayToSource=�   ResultString 5      �� *ResultString+	   StatusString	 5      �� *StatusString+
   ErrorString
 5      �� *ErrorString+�� 5       �� Orientation$ 
Horizontal=�6!   .+�5      �� $��Data info id��6    �� 5      $��2q�6"   $	��200��6-   ��69   �� *
DataInfoId+5      �� 5      $ 
Horizontal=�6!   .+�5      �� -   5         ��Name$ btnUploadAll��5   !   $��
Upload All��67   ��6L   �� *UploadAllCommand+��5   !   �� �� 5      -   5        $ 	btnUpload��5   !   $��Upload��64   ��6E   �� *UploadCommand+��5   !   �� �� 5      -   5        $ btnDownload��5   !   $��Download��66   ��6I   �� *DownloadCommand+��5   !   �� �� 5      -   5        $ btnDownloadAll��5   !   $��Download All��69   ��6P   �� *DownloadAllCommand+��5    !   �� 5!      &   System.Windows.Controls.DataGrid  5"      -   5          Name$ dgSlf��5"       �� SelectedValue 6.   �� *SelectesSlf+��5#      �� *SlfItemSource+5$      5%      �� 5&      $��	Sti Table��6   .��5'      �� .+�5(        -   5        $ 	dgStciRgb��5(      64     5)      -   5        $ dgStciIndexed��5)      68   5*      5+      �� 5,      $��	Jsd Table��6   .��5-        -	   5        $ dgJsd��5-      6,   5.      5/      50      51      
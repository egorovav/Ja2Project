   M S B A M L   `   `   `  ����D  @MapViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MapViewer	MapViewer  5   	       MapViewer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   )localclr-namespace:MapViewer     5   	   -   5          � Title$  JA 2 Map Viewer��5   	   $	��350��6!   $	��525��6.    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$ ApplicationCommands.Open��5      5
      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      .+�5   
   o� .@�5      m� $
��File��6   .@�5      m� $��_Open��6    m� Command$ ApplicationCommands.Open��6+   m� 5      $��_Save As��6   .@�5      m� -   5        $
��JPEG��5      6-   m� 5      -   5        $	��BMP��5      6,   5      m� 5      $��
Convert To��6   .@�5      m� -   5        $
��JPEG��5      6-   m� 5      -   5        $	��BMP��5      6,   5       5!      m� 5"      -   5        $
��Data��5"      6%   5#      �� 5$   
   -   5         ��Name$ tc��5$      $��1��6!   $��1��6.   .@�5%      �� $	��Map��6   .��5&      �� $
��Auto=�6   .��5'         MapViewer.GlobalMapsView  -	   5          Name$	 gmv��5'   +   5(      5)      �� 5*      -
   5        $��Image��5*      .��5-      �  5.      �� 5/      �� 50      $
��Auto �6(   .+�52      �� -   5         ��Name$
 iMap��52   !   .+�53      #   MapViewer.MapFrameworkElement  -   5        	  Name$	 mv��53   8   z�55   "   V� 56   &   .~�57   *   �� 
 �� ScaleX
 69   �� *
ImageScale+ �� ScaleY 6W   �� *
ImageScale+T� 58   *    T� X 6=   �� *ImageX+ T� Y 6R   �� *ImageY+59   '   5;      5<      5=      5?      �� 5@      $��	Structure��6   .��5A         MapViewer.ShapeView3D  -   5          Name$ svMapStructure��5A   $   5B      5C      �� 5D   
   -   5         ��Name$ spLayers��5D      $��1��6'   $��10q�64   .+�5E      �� $��
Load Image��6    d� 	IsChecked 6,   �� *IsLoadImage+�� 5F      $��Load Structure��6    60   �� *IsLoadStructure+�� 5G      5H      �� 5J   
   $��2��6   $��2��6!   .@�5K      �� ��6   �� *	MapInfo+5L      5M      5N      
   M S B A M L   `   `   `  ����L  HCommonWpfControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullP:clr-namespace:CommonWpfControls;assembly=CommonWpfControlsCommonWpfControls  5      D @JsdEditor, Version=2.0.1.1, Culture=neutral, PublicKeyToken=null%clr-namespace:JsdEditor	JsdEditor 5         JsdEditor.JsdFileView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Ocontrols:clr-namespace:CommonWpfControls;assembly=CommonWpfControls     5      )localclr-namespace:JsdEditor     5      .��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      $��2* �6   �� 5      $��3 �6   �� 5      $��1* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Structure Data��6   $��14(�6*   $��Medium"�68   �� 5   
   $��2��6   $��Auxilarity Data��6$   $��14(�6;   $��Medium"�6I   �� 5   
   $��1��6   $	��Top=�6"    �� Orientation$ 
Horizontal=�67   .+�5      �� $��Add Structure��6   ��6-   �� *AddStructureCommand+��6U   �� �� 5      $��Delete Structure��6   ��60   �� *RemoveStructureCommand+��6[   �� �� 5      $��Copy Structure��6   ��6.   �� *CopyStructureCommand+��6W   �� �� 5       � ToolTip$* $Copy and rotate 90 degrees clockwise��6]   ��6   �� *RotateStructureCommand+��6@   �� .��5      F� 
���   �$��3��6&      ClockwiseRotateArrow#��[  6:   5       �� 5!      $1 +Copy and rotate 90 degrees counterclockwise��6a   ��6   �� *AntiRotateStructureCommand+��6D   �� .��5"      F� 
���   �$��3��6&   #��[  6:    J� RenderTransformOrigin$ 0.5 0.5&�6g   z�5#      u� 5$       u� Matrix$ -1, 0, 0, 1, 0, 0x�6*   5&      5'      5(      �� 5)   
   $��1��6   $��2��6"   $	��Top=�62   $ 
Horizontal=�6G   .+�5*      �� $��Add Aux. Data��6   ��6-   �� *AddAuxDataCommand+��6S   �� �� 5+      $��Delete Aux. Data��6   ��60   �� *RemoveAuxDataCommand+��6Y   �� 5,      &   System.Windows.Controls.DataGrid  5-   
   -   5          Name$ dgStruct��5-      $��2��6%      AutoGenerateColumns .  ��5/      �� *	Structs+ �� SelectedItem 50      �� *SelectedStruct+	   Columns	 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 30 53   %      Header$ #��60   1  +System.Windows.Controls.DataGridBoundColumn   Binding 52   %   ��  �� Path$ StructureNumber�53   0   4  .System.Windows.Controls.DataGridComboBoxColumn  54      -   5        dgcbcMaterial��$ Material��54   @      SelectedValuePath$ Index��56   )      DisplayMemberPath$ DisplayName��6C      SelectedValueBinding 55   )   �� $ Armour�56   C     57      $
 30 58   %   $ Armour��60      
IsReadOnly .   ColumnHeaderStyle   HeaderStyle# [ 5:   %    57   %   �� $ Armour�5:   %     5;      $
 30 5<   %   $ 
Hit Points��60   # [ 5=   %    5;   %   �� $ 	HitPoints�5=   %     5>      $
 30 5?   %   $ Density��60   # [ 5@   %    5>   %   �� $ Density�5@   %   4  .System.Windows.Controls.DataGridTemplateColumn  5A      $ Flags��6)   # [ 68      CellTemplate 5B      �� 5C      .R�5D      ]� .+�5E   "   �� -   5        $��Right=�5E   )     ComboBoxStyle#��[ 5F   )   5H   #   �� 5I   "   $��5,0,5,0q�6C   ��6,   �� *Flags+6C   5J      5K      5M        5N      $
 30 5O   %   $ Wall Orientation��60   # [ 5P   %    5N   %   �� $ WallOrientation�5P   %     5Q      $
 30 5R   %   $ Destruction Partner��60   # [ 5S   %    5Q   %   �� $ DestructionPartner�5S   %     5T      $
 30 5U   %   $ Partner Delta��60   # [ 5V   %    5T   %   �� $ PartnerDelta�5V   %     5W      $
 30 5X   %   $ Z Tile Offset X��60   # [ 5Y   %    5W   %   �� $ ZTileOffsetX�5Y   %     5Z      $
 30 5[   %   $ Z Tile Offset Y��60   # [ 5\   %    5Z   %   �� $ ZTileOffsetY�5\   %     5]      $
 30 5^   %   $ Number of Tiles��60   # [ 5_   %    5]   %   �� $ Tiles.Count� �� Mode$ OneWay=�5_   %   5a      �� 5b   
   $��1��6   $��3��6'   $��3��68   $��Stretch=�6B   $��Center=�6^     5c   
   -   5        $ 	dgAuxData��5c      $��2��5d      $��2��6    $��2��60    .  ��5f      �� *	AuxData+ 5g      �� *SelectedAuxData+ �� SelectedIndex 5h      �� *AuxDataSelectedIndex+	 5i        5j      $
 30 5k   %   $ Number of Tiles��60   # [ 5l   %    5j   %   �� $ NumberOfTiles�5l   %     5m      $
 30 5n   %   $ Tile Location Index��60   # [ 5o   %    5m   %   �� $ TileLocIndex�5o   %     5p      $
 30 5q   %   $ Current Frame��60   # [ 5r   %    5p   %   �� $ CurrentFrame�5r   %     5s      $ Flags��6)   # [ 68    5t      �� 5u      .R�5v      ]� .+�5w   "   �� -   5        $��Right=�5w   )   #��[ 5x   )   5y   )   �� 5z   "   $��5,0,5,0q�6C   ��6,   �� *Flags+6C   5{      5|      5~        5      $
 30 5�   %   $ Wall Orientation��60   # [ 5�   %    5   %   �� $ WallOrientation�5�   %     5�      $
 30 6M   $ Number of Frames��5�   %   # [ 5�   %    5�   %   �� $ NumberOfFrames�5�   %   5�      �� 5�   
   -   5         ��Name$ lbImages��5�      $��4��5�      $��4��6"   ��5�      �� *Images+ 5�      �� *ImageSelectedIndex+��5�      �� 5�      .R�5�      �� ._�5�      �� $��5q�6    ��6+   �� *Bitmap+5�      5�      5�        JsdEditor.ShapePreview3D  5�   
   -   5          Name$ svJsdShape3D��5�      $��3��65   $��3��6B   � 5�   
   -   5         �Name$ puStructFlags��5�      $��Mouse=�5�      $��Slide=�5�       � 	StaysOpen .  .��5�      �� $��1q�6   
�������
�������._�5�      �� .+�5�      �� $��2q�6    $��OPENABLE, CLOSEABLE,��6+   �� 5�      $��2q�6    $��SEARCHABLE are synonyms��6+   &	    CommonWpfControls.FlagEditorView	  5�      -	   5         	 Name$ 	fevStruct��5�   .   5�      5�      5�      � 5�   
   -
   5        $ 
puAuxFlags��5�      $��Mouse=�5�      $��Slide=�5�       .  .��5�      �� $��1q�6   
�������
�������._�5�      �� .+�5�      	  -   5        $ fevAux��5�   .   5�      5�      5�      5�      5�      
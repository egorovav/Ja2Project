   M S B A M L   `   `   `  ����  	JsdEditor    JsdEditor.App   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        �� 	Resources  5         ColumnHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value �� RelativeSource �� Path  BasicControlType  ButtonTemplate �� 
TargetType ��Name �� CornerRadius	 �� 
StartPoint
 �� EndPoint � Color � Offset ��Name �� Triggers P� Property P� Value �� 
TargetName :� RoutedEvent ��Name ��Name W�Name �� 
TargetName �� TargetProperty W� From	 W� To g� Duration �� BeginStoryboardName �� BasedOn  ComboBoxStyle  ComboBoxButtonTemplate  ClockwiseRotateArrow E� IsClosed E� 
StartPoint �� Point  �� Size! �� 
IsLargeArc" �� SweepDirection# �� Point��%�  &	        &	 z     &	 �     '�� u    7  7  &	 �    &	 ,    &	 M    �� 5   
   "  6*   .�5      ��  �@�ToolTip 5      ��  �� *Self+$ Column.Header�5	      �� 5
   
   .�5      ��  � ��$ 12(�61   �� 5       � ��$ 2q�6/   5      �� 5   
   " ��61   .R�5      �� $	 brd��6   $ 5��6"   $��1,1,3,3q�63   
���������5      �� 5      ButtonBrush��$	 0.5, 0&�6?   $
 0.5, 1&�6S   .��5      � $ #AAAAAA��6'   $ 0Q�67   � 5      gsWhite��$ #DDDDDD��68   $	 0.5Q�6H   � 5      $ #444444��6'   $ 1Q�67   5      ._�5      �� $	 lbl��6   $��Center=�6%   $��Center=�6@   #��z��5      #��z��5      #��z��5      5       5      P� 5       � �� . . �5      �� $	 brd��6    � ��$ 2,2,2,2q�6I   �� 5      $	 brd��6    � �� 5       
� 5!      .��5"   "   � $ #888888��6/   $ 1Q�6?   � 5#   "   gsLightGray��$ #CCCCCC��6D   $ 0Q�6T   5$      5&      5'      P� 5(       � {� .  . �5)      �� $	 brd��6    � �� 5*      �� 5+      $	 0.5, 0&�62   $
 0.5, 1&�6F   .��5,   "   � $ #AAAAAA��6/   $ 1Q�6?   � 5-   "   $ #CCCCCC��6/   $	 0.5Q�6?   5.      50      51      :� 52      $ Button.MouseEnter��6   [�53      �� 54      $ bsbMouseEnter��6*   .��55      �� $ sbLight��6)   .��56   "   W� $ LightAnimation��62   $ gsWhite��57   1   $ Offset�58   1   $ 0X�59   1   $ 1X�6:   $ 0:0:0.8D�6A   5:      5;      5=      :� 5>      $ Button.MouseLeave��6   [�5?      �� 5@      $ bsbMouseEnter��6)   5B      5D      �� 5F   
   " ��6   8   .�5G      ��  � ��8  6)   5H      �� 5I   
   " ��6   .�5J      ��  � ��$ 0q�6'   �� 5K       � ��$  ... ��6(   5L      �� 5M   
   " ��69   5O      B� 5P   
   .��5Q      E�  .  $	 2,4&�6*   .��5R      �� $
 2,15&�6   $  10,10��6*   ! . $" 	Clockwise=�6I   �� 5S      $
# 2,17&�6   �� 5T      $
# 4,15&�6   �� 5U      $
# 2,15&�6   5V      5W      5X      5Y      
   M S B A M L   `   `   `  ����   	JsdEditor    JsdEditor.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       �� 	Resources     ColumnHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value �� RelativeSource �� Path  BasicControlType  ButtonTemplate �� 
TargetType ��Name �� CornerRadius	 �� 
StartPoint
 �� EndPoint � Color � Offset ��Name �� Triggers P� Property P� Value �� 
TargetName :� RoutedEvent ��Name ��Name W�Name �� 
TargetName �� TargetProperty W� From	 W� To g� Duration �� BeginStoryboardName �� BasedOn  ComboBoxStyle  ComboBoxButtonTemplate  ClockwiseRotateArrow E� IsClosed E� 
StartPoint �� Point  �� Size! �� 
IsLargeArc" �� SweepDirection# �� Point��%�  &	        &	 Q     &	 �     '�� �    7  7  &	 �    &	     &	     �� "  .���  �@�ToolTip ��  �� *Self+$ Column.Header��� .���  � ��$ 12(���  � ��$ 2q��� " ��.R��� $	 brd��$ 5��$��1,1,3,3q�
����������� ButtonBrush��$	 0.5, 0&�$
 0.5, 1&�.��� $ #AAAAAA��$ 0Q�� gsWhite��$ #DDDDDD��$	 0.5Q�� $ #444444��$ 1Q�._��� $	 lbl��$��Center=�$��Center=�#��z��#��z��#��z�� P�  � �� . . ��� $	 brd�� � ��$ 2,2,2,2q��� $	 brd�� � �� 
� .��� $ #888888��$ 1Q�� gsLightGray��$ #CCCCCC��$ 0Q�P�  � {� .  . ��� $	 brd�� � �� �� $	 0.5, 0&�$
 0.5, 1&�.��� $ #AAAAAA��$ 1Q�� $ #CCCCCC��$	 0.5Q�:� $ Button.MouseEnter��[��� $ bsbMouseEnter��.���� $ sbLight��.��W� $ LightAnimation��$ gsWhite��$ Offset�$ 0X�$ 1X�$ 0:0:0.8D�:� $ Button.MouseLeave��[��� $ bsbMouseEnter���� " ��8   .���  � ��8  �� " ��.���  � ��$ 0q���  � ��$  ... ���� " ��B� .��E�  .  $	 2,4&�.���� $
 2,15&�$  10,10��! . $" 	Clockwise=��� $
# 2,17&��� $
# 4,15&��� $
# 2,15&�
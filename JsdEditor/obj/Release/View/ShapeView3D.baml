   M S B A M L   `   `   `  ���� I  E3DTools, Version=1.0.2614.20437, Culture=neutral, PublicKeyToken=null4'clr-namespace:_3DTools;assembly=3DTools_3DTools   	JsdEditor   JsdEditor.ShapeView3D   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9tools'clr-namespace:_3DTools;assembly=3DTools     -     � 	Resources     TileMaterial u� Brush �� Color �� SpecularPower �� Brush  AltTileMaterial  BackTileMaterial  SelectedTileMaterial  AltSelectedTileMaterial  SelectedBackTileMaterial  LandMaterial  LandBackMaterial��%�  &	        &	 M     &	 �     &	 �     &	     &	 R    &	 p    &	 �    �� .��u�  �� $ 	#DD00FF00���� $ 24Q� �� $ 	#FF00FF00���� .��u�  �� $ 	#DD88FF88���� $ 24Q� �� $ 	#FF88FF88��u�  �� $ 	#DD00FF00���� .��u�  �� $ 	#DD0000FF���� $ 24Q� �� $ 	#FF0000FF���� .��u�  �� $ 	#DD8888FF���� $ 24Q� �� $ 	#FF8888FF��u�  �� $ 	#DD000044��u�  �� $ 	#EE660000��u�  �� $ 	#88FF8888��.���� $��1��$��1��
�������$��1q�$��5q�
�������._��  � RowDefinitions �� $
��Auto ��� �� $
��Auto � � ColumnDefinitions �� $
��Auto ��� �� $
��Auto �.+��� $��1��$��1��
�������$��1q�$��5q�
�������._�!   _3DTools.TrackballDecorator  "   _3DTools.Viewport3DDecorator   Content. '�  '� Camera =� 	 � Position	 �� *CameraPosition+
 � LookDirection
 �� *LookDirection+.x�g� 2�!   _3DTools.ScreenSpaceLines3D     	Thickness$ 1Q�   Color$
 Gray��   Points �   �����  $ 1Q�$
 Gray�� �   �����  $ 1Q�$
 Gray�� �   ����� g� Content h� -   
mgRoot��.��t�  �� Color$ White�� t� 	Direction$ 
-2, -4, -15�h� -   mgShape�� j� 	Transform Y� .���  �� Rotation �� -   anrX�� �� Axis$ 1 0 05���  �� -   anrY��$ 0 1 05���  �� -   anrZ��$ 0 0 15��� $��2��$��1��.+���  �� Orientation$ 
Horizontal=�.+��� $��0,5,0,0q�$��Show selected tile only�� d� 	IsChecked �� *ShowSelectedTileOnly+ �� Mode$ TwoWay=��� -   $��5,5,5,0q�$��Transparency���� $��Filled Position Count:��
��������� 
����������� *FilledPositionCount+�� $��Rotate��]� .+��� $
��Left=�$	��X: ����  � Minimum$ 0Q� � Maximum$	 360Q� � Value ��  �� ElementName$
 anrX�� �� Path$ Angle�]� .+��� $
��Left=�$	��Y: ���� $ 0Q�$	 360Q� �� $
 anrY��$ Angle�]� .+��� $
��Left=�$	��Z: ���� $ 0Q�$	 360Q� �� $
 anrZ��$ Angle��� $��1�� �� Orientation$ Vertical=�$ 0Q�$	 100Q� �� *CameraPositionY+$ TwoWay=��� $��1��$ 0Q�$	 100Q� �� *CameraPositionX+$ TwoWay=��� $��2��$��1��$ Vertical=�$ 0Q�$	 100Q� �� *CameraPositionZ+$ TwoWay=�
   M S B A M L   `   `   `  ����@  <Style, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null[2clr-namespace:JournalManagmentStudio.Project.Style$JournalManagmentStudio.Project.Style  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TabItem.Foreground  �� Color  TabItem.Default.Background �� Opacity  TabItem.Default.Foreground  TabItem.Selected.Background  TabItem.Selected.Foreground   TabItem.MouseOver.Background  TabItem.Selected.Border �� 
TargetType �� Property �� Value �� 
TargetType �Name J� SnapsToDevicePixels ��Name	 ��Name
 �� RelativeSource �� AncestorType �� Triggers ^� 
Conditions �� Binding �� Value �� 
TargetName-   'System.Windows.Input.KeyboardNavigation    TabNavigation � ColumnDefinitions ��Name � RowDefinitions ��Name ��Name K� IsItemsHost    TabIndex K� ZIndex    DirectionalNavigation P� Property P� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      <local2clr-namespace:JournalManagmentStudio.Project.Style  5      %H  &	        &	      &	 R     &	 q     &	 �     &	 �     &	 �     '��     '�� �	    �� 5      $  White��61   �� 5      $  	#02FFFFFF��69   $
 0.01Q�6K   �� 5      $  White��69   �� 5	      $  	#FF2D2D30��6:   �� 5
      $  White��6:   �� 5      $  White��6;   $	 0.2Q�6I   �� 5      $  #ACACAC��66   �� 5      " ��6   .�5   
   ��  � ��$ 0q�6#   �� 5   
    � ��$ 6,2,6,2q�6$   �� 5   
    � ��$ 80��6%   �� 5   
    � ��$ 24��6#   �� 5   
    � ��$ Stretch=�67   �� 5   
    � ��$ Stretch=�65   �� 5   
    � �� 5      �� 5      " ��6"   .R�5      � $ templateRoot��6    . .+�5      �� $ 
mainBorder��6!   #��z��5   !   $��0q�5   !   $��0q�5   !   ._�5      �� $ innerBorder��6%   $��0q�5   %   $��0q�5    %   5"      �� 5#      $	 contentPresenter��6+   $��Header��5$   +   |�.  #��z��5'   +   ��. # z 5)   +   ��5&   +   �� *HorizontalContentAlignment+
 �� " ����5*   +   �� *VerticalContentAlignment+
 �� " ��5,       5-      ^� 5.       5/      �� 50   "    6,   �� *IsMouseOver+
 �� *Self+ true6r   �� 51   "    6,   �� *
IsSelected+
 �� *Self+	 false6q   �� 52   "    6,   �� *TabStripPlacement+
 �� " �� Top6�   .0�54      �� $ innerBorder��6;    � ��
 �����6T   �� 55      $ innerBorder��68    �@J�Opacity$	 0.3Q�6Q   �� 56      $ contentPresenter��6G   # �  6e    � ��57      ^� 58       59      �� 5:   "    6,   �� *	IsEnabled+
 �� *Self+	 false6p   �� 5;   "    6,   �� *TabStripPlacement+
 �� " �� Top6�   5>      ^� 5?       5@      �� 5A   "    6,   �� *	IsEnabled+
 �� *Self+ true6p   �� 5B   "    6,   �� *TabStripPlacement+
 �� " �� Top6�   5E      ^� 5F       5G      �� 5H   "    6,   �� *
IsSelected+
 �� *Self+	 false6q   �� 5I   "    6,   �� *TabStripPlacement+
 �� " �� Top6�   .0�5K      �� $ contentPresenter��6G   # �  6e    � ���� 5L      $ 
mainBorder��6;   # �  6S    � ��5M      ^� 5N       5O      �� 5P   "    6,   �� *
IsSelected+
 �� *Self+ true6q   �� 5Q   "    6,   �� *TabStripPlacement+
 �� " �� Top6�   .0�5S      �� $ contentPresenter��6G   # �  6e    � ���� 5T      $ 
mainBorder��6;   # �  6S    � ��5U      5W      5Y      5Z      �� 5\      " ��6   .�5]   
   ��  � ��$ 2q�6$   �� 5^   
    � ��$ Center=�67   �� 5_   
    � ��$ Center=�65   �� 5`   
    � ��
 ���� 6'   �� 5a   
    � ��
 ���� 6(   �� 5b   
    � ��$ 0q�6,   �� 5c   
    � �� 5d      �� 5e      " ��6"   .R�5f      � $ templateRoot��6   }�.  . $ Local=�6`    5g      �� 5h      $ ColumnDefinition0��6/   �� 5i      $ ColumnDefinition1��6/   $��0 �6J    5k      �� 5l      $ RowDefinition0��6,   $
��Auto �6D   �� 5m      $ RowDefinition1��6,   $��* �6D   .+�5o      �� $ headerPanel��6#   
������ $��0��5q   #    . $��0q�5s   #   $��0��5t   #   $ 1��5u   #   $ 1��5v   #   �� 5x      $ contentPanel��6!   #��z��5y   !   #��z��5z   !   #��z��5{   !   $��0��5|   !   $ 	Contained=�5}   !   $��1��5~   !   $ 2��6.   $ Local=�5   !   ._�5�      �� $	 PART_SelectedContentHost��6/   $��SelectedContent��5�   /   #��z��5�   /   # z 5�   /   5�      5�       5�      P� 5�       �@��TabStripPlacement$ Bottom=�6?   . �5�      �� $ headerPanel��69    � ��$ 1��6R   �� 5�      $ contentPanel��69    � ��$ 0��6S   �� 5�      $ RowDefinition0��67    � ��$ * �6S   �� 5�      $ RowDefinition1��67    � ��$
 Auto �6S   �� 5�      $ headerPanel��67    � ��$ 2,0,2,2q�6P   5�      P� 5�       �@��TabStripPlacement$
 Left=�6?   . �5�      �� $ headerPanel��69    � ��$ 0��6R   �� 5�      $ contentPanel��69    � ��$ 0��6S   �� 5�      $ headerPanel��6<    � ��$ 0��6U   �� 5�      $ contentPanel��6<    � ��$ 1��6V   �� 5�      $ ColumnDefinition0��66    � ��$
 Auto �6U   �� 5�      $ ColumnDefinition1��66    � ��$ * �6U   �� 5�      $ RowDefinition0��67    � ��$ * �6S   �� 5�      $ RowDefinition1��67    � ��$ 0 �6S   �� 5�      $ headerPanel��67    � ��$ 2,2,0,2q�6P   5�      P� 5�       �@��TabStripPlacement$ Right=�6?   . �5�      �� $ headerPanel��69    � ��$ 0��6R   �� 5�      $ contentPanel��69    � ��$ 0��6S   �� 5�      $ headerPanel��6<    � ��$ 1��6U   �� 5�      $ contentPanel��6<    � ��$ 0��6V   �� 5�      $ ColumnDefinition0��66    � ��$ * �6U   �� 5�      $ ColumnDefinition1��66    � ��$
 Auto �6U   �� 5�      $ RowDefinition0��67    � ��$ * �6S   �� 5�      $ RowDefinition1��67    � ��$ 0 �6S   �� 5�      $ headerPanel��67    � ��$ 0,2,2,2q�6P   5�      P� 5�       � {� .  . �5�      �� $ templateRoot��6G   # � ��6a    � ��5�      5�      5�      5�      5�      
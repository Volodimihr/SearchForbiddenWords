   M S B A M L   `   `   `  ����^  ZExam_SearchForbiddenWords_Karvatyuk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:Exam_SearchForbiddenWords_Karvatyuk#Exam_SearchForbiddenWords_Karvatyuk  5   	   4    .Exam_SearchForbiddenWords_Karvatyuk.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Clocal1clr-namespace:Exam_SearchForbiddenWords_Karvatyuk     5   	   -   5          �Name$
  main��5
   	   $��16(�5   	    � Title$ Forbidden words Searcher��5	   	   $	��160��5   	   $	��800��6   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      �� 5      $
��auto �6#   .+�5      � $��0��6    5      �� 5      �� 5      .+�5      � $��0��6    5      �� 5      $
��auto �6+   �� 5      $��* �6+   �� 5      $
��auto �6+   �� 5       $
��auto �6+   .+�5"      �� $��0��6    $��5q�60   $��Choose Searching Folder: ��6;    �� TextAlignment$ Center=�6\   $��Center=�6s   6�   �� 5#      -   5         ��Name$ 
tbkSFolder��5#   ;   $��1��6    $��5q�60   $�� ... ��6O   $
 Left=�6\   $��Center=�6q   6�   �� 5$      -   5        $��2��5$      $��5q�6-   $�� ... ��68   6H   �� 5%      -   5         ��Name$ cbExt��5%   :   $��3��6   $��5q�6/    �� SelectedIndex$ 0��6I   $��64��6[   5&      � 5'      $��1��6    5(      �� 5)      $
��auto �6+   �� 5*      $��* �6+   �� 5+      $
��auto �6+   �� 5,      $
��auto �6+   .+�5.      �� $��0��6    $��5q�60   $*��$Choose listfile of forbidden words: ��6;   $ Center=�6g   $��Center=�6~   6�   �� 5/      -   5        $ tbkFileList��5/   ;   $��1��6    $��5q�60   $�� ... ��6P   $
 Left=�6]   $��Center=�6r   6�   �� 50      -   5        $��2��50      $��5q�6-   $�� ... ��68   6H   �� 51      -   5         ��Name$ btStart��51      $��3��6.   $��5q�6>   $��64��6I   $�� Start ��6T   $��18(�6f   $
��Bold"�6t   6�   52      53      54      � 55   
   $��1��6    56      �� 57      $��* �6#   .+�59      � -   5        	 �Name$	 pbSearchProgress��59      $��0��64   
 � SmallChange$

 0.01Q�6}    � LargeChange$	 0.1Q�6�    � Value 6D   �� *P+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5:      -	   5        $ tbkProgress��5:   A   $��0��6   $��5q�6(   $��16(�63   $ Center=�6�   $��Center=�6�   ��6V   �� *Sp+$ PropertyChanged=�6�   �� 5;      $��0��6   $��5q�6(   $��10(�63   $��Search Progress��6A   $ Center=�6X   $��Bottom=�6o   $
��Wrap=�6�   6�   �� 5<      -
   5        $��0��5<      $��5q�6%   $��Right=�60   $
��Info��5=      $��32��6$   $��64��60   $��Center=�6;   y�5>      �� *	Visible+$ PropertyChanged=�{�5?      �� *	Enabled+$ PropertyChanged=�5@      5A      5B      5C      
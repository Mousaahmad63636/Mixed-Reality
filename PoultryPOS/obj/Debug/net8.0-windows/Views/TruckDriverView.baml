   M S B A M L   `   `   `  ����E  APoultryPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:PoultryPOS.ConvertersPoultryPOS.Converters  5      &     PoultryPOS.Views.TruckDriverView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      :
converters#clr-namespace:PoultryPOS.Converters     5        N� Title$)  #الشاحنات والسائقين��5      $��RightToLeft=�5       � 	Resources 5       �� 
TargetType �� Property �� Value��%}   '��       �� 5	   
   " ��6   .�5
      ��  � ��$ Right=�6;   �� 5       � ��$ Center=�69   5      5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��20 �6#   �� 5      $��* �6#   .+�5      �� $��0��6   
����������#DDD$��1q�6K    �� Padding$ 20q�6_   ._�5      �� .+�5      �� $!��إدارة الشاحنات��6    $��20(�66   $
��Bold"�6D   $��0,0,0,20q�6V   $��Center=�6h   � 5       $��0,0,0,20q�6    5!      �� 5"      $��2* �6/   �� 5#      $��10 �6/   �� 5$      $��* �6/   �� 5%      $��* �6/   .+�5(      �� $��0��6%   .+�5)      �� $��اسم الشاحنة:��6(   $
��Bold"�6<   $��0,0,0,5q�6N   �� 5*      -   5        $��txtTruckName��5*   &   $��30��6:   $��0,0,0,10q�6F   �� 5,      $+��%رقم اللوحة (اختياري):��6(   $
��Bold"�6E   $��0,0,0,5q�6W   �� 5-      -   5        $��txtPlateNumber��5-   &   $��30��6<   $��0,0,0,10q�6H   � 5/       50   "   �� 51   &   $��* �67   �� 52   &   $��10 �67   �� 53   &   $��* �67   .+�56   "   �� $��0��6-   .+�57   &   �� $1��+الحمولة الحالية (أقفاص):��60   $
��Bold"�6P   $��0,0,0,5q�6b   �� 58   &   -   5        $��txtCurrentLoad��58   .   $��30��6D   $��0��6P   59   #   �� 5;   "   $��2��6-   .+�5<   &   �� $%��الوزن الصافي (كغ):��60   $
��Bold"�6J   $��0,0,0,5q�6\   �� 5=   &   -   5        $��txtNetWeight��5=   .   $��30��6B   $��0��6N   5>   #   5?      5@      �� 5B      $��2��6%   $��10,0,0,0q�65   .+�5C      �� -   5        $��btnAddTruck��5C   %   $��
إضافة��68   $��30��6H   ���#27AE60
�������$
��Bold"�5D   8   6J   �� 5E      -   5        $��btnUpdateTruck��5E   %   $��
تحديث��6;   $��30��6K   $��0,5,0,0q�6W   ���#3498DB
�������$
��Bold"�5F   8   6J   �� 5G      -   5        $��btnDeleteTruck��5G   %   $��حذف��6;   $��30��6I   $��0,5,0,0q�6U   ���#E74C3C
�������$
��Bold"�5H   8   6J   �� 5I      -   5        $��btnClearTruck��5I   %   $��مسح��6:   $��30��6H   $��0,5,0,0q�6T   ���#95A5A6
�������$
��Bold"�5J   8   6J   5K      5L      &   System.Windows.Controls.DataGrid  5N      -	   5        $��dgTrucks��5N      $	��350��6/      AutoGenerateColumns$ False��6<   	   HeadersVisibility$	 Column=�5O      
   GridLinesVisibility$
 
Horizontal=�6:      AlternatingRowBackground �#F8F9FA$��LeftToRight=�5P   o      Columns 5Q      0  *System.Windows.Controls.DataGridTextColumn  5R      ,  &System.Windows.Controls.DataGridColumn   Header$ 
الاسم��61      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6Y   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6@   �� *Name+6Y     5S      $ عدد الاقفاص��61   $ 60 6f    6F   �� *CurrentLoad+6f     5T      $ وزن البضاعة ��61   $ 80 6v    6G   �� *	NetWeight+ �� StringFormat$ F2��6v     5U      $ اللوحة��61   $	 100 6a    6A   �� *PlateNumber+6a   5W      5X      5Y      �� 5[      $��2��6   
����������#DDD$��1q�6K   $ 20q�6_   ._�5\      �� .+�5]      �� $!��إدارة السائقين��6    $��20(�66   $
��Bold"�6D   $��0,0,0,20q�6V   $��Center=�6h   � 5_      $��0,0,0,20q�6    5`      �� 5a      $��2* �6/   �� 5b      $��10 �6/   �� 5c      $��* �6/   .+�5f      �� $��0��6%   .+�5g      �� $��اسم السائق:��6(   $
��Bold"�6;   $��0,0,0,5q�6M   �� 5h      -
   5        $��txtDriverName��5h   &   $��30��6;   $��0,0,0,15q�6G   �� 5j      $+��%رقم الهاتف (اختياري):��6(   $
��Bold"�6E   $��0,0,0,5q�6W   �� 5k      -   5        $��txtDriverPhone��5k   &   $��30��6<   5l      �� 5n      $��2��6%   $��10,0,0,0q�65   .+�5o      �� -   5        $��btnAddDriver��5o   %   $��
إضافة��69   $��30��6I   ���#27AE60
�������$
��Bold"�5p   8   6J   �� 5q      -   5        $��btnUpdateDriver��5q   %   $��
تحديث��6<   $��30��6L   $��0,5,0,0q�6X   ���#3498DB
�������$
��Bold"�5r   8   6J   �� 5s      -   5        $��btnDeleteDriver��5s   %   $��حذف��6<   $��30��6J   $��0,5,0,0q�6V   ���#E74C3C
�������$
��Bold"�5t   8   6J   �� 5u      -   5        $��btnClearDriver��5u   %   $��مسح��6;   $��30��6I   $��0,5,0,0q�6U   ���#95A5A6
�������$
��Bold"�5v   8   6J   5w      5x        5z      -   5        $��	dgDrivers��5z      $	��350��60   $ False��6=   $	 Column=�5{      $
 
Horizontal=�6:    �#F8F9FA$��LeftToRight=�5|   p    5}        5~      $ 
الاسم��61   $ * 6Y    6@   �� *Name+6Y     5      $ الهاتف��61   $	 120 6[    6A   �� *Phone+6[   5�      5�      5�      5�      �� 5�   
   $��1��6   
����������#DDD$��1q�6D   $ 15q�6X   $��0,20,0,0q�6e   ._�5�      �� .+�5�      ��  �� Orientation$ 
Horizontal=�6   $��Center=�66   $��0,0,0,10q�6S   .+�5�      �� $:��4📊 سجل انحراف أوزان الشاحنات��6    $��16(�6D   $
��Bold"�6R   $��Center=�6d   �� 5�      -   5        $��btnViewVariance��5�      $(��"عرض التقرير الكامل��64   $��25��6Q   $	��120��6]   ���#8E44AD
�������$
��Bold"�5�   0   $��20,0,0,0q�6B   6T   5�        5�      -   5        $��dgVarianceHistory��5�      $	��150��64   $ False��6A   $	 Column=�5�      $
 
Horizontal=�66    �#F8F9FA   
IsReadOnly$
 True��5�   >   $��LeftToRight=�6P    5�        5�      $ الشاحنة��6-   $	 100 6\    6>   �� *	TruckName+6\     5�      $ تاريخ التحميل��6-   $	 120 6h    6D   �� *LoadDateDisplay+6h     5�      $ تاريخ الإكمال��6-   $	 120 6n    6D   �� *CompletionDateDisplay+6n     5�      $ الوزن الأولي��6-   $ 80 6v    6C   �� *InitialWeight+$ F2��6v     5�      $ الوزن النهائي��6-   $ 80 6u    6D   �� *FinalWeight+$ F2��6u     5�      $ الانحراف��6-   $ 80 6c    6?   �� *VarianceDisplay+   	CellStyle 5�      �� 5�   "   *  $System.Windows.Controls.DataGridCell"  6(    �� Triggers 5�   &   �� 5�   *    �� Binding 66   �� *WeightVariance+-   'PoultryPOS.Converters.PositiveConverter   Instance �� 	Converter# Z  �� Value True6�   .a�5�   .   ��  � ��
 �  ��6K   �� 5�   .    � ��$
 Bold"�6K   5�   +   �� 5�   *    66   �� *WeightVariance+# Z 	 False6�   .a�5�   .   ��  � ��
 � � �6K   �� 5�   .    � ��$
 Bold"�6K   5�   +   5�   #   5�        5�      $ الحالة��6-   $ 60 6_    6=   �� *StatusDisplay+6_   5�      �� 5�      -   5        $��lblVarianceSummary��5�      $E��?آخر 10 جلسات | إجمالي الانحراف: 0.00 كغ��66   $��12(�5�      $
��Bold"�6*   $��Center=�6<   $��0,5,0,0q�6Y   5�      5�      5�      5�      
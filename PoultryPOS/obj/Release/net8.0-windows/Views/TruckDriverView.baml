   M S B A M L   `   `   `  ���� E  APoultryPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:PoultryPOS.ConvertersPoultryPOS.Converters  &     PoultryPOS.Views.TruckDriverView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     :
converters#clr-namespace:PoultryPOS.Converters       N� Title$)  #الشاحنات والسائقين��$��RightToLeft=� � 	Resources  �� 
TargetType �� Property �� Value��%J   '��       �� " ��.���  � ��$ Right=���  � ��$ Center=�.��� $��20q� � RowDefinitions �� $��* ��� $
��Auto �.+�� $��0�� � ColumnDefinitions �� $��* ��� $��20 ��� $��* �.+��� $��0��
����������#DDD$��1q� �� Padding$ 20q�._��� .+��� $!��إدارة الشاحنات��$��20(�$
��Bold"�$��0,0,0,20q�$��Center=�� $��0,0,0,20q� �� $��2* ��� $��10 ��� $��* ��� $��* �.+��� $��0��.+��� $��اسم الشاحنة:��$
��Bold"�$��0,0,0,5q��� -   $��txtTruckName��$��30��$��0,0,0,10q��� $+��%رقم اللوحة (اختياري):��$
��Bold"�$��0,0,0,5q��� -   $��txtPlateNumber��$��30��$��0,0,0,10q��  �� $��* ��� $��10 ��� $��* �.+��� $��0��.+��� $1��+الحمولة الحالية (أقفاص):��$
��Bold"�$��0,0,0,5q��� -   $��txtCurrentLoad��$��30��$��0���� $��2��.+��� $%��الوزن الصافي (كغ):��$
��Bold"�$��0,0,0,5q��� -   $��txtNetWeight��$��30��$��0���� $��2��$��10,0,0,0q�.+��� -   $��btnAddTruck��$��
إضافة��$��30�����#27AE60
�������$
��Bold"��� -   $��btnUpdateTruck��$��
تحديث��$��30��$��0,5,0,0q����#3498DB
�������$
��Bold"��� -   $��btnDeleteTruck��$��حذف��$��30��$��0,5,0,0q����#E74C3C
�������$
��Bold"��� -   $��btnClearTruck��$��مسح��$��30��$��0,5,0,0q����#95A5A6
�������$
��Bold"�&   System.Windows.Controls.DataGrid  -	   $��dgTrucks��$	��350��   AutoGenerateColumns$ False��	   HeadersVisibility$	 Column=�
   GridLinesVisibility$
 
Horizontal=�   AlternatingRowBackground �#F8F9FA$��LeftToRight=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 
الاسم��   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+  $ عدد الاقفاص��$ 60  �� *CurrentLoad+  $ وزن البضاعة ��$ 80  �� *	NetWeight+ �� StringFormat$ F2��  $ اللوحة��$	 100  �� *PlateNumber+�� $��2��
����������#DDD$��1q�$ 20q�._��� .+��� $!��إدارة السائقين��$��20(�$
��Bold"�$��0,0,0,20q�$��Center=�� $��0,0,0,20q� �� $��2* ��� $��10 ��� $��* �.+��� $��0��.+��� $��اسم السائق:��$
��Bold"�$��0,0,0,5q��� -
   $��txtDriverName��$��30��$��0,0,0,15q��� $+��%رقم الهاتف (اختياري):��$
��Bold"�$��0,0,0,5q��� -   $��txtDriverPhone��$��30���� $��2��$��10,0,0,0q�.+��� -   $��btnAddDriver��$��
إضافة��$��30�����#27AE60
�������$
��Bold"��� -   $��btnUpdateDriver��$��
تحديث��$��30��$��0,5,0,0q����#3498DB
�������$
��Bold"��� -   $��btnDeleteDriver��$��حذف��$��30��$��0,5,0,0q����#E74C3C
�������$
��Bold"��� -   $��btnClearDriver��$��مسح��$��30��$��0,5,0,0q����#95A5A6
�������$
��Bold"�  -   $��	dgDrivers��$	��350��$ False��$	 Column=�$
 
Horizontal=� �#F8F9FA$��LeftToRight=�   $ 
الاسم��$ *  �� *Name+  $ الهاتف��$	 120  �� *Phone+�� $��1��
����������#DDD$��1q�$ 15q�$��0,20,0,0q�._��� .+���  �� Orientation$ 
Horizontal=�$��Center=�$��0,0,0,10q�.+��� $:��4📊 سجل انحراف أوزان الشاحنات��$��16(�$
��Bold"�$��Center=��� -   $��btnViewVariance��$(��"عرض التقرير الكامل��$��25��$	��120�����#8E44AD
�������$
��Bold"�$��20,0,0,0q�  -   $��dgVarianceHistory��$	��150��$ False��$	 Column=�$
 
Horizontal=� �#F8F9FA   
IsReadOnly$
 True��$��LeftToRight=�   $ الشاحنة��$	 100  �� *	TruckName+  $ تاريخ التحميل��$	 120  �� *LoadDateDisplay+  $ تاريخ الإكمال��$	 120  �� *CompletionDateDisplay+  $ الوزن الأولي��$ 80  �� *InitialWeight+$ F2��  $ الوزن النهائي��$ 80  �� *FinalWeight+$ F2��  $ الانحراف��$ 80  �� *VarianceDisplay+   	CellStyle �� *  $System.Windows.Controls.DataGridCell"   �� Triggers ��  �� Binding �� *WeightVariance+-   'PoultryPOS.Converters.PositiveConverter   Instance �� 	Converter# Z  �� Value True.a���  � ��
 �  ����  � ��$
 Bold"���  �� *WeightVariance+# Z 	 False.a���  � ��
 � � ���  � ��$
 Bold"�  $ الحالة��$ 60  �� *StatusDisplay+�� -   $��lblVarianceSummary��$E��?آخر 10 جلسات | إجمالي الانحراف: 0.00 كغ��$��12(�$
��Bold"�$��Center=�$��0,5,0,0q�
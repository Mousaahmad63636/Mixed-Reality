   M S B A M L   `   `   `  ���� E  APoultryPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:PoultryPOS.ConvertersPoultryPOS.Converters  )    #PoultryPOS.Views.VarianceReportView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     :
converters#clr-namespace:PoultryPOS.Converters       N� Title$,  &تقرير انحراف الأوزان��$��RightToLeft=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0�����#2C3E50 �� Padding$ 20q�$��0,0,0,20q�._��� $>��8📊 تقرير انحراف أوزان الشاحنات��$��24(�$
��Bold"�
�������$��Center=��� $��1��
����������#DDD$��1q�$ 20q�$��0,0,0,20q�._��  � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��0,0,20,0q�.+��� $��من تاريخ:��$
��Bold"�$��0,0,0,5q�(  "System.Windows.Controls.DatePicker  -   $��
dpFromDate��$	��120���� $��1��$��0,0,20,0q�.+��� $��إلى تاريخ:��$
��Bold"�$��0,0,0,5q�  -   $��dpToDate��$	��120���� $��2��$��0,0,20,0q�.+��� $��الشاحنة:��$
��Bold"�$��0,0,0,5q��� -   $��cmbTruckFilter��$	��150���� $��3��$��0,0,20,0q�$��Bottom=�.+��� -   $��btnToday��$��
اليوم��$��60��$��25�����#1ABC9C
�������$
��Bold"��� $��5��$��Bottom=�.+��� -   $��btnClearFilter��$��مسح المرشحات��$	��100��$��25�����#95A5A6
�������$
��Bold"��� $��2��
����������#DDD$��1q�$ 20q�._��� .+��� $7��1📋 جلسات التحميل والانحراف��$��18(�$
��Bold"�$��0,0,0,15q�$��Center=�&   System.Windows.Controls.DataGrid  -   $��dgVarianceReport��$	��400��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   GridLinesVisibility$ 
Horizontal=�   AlternatingRowBackground �#F8F9FA   
IsReadOnly$
 True��$��LeftToRight=�	   Columns	 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn
   Header$
 الشاحنة��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 120 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	TruckName+  $
 تاريخ التحميل��$	 120  �� *LoadDateDisplay+  $
 تاريخ الإكمال��$	 120  �� *CompletionDateDisplay+  $#
 الأقفاص الأولية��$	 100  �� *InitialCages+  $$
 الوزن الأولي (كغ)��$	 120  �� *InitialWeight+ �� StringFormat$ F2��  $&
  الوزن النهائي (كغ)��$	 120  �� *FinalWeight+$ F2��  $
 الانحراف (كغ)��$	 100  �� *VarianceDisplay+   	CellStyle ��  �� 
TargetType*  $System.Windows.Controls.DataGridCell"   �� Triggers ��  �� Binding �� *WeightVariance+-   'PoultryPOS.Converters.PositiveConverter   Instance �� 	Converter# Z  �� Value True.a���  �� Property � �� �� Value
 �  ����  � ��$
 Bold"���  �� *WeightVariance+# Z 	 False.a���  � ��
 � � ���  � ��$
 Bold"�  $
 النسبة %��$ 80  �� *VariancePercentage+$ F2�� �� "   ��  �� *VariancePercentage+# Z  True.a���  � ��
 �  ����  � ��$
 Bold"���  �� *VariancePercentage+# Z 	 False.a���  � ��
 � � ���  � ��$
 Bold"�  $
 الحالة��$ 80  �� *StatusDisplay+�� $��3�����#ECF0F1$ 20q�$��0,20,0,0q�._��  �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0�����#3498DB$ 15q�$��0,0,5,0q�._��� $��Center=�.+��� $!��إجمالي الجلسات��
�������$
��Bold"�$��Center=��� -   $��lblTotalSessions��$��0��
�������$��20(�$
��Bold"�$��Center=��� $��1�����#27AE60$ 15q�$��5,0,5,0q�._��� $��Center=�.+��� $%��الجلسات المكتملة��
�������$
��Bold"�$��Center=��� -   $��lblCompletedSessions��$��0��
�������$��20(�$
��Bold"�$��Center=��� $��2�����#E74C3C$ 15q�$��5,0,5,0q�._��� $��Center=�.+��� $*��$إجمالي الانحراف (كغ)��
�������$
��Bold"�$��Center=��� -	   $��lblTotalVariance��$
��0.00��
�������$��20(�$
��Bold"�$��Center=��� $��3�����#F39C12$ 15q�$��5,0,0,0q�._��� $��Center=�.+��� $(��"متوسط الانحراف (كغ)��
�������$
��Bold"�$��Center=��� -
   $��lblAverageVariance��$
��0.00��
�������$��20(�$
��Bold"�$��Center=�
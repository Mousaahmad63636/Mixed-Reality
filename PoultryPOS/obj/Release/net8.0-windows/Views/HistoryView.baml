   M S B A M L   `   `   `  ����   
PoultryPOS"    PoultryPOS.Views.HistoryView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$  سجل المبيعات��$��RightToLeft=� � 	Resources    PaginationButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property	 P� Value  ActivePageButton
 �� BasedOn��%3  &	        &	 �    7   '�� �    �� " ��.���  � �� �#3498DB��  � ��
 �������  � ��$
 Bold"���  � ��$ 12(���  � ��$ 0q���  � ��$
 10,6q���  � ��$ 2q���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��$ 4��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
	 True��. ���  � �� �#5DADE2P�  � {�$	 False��. ���  � �� �#BDC3C7�� " ��8
   .���  � �� �#E74C3C�� " ��.���  � ��$ Right=���  � ��$ Center=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0�����#2C3E50 �� Padding$ 20q�$��0,0,0,20q�._��� .+��� $"��📊 سجل المبيعات��$��24(�$
��Bold"�
�������$��Center=�$��0,0,0,15q��  � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0�����#34495E$ 15q�$��0,0,5,0q�._��� $��Center=�.+��� $5��/إجمالي المبيعات (كل الوقت)��
�������$
��Bold"�$��Center=��� -   $��lblTotalSales��$��$0.00�����#1ABC9C$��20(�$
��Bold"�$��Center=��� $��1�����#34495E$ 15q�$��5,0,5,0q�._��� $��Center=�.+��� $��مبيعات الفترة��
�������$
��Bold"�$��Center=��� -   $��lblPeriodSales��$��$0.00�����#3498DB$��20(�$
��Bold"�$��Center=��� $��2�����#34495E$ 15q�$��5,0,5,0q�._��� $��Center=�.+��� $!��معاملات الفترة��
�������$
��Bold"�$��Center=��� -   $��lblPeriodTransactions��$��0�����#E74C3C$��20(�$
��Bold"�$��Center=��� $��3�����#34495E$ 15q�$��5,0,0,0q�._��� $��Center=�.+��� $!��الرصيد المستحق��
�������$
��Bold"�$��Center=��� -   $��lblOutstandingBalance��$��$0.00�����#F39C12$��20(�$
��Bold"�$��Center=��� $��1��
����������#DDD$��1q�$ 20q�$��0,0,0,20q�._��  �� $
��Auto ��� $��20 ��� $
��Auto ��� $��20 ��� $
��Auto ��� $��20 ��� $
��Auto ��� $��20 ��� $��* �.+��� $��0��.+��� $��🔍 المرشحات��$��16(�$
��Bold"�$��0,0,0,10q���  �� Orientation$ 
Horizontal=�.+��� $��من:��$
��Bold"�$��Center=�$��0,0,5,0q�(  "System.Windows.Controls.DatePicker  -   $��
dpFromDate��$	��120��$��0,0,10,0q��� $��إلى:��$
��Bold"�$��Center=�$��0,0,5,0q�  -   $��dpToDate��$	��120���� $��2��.+��� $��👤 العميل��$��16(�$
��Bold"�$��0,0,0,10q��� -   $��cmbCustomerFilter��$	��150���� $��4��.+��� $��📋 النوع��$��16(�$
��Bold"�$��0,0,0,10q��� -   $��cmbTypeFilter��$	��120��.@��� $��جميع الأنواع�� �� 
IsSelected$
 True���� $��المبيعات فقط���� $��المدفوعات فقط���� $��6��.+��� $!��⚡ مرشحات سريعة��$��16(�$
��Bold"�$��0,0,0,10q��� $ 
Horizontal=�.+��� -	   $��btnToday��$��
اليوم��$��60��$��25�����#1ABC9C
�������$
��Bold"�$��0,0,5,0q��� -
   $��btnThisWeek��$��هذا الأسبوع��$��80��$��25�����#3498DB
�������$
��Bold"�$��0,0,5,0q��� -   $��btnThisMonth��$��هذا الشهر��$��90��$��25�����#9B59B6
�������$
��Bold"�$��0,0,5,0q��� -   $��btnClearFilter��$��مسح��$��60��$��25�����#95A5A6
�������$
��Bold"��� $��8��$
��Left=�.+��� $(��"📄 العناصر بالصفحة��$��16(�$
��Bold"�$��0,0,0,10q��� -   $��cmbItemsPerPage��$��80��.@��� $��10���� $��25��$
 True���� $��50���� $	��100���� $��2��
����������#DDD$��1q�$ 20q�._��� .+��� $��📋 المعاملات��$��18(�$
��Bold"�$��0,0,0,15q�$��Center=�&   System.Windows.Controls.DataGrid  -   $��dgTransactions��$	��400��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   GridLinesVisibility$ 
Horizontal=�   AlternatingRowBackground �#F8F9FA   
IsReadOnly$
 True��$��LeftToRight=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ التاريخ��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Date+ �� StringFormat$ 
yyyy-MM-dd��  $ 
الوقت��$ 60  �� *Date+$ HH:mm��  $ العميل��$ *  �� *CustomerName+  $ 
النوع��$ 80  �� *
TypeArabic+   	CellStyle �� *  $System.Windows.Controls.DataGridCell"   �� Triggers ��  �� Binding �� *Type+ �� Value Sale.a���  � ��
 ��  ���  � ��$
 Bold"���  �� *Type+ Payment.a���  � ��
 � � ���  � ��$
 Bold"�  $ المبلغ��$	 100  �� *AmountDisplay+ �� "   ��  �� *Type+ Sale.a���  � ��
 ��  ���  � ��$
 Bold"���  �� *Type+ Payment.a���  � ��
 � � ���  � ��$
 Bold"�  $ الشاحنة��$	 100  �� *	TruckName+  $ السائق��$	 100  �� *
DriverName+  $ ملاحظات��$ *  �� *Notes+4  .System.Windows.Controls.DataGridTemplateColumn  $ الإجراء��$	 100    CellTemplate �� .R��� -   $��عرض التفاصيل��$��90��$��25�����#3498DB
�������$
��Bold"�$��10(��� $��3��
����������#DDD$��1q�$ 15q�$��0,20,0,0q�._��  �� $��* ��� $
��Auto ��� $��* �.+��� $��0��$ 
Horizontal=�$��Center=�.+��� -   $��lblPaginationInfo��$!��عرض 1-25 من 100 سجل��$
��Bold"�$��12(����#2C3E50�� $��1��$ 
Horizontal=�$��Center=�.+��� -   $��btnFirstPage��$��⏮️ الأول��#��[  �� -   $��btnPreviousPage��$��⬅️ السابق��#��[  �� -   $��spPageNumbers��$ 
Horizontal=�$
��10,0q��� -   $��btnNextPage��$��التالي ➡️��#��[  �� -   $��btnLastPage��$��الأخير ⏭️��#��[  �� $��2��$ 
Horizontal=�$
��Left=�$��Center=�.+��� $��انتقل للصفحة:��$
��Bold"�$��12(����#2C3E50$��Center=�$��0,0,5,0q��� -   $��txtJumpToPage��$��50��$��25��$��Center=�$��Center=��� -   $��btnJumpToPage��$��
انتقل��#��[  
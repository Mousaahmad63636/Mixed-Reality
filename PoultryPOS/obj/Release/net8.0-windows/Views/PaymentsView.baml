   M S B A M L   `   `   `  ����   
PoultryPOS#    PoultryPOS.Views.PaymentsView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$  المدفوعات��$��RightToLeft=� � 	Resources  �� 
TargetType �� Property �� Value��%J   '��       �� " ��.���  � ��$ Right=���  � ��$ Center=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $��* �.+��� $��0��
����������#DDD$��1q� �� Padding$ 20q�$��0,0,0,20q�._��  � ColumnDefinitions �� $��2* ��� $��20 ��� $��* �.+��� $��0��.+��� $��استقبال دفعة��$��20(�$
��Bold"�$��0,0,0,20q�$��Center=��  �� $��* ��� $��20 ��� $��* ��� $��20 ��� $��* �.+��� $��0��.+��� $��اختر العميل:��$
��Bold"�$��0,0,0,5q��� -   $��cmbCustomer��$��30��$��0,0,0,15q��� -   $��lblCurrentBalance��$�� ��$
��Bold"�
���  ��$��0,0,0,15q��� $��2��.+��� $��مبلغ الدفعة:��$
��Bold"�$��0,0,0,5q��� -   $��txtPaymentAmount��$��30��$��0,0,0,15q��� -   $��btnReceivePayment��$!��استقبال الدفعة��$��35�����#9B59B6
�������$
��Bold"��� $��4��.+��� $"��ملاحظات الدفعة:��$
��Bold"�$��0,0,0,5q��� -   $��txtPaymentNotes��$��80�� �� TextWrapping$
 Wrap=�	 �� AcceptsReturn$
	 True��$��0,0,0,15q��� -   $��btnClear��$��مسح النموذج��$��30�����#95A5A6
�������$
��Bold"��� $��2��.+��� $!��إحصائيات سريعة��$��16(�$
��Bold"�$��0,0,0,15q�$��Center=��� ���#ECF0F1$ 15q�._��� .+��� -   $��lblTodayPayments��$"��دفعات اليوم: $0.00��$
��Bold"�$��0,0,0,5q��� -   $��lblTodayCount��$��عدد الدفعات: 0��$
��Bold"�$��0,0,0,5q��� -	   $��lblTotalOutstanding��$(��"إجمالي المستحق: $0.00��$
��Bold"�$��14(�
���  ���� $��1��
����������#DDD$��1q�$ 20q�._��  �� $��* ��� $��20 ��� $	��300 �.+��� $��0��.+��� 
 �� Orientation$
 
Horizontal=�$��0,0,0,15q�.+��� $��سجل المدفوعات��$��18(�$
��Bold"�$��Center=��� $��من:��$
��Bold"�$��20,0,5,0q�$��Center=�(  "System.Windows.Controls.DatePicker  -
   $��
dpFromDate��$	��120��$��0,0,10,0q��� $��إلى:��$
��Bold"�$��0,0,5,0q�$��Center=�  -   $��dpToDate��$	��120��$��0,0,10,0q��� -   $��btnClearFilter��$��مسح��$��25��$��60�����#95A5A6
�������$
��Bold"�&   System.Windows.Controls.DataGrid  -   $��
dgPayments��$	��400��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   GridLinesVisibility$ 
Horizontal=�   AlternatingRowBackground �#F8F9FA   
IsReadOnly$
 True��$��LeftToRight=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ التاريخ��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 140 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *PaymentDate+ �� StringFormat$ yyyy-MM-dd HH:mm��  $ العميل��$ *  �� *CustomerName+  $ المبلغ��$	 100  �� *Amount+$ C��   	CellStyle �� *  $System.Windows.Controls.DataGridCell"  .���  � ��
 � � ���  � ��$
 Bold"�  $ ملاحظات��$	 150  �� *Notes+�� $
 
Horizontal=�$��0,15,0,0q�$��Center=�.+��� -   $��lblFilteredTotal��$,��&الإجمالي المفلتر: $0.00��$
��Bold"�$��16(�
��� � ��� -   $��lblFilteredCount��$��العدد: 0��$
��Bold"�$��20,0,0,0q��� $��2��.+��� $(��"العملاء لديهم رصيد��$��14(�$
��Bold"�$��0,0,0,15q�$��Center=�  -   $��dgCustomersWithBalance��$	��400��$ False��$ Column=�$ 
Horizontal=� �#F8F9FA$
 True��$��LeftToRight=�   $ العميل��$ *  �� *Name+  $ الرصيد��$ 80  �� *	Balance+$ C�� �� "  .���  � ��
 �  ����  � ��$
 Bold"�
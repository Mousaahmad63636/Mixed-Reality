   M S B A M L   `   `   `  ����   
PoultryPOS/    )PoultryPOS.Views.TransactionDetailsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$#  تفاصيل المعاملة��$	��600��$	��900��$��RightToLeft=� � WindowStartupLocation$ CenterScreen=�.��� $��20q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0�����#2C3E50 �� Padding$ 15q�$��0,0,0,20q�._��� -   $��lblTitle��$#��تفاصيل المعاملة��$��20(�$
��Bold"�
�������$��Center=��� $��1��
����������#DDD$��1q�$ 20q�$��0,0,0,20q�._��  � ColumnDefinitions �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��0,0,10,0q�.+��� $��العميل:��$
��Bold"�$��0,0,0,5q��� -   $��cmbCustomer��$��30��${�False���� $��1��$��0,0,10,0q�.+��� $&�� الشاحنة (اختياري):��$
��Bold"�$��0,0,0,5q��� -   $��cmbTruck��$��30��${�False���� $��2��$��0,0,10,0q�.+��� $$��السائق (اختياري):��$
��Bold"�$��0,0,0,5q��� -   $��	cmbDriver��$��30��${�False���� $��3��.+��� $��سعر الكيلو:��$
��Bold"�$��0,0,0,5q��� -   $��txtPricePerKg��$��30�� �� 
IsReadOnly$
 True���� $��2��
����������#DDD$��1q�$ 20q�._�� .+��� -   $��spSaleDetails��.+�� $��0,0,0,15q� �� $��* ��� $
��Auto ��� $
��Auto �.+��� $��عناصر البيع��$��16(�$
��Bold"�$��Center=��� -   $��
btnAddItem��$��1��$��إضافة عنصر��$	��100��$��30�����#27AE60
�������$
��Bold"�$��0,0,10,0q�$y�	Collapsed=��� -   $��btnEditMode��$��2��$��تفعيل التعديل��$	��100��$��30�����#3498DB
�������$
��Bold"�&   System.Windows.Controls.DataGrid  -	   $��dgSaleItems��$	��300��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   GridLinesVisibility$ 
Horizontal=�	   AlternatingRowBackground	 �#F8F9FA
   
IsReadOnly$

 True��$��LeftToRight=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$! الوزن الإجمالي��   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *GrossWeight+  $ عدد الأقفاص��$ *  �� *NumberOfCages+  $$ وزن القفص الواحد��$ *  �� *SingleCageWeight+  $( "إجمالي وزن الأقفاص��$ *    
IsReadOnly$
 True�� �� *TotalCageWeight+ �� StringFormat$ F2��  $ الوزن الصافي��$ * $
 True�� �� *	NetWeight+$ F2��  $# المبلغ الإجمالي��$ * $
 True�� �� *TotalAmount+$ C��4  .System.Windows.Controls.DataGridTemplateColumn  $ الإجراء��$ 80    CellTemplate �� .R��� -
   $��حذف��$��60��$��25�����#E74C3C
�������$
��Bold"�$��10(�$y�	Collapsed=��� ���#ECF0F1$ 15q�$��0,15,0,0q�._��� .+��� -   $��	lblTotals��$*��$إجمالي الفاتورة: $0.00��$��16(�$
��Bold"�$��Center=����#27AE60�� -   $��lblPaymentStatus��$�� ��$��14(�$
��Bold"�$��Center=�$��0,5,0,0q��� -   $��spPaymentDetails��$y�	Collapsed=�.+��� $��تفاصيل الدفعة��$��16(�$
��Bold"�$��0,0,0,15q��  �� $��* ��� $��* �.+��� $��0��$��0,0,10,0q�.+��� $��مبلغ الدفعة:��$
��Bold"�$��0,0,0,5q��� -   $��txtPaymentAmount��$��30��$
 True���� $��1��.+��� $��ملاحظات:��$
��Bold"�$��0,0,0,5q��� -   $��txtPaymentNotes��$��80�� �� TextWrapping$
 Wrap=� �� AcceptsReturn$
 True��$
 True���� $��3�� �� Orientation$ 
Horizontal=�$��Center=�$��0,20,0,0q�.+��� -   $��
btnReprint��$#��🖨️ إعادة طباعة��$	��120��$��35�����#9B59B6
�������$
��Bold"�$��0,0,10,0q��� -   $��btnSave��$��حفظ��$	��100��$��35�����#27AE60
�������$
��Bold"�$��0,0,10,0q�$y�	Collapsed=��� -   $��	btnCancel��$��
إلغاء��$	��100��$��35�����#95A5A6
�������$
��Bold"�$��0,0,10,0q��� -   $��btnClose��$��
إغلاق��$	��100��$��35�����#34495E
�������$
��Bold"�
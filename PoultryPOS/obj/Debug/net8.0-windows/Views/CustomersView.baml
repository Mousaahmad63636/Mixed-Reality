   M S B A M L   `   `   `  ����E  APoultryPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:PoultryPOS.ConvertersPoultryPOS.Converters  5      $    PoultryPOS.Views.CustomersView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      :
converters#clr-namespace:PoultryPOS.Converters     5        N� Title$  إدارة العملاء��5      $��RightToLeft=�5       � 	Resources 5       �� 
TargetType �� Property �� Value   	CardStyle3  -System.Windows.Media.Effects.DropShadowEffect   Color   	Direction   ShadowDepth   
BlurRadius	   Opacity  PrimaryButton
 �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  HeaderGradient �� 
StartPoint �� EndPoint � Color � Offset��%�  '��       '�� -    &	      &	 �    &	     �� 5	   
   " ��6   .�5
      ��  � ��$ Right=�6;   �� 5       � ��$ Center=�69   �� 5       � ��
 �����6+   �� 5       � �� �#3498DB6,   �� 5       � ��$ 2q�60   �� 5       � ��$ 8q�6(   �� 5       � ��$ 12(�6)   5      �� 5   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � �� �#3498DB6,   �� 5       � ��$ 2q�60   �� 5       � ��$ 8q�6(   �� 5       � ��$ 12(�6)   5      �� 5   
   " ��6"   .�5      ��  � ��
 �����6+   �� 5       � �� �#E8E8E86,   �� 5       � ��$ 1q�60   �� 5       �@��CornerRadius$ 8��6-   �� 5        �@��Padding$ 20q�6(   �� 5!       � ��$ 0,0,0,15q�6'   �� 5"       �@J�Effect 5#        5$      $ #E0E0E0��6'   $	 270Q�67   $ 3Q�6G   $ 10Q�6W   $		 0.3Q�6g   5&      5'      �� 5)   
   " ��6&   .�5*      ��  � �� �#27AE606+   �� 5+       � ��
 �����6+   �� 5,       � ��$
 Bold"�6+   �� 5-       � ��$ 12(�6)   �� 5.       � ��$ 0q�60   �� 5/       � ��$
 15,8q�6(   �� 50       �@�Cursor$
 Hand��6'   �� 51       � �� 52      �� 53      "
 ��6&   .R�54      �� #��z��6!   $ 6��6K   ._�55      �� $��Center=�6/   $��Center=�6L   56       57      P� 58       �@J�IsMouseOver$
 True��6=   . �59   "   ��  � �� �#2ECC716?   5:      5<      5>      5?      �� 5A   
   $	 0,0&�65   $	 1,0&�6F   .��5B      � $ #2C3E50��6   $ 0Q�6+   � 5C      $ #34495E��6   $ 1Q�6+   5D      5E      .��5G      � $��25q�6   ���#F8F9FA � RowDefinitions 5H   
   �� 5I      $
��Auto �6   �� 5J      $��* �6   .+�5M   
   �� $��0��6   #��[ 6   $ 12��5N       �� Padding$ 10q�6#   $��0,0,0,15q�60   ._�5O      �� $-��'👥 مركز إدارة الزبائن��6   $��28(�65   $
��Bold"�6C   
�������$��Center=�5P   +   5Q      �� 5S   
   $��1��6   
������ $��0q�6;    5T      ��%�  '��       �� 5U      " ��6   .�5V      ��  � ��$ 20,10q�60   �� 5W       � ��$
 Bold"�63   �� 5X       � ��$ 14(�61   �� 5Y       � �� �#ECF0F163   �� 5Z       � �� �#BDC3C764   �� 5[       � �� �#2C3E5063   �� 5\       � �� 5]      �� 5^      "
 ��6.   .R�5_   "   �� #��z��6)   #��z��5`   )   $��1,1,1,0q�5a   )   $ 8,8,0,0��5b   )   $��0,0,2,0q�5c   )   ._�5d   &   �� $��Center=�67   $��Center=�5e   7   $��Header��5f   7   5g   #    5h   "   P� 5i   &    �@��
IsSelected$
 True��6D   . �5j   *   ��  � ��
 �����6G   �� 5k   *    � �� �#2C3E506G   5l   '   5n      5p      5q      5r      .@�5t      �� $$��👤 إدارة الزبائن��6   $��0,-5,0,0q�60   .��5u      � 
�������$��0,0,0,10q�6*    5v      �� 5w      $
��Auto �6(   �� 5x      $��* �6(   .+�5{      �� $��0��6   #��[  6*   $��15q�6M   ._�5|      �  � ColumnDefinitions 5}      �� 5~   "   $��2* �63   �� 5   "   $��* �63   .+�5�      �� $��0��6)   $��0,0,20,0q�69   .+�5�   "   �� $<��6📝 إضافة/تعديل معلومات العميل��6,   $��16(�6Q   $
��Bold"�6_   ���#2C3E50$��0,0,0,20q�5�   A   � 5�   "    5�   &   �� 5�   *   $��* �6;   �� 5�   *   $��* �6;   �� 5�   *   $��* �6;   .+�5�   &   �� $��0��61   $��0,0,15,0q�6A   .+�5�   *   �� $��👤 اسم الزبون��64   $
��Bold"�6I   $��12(�6[   ���#2C3E50$��0,0,0,5q�5�   I   �� 5�   *   -   5        $��txtCustomerName��5�   2   $��35��6I   5�   '   �� 5�   &   $��1��61   $��0,0,15,0q�6A   .+�5�   *   �� $(��"📞 الهاتف (اختياري)��64   $
��Bold"�6O   $��12(�6a   ���#2C3E50$��0,0,0,5q�5�   I   �� 5�   *   -   5        $��txtCustomerPhone��5�   2   $��35��6J   5�   '   �� 5�   &   $��2��61   .+�5�   *   �� $$��💰 الرصيد الأولي��64   $
��Bold"�6L   $��12(�6^   ���#2C3E50$��0,0,0,5q�5�   I   �� 5�   *   -   5        $��txtInitialBalance��5�   2   $��35��6K   $��0��6W   5�   '   5�   #   5�      �� 5�      $��1��6)   $��Center=�69   .+�5�   "   �� -   5        $��btnAddCustomer��5�   )   $��➕ إضافة زبون��6?   #��[ 6V   $��0,0,0,10q�5�   )   $��19��6X   �� 5�   "   -   5        $��btnUpdateCustomer��5�   )   $��✏️ تحديث��6B   ���#3498DB#��[ 5�   )   $��0,0,0,10q�6P   $��19��6�   �� 5�   "   -   5        $��btnDeleteCustomer��5�   )   $��🗑️ حذف��6B   ���#E74C3C#��[ 5�   )   $��0,0,0,10q�6P   $��21��6�   �� 5�   "   -   5        $��btnPrintStatement��5�   )   $%��📄 طباعة كشف حساب��6B   ���#8E44AD#��[ 5�   "   $��0,0,0,10q�6I   $��25��6{   �� 5�   "   -   5        $��btnClearCustomer��5�   )   $��🧹 مسح��6A   ���#95A5A6#��[ 5�   )   $��21��6o   5�      5�      5�      � 5�      $��1��6   $
��15,0q�6(    5�      �� 5�      $��2* �6/   �� 5�      $��* �6/   .+�5�      �� $��0��6!   #��[  61   $��0,0,10,0q�6T   ._�5�      �� .+�5�   "   � $��0,0,0,15q�6'    5�   &   �� 5�   *   $
��Auto �6;   �� 5�   *   $��* �6;   �� 5�   *   $
��Auto �6;   .+�5�   &   �� $��0��60   $$��📋 قائمة الزبائن��6@   $��16(�6X   $
��Bold"�6f   ���#2C3E50$��Center=�5�   E   $��0,0,15,0q�6`   �� 5�   &   -	   5        $��txtSearchCustomers��5�   >   $��1��6.   $��30��6X   $��Center=�5�   .   $��0,0,10,0q�6P   .��5�   [   ��5�   *   �� 5�   .   " ��64    �� BasedOn# [B��6I   .�5�   2   ��  � �� 5�   6   �� 5�   :   "
 ��6J   .R�5�   >   �� #��z��6E   #��z��5�   E   #��z��5�   E   $ 15��5�   E   ._�5�   B   � .+�5�   F   ��  ��Name$ PART_ContentHost��6S   $��	30,0,10,0q�5�   S   $��Center=�5�   S   �� 5�   F   $&�� 🔍 البحث عن عميل...��6P   ���#999$��35,0,0,0q�5�   P   $��Center=�5�   P    J� IsHitTestVisible$ False��5�   P   ��5�   J   �� 5�   N   " ��6T   .�5�   R   ��  � y�$ 	Collapsed=�6o    �� Triggers 5�   R   �� 5�   V    �� Binding 6b   �� *Text+ �� RelativeSource ��  �� AncestorType TextBox �� Value  6�   .a�5�   Z   ��  � y�$ Visible=�6w   5�   W   5�   O   5�   G   5�   C   5�   ?   5�   ;   5�   3   5�   /   .��5�   9   5�   '   �� 5�   &   -
   5        $��btnClearSearch��5�   =   $��2��6-   $	��❌��6S   $��30��6_   $��30��6j   ���#E74C3C
�������$
��Bold"�5�   U    � ToolTip$ مسح البحث��5�   J   5�   #   &   System.Windows.Controls.DataGrid  5�   "   -   5        $��dgCustomers��5�   +      AutoGenerateColumns$ False��6>       HeadersVisibility$  Column=�5�   +   !   GridLinesVisibility$! 
Horizontal=�6F   "   AlternatingRowBackground" �#F8F9FA
�������$��LeftToRight=�5�   [   "#   VerticalScrollBarVisibility$
# Auto=�5�   +   $$   HorizontalScrollBarVisibility$
$ Auto=�6N   %   CanUserResizeColumns$
% True��5�   +   &   CanUserSortColumns$
& True��6G    5�   &   =  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell)  #System.Windows.Controls.DataGridRow��%h  '        '  _    '  &    �� 5�   *   "  60   .�5�   .   ��  � �� �#34495E6K   �� 5�   .    � ��
 �����6K   �� 5�   .    � ��$
 Bold"�6K   �� 5�   .    � ��$ 13(�6I   �� 5�   .    � ��$
 10,8q�6H   �� 5�   .    � ��$ Center=�6[   �� 5�   .    � �� �#2C3E506L   �� 5�   .    � ��$ 0,0,1,0q�6P   5   +   �� 5  *   "  60   .�5  .   ��  � ��$ 8q�6H   �� 5  .    � ��$ 0q�6P   �� 5  .    �@�FocusVisualStyle 6Q   Z� �� 5  .    � ��$ Center=�6R   5  +   �� 5  *   "  60   .�5  .   ��  � ��$ 40��6G    5	  .   P� 5
  2    �@J�IsMouseOver$
 True��6Q   . �5  6   ��  � �� �#ECF0F16S   5  3   P� 5  2    �@ 
IsSelected$
 True��6P   . �5  6   ��  � �� �#D5DBDB6S   5  3   5  +   5  '   '   Columns' 5  &   0  *System.Windows.Controls.DataGridTextColumn  5  *   ,  &System.Windows.Controls.DataGridColumn(   Header$( 
الرقم��6=   )   Width5  /System.Windows.Controls.DataGridLengthConverter$) 60 6c   1	  +System.Windows.Controls.DataGridBoundColumn* 	  Binding* 6L   �� *Id++ 	  ElementStyle+ 5  .   �� 5  2   " ��68   .�5  6   ��  � ��$ Center=�6\   �� 5  6    � ��$ Center=�6Z   �� 5  6    � ��$
 Bold"�6S   �� 5  6    � �� �#2980B96S   5  3   5  +     5  *   $( اسم العميل��6=   $) * 6j   ,   MinWidth$	, 150Q�6t   * 6Q   �� *Name++ 5  .   �� 5   2   " ��68   .�5!  6   ��  � ��$ Right=�6\   �� 5"  6    � ��$ Center=�6Z   �� 5#  6    � ��$ SemiBold"�6S   �� 5$  6    � ��$
 10,0q�6O   5%  3   5'  +     5(  *   $( الهاتف��6=   $	) 120 6g   * 6M   �� *Phone++ 5)  .   �� 5*  2   " ��68   .�5+  6   ��  � ��$ Center=�6\   �� 5,  6    � ��$ Center=�6Z   �� 5-  6    � �� �#27AE606S   5.  3   50  +     51  *   $( الرصيد��6=   $	) 120 6y   * 6M   �� *	Balance+- �� StringFormat$- C��+ 52  .   �� 53  2   " ��68   .�54  6   ��  � ��$ Center=�6\   �� 55  6    � ��$ Center=�6Z   �� 56  6    � ��$
 Bold"�6S    57  6   �� 58  :    6F   �� *	Balance+-
   'PoultryPOS.Converters.PositiveConverter. 
  Instance/ �� 	Converter#/ Z.  True6�   .a�59  >   ��  � �� �#E74C3C6[   �� 5:  >    � �� �#FADBD86[   5;  ;   �� 5<  :    6F   �� *	Balance+#/ Z. 	 False6�   .a�5=  >   ��  � �� �#27AE606[   5>  ;   5@  3   5B  +   5D  #   5E     5F     �� 5H     $��1��6!   #��[  61   $��10,0,0,0q�6T   ._�5I     �� .+�5J  "   �� $��📊 ملخص��6,   $��16(�6;   $
��Bold"�6I   ���#2C3E50$��Center=�5K  A   $��0,0,0,15q�6^   �� 5M  "   ���#E8F8F5���#27AE60$��2q�6T   $ 8��5N  )   $ 15q�6:   $��0,0,0,20q�6G   ._�5O  &   �� .+�5P  *   �� -   5        $��lblTotalCustomers��5P  4   $$��إجمالي الزبائن: 0��6M   $
��Bold"�6f   $��12(�5Q  4   $��0,0,0,8q�6B   �� 5R  *   -   5        $��lblCustomersWithBalance��5R  4   $��لديهم رصيد: 0��6S   $
��Bold"�6h   $��12(�5S  4   $��0,0,0,8q�6B   �� 5T  *   -   5        $��lblTotalBalance��5T  4   $&�� إجمالي الرصيد: $0.00��6K   $
��Bold"�6g   $��14(�5U  4   ���#E74C3C6B   5V  '   5W  #   �� 5Y  "   $,��&⚠️ الأرصدة المستحقة��6,   $��14(�6G   $
��Bold"�6U   ���#E74C3C$��Center=�5Z  A   $��0,0,0,10q�6^     5\  "   -   5        $��dgCustomersWithBalance��5\  +   $ False��6I   $  Column=�5]  +   $! 
Horizontal=�6F   " �#F8F9FA$��LeftToRight=�5^  N   $
# Auto=�5_  +   $	��200��6N    5`  &   ��%  '        '      '  �    �� 5a  *   "  60   .�5b  .   ��  � �� �#E74C3C6K   �� 5c  .    � ��
 �����6K   �� 5d  .    � ��$
 Bold"�6K   �� 5e  .    � ��$ 12(�6I   �� 5f  .    � ��$	 8,6q�6H   �� 5g  .    � ��$ Center=�6[   5h  +   �� 5i  *   "  60   .�5j  .   ��  � ��$ 6q�6H   �� 5k  .    � ��$ 0q�6P   �� 5l  .    �@�FocusVisualStyle 6Q   Z� 5m  +   �� 5n  *   "  60   .�5o  .   ��  � ��$ 35��6G   5p  +   5q  '   ' 5r  &     5s  *   $( العميل��6=   $) * 6f   $	, 100Q�6p   * 6M   �� *Name++ 5t  .   �� 5u  2   " ��68   .�5v  6   ��  � ��$ Right=�6\   �� 5w  6    � ��$ Center=�6Z   �� 5x  6    � ��$ SemiBold"�6S   �� 5y  6    � ��$	 5,0q�6O   5z  3   5|  +     5}  *   $( الرصيد��6=   $) 80 6y   * 6M   �� *	Balance+$- C��+ 5~  .   �� 5  2   " ��68   .�5�  6   ��  � ��$ Center=�6\   �� 5�  6    � ��$ Center=�6Z   �� 5�  6    � �� �#E74C3C6S   �� 5�  6    � ��$
 Bold"�6S   5�  3   5�  +   5�  #   5�     5�     5�     5�     5�     �� 5�     $(��"💳 إدارة المدفوعات��6   $��0,-7,0,0q�62   .��5�     � 
�������$��0,0,0,10q�6*    5�     �� 5�     $
��Auto �6(   �� 5�     $��* �6(   .+�5�     �� $��0��6   #��[  6*   $��15q�6M   $	��250��6Y   ._�5�     �� .+�5�     �� $4��.💳 استقبال دفعة من العميل��6(   $��18(�6I   $
��Bold"�6W   ���#2C3E50$��Center=�5�  =   $��0,0,0,30q�6Z   � 5�      5�  "   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   .+�5�  "   �� $��0��6-   $��0,0,15,0q�6=   .+�5�  &   �� $ ��👤 اختر العميل��60   $
��Bold"�6F   $��14(�6X   ���#2C3E50$��0,0,0,8q�5�  E   �� 5�  &   -   5        $��cmbPaymentCustomer��5�  /   $��40��6I   5�  /   �� 5�  &   -   5        $��lblCustomerBalance��5�  0   $�� ��6J   $
��Bold"�6R   $��12(�6d   ���#E74C3C$��0,8,0,0q�5�  E   $��Center=�6V   5�  #   �� 5�  "   $��1��6-   $��0,0,15,0q�6=   .+�5�  &   �� $ ��💰 مبلغ الدفعة��60   $
��Bold"�6F   $��14(�6X   ���#2C3E50$��0,0,0,8q�5�  E   �� 5�  &   -   5        $��txtPaymentAmount��5�  .   $��40��6F   $��16(�6R   $
��Bold"�6`   $��Center=�5�  .   5�  #   �� 5�  "   $��2��6-   $��0,0,15,0q�6=   .+�5�  &   �� $&�� 📝 ملاحظات الدفعة��60   $
��Bold"�6I   $��14(�6[   ���#2C3E50$��0,0,0,8q�5�  E   �� 5�  &   -   5        $��txtPaymentNotes��5�  .   $��40��6E   5�  #   �� 5�  "   $��3��6-   $��Bottom=�6=   .+�5�  &   �� -   5        $��btnReceivePayment��5�  -   $&�� 💳 استقبال الدفعة��6F   ���#9B59B6#��[ 5�  B   $��40��5�  -   $��14(�69   6G   5�  #   5�     5�     5�     �� 5�     $��1��6   #��[  6*   $
��15,0q�6M   ._�5�     �� .+�5�     �� $9��3📋 تعديل المدفوعات الموجودة��6(   $��18(�6K   $
��Bold"�6Y   ���#2C3E50$��Center=�5�  =   $��0,0,0,20q�6Z   � 5�     $��0,0,0,15q�6#    5�  "   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   .+�5�  "   �� $��0��6-   $��0,0,10,0q�6=   .+�5�  &   �� $��رقم الدفعة:��60   $
��Bold"�6C   $��0,0,0,5q�6U   �� 5�  &   -   5        $��txtEditPaymentId��5�  .   $��35��6F   0 �� 
IsReadOnly$
0 True��6R   ���#F5F5F56d   5�  #   �� 5�  "   $��1��6-   $��0,0,10,0q�6=   .+�5�  &   �� $��العميل:��60   $
��Bold"�6?   $��0,0,0,5q�6Q   �� 5�  &   -   5        $��cmbEditPaymentCustomer��5�  /   $��35��6M   5�  #   �� 5�  "   $��2��6-   $��0,0,10,0q�6=   .+�5�  &   �� $ ��المبلغ الجديد:��60   $
��Bold"�6F   $��0,0,0,5q�6X   �� 5�  &   -   5        $��txtEditPaymentAmount��5�  .   $��35��6J   5�  #   �� 5�  "   $��3��6-   $��0,0,10,0q�6=   .+�5�  &   �� $��الملاحظات:��60   $
��Bold"�6B   $��0,0,0,5q�6T   �� 5�  &   -   5        $��txtEditPaymentNotes��5�  .   $��35��6I   5�  #   �� 5�  "   $��4��6-   $��Bottom=�6=   .+�5�  &   �� -   5        $��btnUpdatePayment��5�  -   $��
تحديث��6E   ���#3498DB#��[ 5�  -   $��35��6T   6`   5�  #   5�       5�     -   5        $��dgAllPayments��5�  '   $ False��6<   $  Column=�5�  '   $! 
Horizontal=�6B   " �#F8F9FA$��LeftToRight=�5�  J   $	��300��5�  Y    5�  "   ��%  '        �� 5�  &   "  6,   .�5�  *   ��  � �� �#9B59B66G   �� 5�  *    � ��
 �����6G   �� 5�  *    � ��$
 Bold"�6G   �� 5�  *    � ��$ 13(�6E   �� 5�  *    � ��$ 8q�6D   �� 5�  *    � ��$ Center=�6W   5�  '   5�  #   ' 5�  "     5�  &   $( 
الرقم��69   $) 60 6_   * 6H   �� *Id+6_     5�  &   $( التاريخ��69   $	) 140 6�   * 6J   �� *PaymentDate+$- yyyy-MM-dd HH:mm��6�     5�  &   $( العميل��69   $) * 6j   * 6I   �� *CustomerName+6j     5�  &   $( المبلغ��69   $	) 100 6t   * 6I   �� *Amount+$- C��+ 5�  *   �� 5�  .   " ��64   .�5�  2   ��  � ��
 � � �6O   �� 5   2    � ��$
 Bold"�6O   �� 5  2    � ��$ Center=�6X   5  /   5  '     5  &   $( ملاحظات��69   $	) 150 6d   * 6J   �� *Notes+6d   4  .System.Windows.Controls.DataGridTemplateColumn  5  &   $( الإجراءات��6=   $	) 200 6P   1   CellTemplate1 5  *   �� 5  .   .R�5	  2   �� 2 �� Orientation$2 
Horizontal=�6=   .+�5
  6   �� -   5        $��📝 تعديل��5
  =   $��70��6P   $��25��6[   ���#3498DB
�������$
��Bold"�5  e   $��10(�6w   $��0,0,5,0q�5  Z   �� 5  6   -   5        $��🖨️ طباعة��5  =   $��70��6Q   $��25��6\   ���#9B59B6
�������$
��Bold"�5  e   $��10(�6w   5  =   5  3   5  /   5  '   5     5     5     5     5     5     5     5     
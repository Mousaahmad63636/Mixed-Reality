   M S B A M L   `   `   `  ����   
PoultryPOS&     PoultryPOS.Views.TruckDriverView   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       N� Title$)  #الشاحنات والسائقين��$��RightToLeft=� � 	Resources  �� 
TargetType �� Property �� Value��%J   '��       �� " ��.���  � ��$ Right=���  � ��$ Center=�.��� $��20q� � ColumnDefinitions �� $��* ��� $��20 ��� $��* �.+��� $��0��
����������#DDD$��1q� �� Padding$ 20q�._��� .+��� $!��إدارة الشاحنات��$��20(�$
��Bold"�$��0,0,0,20q�$��Center=�� $��0,0,0,20q� �� $��2* ��� $��10 ��� $��* ��� $��* �.+��� $��0��.+��� $��اسم الشاحنة:��$
��Bold"�$��0,0,0,5q��� -   $��txtTruckName��$��30��$��0,0,0,10q��� $+��%رقم اللوحة (اختياري):��$
��Bold"�$��0,0,0,5q��� -   $��txtPlateNumber��$��30��$��0,0,0,10q��� $1��+الحمولة الحالية (أقفاص):��$
��Bold"�$��0,0,0,5q��� -   $��txtCurrentLoad��$��30��$��0���� $��2��$��10,0,0,0q�.+��� -   $��btnAddTruck��$��
إضافة��$��30�����#27AE60
�������$
��Bold"��� -   $��btnUpdateTruck��$��
تحديث��$��30��$��0,5,0,0q����#3498DB
�������$
��Bold"��� -   $��btnDeleteTruck��$��حذف��$��30��$��0,5,0,0q����#E74C3C
�������$
��Bold"��� -   $��btnClearTruck��$��مسح��$��30��$��0,5,0,0q����#95A5A6
�������$
��Bold"�&   System.Windows.Controls.DataGrid  -   $��dgTrucks��$	��350��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�	   GridLinesVisibility$	 
Horizontal=�
   AlternatingRowBackground
 �#F8F9FA$��LeftToRight=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 
الاسم��   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+  $ الحمولة��$ 60  �� *CurrentLoad+  $ اللوحة��$	 100  �� *PlateNumber+�� $��2��
����������#DDD$��1q�$ 20q�._��� .+��� $!��إدارة السائقين��$��20(�$
��Bold"�$��0,0,0,20q�$��Center=�� $��0,0,0,20q� �� $��2* ��� $��10 ��� $��* �.+��� $��0��.+��� $��اسم السائق:��$
��Bold"�$��0,0,0,5q��� -	   $��txtDriverName��$��30��$��0,0,0,15q��� $+��%رقم الهاتف (اختياري):��$
��Bold"�$��0,0,0,5q��� -
   $��txtDriverPhone��$��30���� $��2��$��10,0,0,0q�.+��� -   $��btnAddDriver��$��
إضافة��$��30�����#27AE60
�������$
��Bold"��� -   $��btnUpdateDriver��$��
تحديث��$��30��$��0,5,0,0q����#3498DB
�������$
��Bold"��� -   $��btnDeleteDriver��$��حذف��$��30��$��0,5,0,0q����#E74C3C
�������$
��Bold"��� -   $��btnClearDriver��$��مسح��$��30��$��0,5,0,0q����#95A5A6
�������$
��Bold"�  -   $��	dgDrivers��$	��350��$ False��$ Column=�$	 
Horizontal=�
 �#F8F9FA$��LeftToRight=�   $ 
الاسم��$ *  �� *Name+  $ الهاتف��$	 120  �� *Phone+
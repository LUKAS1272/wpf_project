   M S B A M L   `   `   `  ����J  FInventorySystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:InventorySystemInventorySystem  5   	   =#clr-namespace:InventorySystem.ViewsInventorySystem.Views  5   	        InventorySystem.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:InventorySystem     5   	   5views#clr-namespace:InventorySystem.Views     5   	     � Title$  Inventory system��5	   	   $	��450��6"   $	��800��6/    � WindowState$ 	Maximized=�6;   .��5
      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      .+�5   
   � ���#ddd � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      �� -   5        $��Admins��5      $��20(�6&   $��20 10q�64   
������ $��0q�6]   6q   �� 5      -   5        $��Rooms��5      $��20(�6%   $��20 10q�63   
������ $��0q�6\   $��1��6p   6�   �� 5      -   5        $��Items��5      $��20(�6%   $��20 10q�63   
������ $��0q�6\   $��2��6p   6�   5       � 5   
   -   5        $
��main��5      $��1��6     � NavigationUIVisibility$ Hidden=�6)   5      5      
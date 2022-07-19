using System;
using AsconTestTask.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.UI.WPF.Views.Windows;

public partial class MainWindow
{
	public MainWindow()
	{
		var dbContext = new AppDbContext();
		try
		{
			dbContext.Database.EnsureCreated();
		}
		catch
		{
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
		}
		
		InitializeComponent();
	}
}
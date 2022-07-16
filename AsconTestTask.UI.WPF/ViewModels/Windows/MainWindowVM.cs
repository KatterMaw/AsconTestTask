using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using AsconTestTask.Backend.Data;
using AsconTestTask.Backend.Data.Members;
using ReactiveUI;

namespace AsconTestTask.UI.WPF.ViewModels.Windows;

public class MainWindowVM : ReactiveObject
{
	public ObservableCollection<TreeNode> Nodes { get; }

	
	private ReactiveCommand<Unit, Unit>? _addNewObjectCommand;
	public ReactiveCommand<Unit, Unit> AddNewObjectCommand => _addNewObjectCommand ??= ReactiveCommand.Create(() =>
	{
		var newObject = new DataObject() {Product = "Новый продукт"};
		Nodes.Add(TreeNode.FromDataObject(newObject));
		using var dbContext = new AppDbContext();
		dbContext.Objects.Add(newObject);
		dbContext.SaveChanges();
	});


	public MainWindowVM()
	{
		using var dbContext = new AppDbContext();
		Nodes = new ObservableCollection<TreeNode>(dbContext.Objects.Select(obj => TreeNode.FromDataObject(obj)));
		Nodes.CollectionChanged += NodesOnCollectionChanged;
	}

	private void NodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(Nodes));
	}
}

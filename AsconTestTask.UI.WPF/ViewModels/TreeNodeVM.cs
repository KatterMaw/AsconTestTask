using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using AsconTestTask.Backend.Data.Members;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DataObject = AsconTestTask.Backend.Data.Members.DataObject;

namespace AsconTestTask.UI.WPF.ViewModels;

public class TreeNodeVM : ReactiveObject
{
	public static TreeNodeVM FromDataObject(DataObject obj, DataLink? link = null)
	{
		return new TreeNodeVM(obj, link, obj.LinksAsParent.Where(link => link.Child != null && link.Parent != null).Select(FromDataLink));
	}

	private static TreeNodeVM FromDataLink(DataLink link)
	{
		return new TreeNodeVM(link.Child, link, link.Child.LinksAsParent.Select(FromDataLink));
	}

	public static async Task<TreeNodeVM> FromDataObjectAsync(DataObject obj)
	{
		return await Task.Run(() => FromDataObject(obj));
	}

	private TreeNodeVM(DataObject source, DataLink? link, IEnumerable<TreeNodeVM> subNodes)
	{
		Source = source;
		Link = link;
		SubNodes = new ObservableCollection<TreeNodeVM>(subNodes);
		foreach (TreeNodeVM treeNode in SubNodes) treeNode.ParentNode = this;
		SubNodes.CollectionChanged += SubNodesOnCollectionChanged;
		this.WhenAnyValue(vm => vm.IsRoot, vm => vm.ProductType)
			.Select(x => x.Item1 && !string.IsNullOrWhiteSpace(x.Item2) ? Visibility.Visible : Visibility.Collapsed)
			.ToPropertyEx(this, vm => vm.TypeVisibility);
	}

	public string? LinkName => Link?.LinkName;
	public bool LinkIsVisible => Link != null;
	public string ProductName => Source.Product;
	public string ProductType => Source.Type;
	public DataObject Source { get; }
	public ObservableCollection<TreeNodeVM> SubNodes { get; }
	private TreeNodeVM? _parentNode;

	public TreeNodeVM? ParentNode
	{
		get => _parentNode;
		private set
		{
			_parentNode = value;
			this.RaisePropertyChanged();
			this.RaisePropertyChanged(nameof(IsRoot));
		}
	}

	public DataLink? Link { get; }
	public bool IsRoot => ParentNode == null;

	[ObservableAsProperty] public Visibility TypeVisibility { get; }

	public void NotifyPropertyChanged()
	{
		this.RaisePropertyChanged(nameof(ProductName));
		this.RaisePropertyChanged(nameof(ProductType));
		this.RaisePropertyChanged(nameof(LinkName));
		this.RaisePropertyChanged(nameof(LinkIsVisible));
	}

	private void SubNodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add)
		{
			foreach (var newItem in e.NewItems!)
			{
				var newTreeNode = (TreeNodeVM) newItem;
				newTreeNode.ParentNode = this;
			}
		}
		else if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			foreach (TreeNodeVM node in SubNodes) node.ParentNode = this;
		}
		
		this.RaisePropertyChanged(nameof(SubNodes));
	}

	public bool ThisOrAnyParentContainsDataObject(DataObject obj)
	{
		TreeNodeVM? currentNode = this;
		while (currentNode != null)
		{
			if (currentNode.Source == obj) return true;
			currentNode = currentNode.ParentNode;
		}
		return false;
	}
}
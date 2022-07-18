using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using AsconTestTask.Backend.Data.Members;
using ReactiveUI;

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
	}

	public string? LinkName => Link?.LinkName;
	public bool LinkIsVisible => Link != null;
	public string ProductName => Source.Product;
	public DataObject Source { get; }
	public ObservableCollection<TreeNodeVM> SubNodes { get; }
	public TreeNodeVM? ParentNode { get; private set; }
	public DataLink? Link { get; }

	public void NotifyPropertyChanged()
	{
		this.RaisePropertyChanged(nameof(ProductName));
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
			foreach (TreeNodeVM node in SubNodes)
			{
				node.ParentNode = this;
			}
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
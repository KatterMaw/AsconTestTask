using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using AsconTestTask.Backend.Data.Members;
using ReactiveUI;

namespace AsconTestTask.UI.WPF.ViewModels;

public class TreeNode : ReactiveObject
{
	public static TreeNode FromDataObject(DataObject obj)
	{
		return new TreeNode(obj, null, obj.LinksAsChild.Select(FromDataLink));
	}

	private static TreeNode FromDataLink(DataLink link)
	{
		return new TreeNode(link.Child, link, link.Child.LinksAsChild.Select(FromDataLink));
	}

	public static async Task<TreeNode> FromDataObjectAsync(DataObject obj)
	{
		return await Task.Run(() => FromDataObject(obj));
	}

	private TreeNode(DataObject source, DataLink? link, IEnumerable<TreeNode> subNodes)
	{
		Source = source;
		Link = link;
		SubNodes = new ObservableCollection<TreeNode>(subNodes);
		foreach (TreeNode treeNode in SubNodes) treeNode.ParentNode = this;
		SubNodes.CollectionChanged += SubNodesOnCollectionChanged;
	}

	public string Name => Link == null ? Source.Product : $"{Link.LinkName}\n{Source.Product}";
	public DataObject Source { get; }
	public ObservableCollection<TreeNode> SubNodes { get; }
	public TreeNode? ParentNode { get; private set; }
	public DataLink? Link { get; }


	private void SubNodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(SubNodes));
	}
}
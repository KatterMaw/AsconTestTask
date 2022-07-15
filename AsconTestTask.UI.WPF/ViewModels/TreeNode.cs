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
		return new TreeNode(obj, obj.Links.Select(link => FromDataObject(link.Child)));
	}

	public static async Task<TreeNode> FromDataObjectAsync(DataObject obj)
	{
		return await Task.Run(() => FromDataObject(obj));
	}

	private TreeNode(DataObject source, IEnumerable<TreeNode> subNodes)
	{
		Source = source;
		SubNodes = new ObservableCollection<TreeNode>(subNodes);
		SubNodes.CollectionChanged += SubNodesOnCollectionChanged;
	}

	public string Name => Source.Product;
	public DataObject Source { get; }
	public ObservableCollection<TreeNode> SubNodes { get; }
	
	
	private void SubNodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(SubNodes));
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using AsconTestTask.Backend.Data;
using AsconTestTask.Backend.Data.Members;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DataObject = AsconTestTask.Backend.Data.Members.DataObject;

namespace AsconTestTask.UI.WPF.ViewModels.Windows;

public class MainWindowVM : ReactiveObject
{
	#region Tree

	public ObservableCollection<TreeNodeVM> Nodes { get; }
	
	private void NodesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(Nodes));
	}

	private TreeNodeVM? _selectedNode;
	public TreeNodeVM? SelectedNode
	{
		get => _selectedNode;
		set
		{
			_selectedNode = value;
			if (value != null)
			{
				ObjectProduct = value.Source.Product;
				ObjectType = value.Source.Type;
				LinkName = value.Link?.LinkName;
			}
			else
			{
				LinkName = null;
			}
			this.RaisePropertyChanged();
		}
	}
	private ObservableAsPropertyHelper<bool> _anyNodeSelected;
	[ObservableAsProperty] public bool AnyNodeSelected { get; }

	
	private ReactiveCommand<Unit, Unit>? _addNewObjectCommand;
	public ReactiveCommand<Unit, Unit> AddNewObjectCommand => _addNewObjectCommand ??= ReactiveCommand.Create(() =>
	{
		var newObject = new DataObject {Product = "Новый продукт"};
		Nodes.Add(TreeNodeVM.FromDataObject(newObject));
		using var dbContext = new AppDbContext();
		dbContext.Objects.Add(newObject);
		dbContext.SaveChanges();
	});

	#endregion

	#region SelectedObjectProperties

	[Reactive] public string ObjectType { get; set; }
	[Reactive] public string ObjectProduct { get; set; }
	[Reactive] public string? LinkName { get; set; }

	private ObservableAsPropertyHelper<bool> _selectedObjectHasLink;
	[ObservableAsProperty] public bool SelectedObjectHasLink { get; }
	

	private ReactiveCommand<Unit, Unit>? _saveObjectPropertiesToDatabaseCommand;
	public ReactiveCommand<Unit, Unit> SaveObjectPropertiesToDatabaseCommand => _saveObjectPropertiesToDatabaseCommand ??= ReactiveCommand.Create(() =>
	{
		if (SelectedNode == null || !HasUnsavedChanges) return;
		DataObject selectedObject = SelectedNode.Source;
		DataLink? selectedLink = SelectedNode.Link;
		using var appDbContext = new AppDbContext();
		appDbContext.Attach(selectedObject);
		selectedObject.Product = ObjectProduct;
		selectedObject.Type = ObjectType;
		if (selectedLink != null)
		{
			appDbContext.Attach(selectedLink);
			selectedLink.LinkName = LinkName!;
		}
		appDbContext.SaveChanges();
		SelectedNode.NotifyPropertyChanged();
		HasUnsavedChanges = false;
	});
	
	[Reactive] public bool HasUnsavedChanges { get; set; }

	#endregion

	#region AddNewLinkOverlay

	public IEnumerable<DataObject>? AvailableDataObjects => SelectedNode == null ?
		null :
		Nodes.Select(node => node.Source).Where(obj => !SelectedNode!.ThisOrAnyParentContainsDataObject(obj) && SelectedNode.SubNodes.All(node => node.Source != obj));

	[Reactive] public bool AddLinkOverlayShouldBeVisible { get; set; }
	
	[Reactive] public DataObject? SelectedObjectToLink { get; set; }

	private ReactiveCommand<Unit, Unit>? _openAddNewLinkOverlayCommand;
	public ReactiveCommand<Unit, Unit> ShowAddNewLinkOverlayCommand => _openAddNewLinkOverlayCommand ??= ReactiveCommand.Create(() =>
	{
		if (SelectedNode == null) return;
		this.RaisePropertyChanged(nameof(AvailableDataObjects));
		AddLinkOverlayShouldBeVisible = true;
	});
	
	private ReactiveCommand<Unit, Unit>? _addNewLinkOverlayCommand;
	public ReactiveCommand<Unit, Unit> AddNewLinkOverlayCommand => _addNewLinkOverlayCommand ??= ReactiveCommand.Create(() =>
	{
		if (SelectedObjectToLink == null) return;
		using var dbContext = new AppDbContext();
		DataObject selectedObject = SelectedNode!.Source;
		var newLink = new DataLink {LinkName = "Новая связь", Parent = selectedObject, Child = SelectedObjectToLink};
		dbContext.Attach(selectedObject);
		dbContext.Attach(SelectedObjectToLink);
		dbContext.Add(newLink);
		dbContext.SaveChanges();
		
		SelectedNode!.SubNodes.Add(TreeNodeVM.FromDataObject(SelectedObjectToLink, newLink));
		AddLinkOverlayShouldBeVisible = false;
	}, this.WhenAnyValue(vm => vm.SelectedObjectToLink).Select(selectedObjectToLink => selectedObjectToLink != null));
	
	private ReactiveCommand<Unit, Unit>? _cancelAddingNewLinkCommand;
	public ReactiveCommand<Unit, Unit> CancelAddingNewLinkCommand => _cancelAddingNewLinkCommand ??= ReactiveCommand.Create(() =>
	{
		AddLinkOverlayShouldBeVisible = false;
	});
	
	#endregion

	#region TopBarMisc

	private ReactiveCommand<Unit, Unit>? _deleteSelectedObjectCommand;
	public ReactiveCommand<Unit, Unit> DeleteSelectedObjectCommand => _deleteSelectedObjectCommand ??= ReactiveCommand.Create(() =>
	{
		if (SelectedNode == null) return;
		using var dbContext = new AppDbContext();
		TreeNodeVM? nodeToDelete = SelectedNode;
		var isRootNode = nodeToDelete.ParentNode == null;
		if (isRootNode)
		{
			Nodes.Remove(nodeToDelete);
			dbContext.Objects.Remove(nodeToDelete.Source);
		}
		else
		{
			SelectedNode.ParentNode!.SubNodes.Remove(nodeToDelete);
			dbContext.Links.Remove(nodeToDelete.Link!);
		}
		dbContext.SaveChanges();
	}, this.WhenAnyValue(vm => vm.SelectedNode).Select(selectedNode => selectedNode != null));
	
	private ReactiveCommand<Unit, Unit>? _exportDbToXMLFileCommand;
	public ReactiveCommand<Unit, Unit> ExportDbToXMLFileCommand => _exportDbToXMLFileCommand ??= ReactiveCommand.Create(() =>
	{
		var dialog = new SaveFileDialog();
		dialog.RestoreDirectory = true ;
		if (dialog.ShowDialog() != DialogResult.OK) return;
		using var dbContext = new AppDbContext();
		using Stream stream = dialog.OpenFile();
		using var streamWriter = new StreamWriter(stream);
		var xmlSerializer = new XmlSerializer(typeof(DbDump), new[] {typeof(DataObject), typeof(DataLink), typeof(DataAttribute)});
		using var xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings { Indent = true });
		DbDump dump = dbContext.CreateDump();
		xmlSerializer.Serialize(xmlWriter, dump);
	});

	#endregion

	#region Attributes

	private ObservableCollection<AttributeVM>? _currentAttributes;
	public ObservableCollection<AttributeVM>? CurrentAttributes
	{
		get => _currentAttributes;
		set
		{
			if (_currentAttributes != null) _currentAttributes.CollectionChanged -= CurrentAttributesOnCollectionChanged;
			_currentAttributes = value;
			if (_currentAttributes != null) _currentAttributes.CollectionChanged += CurrentAttributesOnCollectionChanged;
			this.RaisePropertyChanged();
		}
	}

	private void CurrentAttributesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.RaisePropertyChanged(nameof(CurrentAttributes));
	}

	[Reactive] public AttributeVM? SelectedAttribute { get; set; }
	
	private ReactiveCommand<Unit, Unit>? _addNewAttributeCommand;
	public ReactiveCommand<Unit, Unit> AddNewAttributeCommand => _addNewAttributeCommand ??= ReactiveCommand.Create(() =>
	{
		using var dbContext = new AppDbContext();
		if (SelectedNode == null) return;
		dbContext.Attach(SelectedNode.Source);
		var newAttribute = new DataAttribute {Name = "Новый аттрибут",Value = string.Empty, Object = SelectedNode.Source};
		SelectedNode.Source.Attributes.Add(newAttribute);
		dbContext.SaveChanges();
		CurrentAttributes!.Add(new AttributeVM(newAttribute));
	});
	
	private ReactiveCommand<Unit, Unit>? _deleteAttributeCommand;
	public ReactiveCommand<Unit, Unit> DeleteAttributeCommand => _deleteAttributeCommand ??= ReactiveCommand.Create(() =>
	{
		using var dbContext = new AppDbContext();
		if (SelectedNode == null || SelectedAttribute == null) return;
		dbContext.Attach(SelectedNode.Source);
		SelectedNode.Source.Attributes.Remove(SelectedAttribute.Source);
		dbContext.Attributes.Remove(SelectedAttribute.Source);
		dbContext.SaveChanges();
		CurrentAttributes!.Remove(SelectedAttribute);
	}, this.WhenAnyValue(vm => vm.SelectedNode, vm => vm.SelectedAttribute).Select((selectedNode) => selectedNode.Item1 != null && selectedNode.Item2 != null));

	private ReactiveCommand<Unit, Unit>? _updateAttributesCollectionCommand;
	public ReactiveCommand<Unit, Unit> UpdateAttributesCollectionCommand => _updateAttributesCollectionCommand ??= ReactiveCommand.Create(() =>
	{
		CurrentAttributes = SelectedNode == null ? null : new ObservableCollection<AttributeVM>(SelectedNode.Source.Attributes.Select(attribute => new AttributeVM(attribute)));
	});

	#endregion


	public MainWindowVM()
	{
		using var dbContext = new AppDbContext();
		var objects = dbContext.Objects.Include(obj => obj.LinksAsParent).Include(obj => obj.LinksAsChild).Include(obj => obj.Attributes).ToList(); // if don't convert ToList() cause issue when object link child is null
		Nodes = new ObservableCollection<TreeNodeVM>(objects.Select(obj => TreeNodeVM.FromDataObject(obj, null)));
		Nodes.CollectionChanged += NodesOnCollectionChanged;
		
		_anyNodeSelected = this.WhenAnyValue(vm => vm.SelectedNode).Select(node => node != null)
			.ToPropertyEx(this, vm => vm.AnyNodeSelected);
		_selectedObjectHasLink = this.WhenAnyValue(vm => vm.LinkName).Select(linkName => linkName != null)
			.ToPropertyEx(this, vm => vm.SelectedObjectHasLink);

		this.WhenAnyValue(vm => vm.SelectedNode).Select(selectedNode => Unit.Default).InvokeCommand(UpdateAttributesCollectionCommand);

		this.WhenAnyValue(vm => vm.ObjectType, vm => vm.ObjectProduct, vm => vm.LinkName).Select(x => SelectedNode != null && (SelectedNode.Link != null && x.Item3 != SelectedNode.Link.LinkName || x.Item2 != SelectedNode.Source.Product || x.Item1 != SelectedNode.Source.Type))
			.BindTo(this, vm => vm.HasUnsavedChanges);
		this.WhenAnyValue(vm => vm.ObjectType, vm => vm.ObjectProduct, vm => vm.LinkName).Throttle(TimeSpan.FromMilliseconds(500)).Select(_ => Unit.Default)
			.InvokeCommand(SaveObjectPropertiesToDatabaseCommand);
	}
}

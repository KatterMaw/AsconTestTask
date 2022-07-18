using System;
using System.Reactive;
using System.Reactive.Linq;
using AsconTestTask.Backend.Data;
using AsconTestTask.Backend.Data.Members;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AsconTestTask.UI.WPF.ViewModels;

public class AttributeVM : ReactiveObject
{
	public DataAttribute Source { get; }
	
	[Reactive] public string AttributeName { get; set; }
	[Reactive] public string AttributeValue { get; set; }

	private ObservableAsPropertyHelper<bool> _hasUnsavedChanges;
	[ObservableAsProperty] public bool HasUnsavedChanges { get; }


	public AttributeVM(DataAttribute attribute)
	{
		Source = attribute;
		AttributeName = attribute.Name;
		AttributeValue = attribute.Value;
		_hasUnsavedChanges = this.WhenAnyValue(vm => vm.AttributeName, vm => vm.AttributeValue)
			.Select(x => x.Item1 != Source.Name || x.Item2 != Source.Value)
			.ToPropertyEx(this, vm => vm.HasUnsavedChanges);

		this.WhenAnyValue(vm => vm.AttributeName, vm => vm.AttributeValue).Throttle(TimeSpan.FromMilliseconds(500))
			.Select(x => Unit.Default).InvokeCommand(ReactiveCommand.Create(SaveToDb));
	}

	private void SaveToDb()
	{
		using var dbContext = new AppDbContext();
		dbContext.Attach(Source);
		Source.Name = AttributeName;
		Source.Value = AttributeValue;
		dbContext.SaveChanges();
		this.RaisePropertyChanged(nameof(HasUnsavedChanges));
	}

}

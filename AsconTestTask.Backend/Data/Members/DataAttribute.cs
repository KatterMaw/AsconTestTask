using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

[Keyless]
public class DataAttribute
{
	[Column("Id")]
	public DataObject Obj { get; }
	public string Name { get; }
	public string Value { get; }
}

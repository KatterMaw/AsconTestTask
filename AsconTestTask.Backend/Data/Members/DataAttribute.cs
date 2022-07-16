using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

public class DataAttribute
{
	public int Id { get; set; }
	public DataObject Obj { get; set; }
	public string Name { get; set; }
	public string Value { get; set; }
}

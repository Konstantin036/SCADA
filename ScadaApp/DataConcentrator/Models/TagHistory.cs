using System.ComponentModel.DataAnnotations;

public class TagHistory
{
    [Key]
    public int Id { get; set; }
    public string TagName { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}
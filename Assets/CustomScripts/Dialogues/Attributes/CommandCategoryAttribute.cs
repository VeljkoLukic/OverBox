using System;

public class CommandCategoryAttribute : Attribute
{
    public string CommandCategory { get; set; }

    public CommandCategoryAttribute (string commandCategory)
    {
        CommandCategory = commandCategory;
    }
}

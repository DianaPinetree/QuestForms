namespace QuestForms
{
    /// <summary>
    /// Enables the manager to find this object and save its answer to an exported format
    /// </summary>
    public interface IAnswerElement
    {
        string ID { get; set; }
        object Answer { get; }
    }

}
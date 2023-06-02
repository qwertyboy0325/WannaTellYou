public enum ERelation
{
    Friend,
    Couple,
    Stranger,
    None = -1
}
abstract public class Relation
{
    // store all questions
    public string[] Questions { get; protected set; }
    // Limit the result array size of getQuestion function.
    public int maxQuestionLimit { get; protected set; }

    // abstract the getting function (randomly get question depends on maxQuestionLimit)
    abstract public string[] getQuestions();
    //get single question which is not repeat in 
    abstract public string getQuestion(string[] current);
}

// The set of all relation
//public class RelationSets
//{

//    public Relation[] relations;
//    public RelationSets()
//    {
//        relations = new Relation[3];
//        relations[0] = new Friend();
//        relations[1] = new Couple();
//        relations[2] = new Stranger();
//    }
//}
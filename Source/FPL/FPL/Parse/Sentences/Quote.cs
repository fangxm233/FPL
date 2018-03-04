namespace FPL.Parse.Sentences
{
    public class Quote : Sentence
    {
        public Quote(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            return this;
        }
    }
}

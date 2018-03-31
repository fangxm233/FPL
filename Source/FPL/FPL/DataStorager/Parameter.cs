namespace FPL.DataStorager
{
    public class Parameter
    {
        public Type Type;
        public string Name;

        public Parameter()
        {

        }

        #region operator

        public static bool operator ==(Parameter l, Type r)
        {
            return l.Type == r;
        }
        public static bool operator !=(Parameter l, Type r)
        {
            return !(l == r);
        }
        public static bool operator ==(Parameter l, Parameter r)
        {
            return l.Type == r.Type;
        }
        public static bool operator !=(Parameter l, Parameter r)
        {
            return !(l == r);
        }


        #endregion
    }
}

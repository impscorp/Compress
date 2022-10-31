namespace Compress;

//node
public class Node
{
    #region Properties
    public char Character { get; private set; }
        public int Frequency { get; private set; }
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        public bool IsLeaf { get { return Left == null && Right == null; } }
        
    #endregion
    #region Constructors

    public Node(char character, int frequency, Node left = null, Node right = null)
    {
        Character = character;
        Frequency = frequency;
        Left = left;
        Right = right;
    }
    
    #endregion

    #region Methods
    


    #endregion
}
using System.Collections;
using System.Text;

namespace Compress;

public class Encode
{

    //huffman encoding algorithm
    public static void EncodeString(string input, string path, string savepath)
    {
        //get the frequency of each character
        Dictionary<char, int> freq = new Dictionary<char, int>();
        foreach (char c in input)
        {
            if (freq.ContainsKey(c))
                freq[c]++;
            else
                freq.Add(c, 1);
        }
        freq = freq.OrderBy(x => x.Value).ToDictionary(dict => dict.Key, dict => dict.Value);
        //build the tree
        Node root = BuildTree(freq);
        //build the code table
        Dictionary<char, string> codeTable = new Dictionary<char, string>();
        BuildCodeTable(root, codeTable, "");
        SaveCodeTable(codeTable, path);
        //encode the input
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            sb.Append(codeTable[c]);
        }
        SaveFile(sb.ToString(), savepath);
    }
    
    private static Node BuildTree(Dictionary<char, int> freq)
    {
        //create a leaf node for each character
        List<Node> nodes = new List<Node>();
        foreach (KeyValuePair<char, int> kvp in freq)
        {
            nodes.Add(new Node(kvp.Key, kvp.Value));
        }

        //build the tree
        while (nodes.Count > 1)
        {
            //get the two lowest frequency nodes
            List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();
            if (orderedNodes.Count >= 2)
            {
                //create a parent node by combining the lowest two nodes
                List<Node> taken = orderedNodes.Take(2).ToList();
                Node parent = new Node('\0', taken[0].Frequency + taken[1].Frequency, taken[0], taken[1]);
                nodes.Remove(taken[0]);
                nodes.Remove(taken[1]);
                nodes.Add(parent);
            }
            else
            {
                Node root = orderedNodes.FirstOrDefault();
                return root;
            }
        }
        return nodes.FirstOrDefault();
    }
    
    //build the code table
    public static void BuildCodeTable(Node node, Dictionary<char, string> codeTable, string code)
    {
        if (node.IsLeaf)
        {
            codeTable.Add(node.Character, code);
        }
        else
        {
            BuildCodeTable(node.Left, codeTable, code + "0");
            BuildCodeTable(node.Right, codeTable, code + "1");
        } 
    }
    
    //Save the code table and the string to a byte file
    public static void SaveCodeTable(Dictionary<char, string> codeTable, string path)
    {
        //binary codeTable
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<char, string> kvp in codeTable)
        {
            sb.Append(kvp.Key);
            sb.Append(kvp.Value + "\n");
            
        }
        byte[] codeTableBytes = Encoding.ASCII.GetBytes(sb.ToString());
        //write the code table to a file
        File.WriteAllBytes(path, codeTableBytes);
    }
    
    public static void SaveFile(string code, string savingPath = "huff")
    {
        try
        {
            List<string> str = new List<string>();
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < code.Length - code.Length % 8 ; i += 8)
            {
                str.Add(code.Substring(i, 8));
            }
            if (code.Length % 8 != 0)
            {
                str.Add(code.Substring(code.Length - code.Length % 8));
            }
            foreach (string s in str)
            {
                bytes.Add(Convert.ToByte(s, 2));
            }
            File.WriteAllBytes(savingPath, bytes.ToArray());

        }
        catch (DirectoryNotFoundException ex) { throw new DirectoryNotFoundException("Dictionary not found", ex); }
    }
    
    //load the code table from a file
    public static Dictionary<string, string> LoadCodeTable(string path)
    {
        Dictionary<string, string> codeTable = new Dictionary<string, string>();
        var file = File.ReadAllLines(path);
        for (int i = 0; i < file.Length; i++)
        {
            if (file[i] == "")
            {
                codeTable.Add("\n", file[i + 1]);
                i++;
            }
            else
            {
                codeTable.Add(file[i].Substring(0, 1), file[i].Substring(1));  
            }

        }
        return codeTable;
    }
    
    //use code table to decode the string from a file
    public static string DecodeString(string path, string filepath)
    {
        Dictionary<string, string> codeTable = LoadCodeTable(path);
        byte[] fileBytes = File.ReadAllBytes(filepath);
        
        StringBuilder sb = new StringBuilder();
        //filebytes to string
        foreach (byte b in fileBytes)
        {
            sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        
        string fileString = sb.ToString();

        string code = "";
        sb.Clear();
        
        foreach (char c in fileString)
        {
            code += c;
            if (codeTable.ContainsValue(code))
            {   
                sb.Append(codeTable.FirstOrDefault(x => x.Value == code).Key);
                code = "";
            }
        }
        return sb.ToString();
    }

}
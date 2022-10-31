using System.Collections;
using System.Text;

namespace Compress;

public class Encode
{

    //huffman encoding algorithm
    public static void EncodeString(byte[] input, string path)
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
        SaveFile(sb.ToString());
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
            sb.Append(kvp.Key + "\n");
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

            //Create byteArray with the length of the chars of the parameter code and divided by 8 because 8 bits are 1 byte
            byte[] byteArray = new byte[code.ToCharArray().Length / 8];
            byte currentByte = 0, count = 0, actuallyCount = 0;

            //To effectively save data we add as many bits as possible into a byte 
            //Iterate through every character from the code variable
            code.ToCharArray().ToList().ForEach(character =>
            {
                //If the character is 1, the variable currentBytes binary is set to 1
                if (character == '1') currentByte |= 1; //example 0000 0001;
                //1 byte are 8 bits because of this we count until we reach 7 
                if (count < 8)
                {
                    currentByte <<= 1; //Shifts the binary code of the currentByte to the left by 1 example 0000 0010;
                    count++;
                }
                //If count has reached 7 we created a full byte now we can add this byte to the byteArray
                else
                {
                    count = 0; //Reset variable count for the next byte
                    byteArray[actuallyCount] = currentByte; //Adds the currentByte to the byteArray
                    currentByte = 0; //Reset variable currentByte for the next bit shifts
                    actuallyCount++;
                }
            });

            File.WriteAllBytes(savingPath, byteArray);
        }
        catch (DirectoryNotFoundException ex) { throw new DirectoryNotFoundException("Dictionary not found", ex); }
    }
    
    //load the code table from a file
    public static Dictionary<char, string> LoadCodeTable(string path)
    {
        Dictionary<char, string> codeTable = new Dictionary<char, string>();
        byte[] codeTableBytes = File.ReadAllBytes(path);
        string codeTableString = Encoding.ASCII.GetString(codeTableBytes);
        string[] codeTableArray = codeTableString.Split("\n");
        for (int i = 0; i < codeTableArray.Length; i++)
        {
            codeTable.Add(codeTableArray[i][0], codeTableArray[i + 1]);
            i++;
        }
        return codeTable;
    }
    
    //decode the input
    public static string DecodeString(string path1 , string path2)
    {
        Dictionary<char, string> codeTable = LoadCodeTable(path2);
        var input = File.ReadAllBytes(path1);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in input)
        {
            sb.Append(b);
        }
        string inputString = sb.ToString();
        StringBuilder output = new StringBuilder();
        string code = "";
        foreach (char c in inputString)
        {
            code += c;
            if (codeTable.ContainsValue(code))
            {
                output.Append(codeTable.FirstOrDefault(x => x.Value == code).Key);
                code = "";
            }
        }
        //return Encoding.ASCII.GetBytes(output.ToString());
        return output.ToString();
    }

}
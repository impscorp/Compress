using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace Compress;

public class Encode
{
    public string codeTablepath { get; set; }
    public string Loadpath{ get; set; }
    public Encode()
    {
    }
   
    /// <summary>
    /// The function to encode the input file into a huffman encoded string
    /// </summary>
    /// <param name="loadpath">The path to the input file</param>
    /// <param name="treepath">The path were the tree is being saved to</param>
    /// <param name="savepath">The path were the encoded file will be saved to</param>
    public void EncodeString(string loadpath, string treepath, string savepath)
    {
        //get the frequency of each character
        Loadpath = loadpath;
        var input = File.ReadAllText(Loadpath!);
        if (input.Length == 0)
        {
            return;
        }
        input = input.Replace("\r", "");
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
        SaveCodeTable(codeTable);
        //encode the input
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            sb.Append(codeTable[c]);
        }
        
        SaveFile(sb.ToString(), savepath);
    }
   
    /// <summary>
    /// The function to build the huffman tree
    /// </summary>
    /// <param name="freq">the Freq Dictionary from the input file</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Build the code table from the tree
    /// </summary>
    /// <param name="node">The tree</param>
    /// <param name="codeTable">empty Dictionary from encode method</param>
    /// <param name="code">adds 0 or 1</param>
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
    
    /// <summary>
    /// save the code table to a file in a temp folder
    /// </summary>
    /// <param name="codeTable">the code table to save</param>
    public void SaveCodeTable(Dictionary<char, string> codeTable)
    {
        //binary codeTable
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<char, string> kvp in codeTable)
        {
            sb.Append(kvp.Key);
            sb.Append(kvp.Value + "\n");
        }
        //write the code table to a file
        // string tempPath = Path.GetTempPath();
        // //macos get access to the temp folder
        // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        // {
        //     tempPath = "/private" + tempPath;
        // }
        //get execution path
        string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string filename = Path.GetFileNameWithoutExtension(Loadpath);
        string codeTablePath = exePath + filename + "_codeTable.tree";
        codeTablepath = codeTablePath;
        File.WriteAllText(codeTablePath, sb.ToString());
    }
    
    /// <summary>
    /// saves the encoded string to a file
    /// </summary>
    /// <param name="code">The encoded string</param>
    /// <param name="savingPath"></param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public void SaveFile(string code, string savingPath)
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
    
    /// <summary>
    /// load the code table from a file
    /// </summary>
    /// <param name="path"></param>
    /// <returns>the codetable that is needed to decode the file</returns>
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
    
    /// <summary>
    /// decode the file
    /// </summary>
    /// <param name="treepath">the path to the tree</param>
    /// <param name="filepath">the path to the file</param>
    /// <returns>the decoded string</returns>
    public static string DecodeString(string treepath, string filepath)
    {
        Dictionary<string, string> codeTable = LoadCodeTable(treepath);
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="originalPath"></param>
    /// <param name="decodedPath"></param>
    /// <returns></returns>
    public static double CompareFiles(string originalPath, string decodedPath)
    {
        FileInfo original = new FileInfo(originalPath);
        FileInfo decoded = new FileInfo(decodedPath);
        double difference = (original.Length - decoded.Length) / (double)original.Length * 100;
        return difference;
    }
    
}
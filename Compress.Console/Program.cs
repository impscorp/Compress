using System.Text;
using Compress;

Encode ec = new Encode();
var input = File.ReadAllText("kühl copy.rtf");

Encode.EncodeString(input, "tree","tree");
var y = Encode.DecodeString("tree", "huff");
Console.WriteLine(y);




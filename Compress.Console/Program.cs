using System.Text;
using Compress;

Encode ec = new Encode();
var x = File.ReadAllText("kühl copy.rtf");
byte[] input = Encoding.ASCII.GetBytes(x);
Encode.EncodeString(input, "tree");




# What is Compress?
Compress is a simple tool to compress txt and rtf files. </br>
The Ui was built with the Avalonia Framework and is Crossplatform
# How to use Compress?
i demonstrated how to use the programm in the following video
#
https://user-images.githubusercontent.com/113507940/201334123-c8f6e50a-b8b4-48ef-b93c-45faa0520635.mp4
#
# How does it work?
## Encode
the programm reads the text of the input file and builds a binary tree out of it.
According to the Huffman coding every char in the tree gets a code like "H = 101". https://en.wikipedia.org/wiki/Huffman_coding </br>
Then the encoded string gets turned into bytes and saved as a .huff.
The tree that was used to encode the file gets saved in the runtime folder.
## Decode
to Deode a .huff file the programm needs to load the .tree file form the runtime folder.
after that the .huff file is being read bit by bit and once the code matches the code in the tree it gets translatet to the corresponding a char.

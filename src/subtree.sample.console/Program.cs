// See https://aka.ms/new-console-template for more information
using subtree;
using System.Collections;

Console.WriteLine("Hello, World!");

// create root subtree
var subtree = new Subtree();

// tile availability
var t0 = BitArrayCreator.FromString("10110000");
var t1 = BitArrayCreator.FromString("01001100");
var t2 = BitArrayCreator.FromString("10000000");
subtree.TileAvailability = new List<BitArray>() { t0, t1, t2};

// subtree avaiability
var c0 = BitArrayCreator.FromString("00000000");
var c1 = BitArrayCreator.FromString("00000000");
var c2 = BitArrayCreator.FromString("01100000");
var c3 = BitArrayCreator.FromString("00000110");
var c4 = BitArrayCreator.FromString("01100000");
var c5 = BitArrayCreator.FromString("00000110");
var c6 = BitArrayCreator.FromString("00000000");
var c7 = BitArrayCreator.FromString("00000000");

subtree.ChildSubtreeAvailability = new List<BitArray>() { c0, c2, c3, c4, c5, c6, c7};












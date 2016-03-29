using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapFileReader {
	//Reads a text file containing a map and return a list of Tile objects.
	//A map can be any size but all its columns must have the same length and so must all its rows.
	//A map should be envelopped by walls.
	//A map must contains one Pac-Man starting tile
	//The following symbols can be used to create new maps:
	//	W = Wall
	//	T = Walkable tile
	//	D = Walkable doted tile
	//	B = Walkable boosted tile
	//	P = Walkable Pac-Man starting tile
	//	S = Walkable dumb A Star ai ghost starting tile
	//	A = Walkable wise A Star ai ghost starting tile
	public static List<Tile> Read(TextAsset mapFile) {
		List<Tile> tileList = new List<Tile>();
		List<string> lineList = new List<string>();
		string theWholeFileAsOneLongString = "";
		theWholeFileAsOneLongString = mapFile.text.ToUpper();
		lineList.AddRange (theWholeFileAsOneLongString.Split ("\n" [0]));
		int coordX = 0;
		int coordY = lineList.Count - 1;
		foreach (string s in lineList) {
			foreach (char c in s) {
				switch(c) {
					case 'W':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Wall));
						break;
					case 'T':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Walkable));
						break;
					case 'D':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Doted));
						break;
					case 'B':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Boosted));
						break;
					case 'P':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.PacManStartingPosition));
						break;
					case 'S':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.GhostStartingPositionDumbAStarAi));
						break;
					case 'A':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.GhostStartingPositionWiseAStarAi));
						break;
					default:
						break;
				}
				coordX++;
			}
			coordX = 0;
			coordY--;
		}
		return tileList;
	}
}
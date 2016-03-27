using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapFileReader {
	//Reads a text file containing a map and return a list of Tile objects.
	//A map can be any size but all its columns must have the same length and so must all its rows.
	//A map should be envelopped by walls.
	//A map must contains one Pac-Man starting tile
	//A map must contains at least as many ghost starting tiles as there are ghosts placed in the scene
	//The following symbols can be used to create new maps:
	//	W = Wall
	//	U = Walkable tile
	//	T = Walkable doted tile
	//	B = Walkable boosted tile
	//	P = Walkable Pac-Man starting tile
	//	R = Walkable random ai ghost starting tile
	//	S = Walkable simple ai ghost starting tile
	//	A = Walkable A* ai ghost starting tile
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
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Doted));
						break;
					case 'U':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Walkable));
						break;
					case 'B':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.Boosted));
						break;
					case 'P':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.PacManStartingPosition));
						break;
					case 'R':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.GhostStartingPositionRandomAi));
						break;
					case 'S':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.GhostStartingPositionSimpleAi));
						break;
					case 'A':
						tileList.Add(new Tile(coordX, coordY, Tile.AvailableTileTypes.GhostStartingPositionAStarAi));
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
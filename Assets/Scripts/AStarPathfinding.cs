using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AStarPathfinding {
	
	//Generates a path to a location (A Star Algorithm, I didn't invent this)
	public static List<Tile> GeneratePath(Tile currentTile, Tile targetTile, GameObject grid) {
		//Verify that destination is walkable
		if (targetTile.TileType == Tile.AvailableTileTypes.Wall) {
			return null;
		}
		//Initialize variables
		Dictionary<int, int> distance = new Dictionary<int, int>();
		Dictionary<int, int> previousTile = new Dictionary<int, int>();
		List<int> unvisited = new List<int>(); //A list of nodes that haven't been visited yet
		int currentTileIndex = grid.GetComponent<GridManager>().WalkableTileList.IndexOf(currentTile);
		distance[currentTileIndex] = 0;	//Reference the source tile distance to itself in the dictionnaries
		previousTile[currentTileIndex] = -1;	//Reference the source tile previous tile in the dictionnaries
		//Initialize every tile to have INFINITY distance
		foreach(Tile t in grid.GetComponent<GridManager>().WalkableTileList) {
			int tIndex = grid.GetComponent<GridManager> ().WalkableTileList.IndexOf (t);
			if(t != currentTile) {
				distance[tIndex] = int.MaxValue;
				previousTile[tIndex] = -1;
			}
			unvisited.Add(tIndex);
		}
		//Visit every tile
		while(unvisited.Count > 0) {
			Tile u = null; //"u" is going to be the unvisited node with the smallest distance.
			int uIndex = 0;
			foreach(int possibleU in unvisited) {
				if(u == null || distance[possibleU] < distance[grid.GetComponent<GridManager>().WalkableTileList.IndexOf(u)]) {
					u = grid.GetComponent<GridManager>().WalkableTileList.ElementAt(possibleU);
					uIndex = possibleU;
				}
			}
			if(u == targetTile) {
				break;
			}
			unvisited.Remove(uIndex);
			foreach(TileDirectionPair v in u.NonWallNeighborTilesDirectionPair) {
				int alt = distance[uIndex] + 1;
				int vTileIndex = grid.GetComponent<GridManager> ().WalkableTileList.IndexOf (v.Tile);
				if(alt < distance[vTileIndex]) {
					distance[vTileIndex] = alt;
					previousTile[vTileIndex] = uIndex;
				}
			}
		}
		//Getting here means we found the shortest route or there is no route to our target
		if(previousTile[grid.GetComponent<GridManager>().WalkableTileList.IndexOf(targetTile)] == -1) {	//If no route between our target and the source
			return null;
		}
		Tile curr = targetTile;
		List<Tile> path = new List<Tile> ();
		//Step through the "prev" chain and add it to our path
		while(curr != null) {
			path.Add(curr);
			int currIndex = grid.GetComponent<GridManager> ().WalkableTileList.IndexOf (curr);
			if (previousTile[currIndex] != -1) {
				curr = grid.GetComponent<GridManager>().WalkableTileList.ElementAt(previousTile[currIndex]);
			} else {
				curr = null;
			}
		}
		//Invert the path as it is currently stored backward
		path.Reverse();
		return path;
	}
}

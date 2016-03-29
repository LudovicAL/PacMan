using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tile {

	public enum AvailableTileTypes {
		Wall,
		Walkable,
		Doted,
		Boosted,
		PacManStartingPosition,
		GhostStartingPositionDumbAStarAi,
		GhostStartingPositionWiseAStarAi
	};

	private int coordX;
	private int coordY;
	private AvailableTileTypes tileType;
	private GameObject gObject;
	private Tile[] neighborTiles;	//Where values appear in the following order: Top, Right, Bottom, Left
	private List<TileDirectionPair> nonWallNeighborTilesDirectionPair;


	//Constructors
	public Tile(int coordX, int coordY, AvailableTileTypes tileType, GameObject gObject) {
		this.coordX = coordX;
		this.coordY = coordY;
		this.tileType = tileType;
		this.gObject = gObject;
		this.neighborTiles = new Tile[4];
		this.nonWallNeighborTilesDirectionPair = new List<TileDirectionPair> ();
	}

	public Tile(int coordX, int coordY, AvailableTileTypes tileType) : this(coordX, coordY, tileType, null) {
	}

	public Tile(int coordX, int coordY) : this(coordX, coordY, AvailableTileTypes.Wall, null) {
	}

	//Redefines the equality test (in order for the test to examine coordinates only)
	public override bool Equals(object other) {
		if (other != null && this.GetType() == other.GetType()) {
			Tile tempo = (Tile) other;
			if (this.coordX == tempo.coordX && this.coordY == tempo.coordY) {
				return true;
			}
		}
		return false;
	}

	//Mandatory when redefining Equals
	public override int GetHashCode() {
		return base.GetHashCode ();
	}

	//Calculates the distance from this tile to another one
	public float DistanceTo(Tile other) {
		return Vector2.Distance(
			new Vector2(gObject.transform.position.x, gObject.transform.position.y),
			new Vector2(other.gObject.transform.position.x, other.gObject.transform.position.y)
		);
	}

	//Return the direction of a specified neighbor tile where 0 is up, 1 is right, 2 is bottom, 3 is left and -1 means no result
	public int GetNeighborTileDirection(Tile tile) {
		return Array.IndexOf (neighborTiles, tile);
	}

	//Properties
	public int CoordX {
		get {
			return coordX;
		}
		set {
			coordX = value;
		}
	}

	public int CoordY {
		get {
			return coordY;
		}
		set {
			coordY = value;
		}
	}

	public AvailableTileTypes TileType {
		get {
			return tileType;
		}
		set {
			tileType = value;
		}
	}

	public GameObject GObject {
		get {
			return gObject;
		}
		set {
			gObject = value;
		}
	}

	public Tile[] NeighborTiles {
		get {
			return neighborTiles;
		}
		set {
			if (value.Length == 4) {
				neighborTiles = value;
			} else {
				Debug.Log ("Cannot assign this value to neighborTiles");
			}
		}
	}

	public List<TileDirectionPair> NonWallNeighborTilesDirectionPair {
		get {
			return nonWallNeighborTilesDirectionPair;
		}
		set {
			nonWallNeighborTilesDirectionPair = value;
		}
	}
}

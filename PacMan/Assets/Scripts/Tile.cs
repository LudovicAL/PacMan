using UnityEngine;
using System.Collections;

public class Tile {

	public enum AvailableTileTypes {
		Wall,
		Walkable,
		Doted,
		Boosted,
		PacManStartingPosition,
		GhostStartingPositionRandomAi,
		GhostStartingPositionSimpleAi,
		GhostStartingPositionAStarAi
	};

	private int coordX;
	private int coordY;
	private AvailableTileTypes tileType;
	private GameObject gObject;
	private Tile[] neighborTiles;	//Where values appear in the following order: Top, Right, Bottom, Left

	public Tile(int coordX, int coordY, AvailableTileTypes tileType, GameObject gObject) {
		this.coordX = coordX;
		this.coordY = coordY;
		this.tileType = tileType;
		this.gObject = gObject;
		this.neighborTiles = new Tile[4];
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
}

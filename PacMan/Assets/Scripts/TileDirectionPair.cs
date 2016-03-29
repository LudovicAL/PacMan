using UnityEngine;
using System.Collections;


//As System.Tuple isn't available in Unity, I created a class for the specific use of pairing directions and tiles
public class TileDirectionPair {

	private int direction;
	private Tile tile;

	public TileDirectionPair(int direction, Tile tile) {
		this.direction = direction;
		this.tile = tile;
	}

	//Properties
	public int Direction {
		get {
			return direction;
		}
		set {
			direction = value;
		}
	}

	public Tile Tile {
		get {
			return tile;
		}
		set {
			tile = value;
		}
	}
}

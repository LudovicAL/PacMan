using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject dotPrefab;
	public GameObject boostPrefab;
	public GameObject ghostsContainer;
	public GameObject ghostRandomAiPrefab;
	public GameObject ghostSimpleAiPrefab;
	public GameObject ghostAStarAiPrefab;
	public GameObject pacManContainer;
	public GameObject pacManPrefab;
	public TextAsset mapFile;

	private List<Tile> tileList;
	private List<GameObject> ghostList;

	// Use this for initialization
	void Awake () {
		//Retrieving grid data
		tileList = new List<Tile> ();
		foreach(Transform child in this.transform) {
			int coordX = (int)child.position.x;
			int coordY = (int)child.position.y;
			Tile.AvailableTileTypes tileType;
			switch(child.tag) {
				case "Walkable":
					tileType = Tile.AvailableTileTypes.Walkable;
					break;
				case "Doted":
					tileType = Tile.AvailableTileTypes.Doted;
					break;
				case "Boosted":
					tileType = Tile.AvailableTileTypes.Boosted;
					break;
				case "PacManStartingPosition":
					tileType = Tile.AvailableTileTypes.PacManStartingPosition;
					break;
				case "GhostStartingPositionRandomAi":
					tileType = Tile.AvailableTileTypes.GhostStartingPositionRandomAi;
					break;
				case "GhostStartingPositionSimpleAi":
					tileType = Tile.AvailableTileTypes.GhostStartingPositionSimpleAi;
					break;
				case "GhostStartingPositionAStarAi":
					tileType = Tile.AvailableTileTypes.GhostStartingPositionAStarAi;
					break;
				default:
					tileType = Tile.AvailableTileTypes.Wall;
					break;
			}
			GameObject gObject = child.gameObject;
			tileList.Add (new Tile (coordX, coordY, tileType, gObject));
		}
		foreach(Tile tile in tileList) {
			FindNeighbors(tile);
		}
		//Retrieving ghost data
		ghostList = new List<GameObject> ();
		foreach(Transform child in ghostsContainer.transform) {
			ghostList.Add (child.gameObject);
			child.GetComponent<Ghost> ().CurrentTile = FindTileByCoordinates ((int)child.transform.position.x, (int)child.transform.position.y);
		}
		//Retrieving pacMan data
		foreach(Transform child in pacManContainer.transform) {
			child.GetComponent<PacManControls> ().CurrentTile = FindTileByCoordinates ((int)child.transform.position.x, (int)child.transform.position.y);
		}
		PositionMainCamera ();
	}

	// Use this for initialization
	void Start() {
		
	}

	// Update is called once per frame
	void Update () {
	}

	//Delete all child objects of this transform
	public void ClearTransform(Transform t) {
		var childList = new List<GameObject>();
		foreach(Transform child in t) {
			childList.Add(child.gameObject);
		}
		childList.ForEach(child => DestroyImmediate(child));
	}

	//Build the grid (reads the txt file, builds the grid, find every tile's neighbors and places dots and boosts)
	public void BuildGrid() {
		ClearTransform (this.transform);
		ClearTransform (ghostsContainer.transform);
		ClearTransform (pacManContainer.transform);
		if (tilePrefab != null && mapFile != null) {
			tileList = MapFileReader.Read (mapFile);
			foreach(Tile tile in tileList) {
				BuildTile (tile);
			}
		} else {
			Debug.Log("The script component is missing a reference.");
		}
		foreach(Tile tile in tileList) {
			FindNeighbors(tile);
			AssignWallsSprite (tile);
			SpawnGhost (tile);
			SpawnPacMan (tile);
		}
		PositionMainCamera ();
	}

	//Builds a single tile and reference it in the tileList
	public void BuildTile(Tile tile) {
		GameObject tileSprite = (GameObject)Instantiate (tilePrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
		tileSprite.transform.SetParent(this.transform);
		tile.GObject = tileSprite;
		tileSprite.GetComponent<SpriteRenderer> ().color = Color.black;
		switch(tile.TileType) {
			case Tile.AvailableTileTypes.Wall:
				tileSprite.GetComponent<SpriteRenderer> ().color = Color.blue;
				tileSprite.tag = "Wall";
				break;
			case Tile.AvailableTileTypes.Walkable:
				tileSprite.tag = "Walkable";
				break;
			case Tile.AvailableTileTypes.Doted:
				tileSprite.tag = "Doted";
				GameObject tileDot = (GameObject)Instantiate (dotPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				tileDot.transform.SetParent (tile.GObject.transform);
				break;
			case Tile.AvailableTileTypes.Boosted:
				tileSprite.tag = "Boosted";
				GameObject tileBoost = (GameObject)Instantiate (boostPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				tileBoost.transform.SetParent (tile.GObject.transform);
				break;
			case Tile.AvailableTileTypes.PacManStartingPosition:
				tileSprite.tag = "PacManStartingPosition";
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionRandomAi:
				tileSprite.tag = "GhostStartingPositionRandomAi";
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionSimpleAi:
				tileSprite.tag = "GhostStartingPositionSimpleAi";
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionAStarAi:
				tileSprite.tag = "GhostStartingPositionAStarAi";
				break;
		}
	}

	//Position the main camera on the game field and scales it
	public void PositionMainCamera() {
		Vector3 firstTilePosition = tileList.ElementAt(0).GObject.transform.position;
		Vector3 lastTilePosition = tileList.ElementAt(tileList.Count - 1).GObject.transform.position;
		Camera.main.transform.position = ((firstTilePosition - lastTilePosition) * 0.5f) + lastTilePosition;
		Camera.main.transform.Translate (new Vector3 (0, 0, -10));
		Camera.main.orthographicSize = 1;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		while (!GeometryUtility.TestPlanesAABB (planes, tileList.ElementAt (0).GObject.GetComponent<SpriteRenderer> ().bounds)) {
			planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
			Camera.main.orthographicSize += 0.5f;
		}
	}

	//Finds a tile neighbors and store the data in an array that is copied to the tile reference object
	public void FindNeighbors(Tile tile) {
		Tile[] neighborTiles = new Tile[4];
		neighborTiles[0] = FindTileByCoordinates (tile.CoordX, tile.CoordY + 1);
		neighborTiles[1] = FindTileByCoordinates (tile.CoordX + 1, tile.CoordY);
		neighborTiles[2] = FindTileByCoordinates (tile.CoordX, tile.CoordY - 1);
		neighborTiles[3] = FindTileByCoordinates (tile.CoordX - 1, tile.CoordY);
		tile.NeighborTiles = neighborTiles;
	}

	//Assigns the best suited sprite to a tile
	public void AssignWallsSprite(Tile tile) {
		if (tile.TileType == Tile.AvailableTileTypes.Wall) {
			var sprite = gameObject.GetComponent<WallSpriteAssignator> ().GetWallSprite(tile);
			if (sprite != null) {
				tile.GObject.GetComponent<SpriteRenderer> ().sprite = sprite;
			}
		}
	}

	//Spawns Pac-Man
	private void SpawnPacMan(Tile tile) {
		if (tile.TileType == Tile.AvailableTileTypes.PacManStartingPosition) {
			GameObject pacMan = (GameObject)Instantiate (pacManPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
			pacMan.transform.SetParent (pacManContainer.transform);
			pacMan.GetComponent<PacManControls> ().grid = this.gameObject;
		}
	}

	//Spawns the ghosts
	private void SpawnGhost(Tile tile) {
		switch (tile.TileType) {
			case Tile.AvailableTileTypes.GhostStartingPositionRandomAi:
				GameObject ghostRandomAi = (GameObject)Instantiate (ghostRandomAiPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				ghostRandomAi.transform.SetParent (ghostsContainer.transform);
				ghostRandomAi.GetComponent<GhostRandomAi> ().grid = this.gameObject;
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionSimpleAi:
				GameObject ghostSimpleAi = (GameObject)Instantiate (ghostSimpleAiPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				ghostSimpleAi.transform.SetParent(ghostsContainer.transform);
				ghostSimpleAi.GetComponent<GhostSimpleAi> ().grid = this.gameObject;
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionAStarAi:
				GameObject ghostAStarAi = (GameObject)Instantiate (ghostAStarAiPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				ghostAStarAi.transform.SetParent(ghostsContainer.transform);
				ghostAStarAi.GetComponent<GhostAStarAi> ().grid = this.gameObject;
				break;
		}
	}

	//Finds a tile by its coordinates
	public Tile FindTileByCoordinates(int coordX, int coordY) {
		return tileList.Find(t => t.Equals(new Tile(coordX, coordY)));
	}

	//Returns a list of all tiles of the specified type
	public List<Tile> GetTilesOfType(Tile.AvailableTileTypes type) {
		List<Tile> tilesOfType = new List<Tile> ();
		foreach (Tile tile in tileList) {
			if (tile.TileType == type) {
				tilesOfType.Add (tile);
			}		
		}
		return tilesOfType;
	}

	//Returns the closest non wall Tile to a GameObject
	public Tile FindClosestNonWallTile(GameObject obj) {
		Tile tempoTile = null;
		foreach (Tile tile in tileList) {
			if (tile.TileType != Tile.AvailableTileTypes.Wall) {
				if (tempoTile == null) {
					tempoTile = tile;
				} else if (Vector3.Distance(tile.GObject.transform.position, obj.transform.position) < Vector3.Distance(tempoTile.GObject.transform.position, obj.transform.position)) {
					tempoTile = tile;
				}
			}
		}
		return tempoTile;
	}

	//Properties
	public List<Tile> TileList {
		get {
			return tileList;
		}
	}
}

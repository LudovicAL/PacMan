using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject dotPrefab;
	public GameObject boostPrefab;
	public GameObject ghostsContainer;
	public GameObject ghostDumbAStarAiPrefab;
	public GameObject ghostWiseAStarAiPrefab;
	public GameObject pacManContainer;
	public GameObject pacManPrefab;
	public TextAsset[] mapFile;

	private List<Tile> tileList;
	private List<Tile> walkableTileList;
	private GameObject pacMan;

	// Use this for initialization
	void Awake () {
		tileList = new List<Tile> ();
		walkableTileList = new List<Tile> ();
		LoadLevel (0);
	}

	// Use this for initialization
	void Start() {
		
	}

	// Update is called once per frame
	void Update () {
	}

	//Loads the specified level (reads the txt file, builds the grid, find every tile's neighbors and places dots and boosts)
	public void LoadLevel(int i) {
		//Clear everything
		ClearTransform (this.transform);
		ClearTransform (ghostsContainer.transform);
		ClearTransform (pacManContainer.transform);
		if (tileList != null) {	//This check is made necessary by the fact that this function can be called in the editor
			tileList.Clear ();
		}
		if (walkableTileList != null) {	//This check is made necessary by the fact that this function can be called in the editor
			walkableTileList.Clear ();
		}
		//Get new data from the mapFile
		if (tilePrefab != null && mapFile[i] != null) {
			tileList = MapFileReader.Read (mapFile[i]);
			foreach(Tile tile in tileList) {
				BuildTileGameObject (tile);
				if (tile.TileType != Tile.AvailableTileTypes.Wall) {
					walkableTileList.Add (tile);
				}
			}
		} else {
			Debug.Log("The script component is missing a reference.");
		}
		foreach(Tile tile in tileList) {
			//Find neighbors
			FindNeighbors(tile);
			AssignWallsSprite (tile);
			//Spawn units
			SpawnGhost (tile);
			SpawnPacMan (tile);
		}
		//Retrieving neighbors data
		foreach(Tile tile in tileList) {
			FindNeighbors(tile);
			FindNonWallNeighbors(tile);
		}
		//Position Main Camera
		PositionMainCamera ();
	}

	//Deletes all child objects of this transform
	public void ClearTransform(Transform t) {
		var childList = new List<GameObject>();
		foreach(Transform child in t) {
			childList.Add(child.gameObject);
		}
		childList.ForEach(child => DestroyImmediate(child));
	}

	//Builds a single tile and reference it in the tileList
	public void BuildTileGameObject(Tile tile) {
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
			case Tile.AvailableTileTypes.GhostStartingPositionDumbAStarAi:
				tileSprite.tag = "GhostStartingPositionDumbAStarAi";
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionWiseAStarAi:
				tileSprite.tag = "GhostStartingPositionWiseAStarAi";
				break;
		}
	}

	//Places the main camera on the game field and scales it
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

	//Finds a tile non-wall neighbors and store the data in a list that is copied to the tile reference object
	public void FindNonWallNeighbors(Tile tile) {
		for (int i = 0; i < 4; i++) {
			if (tile.NeighborTiles[i] != null && tile.NeighborTiles[i].TileType != Tile.AvailableTileTypes.Wall) {
				tile.NonWallNeighborTilesDirectionPair.Add (new TileDirectionPair(i, tile.NeighborTiles[i]));
			}
		}
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
		if (tile.TileType == Tile.AvailableTileTypes.PacManStartingPosition && pacMan != null) {
			pacMan = (GameObject)Instantiate (pacManPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
			pacMan.transform.SetParent (pacManContainer.transform);
			pacMan.GetComponent<PacManControls> ().grid = this.gameObject;
			pacMan.GetComponent<PacManControls> ().CurrentTile = tile;
		}
	}

	//Spawns the ghosts
	private void SpawnGhost(Tile tile) {
		switch (tile.TileType) {
			case Tile.AvailableTileTypes.GhostStartingPositionDumbAStarAi:
				GameObject ghostDumbAStarAi = (GameObject)Instantiate (ghostDumbAStarAiPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				ghostDumbAStarAi.transform.SetParent (ghostsContainer.transform);
				ghostDumbAStarAi.GetComponent<GhostDumbAStarAi> ().grid = this.gameObject;
				ghostDumbAStarAi.GetComponent<GhostDumbAStarAi> ().CurrentTile = tile;
				break;
			case Tile.AvailableTileTypes.GhostStartingPositionWiseAStarAi:
				GameObject ghostWiseAStarAi = (GameObject)Instantiate (ghostWiseAStarAiPrefab, new Vector3 (tile.CoordX, tile.CoordY, 0.0f), Quaternion.identity);
				ghostWiseAStarAi.transform.SetParent (ghostsContainer.transform);
				ghostWiseAStarAi.GetComponent<GhostWiseAStarAi> ().grid = this.gameObject;
				ghostWiseAStarAi.GetComponent<GhostWiseAStarAi> ().CurrentTile = tile;
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

	//When users clics the "Load first level" button in the editor
	public void GridBuilderButtonClic() {
		tileList = new List<Tile> ();
		walkableTileList = new List<Tile> ();
		LoadLevel (0);
	}

	//Properties
	public List<Tile> TileList {
		get {
			return tileList;
		}
	}

	public List<Tile> WalkableTileList {
		get {
			return walkableTileList;
		}
	}

	public GameObject PacMan {
		get {
			return pacMan;
		}
	}

	public TextAsset[] MapFile {
		get {
			return mapFile;
		}
	}
}

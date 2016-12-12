using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Manager<ItemManager> {

    public Breakable[] Items;

    public Transform LevelOrigin;
    [Range(0, 10)]
    public int LevelGridSize = 4;
    [Range(0, 3)]
    public float LevelGridStep = 1;
    [Range(0, 10)]
    public float HeightLimit = 4;

    public static int LevelGenerationSeed = 0;
    public int CurrentSeed;
    public List<Breakable>[,] Grid;

    void Awake() {
        Grid = new List<Breakable>[LevelGridSize, LevelGridSize];
        for (int x = 0; x < LevelGridSize; x++) {
            for (int y = 0; y < LevelGridSize; y++) {
                Grid[x, y] = new List<Breakable>();
            }
        }
        GenerateLevel();
    }

    public void GenerateLevel() {
        // randomly arrange level items
        CurrentSeed = LevelGenerationSeed > 0 ? LevelGenerationSeed : Mathf.FloorToInt(Random.value * 1000000);
        var r = new System.Random(CurrentSeed); // use this seeded random instead of unity's random
        for (int x = 0; x < LevelGridSize; x++) {
            for (int y = 0; y < LevelGridSize; y++) {
                if (Grid[x, y].Count > 0) {
                    continue;
                }
                var height = 0f;
                Breakable latestItem = null;
                while (true) {
                    if (r.Next(7) == 0) {
                        break;
                    }
                    if (latestItem && !latestItem.IsTable) {
                        break;
                    }
                    var items = Items.AsEnumerable()
                        .Where(i => i.Width <= Mathf.Min(latestItem != null ? latestItem.Width : LevelGridSize, LevelGridSize - x))
                        .Where(i => i.Length <= Mathf.Min(latestItem != null ? latestItem.Length : LevelGridSize, LevelGridSize - y))
                        ;
                    if (height > 0) {
                        items = items.Where(i => i.IsStackable);
                    }
                    var newItem = items.RandomSeeded(r);
                    var canUse = true;
                    for (int w = 0; w < newItem.Width; w++) {
                        for (int l = 0; l < newItem.Length; l++) {
                            canUse &= Grid[x + w, y + l].Count == Grid[x, y].Count;
                        }
                    }
                    if (!canUse) {
                        break;
                    }
                    latestItem = newItem;
                    if (height + latestItem.Height >= HeightLimit) {
                        break;
                    }
                    height += latestItem.Height;
                    for (int w = 0; w < latestItem.Width; w++) {
                        for (int l = 0; l < latestItem.Length; l++) {
                            Grid[x + w, y + l].Add(latestItem);
                        }
                    }
                }
            }
        }
        // spawn items
        var container = BreakableManager.Inst.BreakableContainer;
        var origin = LevelOrigin.position;
        var coordinatesToSpawn = new List<int>();
        for (int x = 0; x < LevelGridSize; x++) {
            for (int y = 0; y < LevelGridSize; y++) {
                coordinatesToSpawn.Add(x * 1000 + y);
            }
        }
        while (coordinatesToSpawn.Count > 0) {
            var coord = coordinatesToSpawn[0];
            coordinatesToSpawn.RemoveAt(0);
            var x = coord / 1000;
            var y = coord % 1000;
            var height = 0f;
            var itemList = Grid[x, y];
            for (int i = 0; i < itemList.Count; i++) {
                var item = itemList[i];
                for (int w = 0; w < item.Width; w++) {
                    for (int l = 0; l < item.Length; l++) {
                        var id = (x + w) * 1000 + (y + l);
                        coordinatesToSpawn.Remove(id);
                    }
                }
                var newItem = Instantiate(item.gameObject);
                newItem.name = item.name + " " + x + "/" + y + "/" + i; // needed for memo mapping
                newItem.transform.position = origin + new Vector3(LevelGridStep * (x + 0.5f), height, LevelGridStep * (y + 0.5f));
                newItem.transform.rotation = Quaternion.identity;
                newItem.transform.parent = container.transform;
                newItem.GetComponent<Breakable>().Init();
                height += item.Height;
            }
        }
        // init everything else
        TimeManager.Inst.VirtualTime = 0;
        BreakableManager.Inst.CacheBreakables();
        MemoManager.Inst.InitMemos();
    }
}

using UnityEngine;
using System.Collections;

public class RandomPlacer : PipeItemGenerator {

    public PipeItem[] itemPrefabs;

    public override void GenerateItems(Pipe pipe)
    {
        for (int i = 0; i < pipe.CurveSegmentCount; i+= 20)
        {
            int pointer = Random.Range(0, itemPrefabs.Length);
            PipeItem item = Instantiate<PipeItem>(
            itemPrefabs[pointer]);
            
            float pipeRotation =
                (Random.Range(0, pipe.pipeSegmentCount) + 0.5f) *
                360f / pipe.pipeSegmentCount;
            item.Position(pipe, i, pipeRotation, pipe.GetPipeRadiusBySegmentIndex(i));
        }
    }
}

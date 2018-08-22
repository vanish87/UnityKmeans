using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Kmeans : MonoBehaviour
{

    [SerializeField] protected Texture2D _input;
    [SerializeField] protected Color[] _pix;

    public List<Cluster<Color>> clusters = new List<Cluster<Color>>();
    public int numberOFclusters = 8;

    public List<Element<Color>> elements = new List<Element<Color>>();

    [Serializable]
    public class Cluster<T>
    {
        public int id;
        public T centriod;
        public int elementCount;
    }
    [Serializable]
    public class Element<T>
    {
        public int clusterId;
        public T data;
    }

    public Rect sourceRect;

    bool generatePending = false;
        
    // Use this for initialization
    void Start ()
    {
        this.ReadTexture();
    }
    void ReadTexture()
    {
        Debug.Log("ReadTexture");
        this.elements.Clear();
        this.clusters.Clear();

        sourceRect.width = this._input.width;
        sourceRect.height = this._input.height;
        int x = Mathf.FloorToInt(sourceRect.x);
        int y = Mathf.FloorToInt(sourceRect.y);
        int width = Mathf.FloorToInt(sourceRect.width);
        int height = Mathf.FloorToInt(sourceRect.height);

        this._pix = _input.GetPixels(x, y, width, height);
        
        for(var i = 0; i < this.numberOFclusters; ++i)
        {
            var c = this._pix[UnityEngine.Random.Range(0, this._pix.Length)];
            while(c.a < 1)
            {
                c = this._pix[UnityEngine.Random.Range(0, this._pix.Length)];
            }
            this.clusters.Add(new Cluster<Color>() { id = i, centriod = c, elementCount = 0});
        }

        foreach(var c in this._pix)
        {
            if (c.a < 1) continue;

            this.elements.Add(new Element<Color>() { clusterId = -1, data = c });
        }
    }

    void Assign(Element<Color> color)
    {
        float min = float.MaxValue;
        Cluster<Color> candidate = new Cluster<Color>() { id = -1};
        foreach(var cluster in this.clusters)
        {
            var dis = Vector4.Distance(cluster.centriod, color.data);
            if(dis < min)
            {
                min = dis;
                candidate = cluster;
            }
        }

        Assert.IsTrue(candidate.id != -1);
        color.clusterId = candidate.id;
    }

    void UpdateCentroid()
    {
        var clusterList = new List<List<Color>>();
        foreach(var cluster in this.clusters)
        {
            clusterList.Add(new List<Color>());
        }

        foreach(var e in this.elements)
        {
            var clusterBelongs = clusterList[e.clusterId];
            clusterBelongs.Add(e.data);
        }

        var index = 0;
        foreach(var cluster in clusterList)
        {
            if (cluster.Count == 0) continue;

            var sum = Vector4.zero;
            foreach(var c in cluster)
            {
                Vector4 vc = c;
                sum += vc;
            }
            sum /= cluster.Count;
            this.clusters[index].centriod = sum;
            this.clusters[index].elementCount = cluster.Count;
            index++;

            Debug.LogFormat("cluster {0} has {1} elements", index-1, cluster.Count);
        }

    }

    static int count = 0;
    // Update is called once per frame
    void Update ()
    {
        if(this.generatePending)
        {
            this.ReadTexture();
            this.generatePending = false;
            count = 10;
        }

        while(count-- > 0)
        {
            Debug.Log("count " + count.ToString());
            this.Generate();
        }
	}

    IEnumerator StartKmeans()
    {
        yield return null;

    }

    void Generate()
    {
        Debug.Log("Generate");
        var iteration = 1;
        for (var i = 0; i < iteration; ++i)
        {
            foreach (var e in this.elements)
            {
                this.Assign(e);
            }
            this.UpdateCentroid();
        }
    }

    private void OnGUI()
    {
        var sorted = clusters.OrderByDescending(o => o.elementCount).ToList();
        var initx = 50;
        foreach(var c in sorted)
        {
            ColorRectangle.GUIDrawRect(new Rect(initx += 100, 100, 100, 100), c.centriod);
        }

        if(GUILayout.Button("Start"))
        {
            this.generatePending = true;
        }

        if (GUILayout.Button("ReadTexture"))
        {
            this.ReadTexture();
        }

        if (GUILayout.Button("Generate"))
        {
            this.Generate();
        }
        //GUI.DrawTexture(new Rect(0, 0, 200, 200), this._input);
    }
}

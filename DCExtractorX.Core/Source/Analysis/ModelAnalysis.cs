namespace DC.Analysis
{
    /// <summary>
    /// Class   :   "ModelAnalysis"
    /// 
    /// Purpose :   the full set of statistics collected for a single parsed MDS model. Always computed from the
    /// original, in-memory DC.Types.Model produced by MDS.Load - never from an exported .obj/.smd file.
    /// </summary>
    public sealed class ModelAnalysis
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Identity.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ModelName;
        public string FilePath;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Model stats.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int MeshCount;
        public int BoneCount;

        public int VertexCount;
        public int FaceCount;
        public int NormalCount;
        public int UVCount;

        public int MaterialCount;

        public bool HasWGT;
        public bool HasMOT;
        public bool HasBBP;

        public int WeightHeaderCount;
        public int WeightEntryCount;

        public int UniqueMeshBones;
        public int UniqueInfluenceBones;

        public int HierarchyDepth;
        public int MaxHierarchyDepth;

        public double AvgVerticesPerMesh;
        public double AvgVerticesPerBone;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Sub-analyses.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public BoneAnalysis Bones = new BoneAnalysis();
        public WGTAnalysis Weights = new WGTAnalysis();

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classification.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Classification = "Unknown";
        public double ClassificationScore;
    }
}

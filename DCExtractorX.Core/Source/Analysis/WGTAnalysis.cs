namespace DC.Analysis
{
    /// <summary>
    /// Class   :   "WGTAnalysis"
    /// 
    /// Purpose :   statistics about a model's WGT vertex weights (skeletal influences), computed from the
    /// flattened VertexWeight[] array that WGT.Load produces.
    /// </summary>
    public sealed class WGTAnalysis
    {
        /// <summary>
        /// The number of distinct (mesh, bone) groupings found in the weight data. The original WGT file stores
        /// weights grouped under headers keyed by (meshBoneIndex, boneIndex); WGT.Load flattens those headers into
        /// individual VertexWeight entries, so this is reconstructed from the distinct pairs actually present.
        /// </summary>
        public int HeaderCount;
        public int WeightCount;

        public int UniqueMeshBoneIndexes;
        public int UniqueBoneIndexes;

        public double AvgInfluencesPerVertex;
        public int MaxInfluencesPerVertex;
    }
}

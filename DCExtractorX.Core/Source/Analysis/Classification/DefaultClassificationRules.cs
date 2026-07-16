using System;

namespace DC.Analysis.Classification
{
    /// <summary>
    /// Class   :   "StaticPropRule"
    /// 
    /// Purpose :   models with no bones at all can't be animated, so they're classified as static props.
    /// </summary>
    public sealed class StaticPropRule : IModelClassificationRule
    {
        public string Name => "StaticProp (no bones)";

        public bool TryClassify(ModelAnalysis tAnalysis, out ModelClass eClass, out double fScore)
        {
            if (tAnalysis.BoneCount == 0)
            {
                eClass = ModelClass.StaticProp;
                fScore = 1.0;
                return true;
            }

            eClass = ModelClass.Unknown;
            fScore = 0.0;
            return false;
        }
    }

    /// <summary>
    /// Class   :   "AnimatedPropRule"
    /// 
    /// Purpose :   models with a bone hierarchy but no vertex weights are typically simple articulated props
    /// (doors, levers, etc) rather than skinned characters.
    /// </summary>
    public sealed class AnimatedPropRule : IModelClassificationRule
    {
        public string Name => "AnimatedProp (bones, no weights)";

        public bool TryClassify(ModelAnalysis tAnalysis, out ModelClass eClass, out double fScore)
        {
            if (tAnalysis.BoneCount > 0 && tAnalysis.WeightEntryCount == 0)
            {
                eClass = ModelClass.AnimatedProp;
                fScore = 0.6;
                return true;
            }

            eClass = ModelClass.Unknown;
            fScore = 0.0;
            return false;
        }
    }

    /// <summary>
    /// Class   :   "NameHeuristicRule"
    /// 
    /// Purpose :   a simple name-substring heuristic for skinned models that couldn't be told apart otherwise.
    /// Deliberately low-confidence since file/model names aren't a reliable signal; a dedicated rule with better
    /// data (e.g. an asset manifest) should win over this one when added later.
    /// </summary>
    public sealed class NameHeuristicRule : IModelClassificationRule
    {
        public string Name => "Name heuristic (skinned models)";

        public bool TryClassify(ModelAnalysis tAnalysis, out ModelClass eClass, out double fScore)
        {
            if (tAnalysis.BoneCount == 0 || tAnalysis.WeightEntryCount == 0)
            {
                eClass = ModelClass.Unknown;
                fScore = 0.0;
                return false;
            }

            string szName = (tAnalysis.ModelName ?? string.Empty).ToLowerInvariant();

            if (szName.Contains("player") || szName.Contains("pl_") || szName.Contains("hero"))
            {
                eClass = ModelClass.Player;
                fScore = 0.5;
                return true;
            }

            if (szName.Contains("npc") || szName.Contains("vill") || szName.Contains("town"))
            {
                eClass = ModelClass.NPC;
                fScore = 0.5;
                return true;
            }

            if (szName.Contains("mon") || szName.Contains("boss") || szName.Contains("enemy"))
            {
                eClass = ModelClass.Monster;
                fScore = 0.5;
                return true;
            }

            eClass = ModelClass.Unknown;
            fScore = 0.0;
            return false;
        }
    }

    /// <summary>
    /// Class   :   "SkinnedFallbackRule"
    /// 
    /// Purpose :   a low-confidence catch-all for any remaining skinned model (has bones and weights) that no
    /// other rule matched, so models don't fall all the way back to "Unknown" just because they lack a
    /// recognizable name.
    /// </summary>
    public sealed class SkinnedFallbackRule : IModelClassificationRule
    {
        public string Name => "Skinned fallback";

        public bool TryClassify(ModelAnalysis tAnalysis, out ModelClass eClass, out double fScore)
        {
            if (tAnalysis.BoneCount > 0 && tAnalysis.WeightEntryCount > 0)
            {
                eClass = ModelClass.NPC;
                fScore = 0.2;
                return true;
            }

            eClass = ModelClass.Unknown;
            fScore = 0.0;
            return false;
        }
    }
}

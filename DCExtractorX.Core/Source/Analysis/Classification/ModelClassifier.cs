using System;
using System.Collections.Generic;

namespace DC.Analysis.Classification
{
    /// <summary>
    /// Class   :   "ModelClassifier"
    /// 
    /// Purpose :   runs a model's analysis through a list of IModelClassificationRule and keeps whichever
    /// proposed classification has the highest confidence score. Adding a new classification only requires
    /// implementing IModelClassificationRule and registering it here (or via RegisterRule) - no existing rule
    /// needs to change.
    /// </summary>
    public sealed class ModelClassifier
    {
        readonly List<IModelClassificationRule> m_tRules = new List<IModelClassificationRule>();

        /// <summary>
        /// Builds a classifier with the built-in default rule set.
        /// </summary>
        public static ModelClassifier CreateDefault()
        {
            ModelClassifier tClassifier = new ModelClassifier();
            tClassifier.RegisterRule(new StaticPropRule());
            tClassifier.RegisterRule(new AnimatedPropRule());
            tClassifier.RegisterRule(new NameHeuristicRule());
            tClassifier.RegisterRule(new SkinnedFallbackRule());
            return tClassifier;
        }

        /// <summary>
        /// Adds a rule to be considered during classification.
        /// </summary>
        public void RegisterRule(IModelClassificationRule tRule)
        {
            if (tRule != null)
                m_tRules.Add(tRule);
        }

        /// <summary>
        /// Classifies the model, writing the winning classification and its score directly onto the analysis.
        /// </summary>
        public void Classify(ModelAnalysis tAnalysis)
        {
            ModelClass eBestClass = ModelClass.Unknown;
            double fBestScore = 0.0;

            foreach (IModelClassificationRule tRule in m_tRules)
            {
                try
                {
                    if (tRule.TryClassify(tAnalysis, out ModelClass eClass, out double fScore) && fScore > fBestScore)
                    {
                        fBestScore = fScore;
                        eBestClass = eClass;
                    }
                }
                catch (Exception ex)
                {
                    //A misbehaving rule should never abort classification (or the whole analysis run) for a model.
                    Custom.Diagnostics.Logger.Error("Classification rule threw an exception.", szAsset: tAnalysis.ModelName, szModel: tAnalysis.ModelName, tException: ex);
                }
            }

            tAnalysis.Classification = eBestClass.ToString();
            tAnalysis.ClassificationScore = fBestScore;
        }
    }
}

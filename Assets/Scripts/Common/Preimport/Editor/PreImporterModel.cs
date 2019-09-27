/*  主要用于模型资源导入时批量处理   */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Common.PreImprot.Editor
{/*
    public class PreImporterModel : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            string path = assetPath;
            ModelImporter modelImporter = (ModelImporter)assetImporter;
            if (path.Contains("Models"))
            {
                modelImporter.isReadable = false;
                modelImporter.importBlendShapes = false;
                modelImporter.importMaterials = false;
                modelImporter.importAnimation = true;
                modelImporter.importTangents = ModelImporterTangents.None;
                modelImporter.importNormals = ModelImporterNormals.None;
                modelImporter.meshCompression = ModelImporterMeshCompression.High;
            }
        }
    }*/
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace FarmSystem
{
    [System.Serializable]
    public struct SoilStageData
    {
        [Header("Tên trạng thái đất")]
        public string name;
        [Header("Mô tả hình ảnh của đất ở trạng thái này")]
        public Sprite stageImage;
    }
    [System.Serializable]
    public struct SoilData
    {
        [Header("Trạng thái của đất: ")]
        public List<SoilStageData> stageDatas;
    }
    [CreateAssetMenu(fileName = "Land Data", menuName = "Farm System/Data/Land Data")]
    public class SO_SoilData : ScriptableObject
    {
        public SoilData data;
    }
}
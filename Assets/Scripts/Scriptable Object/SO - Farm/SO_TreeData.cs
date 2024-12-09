using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace FarmSystem
{
    [System.Serializable]
    public enum ESeason
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
    [System.Serializable]
    public struct StageData
    {
        [Header("Mô tả hình ảnh của cây ở trạng thái này")]
        public Sprite stageImage;
        [Header("Thời gian cần để chuyển dịch sang trạng thái tiếp theo")]
        public float transitionTime;
        [Header("Có phải là trạng thái cuối cùng hay không ?")]
        [Tooltip("Phụ thuộc vào trạng thái cuối cùng để quyết định là có thu hoạch được hay không")]
        public bool isFinalStage;
    }
    [System.Serializable]
    public struct RewardData
    {
        [Header("Sản lượng thu được sau khi thu hoạch: ")]
        public int harvests;
        [Header("Kinh nghiệm thu được sau khi thu hoạch: ")]
        public float expBonus;
        [Header("Tiền thu được sau khi bán (tính theo từng đơn vị của sản lượng): ")]
        public float sellPrice;
    }
    [System.Serializable]
    public struct TreeData
    {
        [Header("Mùa ưa thích")]
        public ESeason favoriteSeason;
        [Header("Giá trị nhận được sau khi thu hoạch")]
        public RewardData rewardData;
        [Header("Trạng thái của cây: ")]
        public List<StageData> stageDatas;
    }
    [CreateAssetMenu(fileName = "Tree Data", menuName = "Farm System/Data/Tree Data")]
    public class SO_TreeData : ScriptableObject
    {
        public TreeData data;
    }
}
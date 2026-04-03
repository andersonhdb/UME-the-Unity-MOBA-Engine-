using System.Collections.Generic;
using UnityEngine;
using UME.Systems;

namespace UME.Characters
{
    /// <summary>
    /// ScriptableObject that defines a hero's base identity, stats, and skill layout.
    /// Create hero assets via Assets > Create > UME > Hero > HeroData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewHeroData", menuName = "UME/Hero/HeroData")]
    public class HeroData : ScriptableObject
    {
        [Header("Identity")]
        public string heroName = "Hero";
        public string heroTitle = "The Unnamed";
        [TextArea(2, 5)]
        public string description = "";
        public Sprite portrait;
        public Sprite splashArt;

        [Header("Classification")]
        public HeroRole primaryRole  = HeroRole.Fighter;
        public HeroRole secondaryRole = HeroRole.None;
        public HeroDifficulty difficulty = HeroDifficulty.Beginner;

        [Header("Base Stats")]
        public UnitStats baseStats = UnitStats.Default;

        [Tooltip("Stat growth applied each time the hero levels up.")]
        public UnitStats statsPerLevel;

        [Header("Ability Configuration")]
        [Tooltip("Passive ability description.")]
        public SkillData passiveSkill;

        [Tooltip("Active skills Q, W, E, R in order.")]
        public List<SkillData> activeSkills = new List<SkillData>(4);

        [Header("Addressables")]
        [Tooltip("Addressable key for the hero prefab.")]
        public string heroPrefabKey = "";

        [Tooltip("Addressable key for the hero ability VFX bundle.")]
        public string vfxBundleKey = "";
    }

    public enum HeroRole
    {
        None,
        Fighter,
        Mage,
        Assassin,
        Marksman,
        Support,
        Tank
    }

    public enum HeroDifficulty
    {
        Beginner,
        Intermediate,
        Advanced
    }
}

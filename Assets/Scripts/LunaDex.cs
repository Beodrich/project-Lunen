using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class LunaDex : MonoBehaviour
{
    public enum Locations
    {
        Battle = 1,
        TestOverworld = 2
    }

    public enum LunenEnum
    {
        Woufa,
        Wynern,
        Kyoske,
        Debero,
        Yibalis,
        SirDungeness,
        Wavi,
        Wavineer,
        Wavulus,
        Glacus,
        Rootle,
        Stuimp,
        Barix,
        Flamela,
        Fleor,
        Flareo,
        Furo,
        Futetra,
        Icifiz,
        Tabela,
        Drole,
        Ebako,
        NulvaIncarnate,
        zz_TestMonster
    }
    public enum ActionEnum
    {
        Tackle,
        AttackBoost,
        DefenseBoost,
        Execute,
        Streamline,
        Frenzy,
        FrontlineTank,
        NullificationArmor,
        Bite,
        WarCry,
        SuperFang,
        CloseCombat,
        Intimidate,
        UnnervingWill,
        ShiftTactics,
        Trick,
        SourceShift,
        SynchronizedArmor,
        BuffMirror,
        Cleanse,
        Leech,
        RockThrow,
        RockSlide,
        RockStream,
        AncientStorm,
        SteelStrike,
        SpikeArmor,
        Fortify,
        ClawCrush,
        WaterGun,
        TidalWave,
        ScorchingWater,
        Tsunami,
        TidalShift,
        WaterVortex,
        HealingAura,
        RulerOfTheSea,
        LeafScratch,
        Wrap,
        Allergens,
        LeafBlower,
        VineSlash,
        Root,
        MorningWood,
        VineManipulation,
        Spark,
        Enkindle,
        Torch,
        Melt,
        Scorch,
        Incinerate,
        Brimstone,
        PlasmaBolt,
        PlasmaArmor,
        ShockingGaze,
        HighVoltageSwitch,
        WingAttack,
        DrillPeck,
        WindVortex,
        Headwind,
        WindKnives,
        Tailwind,
        SeaStorm,
        ChillAura,
        IceShard,
        Refreeze,
        IceStorm,
        FrozenShell,
        FrozenHellfire,
        TableFlip,
        Confusion,
        CosmicThrust,
        Sacrifice,
        Hysteria,
        StarStrike,
        StarShower,
        Retribution,
        CorruptionBlast,
        CorruptionSurge,
        CorruptionAura,
        NulvaBeam,
        zz_DebugMove
    }
    public enum StatusEffectEnum
    {
        Bleeding,
        BrokenArmor,
        Burned,
        Confused,
        Constrained,
        Corruptible,
        Freezing,
        Grounded,
        Oiled,
        Pacified,
        Reversed,
        Sanctuary,
        Shocked,
        Staggered,
        WeakMinded
    }

    public bool DEBUG;
    public bool ShowTemplates;
    [ConditionalField(nameof(ShowTemplates))] public GameObject LunenTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject MonsterTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject ActionTemplate;
    [ConditionalField(nameof(ShowTemplates))] public GameObject EffectTemplate;
    [EnumNamedArray(typeof(LunenEnum))]
    public List<GameObject> LunenList = new List<GameObject>();
    [EnumNamedArray(typeof(ActionEnum))]
    public List<GameObject> ActionList = new List<GameObject>();
    [EnumNamedArray(typeof(StatusEffectEnum))]
    public List<GameObject> StatusEffectList = new List<GameObject>();

    public GameObject GetLunenObject(LunenEnum lunen)
    {
        return LunenList[(int)lunen];
    }

    public Lunen GetLunen(LunenEnum lunen)
    {
        return LunenList[(int)lunen].GetComponent<Lunen>();
    }

    public GameObject GetActionObject(ActionEnum action)
    {
        return ActionList[(int)action];
    }

    public Action GetAction(ActionEnum action)
    {
        return ActionList[(int)action].GetComponent<Action>();
    }

    public GameObject GetStatusEffectObject(StatusEffectEnum effect)
    {
        return StatusEffectList[(int)effect];
    }

    public Effects GetStatusEffect(StatusEffectEnum effect)
    {
        return StatusEffectList[(int)effect].GetComponent<Effects>();
    }
}
